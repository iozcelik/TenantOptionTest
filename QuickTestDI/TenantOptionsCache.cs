using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QuickTestDI {
    public class TenantOptionsCache<TOptions> : IOptionsMonitorCache<TOptions> where TOptions : class {

        private readonly ITenantService _tenantAccessor;
        private readonly TenantOptionsCacheDictionary<TOptions> _tenantSpecificOptionsCache =
            new TenantOptionsCacheDictionary<TOptions>();

        public TenantOptionsCache(ITenantService tenantAccessor) {
            _tenantAccessor = tenantAccessor;
        }

        public void Clear() {
            _tenantSpecificOptionsCache.Get(_tenantAccessor.Id).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions) {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Id)
                .GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options) {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Id)
                .TryAdd(name, options);
        }

        public bool TryRemove(string name) {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Id).TryRemove(name);
        }
    }

    public class TenantOptionsCacheDictionary<TOptions> where TOptions : class {
        /// <summary>
        /// Caches stored in memory
        /// </summary>
        private readonly ConcurrentDictionary<int, IOptionsMonitorCache<TOptions>> _tenantSpecificOptionCaches =
            new ConcurrentDictionary<int, IOptionsMonitorCache<TOptions>>();

        /// <summary>
        /// Get options for specific tenant (create if not exists)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IOptionsMonitorCache<TOptions> Get(int tenantId) {
            return _tenantSpecificOptionCaches.GetOrAdd(tenantId, new OptionsCache<TOptions>());
        }
    }

    internal class TenantOptionsFactory<TOptions> : IOptionsFactory<TOptions>
    where TOptions : class, new() {

        private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
        private readonly Action<TOptions, int> _tenantConfig;
        private readonly ITenantService _tenantAccessor;

        public TenantOptionsFactory(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, Action<TOptions, int> tenantConfig, ITenantService tenantAccessor) {
            _setups = setups;
            _postConfigures = postConfigures;
            _tenantAccessor = tenantAccessor;
            _tenantConfig = tenantConfig;
        }

        /// <summary>
        /// Create a new options instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TOptions Create(string name) {
            var options = new TOptions();

            //Apply options setup configuration
            foreach (var setup in _setups) {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup) {
                    namedSetup.Configure(name, options);
                }
                else {
                    setup.Configure(options);
                }
            }

            //Apply tenant specifc configuration (to both named and non-named options)
            if (_tenantAccessor != null)
                _tenantConfig(options, _tenantAccessor.Id);

            //Apply post configuration
            foreach (var postConfig in _postConfigures) {
                postConfig.PostConfigure(name, options);
            }

            return options;
        }
    }

    public class TenantOptions<TOptions> :
    IOptions<TOptions>, IOptionsSnapshot<TOptions> where TOptions : class, new() {
        private readonly IOptionsFactory<TOptions> _factory;
        private readonly IOptionsMonitorCache<TOptions> _cache;

        public TenantOptions(IOptionsFactory<TOptions> factory, IOptionsMonitorCache<TOptions> cache) {
            _factory = factory;
            _cache = cache;
        }

        public TOptions Value => Get(Options.DefaultName);

        public TOptions Get(string name) {
            return _cache.GetOrAdd(name, () => _factory.Create(name));
        }
    }
}
