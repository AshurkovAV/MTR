using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Examples
{
    public class StartPageViewModel : ViewModelBase
    {
        private ObservableCollection<ExampleData> _examples;
        private ExampleData _selectedExample;
        private RelayCommand<object> _navigateCommand; 

        public ObservableCollection<ExampleData> Examples
        {
            get { return _examples; }
            set
            {
                _examples = value; 
                RaisePropertyChanged(()=>Examples);
            }
        }

        public ExampleData SelectedExample
        {
            get { return _selectedExample; }
            set { 
                _selectedExample = value; 
                RaisePropertyChanged(()=>SelectedExample); 
                RaisePropertyChanged(()=>Descriptioin);
            }
        }

        public string Descriptioin
        {
            get
            {
                if (SelectedExample != null)
                {
                    return SelectedExample.Description;
                }
                return "Выберете пример";
            }
        }

        public ICommand NavigateCommand
        {
            get { return _navigateCommand ?? (_navigateCommand = new RelayCommand<object>(Navigate)); }
        }

        private void Navigate(object obj)
        {
            var example = obj as ExampleData;
            if (example != null)
            {
                AppHelper.Navigator.Navigate(new Uri(example.Url, UriKind.Relative));
            }

        }

        public StartPageViewModel()
        {
            Examples = new ObservableCollection<ExampleData>
            {
                new ExampleData
                {
                    Title = "MVVM базовый уровень",
                    Url = "MVVM/Basic/View.xaml",
                    Description = "Базовый пример использования паттерна MVVM"
                },
                new ExampleData
                {
                    Title = "MVVM продвинутый уровень",
                    Url = "MVVM/Advanced/View.xaml",
                    Description = "Продвинутый пример использования паттерна MVVM"
                }
            };
        }
    }
}