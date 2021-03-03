using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace WebApplication {
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
            Value = GetValues();
        }

        private void SetTenantSettings() {
            if (_tenantService != null && _tenantService.Id != null) {
                var settings = _context.TenantSettings.Where(w => w.TenantId == _tenantService.Id).Select(s => new { s.Key, s.Value }).ToDictionary(s => s.Key, s => s.Value);
                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(settings);

                IConfigurationRoot configurationRoot = builder.Build();
                DemoOptions tenant = new();
                configurationRoot.GetSection($"{nameof(DemoOptions)}:{DemoOptions.Tenant}").Bind(tenant);

                Value = tenant;
            }
        }

        private DemoOptions GetValues() {
            if (_tenantService != null && _tenantService.Id != null) {
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
