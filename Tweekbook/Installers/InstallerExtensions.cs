using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tweetbook.Installers
{
    public static class InstallerExtensions 
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.                      
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
                typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract) // Get all classes which implement IInstaller
                .Select(Activator.CreateInstance) // Create instance of these classes
                .Cast<IInstaller>() // Cast instances to IInstaller
                .ToList(); // Turn this into a List

            // Install Services for each 
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
