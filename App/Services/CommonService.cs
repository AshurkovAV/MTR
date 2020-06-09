using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Core;
using Core.DataStructure;
using Core.Extensions;
using DataModel;
using Medical.AppLayer.Economic.Models;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using Newtonsoft.Json.Linq;

namespace Medical.AppLayer.Services
{
    public class CommonService : ICommonService
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;

        private Stopwatch _sw;

        public CommonService(
            IMedicineRepository repository,
            IMessageService messageService)
        {
            _repository = repository;
            _messageService = messageService;
        }

        public void StartWatch()
        {
            if (_sw != null)
            {
                _sw.Restart();
                //TODO warning
            }
            _sw = Stopwatch.StartNew();
        }

        public double StopWatchAndGetSeconds()
        {
            _sw.Stop();
            var result = Convert.ToDouble(_sw.ElapsedMilliseconds)/1000;
            _sw = null;
            return result;
        }

        public bool IsDatabaseConnectionString(object obj)
        {
            try
            {
                var checkList = new List<string>
                {
                    "Name",
                    "DataSource",
                    "Database",
                    "Provider",
                    "Timeout",
                    "IsWindowsAuth"
                };

                var tmp = JObject.FromObject(obj);
                return checkList.All(name => tmp.GetValue(name) != null);
            }
            catch (Exception)
            {
                return false;
            }

        }

