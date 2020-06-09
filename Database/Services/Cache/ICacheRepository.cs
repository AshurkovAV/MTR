namespace Medical.DatabaseCore.Services.Cache
{
    public interface ICacheRepository
    {
        ICache Get(string name);
        bool Has(string name);
        void Put(string name, ICache cache);
    }
}
