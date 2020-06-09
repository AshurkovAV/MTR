using System.Collections;
using DevExpress.Xpf.Grid;
using Medical.AppLayer.Models.PolicySearch;

namespace Medical.AppLayer.Processing.Views
{
    public class ChildNodesSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item is PolicyCheckMedicalEventModel)
                return null;
            else if (item is PolicyCheckPatientModel)
                return (item as PolicyCheckPatientModel).Mevents;
            return null;
        }
    }
}
