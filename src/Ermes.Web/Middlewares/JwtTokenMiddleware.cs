using io.fusionauth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public JwtTokenMiddleware(RequestDelegate next, FusionAuthClient _client)
        {
            client = _client;
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString();
            if(token == null || token == string.Empty)
                token = context.Request.Cookies["X-Access-Token"];
            if (token != "" && await VerifyToken(token))
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
                //Incapso l'identità in una ClaimsPrincipal e la associo alla richiesta corrente
                context.User = new ClaimsPrincipal(identity);
            }

            await next.Invoke(context);
        }

        private async Task<bool> VerifyToken(string token)
        {
            var response = await client.ValidateJWTAsync(token);

            if (response.WasSuccessful())
                return true;

            return false;
        }
    }
}
