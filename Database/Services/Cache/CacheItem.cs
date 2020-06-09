namespace Medical.DatabaseCore.Services.Cache
{
    public class CacheItem<T>
    {   
        public int? Id { get; set; }
        public T Data { get; set; }
    }
}
