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
    public class EditZSlKoefViewModel : ClassifierBaseVm<ZFactSlKoef>
    {
        public EditZSlKoefViewModel(ZFactSlKoef classifier)
            : base(classifier)
        {
        }

        public EditZSlKoefViewModel()
        {

        }

        [Display(Name = @"ID Коэффициента")]
        [ReadOnly(true)]
        public int ZslKoefId
        {
            get { return Classifier.ZslKoefId; }
            set { Classifier.ZslKoefId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public int? ZksgKpgId
        {
            get { return Classifier.ZksgKpgId; }
            set { Classifier.ZksgKpgId = value; }
        }

        [Display(Name = @"Номер коэффициента сложности лечения пациента"), DataType(DataType.Currency)]
        public int? NumberDifficultyTreatment
        {
            get { return _classifier.NumberDifficultyTreatment; }
            set { _classifier.NumberDifficultyTreatment = value; }
        }

        [Display(Name = @"Значение коэффициента сложности лечения пациента")]
        public decimal? ValueDifficultyTreatment
        {
            get { return _classifier.ValueDifficultyTreatment; }
            set { _classifier.ValueDifficultyTreatment = value; }
        }
    }
}
