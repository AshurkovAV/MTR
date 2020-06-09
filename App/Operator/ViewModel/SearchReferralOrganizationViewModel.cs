using System;
using System.Windows.Input;
using Core.DataStructure;
using Core.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SearchReferralOrganizationViewModel : ViewModelBase
    {
        private readonly ICacheRepository _cache;

        private readonly int? _id;

        private int? _selectedId;
        private CommonTuple _selected;
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

        public SearchReferralOrganizationViewModel(ICacheRepository cache, int? code)
        {
            _cache = cache;

            //_id = _cache.Get(CacheRepository.F003Cache).GetBack(code);
            _id = code;
            
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
                OkCallback(SelectedId);
                //OkCallback(_cache.Get(CacheRepository.F003Cache).Get(SelectedId) as int?);
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
