using Ermes.Interfaces;
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
        [Route("api/services/app/auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token. The API returns token and refresh token to the final client")]
        public async Task<IActionResult> TokenRetrieve(string code, string userState, string state, string error, string errorReason, string errorDescription)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            //TODO: to be improved by editing nginx configuration
            string scheme = "http";
            if (Request.Headers != null && Request.Headers.Count > 0 && Request.Headers.ContainsKey("X-Forwarded-Proto"))
                scheme = Request.Headers["X-Forwarded-Proto"];
            string redirectUri = $"{scheme}://{Request.Host}/api/services/app/auth/oauth-callback";
            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, redirectUri);
            if (tokenResponse.WasSuccessful())
            {
                Response.Cookies.Append(
                    "app.at",
                    tokenResponse.successResponse.access_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = scheme == "https"
                    }
                );
                Response.Cookies.Append(
                    "app.rt",
                    tokenResponse.successResponse.refresh_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Secure = scheme == "https"
                    }
                );

                Response.Cookies.Append(
                    "app.at_exp",
                    DateTime.Now.AddSeconds(tokenResponse.successResponse.expires_in.Value).Ticks.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Secure = scheme == "https"
                    }
                );

                Response.Cookies.Append(
                    "app.idt",
                    tokenResponse.successResponse.id_token,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Secure = scheme == "https"
                    }
                );

                return Redirect($"{_ermesSettings.Value.WebAppBaseUrl}/callback?userState={userState}&state={state}");
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
            Response.Cookies.Append("app.at", string.Empty);
            Response.Cookies.Append("app.rt", string.Empty);
            Response.Cookies.Append("app.at_exp", string.Empty);
            Response.Cookies.Append("app.idt", string.Empty);
            return Redirect($"{_ermesSettings.Value.WebAppBaseUrl}/logout-callback");
        }

    }
}
