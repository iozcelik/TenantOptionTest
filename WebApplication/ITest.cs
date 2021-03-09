namespace WebApplication {
    public interface ITest<TKey> {
        TKey Key { get; }
    }

    public class Test : ITest<long> {
        public long Key { get; set; }
    }
}