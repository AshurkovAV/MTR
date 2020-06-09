using System;
using System.Windows.Input;
using Core.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.CoreLayer.Models.Common;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SearchDiagnosisViewModel : ViewModelBase
    {
        private readonly int? _id;

        private int? _selectedDiagnosisId;
        private CommonItem _selectedDiagnosis;
        private Action<int?> _okCallback;
        private Action _closeCallback;

        private RelayCommand _applyCommand;
        private RelayCommand _closeCommand;
        
        public Action<int?> OkCallback
        {
            get { return _okCallback; }
            set
            {
                _okCallback = value; 
                RaisePropertyChanged(() => OkCallback);
                RaisePropertyChanged(() => IsApplyable);
            }
        }

        public Action CloseCallback
        {
            get { return _closeCallback; }
            set { _closeCallback = value; RaisePropertyChanged(() => CloseCallback); }
        }

        public SearchDiagnosisViewModel(int? id = null)
        {
            _id = id;

            Init();
        }

        private void Init()
        {
            _selectedDiagnosisId = _id;
        }

        public bool IsApplyable
        {
            get
            {
                return OkCallback.IsNotNull();
            }
        }

        public int? SelectedDiagnosisId
        {
            get { return _selectedDiagnosisId; }
            set
            {
                _selectedDiagnosisId = value;
                RaisePropertyChanged(() => SelectedDiagnosisId);
            }
        }

        public CommonItem SelectedDiagnosis
        {
            get { return _selectedDiagnosis; }
            set
            {
                _selectedDiagnosis = value;
                RaisePropertyChanged(() => SelectedDiagnosis);
                RaisePropertyChanged(() => IsPayableText);
                RaisePropertyChanged(() => SelectedDiagnosisText);
            }
        }

        public string IsPayableText
        {
            get
            {
                if (SelectedDiagnosis.IsNotNull())
                {
                    return Convert.ToBoolean(SelectedDiagnosis.DataField) ? "Оплачиваемый" : "Неоплачиваемый";
                }
                return "Статус не установлен";
            }
        }

        public string SelectedDiagnosisText
        {
            get
            {
                if (SelectedDiagnosis.IsNotNull())
                {
                    return SelectedDiagnosis.DisplayField;
                }
                return "Диагноз не выбран";
            }
        }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply, CanApply)); }
        }

        private bool CanApply()
        {
            return SelectedDiagnosisId.HasValue;
        }

        private void Apply()
        {
            if (OkCallback.IsNotNull())
            {
                OkCallback(SelectedDiagnosisId);
            }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(Close,CanClose)); }
        }

        private bool CanClose()
        {
            return CloseCallback.IsNotNull();
        }

        private void Close()
        {
            if (CloseCallback.IsNotNull())
            {
                CloseCallback();
            }
        }

        
    }
}
