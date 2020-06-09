using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using BLToolkit.Data.Linq;
using Core.Extensions;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Models.Common;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class CreateSrzQueryViewModel : ViewModelBase 
    {
        private readonly IMedicineRepository _repository;
        private readonly ITextService _textService;
        private readonly ITortillaRepository _tortillaRepository;

        private readonly int _patientId;

        private PatientShortView _patient;
        private string _notes;
        private List<object> _selectedList;

        public ObservableCollection<CommonItem> FieldsList { get; set; }
        public List<object> SelectedList {
            get { return _selectedList; }
            set { _selectedList = value; RaisePropertyChanged(()=>SelectedList); }
        }
        public StringBuilder ResultText { get; set; }

        private RelayCommand _createQueryCommand;
        


        public string Notes {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }
        public CreateSrzQueryViewModel(int patientId, IMedicineRepository repository, ITortillaRepository tortillaRepository, ITextService textService)
        {
            _repository = repository;
            _tortillaRepository = tortillaRepository;
            _textService = textService;
            _patientId = patientId;

            ResultText = new StringBuilder();
            FieldsList = new ObservableCollection<CommonItem>();
            SelectedList = new List<object>();

            Initialize();
        }

        private void Initialize()
        {
            var patientResult = _repository.GetPatientShortViewByPatientId(_patientId);
            if (patientResult.Success && patientResult.Data.IsNotNull())
            {
                _patient = patientResult.Data;
                if (!string.IsNullOrWhiteSpace(_patient.SNILS))
                {
                    FieldsList.Add(new CommonItem {DisplayField = "СНИЛС", ValueField = 1});
                }

                if (_patient.DocType.HasValue && _patient.DocNum.IsNotNullOrWhiteSpace())
                {
                    FieldsList.Add(new CommonItem { DisplayField = "УДЛ", ValueField = 2 });
                }

                if (!string.IsNullOrWhiteSpace(_patient.INP))
                {
                    FieldsList.Add(new CommonItem {DisplayField = "ЕНП", ValueField = 3});
                }

                if (!string.IsNullOrWhiteSpace(_patient.InsuranceDocNumber))
                {
                    if (_patient.InsuranceDocType == 1)
                    {
                        FieldsList.Add(new CommonItem {DisplayField = "Полис старого образца", ValueField = 4});
                    }
                    else if (_patient.InsuranceDocType == 2)
                    {
                        FieldsList.Add(new CommonItem {DisplayField = "Временное свидетельство", ValueField = 5});
                    }
                }
            }
            else
            {
                ResultText.AppendFormat("При запросе данных пациента произошла ошибка {0}", patientResult.LastError);
                var toRtfResult = _textService.ToRtf(ResultText.ToString());
                if (toRtfResult.Success)
                {
                    Notes = toRtfResult.Data;
                }
            }
        }

        public ICommand CreateQueryCommand
        {
            get { return _createQueryCommand ?? (_createQueryCommand = new RelayCommand(CreateQuery, CanCreateQuery)); }
        }

        private void CreateQuery()
        {
            ResultText = new StringBuilder();
            ResultText.AppendFormat("<u><br>Начало отправки запроса в СРЗ<br></u>");

            try
            {
                foreach (int item in SelectedList)
                {
                    AddSrzQuery(item);
                }
            }
            catch (Exception exception)
            {
                ResultText.AppendFormat("<u><br>При формировании запроса произошло исключение<br></u><br/>{0}", exception.Message);
            }

            var toRtfResult = _textService.ToRtf(ResultText.ToString());
            if (toRtfResult.Success)
            {
                Notes = toRtfResult.Data;
            }
        }

        private void AddSrzQuery(int type)
        {
            
            var guid = Guid.NewGuid().ToString();

            var model = new SrzQueryModel
                            {
                                guid = guid,
                                fam = _patient.Surname.ToUpperInvariant(),
                                im = _patient.Name.ToUpperInvariant(),
                                ot = _patient.Patronymic.ToUpperInvariant(),
                                dr = _patient.Birthday.HasValue ? _patient.Birthday.Value.ToString("yyyy-MM-dd") : null,
                                sex = _patient.SexCode.ToNullableString(),
                                form_date = Sql.GetDate(),
                                passp = "none",
                                dultype = "none",
                                enp = "none",
                                pen = "none",
                                dulsernum = "none"
                            };

            switch (type)
            {
                case 1:
                    if (!string.IsNullOrWhiteSpace(_patient.SNILS))
                    {
                        model.pen = _patient.SNILS.Replace("-", "").Replace(" ", "");
                    }
                    break;
                case 2:

                    model.dultype = _patient.DocType.ToString();
                    if (!string.IsNullOrWhiteSpace(_patient.DocSeries) && !string.IsNullOrWhiteSpace(_patient.DocNum))
                    {
                        //FIX формат данных паспорта XX XX № XXXXXX
                        model.dulsernum = string.Format("{0} № {1}", _patient.DocSeries.Trim(), _patient.DocNum.Trim().Replace(" ", ""));
                    }
                    else if (!string.IsNullOrWhiteSpace(_patient.DocNum))
                    {
                        model.dulsernum = string.Format("{0}", _patient.DocNum.Trim().Replace(" ", ""));
                    }
                    break;
                case 3:
                    if (!string.IsNullOrWhiteSpace(_patient.INP))
                    {
                        model.enp = _patient.INP.Trim();
                    }
                    break;
                case 4:
                    if (!string.IsNullOrWhiteSpace(_patient.InsuranceDocSeries))
                    {
                        model.instype = "С";
                        model.inssernum = string.Format("{0} № {1}", _patient.InsuranceDocSeries.Trim().Replace(" ", ""), _patient.InsuranceDocNumber.Trim().Replace(" ", ""));
                    }
                    else
                    {
                        model.instype = "С";
                        model.inssernum = string.Format("{0}", _patient.InsuranceDocNumber.Trim().Replace(" ", ""));
                     
                    }
                    break;
                case 5:
                    if (!string.IsNullOrWhiteSpace(_patient.InsuranceDocNumber))
                    {
                        model.instype = "В";
                        model.inssernum = _patient.InsuranceDocNumber.Trim().Replace(" ", "");
                    }
                    break;
            }
            var addSrzQueryResult = _tortillaRepository.AddSrzQuery(model);
            if (addSrzQueryResult.Success)
            {
                ResultText.AppendFormat("Добавлен запрос в Тортиллу, ID = {0}<br/>", addSrzQueryResult.Id);

                var insertSrzQueryResult = _repository.InsertSrzQuery(_patientId, model.guid, type);
                if (insertSrzQueryResult.Success)
                {
                    ResultText.AppendFormat("Добавлен запрос в БД Medicine, ID = {0}<br/>", insertSrzQueryResult.Id);
                    
                }
            }
        }

        private bool CanCreateQuery()
        {
            return SelectedList.IsNotNull() && SelectedList.Count > 0;
        }
    }
}
