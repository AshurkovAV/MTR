using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Core.Extensions;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;
using Core;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class EditZContraindicationskViewModel : ClassifierBaseVm<ZFactContraindications>
    {
        public EditZContraindicationskViewModel(ZFactContraindications classifier)
            : base(classifier)
        {
        }

        public EditZContraindicationskViewModel()
        {

        }

        [Display(Name = @"ID")]
        [ReadOnly(true)]
        public int ZContraindicationsId
        {
            get { return Classifier.ZContraindicationsId; }
            set { Classifier.ZContraindicationsId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int ZMedicalEventOnkId
        {
            get { return Classifier.ZMedicalEventOnkId; }
            set { Classifier.ZMedicalEventOnkId = value; }
        }

        [Display(Name = @"Код противопоказания или отказа")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N001CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Prot
        {
            get { return _classifier.Prot; }
            set { _classifier.Prot = value; }
        }

        [Display(Name = @"Дата регистрации противопоказания или отказа")]
        public DateTime? Dprot
        {
            get { return _classifier.Dprot; }
            set { _classifier.Dprot = value; }
        }
    }
}
