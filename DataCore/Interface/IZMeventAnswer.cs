
using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZMeventAnswer
    {
        List<IRefusal> InnerRefusals { get; set; }
        //List<object> RefusalsXml { get; set; }
    }
}