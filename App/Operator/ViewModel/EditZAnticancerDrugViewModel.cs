using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using Core.Extensions;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;
using Core;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class EditZAnticancerDrugViewModel : ClassifierBaseVm<ZFactAnticancerDrug>
    {
        public EditZAnticancerDrugViewModel(ZFactAnticancerDrug classifier)
            : base(classifier)
        {
        }

        public EditZAnticancerDrugViewModel()
        {

        }

        [Display(Name = @"ID Диагностического блока")]
        [ReadOnly(true)]
        public int ZAnticancerDrugId
        {
            get { return Classifier.ZAnticancerDrugId; }
            set { Classifier.ZAnticancerDrugId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int ZMedicalServiceOnkId
        {
            get { return Classifier.ZMedicalServiceOnkId; }
            set { Classifier.ZMedicalServiceOnkId = value; }
        }

        [Display(Name = @"Лекарственный препарат")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N020CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string RegNum
        {
            get { return _classifier.RegNum; }
            set { _classifier.RegNum = value; }
        }

        [Display(Name = @"Код схемы лекарственной терапии")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V024CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string CodeSh
        {
            get { return _classifier.CodeSh; }
            set { _classifier.CodeSh = value; }
        }

        [Display(Name = @"Дата введения лекарственного препарата")]
        public DateTime? DataInj
        {
            get { return _classifier.DataInj; }
            set { _classifier.DataInj = value; }
        }
    }
}
