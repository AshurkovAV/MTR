namespace Medical.DatabaseCore.Services.Cache
{
    public interface ICache
    {
        void Put(int? key, object value);
        object Get(int? key);
        string GetString(int? key);
        void PutBack(object key, int? value);
        int? GetBack(object key);
        int GetBackInt(object key);
        void Info();
        bool KeyExists(int? key);
        bool ValueExists(object key);
    }
}
