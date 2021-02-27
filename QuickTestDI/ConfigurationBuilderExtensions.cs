using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace QuickTestDI {
    public static class ConfigurationBuilderExtensions {
        public static IConfigurationBuilder AddEntityConfiguration(
            this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction) =>
            builder.Add(new EntityConfigurationSource(optionsAction));
    }
}
