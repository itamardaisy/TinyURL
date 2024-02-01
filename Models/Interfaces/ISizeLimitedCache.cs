namespace TinyUrl.Models.Interfaces
{
    public interface ISizeLimitedCache<TKey, TValue>
    {
        public bool Add(TKey key, TValue value);
        public TValue Get(TKey key, TValue value);
    }
}
