using Ermes.Interfaces;
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

        public AuthController(IOptions<FusionAuthSettings> fusionAuthSettings)
        {
            _fusionAuthSettings = fusionAuthSettings;
        }

        /// <summary>
        /// This API checks Origin header to understand where the request is coming from. This is necessary 
        /// because we want to use the same FusionAuth tenant for both DEV and LOCAL env
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/services/app/auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token")]
        public async Task<IActionResult> TokenRetrieve(string code)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            var isThereOriginHeader = Request.Headers.TryGetValue("Origin", out StringValues source);
            string redirectUri;
            if (isThereOriginHeader)
                redirectUri = $"{source[0]}/login-callback";
            else
                return BadRequest("Invalid Origin Header");

            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, redirectUri);
            if (tokenResponse.WasSuccessful())
                return Ok(new { tokenResponse.successResponse.access_token, tokenResponse.successResponse.refresh_token });
            else
            {
                var faError = FusionAuth.ManageErrorResponse(tokenResponse);
                return BadRequest(faError.Message);
            }
        }

        [HttpGet]
        [Route("api/services/app/auth/refresh-token")]
        [OpenApiOperation("Exchange refresh token for a new access token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var response = await client.ExchangeRefreshTokenForAccessTokenAsync(refreshToken, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret, "openid offline_access", "");

            if (response.WasSuccessful())
                return Ok(response.successResponse);
            else
            {
                var faError = FusionAuth.ManageErrorResponse(response);
                return BadRequest(faError.Message);
            }
        }
    }
}
