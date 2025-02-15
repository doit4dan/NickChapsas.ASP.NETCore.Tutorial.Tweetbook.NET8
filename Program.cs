using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Installers;
using Tweetbook.Options;

namespace Tweetbook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Optimized and Clean Service Setup for DI
            builder.Services.InstallServicesInAssembly(builder.Configuration);

            var app = builder.Build();

            var swaggerOptions = new SwaggerOptions();
            builder.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            if (app.Environment.IsDevelopment())
            {
            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOptions.JsonRoute;
            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
            });
            }            

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
