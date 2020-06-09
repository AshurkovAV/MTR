using System;

namespace Medical.DataCore.Interface
{
    public interface IHeader
    {
        int? Version { get; set; }
        DateTime? Date { get; set; }
        string SourceOkato { get; set; }
        string TargetOkato { get; set; }
    }
}