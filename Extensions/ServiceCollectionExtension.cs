using Michaelsoft.Nexi.Interfaces;
using Michaelsoft.Nexi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Michaelsoft.Nexi.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static void AddNexi(this IServiceCollection services,
                                   IConfiguration configuration)
        {
            services.Configure<NexiSettings>(configuration.GetSection("Nexi"));

            services.AddSingleton<INexiSettings>
                (sp => sp.GetRequiredService<IOptions<NexiSettings>>().Value);
            
            services.AddSingleton<INexi, Services.Nexi>();
        }

    }
}