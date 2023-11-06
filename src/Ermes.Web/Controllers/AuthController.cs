using Ermes.Interfaces;
using Ermes.Web.Utils;
using FusionAuthNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
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
        [Route("auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token. The API returns token and refresh token to the final client")]
        public async Task<IActionResult> TokenRetrieve(string code, string userState, string state, string error, string errorReason, string errorDescription)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, $"{Request.Scheme}://{Request.Host}/auth/oauth-callback");
            if (tokenResponse.WasSuccessful())
            {
                Response.Cookies.Append(
                    "app.at",
                    tokenResponse.successResponse.access_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = false
                    }
                );
                Response.Cookies.Append(
                    "app.rt",
                    tokenResponse.successResponse.refresh_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = false
                    }
                );

                Response.Cookies.Append(
                    "app.at_exp",
                    DateTime.Now.AddSeconds(tokenResponse.successResponse.expires_in.Value).Ticks.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Secure = false
                    }
                );

                Response.Cookies.Append(
                    "app.idt",
                    tokenResponse.successResponse.id_token,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Secure = false
                    }
                );

                return Redirect($"{_fusionAuthSettings.Value.ClientBasePath}/callback?userState={userState}&state={state}");
            }
            
            return BadRequest();
        }

        [HttpGet]
        [Route("auth/logout-callback")]
        public virtual IActionResult Logout()
        {
            Response.Cookies.Append("app.at", string.Empty);
            Response.Cookies.Append("app.rt", string.Empty);
            Response.Cookies.Append("app.at_exp", string.Empty);
            Response.Cookies.Append("app.idt", string.Empty);
            return Redirect($"{_ermesSettings.Value.WebAppBaseUrl}/logout-callback");
        }

    }
}
