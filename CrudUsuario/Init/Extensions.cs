using CrudUsuario.Business;
using CrudUsuario.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CrudUsuario.Init
{
    public static class Extensions
    {
        public static void ConfigureDb(this IApplicationBuilder app)
        {
            //var db = app.ApplicationServices.GetService<IUserService>();
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var encrypter = serviceScope.ServiceProvider.GetService<IEncrypter>();
                var user = serviceScope.ServiceProvider.GetService<IUserService>();
                var u1 = new User
                {
                    Email = "teste01@teste.com",
                    Name = "Teste 01"
                };
                u1.SetPassword("pass123", encrypter);
                var u2 = new User
                {
                    Email = "teste02@teste.com",
                    Name = "Teste 02"
                };
                u2.SetPassword("pass123", encrypter);
                var u3 = new User
                {
                    Email = "teste03@teste.com",
                    Name = "Teste 03"
                };
                u3.SetPassword("pass123", encrypter);
                user.Insert(u1);
                user.Insert(u2);
                user.Insert(u3);
            }
        }

        public static void ConfigureJwtService(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();
            services.AddSingleton(jwtConfiguration);

            var tokenConfig = new TokenConfig();


            var section = configuration.GetSection("TokenConfig");
            section.Bind(tokenConfig);
            services.AddSingleton(tokenConfig);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = jwtConfiguration.Key;
                paramsValidation.ValidAudience = tokenConfig.Audience;
                paramsValidation.ValidIssuer = tokenConfig.Issuer;

                paramsValidation.ValidateIssuerSigningKey = true;

                paramsValidation.ValidateLifetime = true;

                paramsValidation.ClockSkew = TimeSpan.Zero;
            });


            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
    }
}
