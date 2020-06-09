namespace Core.Infrastructure
{

    public interface IIdFactory
    {
        int CurrentId { get; }
        int NextId(string category = null);
    }
}