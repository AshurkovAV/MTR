
using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IMEventAnswer
    {
        List<object> InnerRefusals { get; set; }
        decimal? RefusalPrice { get; set; }
        decimal? Price { get; set; }
    }
}