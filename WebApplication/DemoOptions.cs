using System;

namespace WebApplication {
    public record DemoOptions {
        public const string Global = nameof(Global);
        public const string Tenant = nameof(Tenant);

        public bool Enabled { get; set; }
        public TimeSpan AutoRetryDelay { get; set; }
        public IdentityOptions IdentityOptions { get; set; }
    }

    public record IdentityOptions {
        public int MaxUserNameLength { get; set; }
    }
}
