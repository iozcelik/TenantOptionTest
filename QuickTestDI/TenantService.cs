using System.Threading;

namespace QuickTestDI {
    public class TenantService : ITenantService {
        public int Id => int.Parse(Thread.CurrentThread.Name);
    }
}
