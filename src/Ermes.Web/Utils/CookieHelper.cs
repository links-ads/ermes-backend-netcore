using Azure;
using io.fusionauth.domain.oauth2;
using Microsoft.AspNetCore.Http;
using System;

namespace Ermes.Web.Utils
{
    public static class CookieHelper
    {
        public static void AddAuthCookies(HttpResponse response, string scheme, string appDomain, AccessToken tokenResponse)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = scheme == "https",
                Domain = $".{appDomain}"
            };
            response.Cookies.Append(
                "app.at",
                tokenResponse.access_token,
                cookieOptions
            );
            response.Cookies.Append(
                "app.rt",
                tokenResponse.refresh_token,
                cookieOptions
            );
            cookieOptions.HttpOnly = false;
            response.Cookies.Append(
                "app.at_exp",
                DateTime.Now.AddSeconds(tokenResponse.expires_in.Value).Ticks.ToString(),
                cookieOptions
            );

            response.Cookies.Append(
                "app.idt",
                tokenResponse.id_token,
                cookieOptions
            );

        }

        public static void ResetAuthCookies(HttpResponse response)
        {
            response.Cookies.Append("app.at", string.Empty);
            response.Cookies.Append("app.rt", string.Empty);
            response.Cookies.Append("app.at_exp", string.Empty);
            response.Cookies.Append("app.idt", string.Empty);
        }
    }
}
