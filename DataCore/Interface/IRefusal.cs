using System;

namespace Medical.DataCore.Interface
{
    public interface IRefusal
    {
        int? RefusalSource { get; set; }
        int? RefusalCode { get; set; }
        decimal? RefusalRate { get; set; }
        int? RefusalType { get; set; }
        string Comments { get; set; }
        string ExternalGuid { get; set; }
        string SlidGuid { get; set; }
        DateTime? Date { get; set; }
        string NumAct { get; set; }
        string CodeExp { get; set; }
    }
}