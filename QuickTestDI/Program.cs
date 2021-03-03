using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuickTestDI {
    class Program {
        static void Main(string[] args) {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            new Thread(() => { serviceProvider.GetService<App>().Run(); Thread.Sleep(5000); }) { Name = "1" }.Start();
            new Thread(() => serviceProvider.GetService<App>().Run()) { Name = "2" }.Start();
        }

        private static IServiceCollection ConfigureServices() {
            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<DemoDbContext>(o => o.UseInMemoryDatabase("InMemoryDb"));

            var context = services.BuildServiceProvider().GetService<DemoDbContext>();
            SeedDemoData(context);

            //Load global configuration from database and bind them.
            var config = LoadGlobalConfiguration(context);
            var mySection = config.GetSection($"{nameof(DemoOptions)}:{DemoOptions.Global}");
            services.Configure<DemoOptions>(c => mySection.Bind(c));

            services.AddSingleton(config);
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IOptionsManager<DemoOptions>, DemoOptionsManager>();

            // required to run the application
            services.AddTransient<App>();

            return services;
        }

        private static void SeedDemoData(DemoDbContext context) {
            context.DemoSettings.AddRange(new List<DemoSettings>() {
                 new DemoSettings("DemoOptions:Global:Enabled", "true"){Id=1},
                 new DemoSettings("DemoOptions:Global:AutoRetryDelay", "14:58:00"){Id=2},
                 new DemoSettings("DemoOptions:Global:IdentityOptions:MaxUserNameLength", "5"){Id=3},
            });

            context.TenantSettings.AddRange(new List<TenantSettings>() {
                 new TenantSettings("DemoOptions:Tenant:Enabled", "false",1){Id=1},
                 new TenantSettings("DemoOptions:Tenant:AutoRetryDelay", "17:58:00",1){Id=2},
                 new TenantSettings("DemoOptions:Tenant:IdentityOptions:MaxUserNameLength", "5",1){Id=3},

                 new TenantSettings("DemoOptions:Tenant:Enabled", "true",2){Id=4},
                 new TenantSettings("DemoOptions:Tenant:AutoRetryDelay", "12:58:00",2){Id=5},
                 new TenantSettings("DemoOptions:Tenant:IdentityOptions:MaxUserNameLength", "8",2){Id=6},
            });

            context.SaveChanges();
        }

        public static IConfiguration LoadGlobalConfiguration(DemoDbContext context) {
            var builder = new ConfigurationBuilder();
            builder.Sources.Clear();

            var settings = context.DemoSettings.Select(s => new { s.Key, s.Value }).ToDictionary(s => s.Key, s => s.Value);
            builder.AddInMemoryCollection(settings);

            return builder.Build();
        }
    }

    public class DemoOptionsManager : IOptionsManager<DemoOptions> {
        private readonly DemoDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IOptions<DemoOptions> _options;

        public DemoOptions Value { get; private set; }

        public DemoOptionsManager(DemoDbContext context, ITenantService tenantService, IOptions<DemoOptions> options) {
            _context = context;
            _tenantService = tenantService;
            _options = options;
            SetTenantSettings();
            Value = Get();
        }

        private void SetTenantSettings() {
            if (_tenantService != null) {
                var settings = _context.TenantSettings.Where(w => w.TenantId == _tenantService.Id).Select(s => new { s.Key, s.Value }).ToDictionary(s => s.Key, s => s.Value);
                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(settings);

                IConfigurationRoot configurationRoot = builder.Build();
                DemoOptions tenant = new();
                configurationRoot.GetSection($"{nameof(DemoOptions)}:{DemoOptions.Tenant}").Bind(tenant);

                Value = tenant;
            }
        }


        public DemoOptions Get() {
            if (_tenantService != null) {
                return Value;
            }
            else {
                return _options.Value;
            }
        }
    }

    public interface IOptionsManager<TOptions> {
        public TOptions Value { get; }
    }
}
