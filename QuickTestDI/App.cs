using Microsoft.Extensions.Options;
using System;

namespace QuickTestDI {
    public class App {
        private readonly IOptionsSnapshot<DemoOptions> _options;

        public App(IOptionsSnapshot<DemoOptions> options) {
            _options = options;
        }

        public void Run() {
            Console.WriteLine("Hello from App.cs");
            Console.WriteLine($"DemoOptions:Global:Enabled={_options.Value.Enabled}");
            Console.WriteLine($"DemoOptions:Global:AutoRetryDelay={_options.Value.AutoRetryDelay}");
            Console.WriteLine($"DemoOptions:Global:IdentityOptions:MaxUserNameLength={_options.Value.IdentityOptions.MaxUserNameLength}");
        }
    }
}
