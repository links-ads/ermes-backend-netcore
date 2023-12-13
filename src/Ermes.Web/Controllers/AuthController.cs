using Ermes.Interfaces;
using Ermes.Web.Utils;
using FusionAuthNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSwag.Annotations;
using System.Threading.Tasks;

namespace Ermes.Web.Controllers
{
    public class AuthController : ErmesControllerBase, IBackofficeApi
    {
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly IOptions<ErmesSettings> _ermesSettings;

        public AuthController(IOptions<FusionAuthSettings> fusionAuthSettings, IOptions<ErmesSettings> ermesSettings)
        {
            _fusionAuthSettings = fusionAuthSettings;
            _ermesSettings = ermesSettings;
        }

        /// <summary>
        /// This API manually manage scheme and domain because we want to use the same FusionAuth tenant for both DEV and LOCAL env
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/services/app/auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token. The token and refresh token are returned inside cookies")]
        public async Task<IActionResult> TokenRetrieve(string code)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            string scheme = UrlHelper.HTTP_SSL_SCHEME;
            string domain = _ermesSettings.Value.AppDomain;
            var isThereOrigin = Request.Headers.TryGetValue("Origin", out StringValues source);

            string redirectUri;
            if (isThereOrigin)
                redirectUri = $"{source[0]}/login-callback";
            else
                return BadRequest("Invalid Origin Header");

            Logger.InfoFormat("Scheme: {0}, Domain: {1}, isThereOrigin; {2}, RedirectUri: {3}", scheme, domain, isThereOrigin, redirectUri);
            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, redirectUri);
            if (tokenResponse.WasSuccessful())
            {
                if (isThereOrigin && source[0].Contains("localhost"))
                {
                    domain = _ermesSettings.Value.AppDomainLocal;
                    scheme = UrlHelper.HTTP_SCHEME;
                }
                Logger.InfoFormat("Domain: {0}, token:{1}", domain, tokenResponse.successResponse.access_token);
                CookieHelper.AddAuthCookies(Response, scheme, domain, tokenResponse.successResponse);
                return Ok();
            }
            else
            {
                var faError = FusionAuth.ManageErrorResponse(tokenResponse);
                return BadRequest(faError.Message);
            }
        }

        [HttpGet]
        [Route("api/services/app/auth/logout-callback")]
        [OpenApiOperation("Logout from the application. This funcion resets cookies")]
        public virtual IActionResult Logout()
        {
            string scheme = UrlHelper.HTTP_SSL_SCHEME;
            string domain = _ermesSettings.Value.AppDomain;
            var isThereOrigin = Request.Headers.TryGetValue("Origin", out StringValues source);
            if (isThereOrigin && source[0].Contains("localhost"))
            {
                domain = _ermesSettings.Value.AppDomainLocal;
                scheme = UrlHelper.HTTP_SCHEME;
            }
            CookieHelper.ResetAuthCookies(Response, scheme, domain);
            return Ok();
        }
    }
}
