using System;
using System.Text;
using System.Windows.Input;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using DevExpress.Xpf.RichEdit;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;

namespace Medical.AppLayer.Register.ViewModels
{
    public class TerritoryAccountSummaryViewModel : ViewModelBase
    {
        private readonly IComplexReportRepository _repository;
        private readonly int _version;

        private RelayCommand<object> _saveLogCommand;

        private string _notes;

        public TerritoryAccountSummaryViewModel(IComplexReportRepository repository,
            int accountId,
            int version)
        {
            _repository = repository;
            _version = version;
            
            AccountId = accountId;

            Init();
        }

        private void Init()
        {
            
            DoStats();
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(() => Notes);
            }
        }

        public int AccountId { get; set; }

        public ICommand SaveLogCommand
        {
            get { return _saveLogCommand ?? (_saveLogCommand = new RelayCommand<object>(SaveLog, CanSaveLog)); }
        }

        private bool CanSaveLog(object control)
        {
            return Notes.IsNotNullOrWhiteSpace();
        }

        private void SaveLog(object control)
        {
            var richEditControl = control as RichEditControl;
            if (richEditControl != null)
            {
                richEditControl.SaveDocumentAs();
            }
        }

        private void DoStats()
        {
            try
            {
                OperationResult<string> result = _repository.TerritoryAccountSummary(AccountId, _version);
                if (result.Success)
                {
                    Notes = result.Data;
                }
                else
                {
                    Notes = result.LastError.ToString();
                }

            }
            catch (Exception exception)
            {
                Notes = "При обработке данных произошло исключение\n{0}".F(exception.Message);
            }
        }
    }
}
