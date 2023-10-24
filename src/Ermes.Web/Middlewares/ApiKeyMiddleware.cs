using Ermes.ExternalServices.Externals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ermes.Web.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptions<ExternalsSettings> _externalSettings;

        public ApiKeyMiddleware(RequestDelegate next, IOptions<ExternalsSettings> externalSettings)
        {
            _externalSettings = externalSettings;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var key = context.Request.Headers["X-API-Key"].ToString();
            if (key != string.Empty && VerifyApiKey(key))
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ErmesConsts.ApiKeyClaim, key));
                context.User = new ClaimsPrincipal(identity);
            }

            await next.Invoke(context);
        }

        private bool VerifyApiKey(string apiKey)
        {
            return _externalSettings.Value.ApiKey == apiKey;
        }
    }
}
