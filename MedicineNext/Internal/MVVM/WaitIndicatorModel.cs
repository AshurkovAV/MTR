using System;
using Core;
using Core.Extensions;
using GalaSoft.MvvmLight;
using Medical.AppLayer;

namespace MedicineNext.Internal.MVVM
{
    public class WaitIndicatorModel : ViewModelBase
    {
        private string _title;
        private string _text;
        private double _progress;
        private double _percent;
        private double _total;

        private DateTime _starttime;
        private string _remain;

        public string Title {
            get { return _title; }
            set
            {
                _title = value; 
                RaisePropertyChanged(()=>Title);
                _starttime = DateTime.Now;
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; RaisePropertyChanged(() => Text); }
        }

        public double Percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                RaisePropertyChanged(() => Percent);
                RaisePropertyChanged(() => IsNotNan);
            }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value; 
                RaisePropertyChanged(() => Progress);
                RaisePropertyChanged(()=>IsNotNan);
                Percent = 100 / _total * _progress;
                TimeSpan timespent = DateTime.Now - _starttime;
                double msRemaining = (timespent.TotalMilliseconds / _progress * (_total - _progress));
                Remain = Constants.RemainMsg.F(msRemaining.FormatTimeInterval());
            }
        }

        public double Total
        {
            get { return _total; }
            set { _total = value; RaisePropertyChanged(() => Total); }
        }

        public bool IsNotNan
        {
            get { return !double.IsNaN(Percent); }
        }

        public string Remain
        {
            get { return _remain; }
            set { _remain = value; RaisePropertyChanged(() => Remain); }
        }
    }
}
