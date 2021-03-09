using System;

namespace WebApplication1 {
    internal class TestEntity : BaseEntity<long> {
    }

    internal class BaseEntity<TKey> where TKey : IEquatable<TKey> {
        public TKey Id { get; set; }
    }
}