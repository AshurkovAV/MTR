using System;
using System.ComponentModel;
using System.Windows.Input;
using Core.Extensions;
using Core.Infrastructure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Medical.CoreLayer.PropertyGrid
{
    public class PropertyGridViewModel<T> : ViewModelBase, ICallbackable where T : IDataErrorInfo, new()
    {
        private bool _isCreate;
        public bool? DialogResult { get; set; }
        public T SelectedObject { get; set; }
        public string Title { get; set; }
        public bool IsCreate {
            get { return _isCreate; }
            set { _isCreate = value; RaisePropertyChanged(()=>IsCreate); }
        }

        public bool IsCancelVisible
        { 
            get { return CancelCallback.IsNotNull(); }
        }
        
        public Action OkCallback { get; set; }
        public Action CancelCallback { get; set; }
        public Action CreateCallback { get; set; }

        private RelayCommand _okCommand;
        private RelayCommand _cancelCommand;
        private RelayCommand _createCommand;

        public PropertyGridViewModel()
        {
            SelectedObject = new T();
            IsCreate = true;
            Title = "Создание/редактирование данных";
        }

        public PropertyGridViewModel(T obj,bool isCreate = false)
        {
            SelectedObject = obj;
            IsCreate = isCreate;
            Title = "Создание/редактирование данных";
        }

        public ICommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand(Ok, CanOk)); }
        }

        private void Ok()
        {
            if (OkCallback != null)
            {
                OkCallback.Invoke();
            }
            DialogResult = true;
        }

        private bool CanOk()
        {
            return SelectedObject.Error.Length == 0;
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        private void Cancel()
        {
            if (CancelCallback != null)
            {
                CancelCallback.Invoke();
            }
            DialogResult = true;
        }

        public ICommand CreateCommand
        {
            get { return _createCommand ?? (_createCommand = new RelayCommand(Create, CanCreate)); }
        }
        private void Create()
        {
            if (CreateCallback != null)
            {
                CreateCallback.Invoke();
                SelectedObject = new T();
            }
        }

        private bool CanCreate()
        {
            return SelectedObject.Error.Length == 0;
        }
    }
}
