using Ermes.Interfaces;
using Ermes.Web.Utils;
using FusionAuthNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSwag.Annotations;
using System;
using System.Linq;
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

        [HttpGet]
        [Route("api/services/app/auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token. The API returns token and refresh token to the final client")]
        public async Task<IActionResult> TokenRetrieve(string code)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            
            string scheme = UrlHelper.GetSchemeFromRequest(Request);
            string domain = _ermesSettings.Value.AppDomain;
            var isThereOrigin = Request.Headers.TryGetValue("Origin", out StringValues source);

            string redirectUri;
            if (isThereOrigin)
                redirectUri = $"{source[0]}/login-callback";
            else
                return BadRequest("Invalid Origin Header");

            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, redirectUri);
            if (tokenResponse.WasSuccessful())
            {
                if (isThereOrigin && source[0].Contains("localhost"))
                    domain = _ermesSettings.Value.AppDomainLocal;
                
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
        public virtual IActionResult Logout()
        {
            CookieHelper.ResetAuthCookies(Response);
            return Ok();
        }
    }
}
