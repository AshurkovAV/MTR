using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Report.ViewModels
{
    [DisplayName(@"Редактирование готового отчета")]
    public class EditPreparedReportViewModel : ClassifierBaseVm<FactPreparedReport>
    {
        public EditPreparedReportViewModel(FactPreparedReport classifier)
            : base(classifier)
        {
            
        }

        public EditPreparedReportViewModel()
        {

        }

        [Display(GroupName = "Общие параметры", Name = "Дата отчета"), DataType(DataType.Date)]
        public DateTime Date
        {
            get { return Classifier.Date; }
            set { Classifier.Date = value; RaisePropertyChanged(() => Date); RaisePropertyChanged(() => Error); }
        }

        [Display(GroupName = "Общие параметры", Name = "Порядковый номер отчета")]
        public int Number
        {
            get { return Classifier.Number; }
            set { Classifier.Number = value; RaisePropertyChanged(() => Number); RaisePropertyChanged(() => Error); }
        }

        [Display(GroupName = "Общие параметры", Name = "Комментарии"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return Classifier.Comments; }
            set { Classifier.Comments = value; RaisePropertyChanged(() => Comments); RaisePropertyChanged(() => Error); }
        }
        
    }
}
