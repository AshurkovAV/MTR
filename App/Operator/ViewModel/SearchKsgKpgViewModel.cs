using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BLToolkit.Mapping;
using Core.Extensions;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Models.EditableModels;
using Medical.CoreLayer.Models.Common;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SearchKsgKpgViewModel : ViewModelBase
    {
        private int? _ksgKpgId;
        public ZKsgKpgModel KsgKpg { get; set; }
        public ZSlCritModel Crit { get; set; }
        private CommonItem _ksgKpg;
        private Action<ZKsgKpgModel> _okCallback;
        private Action _closeCallback;

        private RelayCommand _applyCommand;
        private RelayCommand _closeCommand;
        private RelayCommand _addSlkoefCommand;
        private RelayCommand _addSlCritCommand;
        private RelayCommand _deleteSlkoefCommand;
        private RelayCommand _deleteCritCommand;
        public ZSlKoefContainer Slkoef { get; set; }
        public ZSlCritContainer Slcrit { get; set; }
        public Action<ZKsgKpgModel> OkCallback
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

        public SearchKsgKpgViewModel(ZKsgKpgModel ksgKpg, ZSlKoefContainer slkoef, ZSlCritContainer crit)
        {
            KsgKpg = ksgKpg;
            Slkoef = slkoef;
            Slcrit = crit;
            
             Init();
            // RaisePropertyChanged(() => KsgKpg);
        }

        public void Init()
        {
            Slkoef?.ReloadSlKoef();
            Slcrit?.ReloadSlCrit();
        }

        public bool IsApplyable
        {
            get
            {
                return OkCallback.IsNotNull();
            }
        }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply)); }
        }

        public ICommand DeleteSlKoefCommand
        {
            get
            {
                return _deleteSlkoefCommand ?? (_deleteSlkoefCommand = new RelayCommand(DeleteSlKoef));
            }
        }

        private void DeleteSlKoef()
        {
            Slkoef?.Delete();
        }

        public ICommand DeleteSlCritCommand
        {
            get
            {
                return _deleteCritCommand ?? (_deleteCritCommand = new RelayCommand(DeleteCrit));
            }
        }

        private void DeleteCrit()
        {
            Slcrit?.Delete();
        }

        public ICommand AddSlKoefCommand
        {
            get
            {
                return _addSlkoefCommand ?? (_addSlkoefCommand = new RelayCommand(AddSlKoef));
            }
        }

        private void AddSlCrit()
        {
            ZFactCrit sl = new ZFactCrit();
            sl.ZksgKpgId = KsgKpg.ZksgKpgId;
            var critModel = Map.ObjectToObject<ZSlCritModel>(sl, sl);
            critModel.Crit = sl;
            critModel.ZksgKpgId = KsgKpg.ZksgKpgId;
            Slcrit?.MedicalCrit.Add(critModel);
            RaisePropertyChanged(() => Slcrit);
        }

        private void AddSlKoef()
        {
            ZFactSlKoef sl = new ZFactSlKoef();
            sl.ZksgKpgId = KsgKpg?.ZksgKpgId;
            var slkoefModel = Map.ObjectToObject<ZSlkoefModel>(sl, sl);
            slkoefModel.Slkoef = sl;
            slkoefModel.ZksgKpgId = KsgKpg?.ZksgKpgId;
            Slkoef?.MedicalSlkoef.Add(slkoefModel);
            RaisePropertyChanged(() => Slkoef);
        }

        public ICommand AddSlCritCommand
        {
            get
            {
                return _addSlCritCommand ?? (_addSlCritCommand = new RelayCommand(AddSlCrit));
            }
        }

        private void Apply()
        {
            if (OkCallback.IsNotNull())
            {
                OkCallback(KsgKpg);
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
