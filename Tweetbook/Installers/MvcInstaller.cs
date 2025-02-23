
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tweetbook.Authorization;
using Tweetbook.Filters;
using Tweetbook.Options;
using Tweetbook.Services;

namespace Tweetbook.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(JwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            // AddControllers instead of AddMvc                
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // Method used in Nick's tutorial has been deprecated
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddScoped<IIdentityService, IdentityService>();

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,         // this would be reqd in Production scenario
                ValidateAudience = false,       // this would be reqd in Production scenario
                RequireExpirationTime = false,  // this would be reqd in Production scenario
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParams);

            // Add Authentication: Jwt Bearer Token
            // Authentication: All about logging in
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParams;
                });

            // Authorization: What can users do after logging in
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustWorkForApei", policy =>
                {
                    // You can add additional requirements to any given policy, e.g. Check for Claims/Roles
                    policy.AddRequirements(new WorksForCompanyRequirement("apei.com"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();

            // You can add policies by specifying options for Authorization
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Test", policy => policy.RequireRole(new string[] { "Admin" }));
            //});

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Tweetbook API", Version = "v1" });

                // refactored a bit from Nick's implementation to work w/ .NET 8                                                            
                x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
    }
}
