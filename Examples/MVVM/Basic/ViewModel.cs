using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Examples.MVVM.Basic
{
    public class ViewModel : ViewModelBase
    {
        private string _message;
        private string _messageParam;

        private RelayCommand _setMessageCommand;
        private RelayCommand<object> _setMessageParamCommand;
        private RelayCommand _addElementCommand;
        private RelayCommand<object> _removeElementCommand;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value; 
                RaisePropertyChanged(()=>Message);
            }
        }

        public string MessageParam
        {
            get { return _messageParam; }
            set
            {
                _messageParam = value; 
                RaisePropertyChanged(()=>MessageParam);
            }
        }

        public ObservableCollection<string> StringCollection { get; set; }

        public ICommand SetMessageCommand
        {
            get { return _setMessageCommand ?? (_setMessageCommand = new RelayCommand(SetMessage)); }
        }

        public ICommand SetMessageParamCommand
        {
            get { return _setMessageParamCommand ?? (_setMessageParamCommand = new RelayCommand<object>(SetMessageParam, CanSetMessageParam)); }
        }

        public ICommand AddElementCommand
        {
            get { return _addElementCommand ?? (_addElementCommand = new RelayCommand(AddElement)); }
        }

        public ICommand RemoveElementCommand
        {
            get { return _removeElementCommand ?? (_removeElementCommand = new RelayCommand<object>(RemoveElement, CanRemoveElement)); }
        }

        private bool CanSetMessageParam(object param)
        {
            return !string.IsNullOrWhiteSpace(MessageParam);
        }

        private void SetMessageParam(object param)
        {
            Message = param.ToString();
        }

        private void SetMessage()
        {
            Message = "Message from command";
        }

        private void AddElement()
        {
            StringCollection.Add(DateTime.Now.ToString());
        }

        private bool CanRemoveElement(object element)
        {
            return element != null;
        }

        private void RemoveElement(object element)
        {
            StringCollection.Remove((string)element);
        }

        public ViewModel()
        {
            Message = "TestMessage";
            StringCollection = new ObservableCollection<string>();
            StringCollection.Add("Record 1");
            StringCollection.Add("Record 2");
        }
    }
}