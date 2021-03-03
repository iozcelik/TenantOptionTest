using Microsoft.EntityFrameworkCore;

namespace QuickTestDI {
    public class DemoDbContext : DbContext {
        public DbSet<DemoSettings> DemoSettings { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }

        public DemoDbContext(DbContextOptions options)
            : base(options) {
        }
    }

    public record DemoSettings(string Key, string Value) {
        public int Id { get; set; }
    }

    public record TenantSettings(string Key, string Value, int TenantId) {
        public int Id { get; set; }
    }
}
