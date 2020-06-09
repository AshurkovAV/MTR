using System;

namespace Medical.DataCore.Interface
{
    public interface IHeaderAnswer
    {
        int? Version { get; set; }
        DateTime? Date { get; set; }
        string TargetOkato { get; set; }
    }
}