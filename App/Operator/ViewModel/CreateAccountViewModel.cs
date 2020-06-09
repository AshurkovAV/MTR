using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Core.Builders;
using Core.Extensions;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    /// <summary>
    /// Быстрое создание счета на территорию
    /// </summary>
    public class CreateAccountViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;

        private string _notes;
        private int? _selectedMonth;
        private int? _selectedYear;
        private string _territoryOkato;
        private int _version;
        private F010 _territory;
        public int AccountId { get; set; }
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(() => Notes); }
        }
        public ObservableCollection<object> MonthCollection { get; set; }
        public ObservableCollection<object> YearCollection { get; set; }

        private RelayCommand _createAccountCommand;
        
        public CreateAccountViewModel(IMedicineRepository repository,
            string territoryOkato,
            int version)
        {
            _repository = repository;
            _version = version;

            var culture = new CultureInfo("ru-RU");
            MonthCollection = new ObservableCollection<object>(culture.DateTimeFormat.MonthNames.Take(12).Select((p, index)=>new { Name = p, Value = index+1}));
            //var currentYear = DateTime.Now.Year;
            //YearCollection = new ObservableCollection<object>(Enumerable.Range(currentYear - 3, 4).Select(x => new { Name = "{0}г.".F(x), Value = x }));

            var result = _repository.GetTerritoryAccountDate();
            if (result.Success) {
                var currentYear = DateTime.Now.Year;
                var yearResult = result.Data.ToList();
                if (!yearResult.Contains(currentYear))
                {
                    yearResult.Add(currentYear);
                }

                if (!yearResult.Contains(currentYear+1))
                {
                    yearResult.Add(currentYear+1);
                }

                YearCollection = new ObservableCollection<object>(yearResult.Select(x => new { Name = "{0}г.".F(x), Value = x }));
            }
            TerritoryOkato = territoryOkato;

            var territoryResult = _repository.GetAll<F010>(p => p.KOD_OKATO == TerritoryOkato);
            if (territoryResult.Success)
            {
                Territory = territoryResult.Data.FirstOrDefault();
            }
        }
        public int? SelectedMonth
        {
            get { return _selectedMonth; }
            set { 
                _selectedMonth = value; 
                RaisePropertyChanged("SelectedMonth");
            }
        }

        public int? SelectedYear
        {
            get { return _selectedYear; }
            set { 
                _selectedYear = value; 
                RaisePropertyChanged("SelectedYear");
            }
        }

        public string TerritoryOkato
        {
            get { return _territoryOkato; }
            set
            {
                _territoryOkato = value;
                RaisePropertyChanged("TerritoryOkato");
            }
        }

        public F010 Territory
        {
            get { return _territory; }
            set
            {
                _territory = value;
                RaisePropertyChanged("Territory");
            }
        }

        public ICommand CreateAccountCommand
        {
            get { return _createAccountCommand ?? (_createAccountCommand = new RelayCommand(CreateAccount, CanCreateAccount)); }
        }

        private void CreateAccount()
        {
            var html = new HtmlBuilder();
           
            if (!SelectedYear.HasValue || !SelectedMonth.HasValue )
            {
                html.BeginFormat(3, Color.Brown)
                        .Text("Отсутствует отчетный месяц и год")
                    .EndFormat();
                return;
            }
            html.Header("Создание счета на территорию: {0}".F(Territory.SUBNAME), 4);
            var createResult = _repository.CreateTerritoryAccountAuto(TerritoryOkato,SelectedYear.Value,SelectedMonth.Value, _version);
            if (createResult.Success)
            {
                AccountId = createResult.Id;
                html.Text("Счет успешно создан, ID {0}", AccountId);
            }
            else
            {
                html.BeginFormat(3, Color.Brown)
                        .Text("Ошибка при создании счета на территорию")
                        .Text(createResult.LastError.ToString())
                    .EndFormat();
            }
            Notes = html.Value();
        }

        private bool CanCreateAccount()
        {
            return SelectedYear.HasValue && SelectedMonth.HasValue;
        }
    }
}
