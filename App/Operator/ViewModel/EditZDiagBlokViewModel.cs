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
    public class EditZDiagBlokViewModel : ClassifierBaseVm<ZFactDiagBlok>
    {
        public EditZDiagBlokViewModel(ZFactDiagBlok classifier)
            : base(classifier)
        {
        }

        public EditZDiagBlokViewModel()
        {

        }

        [Display(Name = @"ID Диагностического блока")]
        [ReadOnly(true)]
        public int ZDiagBlokId
        {
            get { return Classifier.ZDiagBlokId; }
            set { Classifier.ZDiagBlokId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int ZMedicalEventOnkId
        {
            get { return Classifier.ZMedicalEventOnkId; }
            set { Classifier.ZMedicalEventOnkId = value; }
        }

        [Display(Name = @"Дата взятия материала")]
        public DateTime? DiagDate
        {
            get { return _classifier.DiagDate; }
            set { _classifier.DiagDate = value; }
        }

        [Display(Name = @"Тип диагностического показателя")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(DiagTypeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DiagTip
        {
            get { return _classifier.DiagTip; }
            set { _classifier.DiagTip = value; }
        }

        [Display(Name = @"Код диагностического показателя")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N007CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DiagCode7
        {
            get { return _classifier.DiagCode7; }
            set { _classifier.DiagCode7 = value; }
        }

        [Display(Name = @"Код диагностического показателя2")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N010CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DiagCode10
        {
            get { return _classifier.DiagCode10; }
            set { _classifier.DiagCode10 = value; }
        }

        [Display(Name = @"Код результата диагностики")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N008CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DiagRslt8
        {
            get { return _classifier.DiagRslt8; }
            set { _classifier.DiagRslt8 = value; }
        }

        [Display(Name = @"Код результата диагностики2")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(N011CacheItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DiagRslt11
        {
            get { return _classifier.DiagRslt11; }
            set { _classifier.DiagRslt11 = value; }
        }
        [Display(Name = @"Признак получения результата диагностики")]
        [CustomEditor(EditorType = typeof(CheckBox))]
        public int? RecRslt
        {
            get { return _classifier.RecRslt; }
            set { _classifier.RecRslt = value; }
        }
    }
}
