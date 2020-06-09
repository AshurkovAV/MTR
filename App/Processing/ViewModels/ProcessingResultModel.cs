using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Processing.ViewModels
{
    public class ProcessingResultModel : ViewModelBase
    {
        public int? Id { get; set; }
        public int? Affected { get; set; }
        public string Name { get; set; }

    }
}
