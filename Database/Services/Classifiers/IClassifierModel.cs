namespace Medical.DatabaseCore.Services.Classifiers
{
    public interface IClassifierModel<T>
    {
        int Id { get; set; }
        T Classifier { get; set; }
    }
}