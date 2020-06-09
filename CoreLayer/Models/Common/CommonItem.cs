using System;
using GalaSoft.MvvmLight;

namespace Medical.CoreLayer.Models.Common
{
    [Obsolete("Use CommonTuple")]
    public class CommonItem : ViewModelBase
    {
        private string _displayField;
        private string _valueFieldStr;
        private int _valueField;
        private object _dataField;

        public string DisplayField
        {
            get { return _displayField; }
            set { _displayField = value; RaisePropertyChanged(() => DisplayField); }
        }
        public int ValueField
        {
            get { return _valueField; }
            set { _valueField = value; RaisePropertyChanged(() => ValueField); }
        }

        public string ValueFieldStr
        {
            get { return _valueFieldStr; }
            set { _valueFieldStr = value; RaisePropertyChanged(() => ValueFieldStr); }
        }

        public object DataField
        {
            get { return _dataField; }
            set { _dataField = value; RaisePropertyChanged(() => DataField); }
        }

        public CommonItem()
        {
        }

        public CommonItem(string displayFiled, int valueField, object dataField)
        {
            if (IsInDesignMode)
            {
                DisplayField = "DisplayField";
                ValueField = 0;
                DataField = new object();
            }
            else
            {
                // Code runs "for real"
                DisplayField = displayFiled;
                ValueField = valueField;
                DataField = dataField;
            }

        }
    }

    [Obsolete("Use CommonTuple<T>")]
    public class CommonItem<T> : ViewModelBase
    {
        private string _displayField;
        private T _valueField;

        public string DisplayField
        {
            get { return _displayField; }
            set { _displayField = value; RaisePropertyChanged(() => DisplayField); }
        }
        public T ValueField
        {
            get { return _valueField; }
            set { _valueField = value; RaisePropertyChanged(() => ValueField); }
        }
    }
}
