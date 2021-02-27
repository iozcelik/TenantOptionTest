using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace QuickTestDI {
    //public class TenantOptionsFactory2<TOptions> : IOptionsFactory<TOptions> where TOptions : class, new() {
    //    private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
    //    private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
    //    private readonly ITenantService _tenantService;

    //    /// <summary>
    //    /// Initializes a new instance with the specified options configurations.
    //    /// </summary>
    //    /// <param name="setups">The configuration actions to run.</param>
    //    /// <param name="postConfigures">The initialization actions to run.</param>
    //    public TenantOptionsFactory(IEnumerable<IConfigureOptions<TOptions>> setups, IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, ITenantService tenantService) {
    //        _setups = setups;
    //        _postConfigures = postConfigures;
    //        _tenantService = tenantService;
    //    }

    //    public TOptions Create(string name) {
    //        var options = new TOptions();
    //        foreach (var setup in _setups) {
    //            if (setup is IConfigureNamedOptions<TOptions> namedSetup) {
    //                namedSetup.Configure(name, options);
    //            }
    //            else if (name == Options.DefaultName) {
    //                setup.Configure(options);
    //            }
    //        }





    //        foreach (var post in _postConfigures) {
    //            post.PostConfigure(name, options);
    //        }
    //        return options;
    //    }
    //}
}
