using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditRefusalPenalty : ClassifierBaseVm<globalRefusalPenalty>
    {
        public EditRefusalPenalty(globalRefusalPenalty classifier)
            : base(classifier)
        {
            
        }

        public EditRefusalPenalty()
        {

        }

        [Display(AutoGenerateField = false)]
        public int RefusalPenaltyId
        {
            get { return Classifier.RefusalPenaltyId; }
            set { Classifier.RefusalPenaltyId = value; }
        }

        [Display(Name = "Код причины отказа")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F014OsnItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string Reason
        {
            get { return Classifier.Reason; }
            set { Classifier.Reason = value; }
        }

        [NumericMask(Mask = PredefinedMasks.Numeric.Percent)]
        [Display(Name = "Сумма отказа в %")]
        public int Percent
        {
            get { return Classifier.Percent; }
            set { Classifier.Percent = value; }
        }

        [Display(Name = "Сумма штрафа"), DataType(DataType.Currency)]
        public decimal Penalty
        {
            get { return Classifier.Penalty; }
            set { Classifier.Penalty = value; }
        }

        [Display(Name = "Сумма уменьшения"), DataType(DataType.Currency)]
        public decimal Decrease
        {
            get { return Classifier.Decrease; }
            set { Classifier.Decrease = value; }
        }

        [Display(Name = "Комментарий"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return Classifier.Comments; }
            set { Classifier.Comments = value; }
        } 

    }
}
