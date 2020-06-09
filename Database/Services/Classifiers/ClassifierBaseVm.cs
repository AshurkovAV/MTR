using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Core.Validation;
using GalaSoft.MvvmLight;

namespace Medical.DatabaseCore.Services.Classifiers
{
    public class ClassifierBaseVm<T> : ViewModelBase,  IDataErrorInfo, IClassifierModel<T> where T : new()
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        protected T _classifier;

        public ClassifierBaseVm(T classifier)
        {
            _classifier = classifier;
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public ClassifierBaseVm()
        {
            _classifier = new T();
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        public T Classifier
        {
            get { return _classifier; }
            set
            {
                _classifier = value;
                RaisePropertyChanged("Classifier");
            }
        }

        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        public virtual int Id { get; set; }

        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        public new bool IsInDesignMode { get; set; }


        string IDataErrorInfo.Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }

        string IDataErrorInfo.this[string memberName]
        {
            get { return _dataErrorInfoSupport[memberName]; }
        }

        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        public string Error
        {
            get
            {
                return _dataErrorInfoSupport.Error;
            }
        }
    }

    
}
