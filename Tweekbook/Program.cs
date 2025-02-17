using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tweetbook.Data;
using Tweetbook.Installers;
using Tweetbook.Options;

namespace Tweetbook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);            

            // Optimized and Clean Service Setup for DI
            builder.Services.InstallServicesInAssembly(builder.Configuration);

            var app = builder.Build();

            // Enable automatic EF Migrations ( DO NOT ENABLE FOR PRODUCTION )
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

                await dbContext.Database.MigrateAsync();
            }

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
