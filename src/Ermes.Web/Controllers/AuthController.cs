using Ermes.Interfaces;
using Ermes.Web.Controllers.Dto;
using FusionAuthNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
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

        [HttpGet]
        [Route("auth/oauth-callback")]
        [OpenApiOperation("Exchange authorization code for token. The API returns token and refresh token to the final client")]
        public async Task<IActionResult> TokenRetrieve(string code, string userState, string state, string error, string errorReason, string errorDescription)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            if (code == null || userState != "Authenticated") 
            {
                return BadRequest(error);
            }

            var tokenResponse = await client.ExchangeOAuthCodeForAccessTokenAsync(code, _fusionAuthSettings.Value.ClientId, _fusionAuthSettings.Value.ClientSecret,  $"{Request.Scheme}://{Request.Host}/auth/oauth-callback");
            if (tokenResponse.WasSuccessful())
            {
                Response.Cookies.Append(
                    "X-Access-Token",
                    tokenResponse.successResponse.access_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite=  SameSiteMode.Strict,
                        Secure = false
                    }
                );
                Response.Cookies.Append(
                    "X-Refresh-Token",
                    tokenResponse.successResponse.refresh_token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = false
                    }
                );

                return Redirect(string.Format("{0}/callback/?state={1}", _fusionAuthSettings.Value.ClientBasePath, userState));
                //return Redirect(string.Format("{0}/callback&state={1}&userId={2}", _fusionAuthSettings.Value.ClientBasePath, userState, tokenResponse.successResponse.userId));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("auth/logout-callback")]
        [OpenApiOperation("Perform logout operation")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Append(
                "X-Access-Token",
                "expired",
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddDays(-1)
                }
            );
            Response.Cookies.Append(
                "X-Refresh-Token",
                "expired",
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddDays(-1)
                }
            );
            return Redirect(string.Format("{0}/logout-callback", _fusionAuthSettings.Value.ClientBasePath));

        }
    }
}
