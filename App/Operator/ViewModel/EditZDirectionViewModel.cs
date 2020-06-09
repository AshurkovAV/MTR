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
    public class EditZDirectionViewModel : ClassifierBaseVm<ZFactDirection>
    {
        public EditZDirectionViewModel(ZFactDirection classifier)
            : base(classifier)
        {
        }

        public EditZDirectionViewModel()
        {

        }

        [Display(Name = @"ID Направления")]
        [ReadOnly(true)]
        public int ZDirectionId
        {
            get { return Classifier.ZDirectionId; }
            set { Classifier.ZDirectionId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int ZmedicalEventId
        {
            get { return Classifier.ZmedicalEventId; }
            set { Classifier.ZmedicalEventId = value; }
        }

        [Display(Name = @"Дата направления")]
        public DateTime? DirectionDate
        {
            get { return _classifier.DirectionDate; }
            set { _classifier.DirectionDate = value; }
        }

        [Display(Name = @"Вид направления")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(DirectionViewItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? DirectionViewId
        {
            get { return _classifier.DirectionViewId; }
            set { _classifier.DirectionViewId = value; }
        }

        [Display(Name = @"Метод диагностического исследования")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(MetIsslItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? MetIsslId
        {
            get { return _classifier.MetIsslId; }
            set { _classifier.MetIsslId = value; }
        }

        [Display(Name = @"Медицинская услуга (код), указанная в направлении")]
        public string DirectionService
        {
            get { return _classifier.DirectionService; }
            set { _classifier.DirectionService = value; }
        }
    }
}
