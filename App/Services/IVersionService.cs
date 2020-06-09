using Medical.AppLayer.Models;

namespace Medical.AppLayer.Services
{
    public interface IVersionService
    {
        int? VersionId { get; }
        VersionInfoModel VersionInfo { get; }
        bool Version();
    }
}