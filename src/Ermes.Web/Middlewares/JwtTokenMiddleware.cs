using io.fusionauth;
using io.fusionauth.domain.oauth2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ermes.Web.Middlewares
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly FusionAuthClient client;
        private readonly IConfiguration _appConfiguration;
        public JwtTokenMiddleware(RequestDelegate next, FusionAuthClient _client, IConfigurationRoot appConfiguration)
        {
            client = _client;
            _appConfiguration = appConfiguration;
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString();
            if (token != "" && await VerifyTokenAsync(token))
            {
                if (!context.Response.Headers.ContainsKey("Token"))
                    context.Response.Headers.Add("Token", token);

                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;

                var identity = new ClaimsIdentity();
                //Aggiungo uno o più claim relativi all'utente loggato
                identity.AddClaim(new Claim(ErmesConsts.TokenClaim, token));
                identity.AddClaim(new Claim(ErmesConsts.FusionAuthUserGuidClaim, tokenS.Subject));
                identity.AddClaim(new Claim(ErmesConsts.ValidFromClaim, tokenS.ValidFrom.ToString()));
                identity.AddClaim(new Claim(ErmesConsts.ValidToClaim, tokenS.ValidTo.ToString()));
                identity.AddClaim(new Claim(ErmesConsts.RolesClaim, string.Join(',', tokenS.Claims.Where(a => a.Type == "roles").Select(a => a.Value).ToList())));
                //Incapsulo l'identità in una ClaimsPrincipal e la associo alla richiesta corrente
                context.User = new ClaimsPrincipal(identity);
            }

            await next.Invoke(context);
        }

        private async Task<bool> VerifyTokenAsync(string token)
        {
            var response = await client.ValidateJWTAsync(token);

            if (response.WasSuccessful())
                return true;

            return false;
        }

        private async Task<AccessToken> RefreshTokenAsync(string refreshToken)
        {
            var clientId = _appConfiguration["FusionAuth:ClientId"];
            var clientSecret = _appConfiguration["FusionAuth:ClientSecret"];
            var response = await client.ExchangeRefreshTokenForAccessTokenAsync(refreshToken, clientId, clientSecret, "openid offline_access", "");

            if (response.WasSuccessful())
                return response.successResponse;

            return null;
        }
    }
}
