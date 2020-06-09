using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Extensions;
using DevExpress.XtraEditors.DXErrorProvider;

namespace DataModel
{
    /// <summary>
    /// Базовый класс для сущностей БД
    /// </summary>
    public class BaseEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// Обновить свойство по имени
        /// </summary>
        /// <param name="st"></param>
        public void UpdateProperty(string st)
        {
            RaisePropertyChanged(st);
        }

        /// <summary>
        /// Обновить все свойства
        /// </summary>
        public void UpdateAll()
        {
            GetType().GetProperties().Select(p=>p.Name).ForEachAction(RaisePropertyChanged);
        }

        /// <summary>
        /// Обновить свойства по имени из списка
        /// </summary>
        /// <param name="list"></param>
        public void Update(IEnumerable<string> list)
        {
            list.ForEachAction(RaisePropertyChanged);
        }

        #region INotifyPropertyChanged implement
        private void RaisePropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected void SetErrorInfo(ErrorInfo info, string errorText, ErrorType errorType)
        {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }
    }
}
