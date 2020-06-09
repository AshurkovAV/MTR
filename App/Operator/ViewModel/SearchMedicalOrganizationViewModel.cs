using System;
using System.Windows.Input;
using Core.DataStructure;
using Core.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SearchMedicalOrganizationViewModel : ViewModelBase
    {
        private readonly ICacheRepository _cache;

        private readonly int? _id;

        private int? _selectedId;
        private CommonTuple _selected;
        private Action<string> _okCallback;
        private Action _closeCallback;

        private RelayCommand _applyCommand;
        private RelayCommand _closeCommand;
        

        public Action<string> OkCallback
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

        public SearchMedicalOrganizationViewModel(ICacheRepository cache, string code)
        {
            _cache = cache;

            _id = _cache.Get(CacheRepository.F003Cache).GetBack(code);

            Init();
        }

        private void Init()
        {
            _selectedId = _id;
        }

        public bool IsApplyable
        {
            get
            {
                return OkCallback.IsNotNull();
            }
        }

        public int? SelectedId
        {
            get { return _selectedId; }
            set
            {
                _selectedId = value;
                RaisePropertyChanged(() => SelectedId);
            }
        }

        public CommonTuple Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged(() => Selected);
                RaisePropertyChanged(() => SelectedText);
            }
        }

        public string SelectedText
        {
            get
            {
                if (Selected.IsNotNull())
                {
                    return Selected.DisplayField;
                }
                return "Не выбрано";
            }
        }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply, CanApply)); }
        }

        private bool CanApply()
        {
            return SelectedId.HasValue;
        }

        private void Apply()
        {
            if (OkCallback.IsNotNull())
            {
                OkCallback(_cache.Get(CacheRepository.F003Cache).Get(SelectedId) as string);
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
