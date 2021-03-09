using System.Threading.Tasks;

namespace WebApplication1 {
    public interface ITestManager<TUser> where TUser : class {
        string GetId(TUser user);
        Task<bool> TestMethod();
    }
}