        public string ConstructDatabaseConnectionString(object value)
        {
            var result = string.Empty;
            try
            {
                dynamic tmp = value;
                if ((bool)tmp.IsWindowsAuth)
                {
                    result = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;Application Name={2};Connect Timeout={3};", 
                        tmp.DataSource, 
                        tmp.Database, 
                        GlobalConfig.AppName, 
                        tmp.Timeout);
                }
                else
                {
                    result = string.Format(
                        "Data Source={0};Initial Catalog={1};Application Name={2};User Id={3};Password={4};Connect Timeout={5};",
                        tmp.DataSource,
                        tmp.Database,
                        GlobalConfig.AppName,
                        tmp.UserId,
                        tmp.Password,
                        tmp.Timeout);
                }

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public string GetTitleByTerritoryAccount(FactTerritoryAccount account)
        {
            return "№{0}{1} от {2}/{3}".F(
                                account.AccountNumber,
                                account.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                                (DirectionType?)account.Direction == DirectionType.Out ? "Исх." : "Вх."
                                );
        }

        public string GetTitleByIdForOperatorMode(int id, OperatorMode mode)
        {
            var prefix = string.Empty;
            var postfix = string.Empty;
            const string template = "{0} {1}";
            try
            {
                switch (mode)
                {
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.ZInterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        prefix = mode.GetDisplayShortName();
                        var accountResult = _repository.GetTerritoryAccountViewById(id);
                        if (accountResult.Success)
                        {
                            var account = accountResult.Data;
                            postfix = "№{0}{1} от {2}/{3}/{4}".F(
                                account.AccountNumber,
                                account.Type == Constants.CorrectedPart ? "/испр.часть " + account.TerritoryAccountId : string.Empty,
                                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                                (DirectionType?)account.Direction == DirectionType.Out ? account.DestinationName : account.SourceName,
                                (DirectionType?)account.Direction == DirectionType.Out ? "Исх." : "Вх."
                                );
                        }
                        break;
                    case OperatorMode.Local:
                    case OperatorMode.Zlocal:
                    case OperatorMode.LocalError:
                    case OperatorMode.ZLocalError:
                        prefix = mode.GetDisplayShortName();
                        var accountMoResult = _repository.GetMedicalAccountViewById(id);
                        if (accountMoResult.Success)
                        {
                            var account = accountMoResult.Data;
                            postfix = "№{0} от {1} - {2}".F(
                                account.AccountNumber,
                                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                                account.ShortNameMo);
                        }
                        break;
                    case OperatorMode.Patient:
                        prefix = mode.GetDisplayShortName();
                        var patientResult = _repository.GetPatientShortViewByPatientId(id);
                        if (patientResult.Success)
                        {
                            var patient = patientResult.Data;
                            postfix = "{0} {1} {2} {3}".F(
                                patient.Surname, 
                                patient.Name, 
                                patient.Patronymic, 
                                patient.Birthday.HasValue ? patient.Birthday.Value.ToString("d") : "нет");
                        }
                        break;
                }
            }
            catch(Exception exception)
            {
                _messageService.ShowException(exception,"Исключение при получении названия для редактора по id счета и режиму", typeof(CommonService));
            }

            return template.F(prefix, postfix);
        }

        public string GetTitleByTerritoryAccountView(FactTerritoryAccount account)
        {
            return "№{0}{1} от {2}/{3}/{4}".F(
                account.AccountNumber,
                account.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                (DirectionType?)account.Direction == DirectionType.Out ? account.TerritoryAccountId : account.ExternalId,
                (DirectionType?)account.Direction == DirectionType.Out ? "Исх." : "Вх."
                );
        }
        public string GetTitleByTerritoryAccountView(TerritoryAccountView account)
        {
            return "№{0}{1} от {2}/{3}/{4}".F(
                account.AccountNumber,
                account.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                (DirectionType?)account.Direction == DirectionType.Out ? account.DestinationName : account.SourceName,
                (DirectionType?)account.Direction == DirectionType.Out ? "Исх." : "Вх."
                );
        }

        public string GetTitleByEconomicAccountView(EconomicAccountCustomModel account)
        {
            var resultTerritoryacc = _repository.GetTerritoryAccountById(account.Account.AccountId);

            return "№{0}{1} от {2}/{3}/{4}".F(
                resultTerritoryacc.Data.AccountNumber,
                resultTerritoryacc.Data.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                resultTerritoryacc.Data.AccountDate.HasValue ? resultTerritoryacc.Data.AccountDate.Value.ToString("d") : "нет",
                (DirectionType?)account.Direction == DirectionType.Out ? resultTerritoryacc.Data.Destination: resultTerritoryacc.Data.Source,
                (DirectionType?)account.Direction == DirectionType.Out ? "Исх." : "Вх."
                );
        }


        public string GetTitleByMedicalAccountView(MedicalAccountView account)
        {
            return "№{0} код МО {1} от {2}/{3}".F(
                account.AccountNumber,
                account.CodeMo ,
                account.AccountDate.HasValue ? account.AccountDate.Value.ToString("d") : "нет",
                account.ShortNameMo);
        }

        public string GenerateAccountData(EventShortView data)
        {
            if (data.AccountNumber.IsNotNullOrWhiteSpace())
            {
                return "Счет №{0}{1} от {2}/{3}/{4}".F(
                data.AccountNumber,
                data.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                data.AccountDate.HasValue ? data.AccountDate.Value.ToString("d") : "нет",
                data.SourceName + "\u21D2" + data.DestinationName,
                (DirectionType?)data.Direction == DirectionType.Out ? "Исх." : "Вх."
                );
            }

            return "";

        }

        public string GenerateAccountData(GeneralEventShortView data)
        {
            if (data.AccountNumber.IsNotNullOrWhiteSpace())
            {
                return "Счет №{0}{1} от {2}/{3}/{4}".F(
                data.AccountNumber,
                data.Type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                data.AccountDate.HasValue ? data.AccountDate.Value.ToString("d") : "нет",
                data.SourceName + "\u21D2" + data.DestinationName,
                (DirectionType?)data.Direction == DirectionType.Out ? "Исх." : "Вх."
                );
            }

            return "";

        }

        public string GenerateAccountData(string accountNumber, int type, DateTime? accountDate, int direction, string sourceName, string destName)
        {
            return "Счет №{0}{1} от {2}/{3}/{4}".F(
                accountNumber,
                type == Constants.CorrectedPart ? "/испр.часть" : string.Empty,
                accountDate.HasValue ? accountDate.Value.ToString("d") : "нет",
                (DirectionType?)direction == DirectionType.Out ? sourceName : destName,
                (DirectionType?)direction == DirectionType.Out ? "Исх." : "Вх."
                );

        }

        public IEnumerable<CommonTuple> GetMonths()
        {
            var culture = new CultureInfo("ru-RU");
            return new List<CommonTuple>(culture.DateTimeFormat.MonthNames.Take(12).Select((p, idx) => new CommonTuple
            {
                DisplayField = p,
                ValueField = idx + 1
            }));
        }


        public int GetNumberOfDays(DateTime? begin, DateTime? end)
        {
            if (!begin.HasValue || !end.HasValue)
            {
                return 0;
            }

            return Enumerable.Range(0, (end.Value - begin.Value).Days + 1)
                .Where(p => Enumerable.Range(1, 5).Contains((int)begin.Value.AddDays(p).DayOfWeek))
                .Count();
        }
    }
}
