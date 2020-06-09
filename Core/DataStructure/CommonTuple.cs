using GalaSoft.MvvmLight;

namespace Core.DataStructure
{
    /// <summary>
    /// Тип данных кортеж для справочников (ключ/значение)
    /// </summary>
    public class CommonTuple : ViewModelBase
    {
        private string _displayField;
        private int _valueField;
        private object _dataField;
        private object _dataFieldV;
        /// <summary>
        /// Отображаемое поле
        /// </summary>
        public string DisplayField
        {
            get { return _displayField; }
            set { _displayField = value; RaisePropertyChanged(() => DisplayField); }
        }

        /// <summary>
        /// Поле данных (ключ)
        /// </summary>
        public int ValueField
        {
            get { return _valueField; }
            set { _valueField = value; RaisePropertyChanged(() => ValueField); }
        }

        /// <summary>
        /// Дополнительные данные
        /// </summary>
        public object DataField
        {
            get { return _dataField; }
            set { _dataField = value; RaisePropertyChanged(() => DataField); }
        }

        /// <summary>
        /// Дополнительные данные
        /// </summary>
        public object DataFieldV
        {
            get { return _dataFieldV; }
            set { _dataFieldV = value; RaisePropertyChanged(() => DataFieldV); }
        }

        public CommonTuple()
        {
        }

        public CommonTuple(string displayFiled, int valueField, object dataField)
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

    /// <summary>
    /// Тип данных кортеж для справочников (ключ/значение) с типом ключа Т
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonTuple<T> : CommonTuple
    {
        private T _valueField;

        public new T ValueField
        {
            get { return _valueField; }
            set { _valueField = value; RaisePropertyChanged(() => ValueField); }
        }
    }

    /// <summary>
    /// Тип данных кортеж для справочников (ключ/значение) с типом ключа Т1 и дополнительными данными типа T2
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class CommonTuple<T1, T2> : CommonTuple<T1>
    {
        private T2 _dataField;
        public new T2 DataField
        {
            get { return _dataField; }
            set { _dataField = value; RaisePropertyChanged(() => DataField); }
        }
    }
}
