using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using GalaSoft.MvvmLight;
using Medical.AppLayer.Managers;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SrzResultViewModel : ViewModelBase 
    {
        private readonly IMedicineRepository _repository;
        private readonly ITortillaRepository _tortillaRepository;
        private readonly INotifyManager _notifyManager;

        private List<SrzAnswerViewModel> _answers;
        private List<SrzErrorViewModel> _errors;
        private List<SrzQueryViewModel> _srzQueryList;
        private SrzQueryViewModel _selectedSrzQuery;

        private int _patientId;

        public SrzResultViewModel(int patientId, 
            IMedicineRepository repository, 
            ITortillaRepository tortillaRepository, 
            INotifyManager notifyManager)
        {
            PatientId = patientId;

            _repository = repository;
            _tortillaRepository = tortillaRepository;
            _notifyManager = notifyManager;

            Initialize();
        }

        private void Initialize()
        {
            var srzQueryResult = _repository.GetSrzQueryByPatientId(PatientId);
            if (srzQueryResult.Success)
            {
                SrzQueryList = srzQueryResult.Data.Select(p => new SrzQueryViewModel(p)).ToList();
            }
        }

        public int PatientId {
            get { return _patientId; }
            set { _patientId = value; RaisePropertyChanged(()=>PatientId); }
        }


        public List<SrzAnswerViewModel> Answers
        {
            get { return _answers; }
            set { 

                _answers = value; RaisePropertyChanged("Answers"); 
            }
        }

        public List<SrzErrorViewModel> Errors
        {
            get { return _errors; }
            set { _errors = value; RaisePropertyChanged("Errors"); }
        }

        public List<SrzQueryViewModel> SrzQueryList
        {
            get { return _srzQueryList; }
            set { 
                _srzQueryList = value; 
                RaisePropertyChanged("SrzQueryList");
                if (_srzQueryList != null && _srzQueryList.Count > 0)
                {
                    SelectedSrzQuery = _srzQueryList[_srzQueryList.Count - 1];
                }
            }
        }

        public SrzQueryViewModel SelectedSrzQuery
        {
            get { return _selectedSrzQuery; }
            set 
            { 
                _selectedSrzQuery = value; 
                RaisePropertyChanged("SelectedSrzQuery");

                if (value.IsNotNull())
                {
                    var srzQueryResult = _repository.GetSrzQueryById(SelectedSrzQuery.SrzQueryId);
                    if (srzQueryResult.Success && 
                        srzQueryResult.Data.IsNotNull() && 
                        srzQueryResult.Data.Guid.IsNotNullOrWhiteSpace() && 
                        srzQueryResult.Data.Guid.IsGuid())
                    {
                        var answerResult = _tortillaRepository.GetSrzAnswer(srzQueryResult.Data.Guid);
                        if (answerResult.Success)
                        {
                            if (answerResult.Data.Any())
                            {
                                var updateResult = _repository.MarkSrzQueryAsReadById(SelectedSrzQuery.SrzQueryId);
                                if (updateResult.Success)
                                {
                                    _notifyManager.ShowNotify("Запросы в SRZ успешно помечены как прочитанные");
                                }
                                Answers = answerResult.Data.Select(p => new SrzAnswerViewModel(p)).ToList();
                            }
                            else
                            {
                                var errorResult = _tortillaRepository.GetSrzError(srzQueryResult.Data.Guid);
                                if (errorResult.Success && errorResult.Data.Any())
                                {
                                    Errors = errorResult.Data.Select(p => new SrzErrorViewModel(p)).ToList();
                                }
                                else
                                {
                                    Errors = null;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
