using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace QuickTestDI {
    public class App {
        private readonly IOptionsManager<DemoOptions> _options;
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;

        public App(IOptionsManager<DemoOptions> options, IConfiguration configuration, ITenantService tenantService) {
            _options = options;
            _configuration = configuration;
            _tenantService = tenantService;
        }

        public void Run() {
            Console.WriteLine($"Current Thread:{Thread.CurrentThread.Name} Current Tenant:{_tenantService.Id}");
            Console.WriteLine($"Enabled={_options.Value.Enabled}");
            Console.WriteLine($"AutoRetryDelay={_options.Value.AutoRetryDelay}");
            Console.WriteLine($"IdentityOptions:MaxUserNameLength={_options.Value.IdentityOptions.MaxUserNameLength}");
        }
    }
}
