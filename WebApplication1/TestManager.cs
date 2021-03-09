using System.Threading.Tasks;

namespace WebApplication1 {
    public class TestManager<TUser> : ITestManager<TUser> where TUser : class {
        public string GetId(TUser user) {
            return ConvertIdToString(user);
        }

        public string ConvertIdToString(TUser id) {
            if (object.Equals(id, default(TUser))) {
                return null;
            }
            return id.ToString();
        }

        public Task<bool> TestMethod() {
            return Task.FromResult(true);
        }
    }
}