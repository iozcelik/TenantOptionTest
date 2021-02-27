using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickTestDI {
    public class EntityConfigurationProvider : ConfigurationProvider {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public EntityConfigurationProvider(
            Action<DbContextOptionsBuilder> optionsAction) =>
            _optionsAction = optionsAction;

        public override void Load() {
            var builder = new DbContextOptionsBuilder<EntityConfigurationContext>();

            _optionsAction(builder);

            using var dbContext = new EntityConfigurationContext(builder.Options);

            dbContext.Database.EnsureCreated();

            Data = dbContext.DemoSettings.Any()
                ? dbContext.DemoSettings.ToDictionary(c => c.Key, c => c.Value)
                : CreateAndSaveDefaultValues(dbContext);
        }

        static IDictionary<string, string> CreateAndSaveDefaultValues(
            EntityConfigurationContext context) {
            var settings = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase) {
                ["DemoOptions:Global:Enabled"] = "true",
                ["DemoOptions:Global:AutoRetryDelay"] = "12:58:00",
                ["DemoOptions:Global:IdentityOptions:MaxUserNameLength"] = "5"
            };

            context.DemoSettings.AddRange(
                settings.Select(kvp => new DemoSettings(kvp.Key, kvp.Value))
                        .ToArray());

            var tenant1settings = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase) {
                ["DemoOptions:Tenant:Enabled"] = "true",
                ["DemoOptions:Tenant:AutoRetryDelay"] = "12:58:00",
                ["DemoOptions:Tenant:IdentityOptions:MaxUserNameLength"] = "5"
            };


            context.TenantSettings.AddRange(
                tenant1settings.Select(kvp => new TenantSettings(kvp.Key, kvp.Value, 1))
                        .ToArray());

            var tenant2settings = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase) {
                ["DemoOptions:Tenant:Enabled"] = "true",
                ["DemoOptions:Tenant:AutoRetryDelay"] = "12:58:00",
                ["DemoOptions:Tenant:IdentityOptions:MaxUserNameLength"] = "5"
            };


            context.TenantSettings.AddRange(
                tenant2settings.Select(kvp => new TenantSettings(kvp.Key, kvp.Value, 2))
                        .ToArray());

            context.SaveChanges();

            return settings;
        }
    }

   
}
