using System.Xml.Serialization;

namespace Medical.DataCore.Interface
{
    public interface IAccount
    {
        decimal? Price { get; set; }
        decimal? AcceptPrice { get; set; }
        int? Year { get; set; }
        int? Month { get; set; }
    }
}