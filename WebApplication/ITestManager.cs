namespace WebApplication {
    public interface ITestManager<T> {
        long GetId(ITest<long> test);
      
    }

    public class TestManager<T> : ITestManager<T> {
        public long GetId(ITest<long> test) {
            return test.Key;
            //return ConvertIdToString(test.Key);
        }

        public string ConvertIdToString(T id) {
            if (object.Equals(id, default(T))) {
                return null;
            }
            return id.ToString();
        }
    }

    
}