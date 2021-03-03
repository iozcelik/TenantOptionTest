using Microsoft.AspNetCore.Http;

namespace WebApplication {
    public class TenantService : ITenantService {
        private IHttpContextAccessor _httpContextAccessor;
        public TenantService(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? Id => int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value, out int result) ? result : (int?)null;
    }

    public interface ITenantService {
        public int? Id { get; }
    }
}
