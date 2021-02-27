using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace QuickTestDI {
    class Program {
        static void Main(string[] args) {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            // calls the Run method in App, which is replacing Main
            serviceProvider.GetService<App>().Run();
        }

        private static IServiceCollection ConfigureServices() {
            IServiceCollection services = new ServiceCollection();

            //Load global configuration from database and use them.
            var config = LoadConfiguration();
            services.AddSingleton(config);
            
            services.AddDbContext<EntityConfigurationContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
            services.AddScoped<ITenantService, TenantService>();

            
            //I take this part from this example. But I am not successed.


            // required to run the application
            services.AddTransient<App>();

            return services;
        }

        public static IConfiguration LoadConfiguration() {
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();

            builder.AddEntityConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));

            IConfigurationRoot configurationRoot = builder.Build();
            DemoOptions options = new();
            configurationRoot.GetSection($"{nameof(DemoOptions)}:{DemoOptions.Global}").Bind(options);

            Console.WriteLine($"DemoOptions:Global:Enabled={options.Enabled}");
            Console.WriteLine($"DemoOptions:Global:AutoRetryDelay={options.AutoRetryDelay}");
            Console.WriteLine($"DemoOptions:Global:IdentityOptions:MaxUserNameLength={options.IdentityOptions.MaxUserNameLength}");

            return builder.Build();
        }
    }


}
