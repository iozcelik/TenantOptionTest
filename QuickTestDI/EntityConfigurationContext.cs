﻿using Microsoft.EntityFrameworkCore;

namespace QuickTestDI {
    public class EntityConfigurationContext : DbContext {
        public DbSet<DemoSettings> AnatolioSettings { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }

        public EntityConfigurationContext(DbContextOptions options)
            : base(options) {
        }
    }
}