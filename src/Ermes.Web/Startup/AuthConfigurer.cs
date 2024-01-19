using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Ermes.Web.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var authenticationBuilder = services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                authenticationBuilder.AddJwtBearer(options =>
                {
                    options.Authority = configuration["FusionAuth:Url"];
                    options.Audience = configuration["FusionAuth:ClientId"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,

                        //Important: use the same Issuer and Audience in JwtTokenMiddleware
                        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],
                        ValidAudience = configuration["Authentication:JwtBearer:Audience"]
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration["FusionAuth:Url"];
                    options.ClientId = configuration["FusionAuth:ClientId"];
                    options.ClientSecret = configuration["FusionAuth:ClientSecret"];

                    options.UsePkce = true;
                    options.ResponseType = "code";
                    options.RequireHttpsMetadata = false;
                });

            }
        }
    }
}
