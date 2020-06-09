using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.Extensions;
using Core.Infrastructure.Compiler;
using DevExpress.Mvvm;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Database;
using Newtonsoft.Json;

namespace Medical.AppLayer.Settings.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IAppRemoteSettings _settings;
        private readonly IScriptCompiler _compiler;

        private object _selectedObject;
        private string _selectedSection;

        public ObservableCollection<object> Sections { get; set; }
        public Dictionary<string, string> Defaults { get; set; }
        public Dictionary<string, object> Objects { get; set; }

        private ICommand _saveCommand;
        private ICommand _testCommand;
        
        public string SelectedSection {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value; RaisePropertyChanged(()=>SelectedSection);
                if (_selectedSection.IsNotNullOrEmpty() && Objects.ContainsKey(_selectedSection))
                {
                    SelectedObject = Objects[_selectedSection];
                }
            }
        }

        public object SelectedObject {
            get { return _selectedObject; }
            set { _selectedObject = value;RaisePropertyChanged(()=>SelectedObject); RaisePropertyChanged(()=>IsTestAvailable); }
        }

        public SettingsViewModel(IAppRemoteSettings settings, IScriptCompiler compiler)
        {
            _compiler = compiler;
            _settings = settings;

            Init();
        }

        private void Init()
        {
            Sections = new ObservableCollection<object>();
            var allKeys = _settings.GetAllKeys();
            var allSettings = _settings.GetAll();
            var allMetadata = _settings.GetAllMetadata();

            if (allMetadata.IsNotNull())
            {
                Sections = new ObservableCollection<object>();
                foreach (var key in allKeys)
                {
                    dynamic metadata = _settings.GetMetadata(key);
                    if (metadata != null)
                    {
                        Sections.Add(new
                        {
                            metadata.Title,
                            Id = key
                        });
                    }
                    
                }
                RaisePropertyChanged(() => Sections);
                Objects = new Dictionary<string, object>();
                foreach (dynamic section in Sections)
                {
                    Type type = _compiler.FindTypeByName(section.Id);
                    var config = _settings.Get((string)section.Id);
                    var configJson = JsonConvert.SerializeObject(config);
                    var configTyped = JsonConvert.DeserializeObject(configJson, type);

                    Objects.Add(section.Id, configTyped);
                }

                RaisePropertyChanged(() => Objects);
            }
            else
            {
                //TODO log error
            }
            
        }

        public bool IsTestAvailable {
            get { return SelectedObject.HasMethod("Test"); }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave)); }
        }

        private bool CanSave()
        {
            return SelectedObject.IsNotNull();
        }

        private void Save()
        {
            _settings.Put(SelectedSection,SelectedObject);

        }

        public ICommand TestCommand
        {
            get { return _testCommand ?? (_testCommand = new RelayCommand(Test, CanTest)); }
        }

        private bool CanTest()
        {
            return SelectedObject.HasMethod("Test");
        }

        private void Test()
        {
            SelectedObject.GetMethodAndInvoke("Test");
        }
    }
}
