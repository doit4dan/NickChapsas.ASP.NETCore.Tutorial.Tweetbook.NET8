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

                // use role manager to create roles if they do not exist already
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if(!await roleManager.RoleExistsAsync("Admin"))
                {
                    var adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);
                }

                if (!await roleManager.RoleExistsAsync("Poster"))
                {
                    var posterRole = new IdentityRole("Poster");
                    await roleManager.CreateAsync(posterRole);
                }

                // use below after creating users to add to role
                //var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                //var adminUser = await userManager.FindByNameAsync("admin@test.com");

                //if(adminUser != null)
                //{
                //    await userManager.AddToRoleAsync(adminUser, "Admin");
                //}

                //var posterUser = await userManager.FindByNameAsync("poster@test.com");               
                //if(posterUser != null)
                //{
                //    await userManager.AddToRoleAsync(posterUser, "Poster");
                //}
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
