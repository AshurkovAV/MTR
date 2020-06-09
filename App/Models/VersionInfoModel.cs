using System.Linq;
using Core.Extensions;
using DataModel;

namespace Medical.AppLayer.Models
{
    public class VersionInfoModel
    {
        private Version _version;

        public Version Version
        {
            get { return _version; }
        }

        public int? VersionId
        {
            get { return _version != null ? _version.id : 0; }
        }

        internal void SetVersion(Version version)
        {
            _version = version;
        }
    }
}
