namespace Core.Infrastructure
{
    public interface IOperationResult<T>
    {
        int Id { get; set; }
        T Data { get; set; }
    }

    public interface IOperationResult<T1, T2>
    {
        int Id { get; set; }
        T1 Data1 { get; set; }
        T2 Data2 { get; set; }
    }

    public interface IOperationResult
    {
        int Id { get; set; }
    }
}