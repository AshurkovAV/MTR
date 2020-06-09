using Medical.DataCore.v30.EAnswer;
using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZslMeventAnswer
    {
        List<IZMeventAnswer> InnerEventCollection { get; set; }
        List<IRefusal> InnerRefusals { get; set; }
        decimal? RefusalPrice { get; set; }
        decimal? Price { get; set; }
    }
}