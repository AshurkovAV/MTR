using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using Core.MarkupConverter;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Medical.AppLayer.Register.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        private RelayCommand _saveLogCommand;
        private RelayCommand _processCommand;

        private int _medicalAccountId;
        private bool _isTest;
        private bool _isDebug;

        public InfoViewModel(int medicalAccountId)
        {
            _medicalAccountId = medicalAccountId;
            //Notes = new MarkupConverter().ConvertHtmlToRtf(ResultText);//"Simple<b>Bold</b> <color=#0000FF>Blue</color> <backcolor=red>BackRed</backcolor> <size=+4>+4</size> <br>br <i>Italic</i> <u>Underline</u>");
        }

        public bool IsTest
        {
            get { return _isTest; }
            set { _isTest = value; RaisePropertyChanged("IsTest"); }
        }

        public bool IsDebug
        {
            get { return _isDebug; }
            set { _isDebug = value; RaisePropertyChanged("IsDebug"); }
        }

        public bool? DialogResult { get; set; }
        public string ResultText { get; set; }
        public string Notes { get; set; }

        public ICommand SaveLogCommand
        {
            get { return _saveLogCommand ?? (_saveLogCommand = new RelayCommand(SaveLog, CanSaveLog)); }
        }

        private bool CanSaveLog()
        {
            return !string.IsNullOrWhiteSpace(Notes);
        }

        private void SaveLog()
        {
            var dlg = new SaveFileDialog { DefaultExt = ".rtf", Filter = "Rtf файлы (.rtf)|*.rtf" };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var writer = new StreamWriter(dlg.FileName))
                {
                    writer.Write(Notes);
                }
            }
        }

        public ICommand ProcessCommand
        {
            get { return _processCommand ?? (_processCommand = new RelayCommand(Process, CanProcess)); }
        }

        private bool CanProcess()
        {
            return _medicalAccountId > 0;
        }

        private void Process()
        {
            /*var policySearch = new PolicySearch();
            policySearch.Process(_medicalAccountId, IsTest, IsDebug);
            Notes = policySearch.LogRtf;
            RaisePropertyChanged("Notes");*/
        }

        
        public void AddNote(string note)
        {
            ResultText += note;
        }
        public void ShowNotes()
        {
            Notes = new MarkupConverter().ConvertHtmlToRtf(ResultText);
            RaisePropertyChanged("Notes");
        }
    }
}