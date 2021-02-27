namespace QuickTestDI {
    public record TenantSettings(string Key, string Value, int TenantId) {
        public int Id { get; set; }
    }
}
