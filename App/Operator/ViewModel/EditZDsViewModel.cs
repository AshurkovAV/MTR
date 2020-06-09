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
    public class EditZDsViewModel : ClassifierBaseVm<ZFactDs>
    {
        public EditZDsViewModel(ZFactDs classifier)
            : base(classifier)
        {
        }

        public EditZDsViewModel()
        {

        }

        [Display(Name = @"ID диагноза")]
        [ReadOnly(true)]
        public int ZFactDsId
        {
            get { return Classifier.ZFactDsId; }
            set { Classifier.ZFactDsId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int ZMedicalEventOnkId
        {
            get { return Classifier.ZmedicalEventId; }
            set { Classifier.ZmedicalEventId = value; }
        }

        [Display(Name = @"Код по МКБ")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(M001_CacheItemsSource), Value = "ValueFieldStr", DisplayName = "DisplayField")]
        public string Ds
        {
            get { return _classifier.Ds; }
            set { _classifier.Ds = value; }
        }

        [Display(Name = @"Тип диагноза")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(DsItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DsType
        {
            get { return _classifier.DsType; }
            set { _classifier.DsType = value; }
        }
    }
}
