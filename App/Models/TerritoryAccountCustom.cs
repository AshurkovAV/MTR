using DataModel;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Models
{
    public class TerritoryAccountCustom : ViewModelBase 
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public TerritoryAccountView View { get; set; }
    }
}