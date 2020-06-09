using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace Examples
{
    public class ExampleData : ViewModelBase
    {
        private string _title;
        private string _url;
        private string _description;


        public string Title {
            get { return _title; }
            set
            {
                _title = value; 
                RaisePropertyChanged(()=>Title);
            }
        }

        public string Url {
            get { return _url; }
            set { _url = value; RaisePropertyChanged(()=>Url); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged(() => Description); }
        }
    }
}
