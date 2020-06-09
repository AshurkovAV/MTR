using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BLToolkit.Data.Linq;
using BLToolkit.Mapping;
using Core;
using Core.Builders;
using Core.Extensions;
using Core.Infrastructure;
using Core.Linq;
using Core.Services;
using DataModel;
using Medical.AppLayer.Economic.Models.Reports;
using Medical.DataCore.Interface;
using Medical.DataCore.v10PL.PL;
using Medical.DataCore.v21K2.D;
using v30K1 = Medical.DataCore.v30K1;
using v31K1 = Medical.DataCore.v31K1;
using v32K1 = Medical.DataCore.v32K1;

namespace Medical.AppLayer.Services
{
    public class ComplexReportRepository : IComplexReportRepository
    {
        private readonly string _defaultProvider;
        private readonly string _defaultName;

        public ComplexReportRepository(IAppShareSettings settings)
        {
            dynamic databaseConfig = settings.Get("database");
            _defaultProvider = databaseConfig.Provider;
            _defaultName = databaseConfig.Name;
        }

        private MedicineContext CreateContext()
        {
            return new MedicineContext(_defaultProvider, _defaultName);
        }
        public OperationResult<Tuple<List<ReportForm2CustomModel>,int>> CreateForm2Report(DateTime beginDate, DateTime endDate, int direction)
        {
            var outResult = new OperationResult<Tuple<List<ReportForm2CustomModel>, int>>();
            try
            {
                var result = new List<ReportForm2CustomModel>();
                //var workDays = Enumerable.Range(0, (endDate - beginDate).Days + 1).Where(p=>Enumerable.Range(1, 5).Contains((int)beginDate.AddDays(p).DayOfWeek)).Count();
                
                using (var db = CreateContext())
                {
                    //Получаем список округов
                    var district = db.GetTable<F015>().ToList();
                    //Получаем список всех территорий со счетами попадающими в заданный период и направление счета
                    Expression<Func<FactTerritoryAccount, bool>> territoryPredicate = PredicateBuilder.True<FactTerritoryAccount>();
                    territoryPredicate = direction == 0 ?
                        territoryPredicate.And(p => p.Direction == 0) :
                        territoryPredicate.And(p => p.Direction == 1);


                    territoryPredicate = territoryPredicate.And(p => 
                        p.EconomicDate.HasValue && 
                        p.EconomicDate >= beginDate && 
                        p.EconomicDate <= endDate);

                    //Группируем по источнику или цели в зависимости от направления
                    var data = direction == 0 ?
                        new List<ReportForm2Entry>(db.GetTableQuery<FactTerritoryAccount>()
                            .Where(territoryPredicate)
                            .GroupBy(p => p.Destination)
                            .Select(g => new ReportForm2Entry
                            {
                                TerritoryOkato = g.Key
                            }))
                        : new List<ReportForm2Entry>(db.GetTableQuery<FactTerritoryAccount>()
                            .Where(territoryPredicate)
                            .GroupBy(p => p.Source)
                            .Select(g => new ReportForm2Entry
                            {
                                TerritoryOkato = g.Key
                            }));


                    
                    foreach (ReportForm2Entry model in data)
                    {
                        var f010 = db.GetTableQuery<F010>().FirstOrDefault(p => p.KOD_OKATO == model.TerritoryOkato);
                        if (f010 == null)
                        {
                            outResult.AddError("Не найдена территория, код ОКАТО {0}".F(model.TerritoryOkato));
                            return outResult;
                        }
                        model.TerritoryName = f010.SUBNAME;
                        if (result.All(p => p.DistrictCode != f010.OKRUG))
                        {
                            var currentDistrict = district.FirstOrDefault(p=>p.KOD_OK == f010.OKRUG);
                            if (currentDistrict == null)
                            {
                                outResult.AddError("Не найден округ, код {0}".F(f010.OKRUG));
                                return outResult;
                            }
                            result.Add(new ReportForm2CustomModel
                            {
                                DistrictName = currentDistrict.OKRNAME,
                                DistrictCode = currentDistrict.Id
                            });
                        }

                        var reportModel = result.FirstOrDefault(p => p.DistrictCode == f010.OKRUG);
                        if (reportModel == null)
                        {
                            outResult.AddError("Не найдена модель округа, код {0}".F(f010.OKRUG));
                            return outResult;
                        }

                        //Предъявлено в отчетном периоде
                        var currentPayments = direction == 0 ? 
                            db.GetTableQuery<FactMedicalEvent>()
                            .Where(p =>
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.EconomicDate >= beginDate &&
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.EconomicDate <= endDate &&
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.Destination == model.TerritoryOkato)
                            :
                            db.GetTableQuery<FactMedicalEvent>()
                            .Where(p =>
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.EconomicDate >= beginDate &&
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.EconomicDate <= endDate && 
                                p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID.Source == model.TerritoryOkato)
                            ;


                        model.AmountPayable = currentPayments.Sum(p => p.AcceptPrice) ?? 0;
                        
                        //Долг на начало периода    
                        var debt = db.GetTableQuery<FactEconomicDebt>().FirstOrDefault(p => p.Territory == model.TerritoryOkato && p.Date <= beginDate);
                        if (debt != null)
                        {
                            
                            //для входящих счетов собственный, для исходящих территории
                            model.BeginDebtTotal = direction == 0 ? debt.TerritoryAmount : debt.OwnAmount;
                            model.BeginDebt25Days = direction == 0 ? debt.TerritoryAmount25 : debt.OwnAmount25;
                        }

                        //Доплаты прошлых лет
                        var surcharge = direction == 0 ? 
                            db.GetTableQuery<FactEconomicSurcharge>()
                            .Where(p =>
                                p.FACTECONSURTERRACC.Destination == model.TerritoryOkato && 
                                p.FACTECONSURTERRACC.EconomicDate <= beginDate && 
                                p.SurchargeDate >= beginDate && p.SurchargeDate <= endDate)
                            .Sum(p => p.SurchargeTotalAmount)
                            :
                            db.GetTableQuery<FactEconomicSurcharge>()
                            .Where(p =>
                                p.FACTECONSURTERRACC.Source == model.TerritoryOkato &&
                                p.FACTECONSURTERRACC.EconomicDate <= beginDate && 
                                p.SurchargeDate >= beginDate && p.SurchargeDate <= endDate)
                            .Sum(p => p.SurchargeTotalAmount)
                            ;
                        model.AmountFactLastYears = surcharge;

                        //Отказов по счетам отчетного периода
                        model.AmountRefuseCurrentPeriod = direction == 0 ?
                            db.GetTableQuery<FactEconomicRefuse>()
                                .Where(p =>
                                    p.FACTECONREFTERRACC.EconomicDate >= beginDate && p.RefuseDate >= beginDate &&
                                    p.FACTECONREFTERRACC.EconomicDate <= endDate && p.RefuseDate <= endDate &&
                                    p.FACTECONREFTERRACC.Destination == model.TerritoryOkato)
                                .Sum(p => p.RefuseTotalAmount) :
                            db.GetTableQuery<FactEconomicRefuse>()
                                .Where(p =>
                                    p.FACTECONREFTERRACC.EconomicDate >= beginDate && p.RefuseDate >= beginDate &&
                                    p.FACTECONREFTERRACC.EconomicDate <= endDate && p.RefuseDate <= endDate &&
                                    p.FACTECONREFTERRACC.Source == model.TerritoryOkato)
                                .Sum(p => p.RefuseTotalAmount);

                        //Всего отказов
                        model.AmountRefuseTotal = direction == 0 ?
                            db.GetTableQuery<FactEconomicRefuse>()
                                .Where(p =>
                                    p.RefuseDate >= beginDate &&
                                    p.RefuseDate <= endDate &&
                                    p.FACTECONREFTERRACC.Destination == model.TerritoryOkato)
                                .Sum(p => p.RefuseTotalAmount) :
                            db.GetTableQuery<FactEconomicRefuse>()
                                .Where(p =>
                                    p.RefuseDate >= beginDate &&
                                    p.RefuseDate <= endDate &&
                                    p.FACTECONREFTERRACC.Source == model.TerritoryOkato)
                                .Sum(p => p.RefuseTotalAmount);

                        //Всего фактически оплачено
                        model.AmountFactTotal = direction == 0 ?
                            db.GetTableQuery<FactEconomicAccount>()
                                .Where(p =>
                                    p.PaymentDate >= beginDate &&
                                    p.PaymentDate <= endDate &&
                                    p.PaymentStatus == 2 &&
                                    p.FACTTERRACCACCID.Destination == model.TerritoryOkato)
                                .Sum(p => p.TotalAmount) ?? 0 :
                            db.GetTableQuery<FactEconomicAccount>()
                                .Where(p =>
                                    p.PaymentDate >= beginDate &&
                                    p.PaymentDate <= endDate &&
                                     p.PaymentStatus == 2 &&
                                    p.FACTTERRACCACCID.Source == model.TerritoryOkato)
                                .Sum(p => p.TotalAmount) ?? 0;

                        //Фактически оплачено по счетам отчетного периода
                        model.AmountFactCurrentPeriod = direction == 0 ?
                            db.GetTableQuery<FactEconomicAccount>()
                                .Where(p =>
                                    p.FACTTERRACCACCID.EconomicDate >= beginDate && p.PaymentDate >= beginDate &&
                                    p.FACTTERRACCACCID.EconomicDate <= endDate && p.PaymentDate <= endDate &&
                                    p.PaymentStatus == 2 &&
                                    p.FACTTERRACCACCID.Destination == model.TerritoryOkato)
                                .Sum(p => p.TotalAmount) ?? 0 :
                            db.GetTableQuery<FactEconomicAccount>()
                                .Where(p =>
                                    p.FACTTERRACCACCID.EconomicDate >= beginDate && p.PaymentDate >= beginDate &&
                                    p.FACTTERRACCACCID.EconomicDate <= endDate && p.PaymentDate <= endDate &&
                                    p.PaymentStatus == 2 &&
                                    p.FACTTERRACCACCID.Source == model.TerritoryOkato)
                                .Sum(p => p.TotalAmount) ?? 0;

                        //Долг на конец периода
                        model.EndDebtTotal = model.BeginDebtTotal + model.AmountPayable - model.AmountRefuseTotal - model.AmountFactTotal + model.AmountFactLastYears;

                        model.EndDebt25Days = direction == 0 ? 
                            db.GetTableQuery<FactEconomicPaymentDetail>()
                            .Where(p => p.FACTECONPAYDTERRACC.Destination == model.TerritoryOkato &&
                                        p.FACTECONPAYDTERRACC.EconomicDate >= beginDate &&
                                        p.FACTECONPAYDTERRACC.EconomicDate <= endDate &&
                                        p.AmountDebt > 0 &&
                                        Sql.DateDiff(Sql.DateParts.Day, p.FACTECONPAYDTERRACC.EconomicDate, endDate) >= 25)
                            .Sum(p => p.AmountDebt) :
                            db.GetTableQuery<FactEconomicPaymentDetail>()
                                .Where(p => p.FACTECONPAYDTERRACC.Source == model.TerritoryOkato &&
                                            p.FACTECONPAYDTERRACC.EconomicDate >= beginDate &&
                                            p.FACTECONPAYDTERRACC.EconomicDate <= endDate &&
                                            p.AmountDebt > 0 &&
                                            Sql.DateDiff(Sql.DateParts.Day, p.FACTECONPAYDTERRACC.EconomicDate, endDate) >= 25)
                                .Sum(p => p.AmountDebt);

                        reportModel.Entry.Add(model);
                    }

                    var count = 1;
                    
                    result = result.OrderBy(p => p.DistrictCode).ToList();
                    
                    foreach (var reportModel in result)
                    {
                        reportModel.Row = count++;
                        reportModel.Entry = reportModel.Entry.OrderBy(p => p.TerritoryName).ToList();
                        foreach (var entryModel in reportModel.Entry)
                        {
                            entryModel.Row = count++;
                        }
                        reportModel.DistrictEntry = new ReportForm2Entry
                        {
                            AmountFactCurrentPeriod = reportModel.Entry.Sum(p=>p.AmountFactCurrentPeriod),
                            AmountFactLastYears = reportModel.Entry.Sum(p => p.AmountFactLastYears),
                            AmountFactTotal = reportModel.Entry.Sum(p => p.AmountFactTotal),
                            AmountPayable = reportModel.Entry.Sum(p => p.AmountPayable),
                            AmountRefuseCurrentPeriod = reportModel.Entry.Sum(p => p.AmountRefuseCurrentPeriod),
                            AmountRefuseTotal = reportModel.Entry.Sum(p => p.AmountRefuseTotal),
                            BeginDebt25Days = reportModel.Entry.Sum(p => p.BeginDebt25Days),
                            BeginDebtTotal = reportModel.Entry.Sum(p => p.BeginDebtTotal),
                            EndDebt25Days = reportModel.Entry.Sum(p => p.EndDebt25Days),
                            EndDebtTotal = reportModel.Entry.Sum(p => p.EndDebtTotal),
                        };
                    }

                    outResult.Data = Tuple.Create(result,count);
                }
            }
            catch (Exception exception)
            {
                 outResult.AddError(exception);
            }

            return outResult;
        }

        public OperationResult<Tuple<List<ReportReviseCustomModel>,List<ReportReviseCustomModel>, localF001, localF001>> CreateActReviseReport(DateTime beginDate, DateTime endDate, string territory)
        {
            var outResult = new OperationResult<Tuple<List<ReportReviseCustomModel>, List<ReportReviseCustomModel>, localF001, localF001>>();
            try
            {


                using (var db = CreateContext())
                {
                    var territoryDst = db.GetTableQuery<localF001>().FirstOrDefault(p => p.OKATO == territory);
                    //код ОКАТО Курская область
                    var territorySrc = db.GetTableQuery<localF001>().FirstOrDefault(p => p.OKATO == "38000");

                    if (territoryDst == null || territorySrc == null)
                    {
                       outResult.AddError("При создании отчета 'Акт сверки счетов за МП' не найдена территория с кодом ОКАТО {0}.".F(territory));
                        return outResult;
                    }

                    //все исходящии счета за отчетный период
                    var data = new List<ReportReviseCustomModel>(db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p =>
                            p.Destination == territory && 
                            p.EconomicDate!=null && 
                            p.EconomicDate >= beginDate && 
                            p.EconomicDate <= endDate && 
                            p.Direction == 0)
                        .Select(p => new ReportReviseCustomModel
                        {
                            CurrentBalance = new ReportReviseBalanceCustomModel
                            {
                                AccountId = p.TerritoryAccountId,
                                AccountNumber = p.AccountNumber,
                                EconomicDate = p.EconomicDate ?? DateTime.Now,
                                AccountDate = p.AccountDate
                            },

                            AmountFact = p.FACTTERRACCACCIDs.Any(r => r.TotalAmount > 0 && r.PaymentStatus == 2 && r.PaymentDate >= beginDate && r.PaymentDate <= endDate) ? p.FACTTERRACCACCIDs.Where(r => r.PaymentStatus == 2 && r.PaymentDate >= beginDate && r.PaymentDate <= endDate ).Sum(r => r.TotalAmount) : 0m,
                            AmountPayable = p.FACTECONPAYDTERRACCs.Any(r => r.AmountPayable > 0) ? p.FACTECONPAYDTERRACCs.Sum(r => r.AmountPayable) : 0m,
                            AmountRefuse = p.FACTECONREFTERRACCs.Any(r => r.RefuseTotalAmount > 0 && r.RefuseDate >= beginDate && r.RefuseDate <= endDate) ? p.FACTECONREFTERRACCs.Where(r => r.RefuseDate >= beginDate && r.RefuseDate <= endDate).Sum(r => r.RefuseTotalAmount) : 0m,
                        }));

                    foreach (var model in data)
                    {
                        var localModel = model;
                        //сальдо на конец отчетного периода
                        var currAccount = db.GetTableQuery<FactTerritoryAccount>()
                                .Where(p => p.TotalPaymentAmount.HasValue && p.TerritoryAccountId == localModel.CurrentBalance.AccountId)
                                .Select(p => new
                                {
                                    p.AccountNumber, 
                                    p.AccountDate, 
                                    p.AcceptPrice,
                                    FactAmount = p.FACTTERRACCACCIDs.Where(r => r.PaymentDate <= endDate && r.PaymentStatus == 2).Sum(r => r.TotalAmount)
                                })
                                .FirstOrDefault();

                        if (currAccount == null)
                        {
                            model.EndBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(model.CurrentBalance);
                            model.EndBalance.Amount = model.AmountPayable;
                           continue;
                        }
 
                        //Проверяем оплачен ли счет на конец отчетного периода
                        if (currAccount.AcceptPrice > (currAccount.FactAmount ?? 0))
                        {
                            model.EndBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(currAccount);
                            model.EndBalance.Amount = currAccount.AcceptPrice - ( currAccount.FactAmount ?? 0 );
                        }
                    }

                    //все счета с долгом на начало отчетного периода
                    var beginAccount = db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p => 
                            (p.TotalPaymentAmount??0) < p.AcceptPrice && 
                            beginDate > p.EconomicDate && 
                            p.Destination == territory)
                        .Select(p => new
                        {
                            p.AccountNumber, 
                            p.AccountDate,
                            p.EconomicDate,
                            Amount = (p.AcceptPrice ?? 0) - (p.TotalPaymentAmount ?? 0)

                        }).ToList();

                    //Обрабатываем все найденные счета
                    for (var i = 0; i < beginAccount.Count; i++)
                    {
                        //Если в data есть место, записываем данные о долге
                        if (data.Count > i)
                        {
                            data[i].BeginBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(beginAccount[i]);
                        }
                        //Иначе добавляем новую модель, только с заполненным полем долга
                        else
                        {
                            data.Add( new ReportReviseCustomModel
                            {
                                BeginBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(beginAccount[i])
                            });
                        }
                    }

                    var data2 = new List<ReportReviseCustomModel>(db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p => 
                            p.Source == territory && 
                            p.EconomicDate != null && 
                            p.EconomicDate >= beginDate && 
                            p.EconomicDate <= endDate && 
                            p.Direction == 1)
                        .Select(p => new ReportReviseCustomModel
                        {
                            CurrentBalance = new ReportReviseBalanceCustomModel
                            {
                                AccountId = p.TerritoryAccountId,
                                AccountNumber = p.AccountNumber,
                                EconomicDate = p.EconomicDate ?? DateTime.Now,
                                AccountDate = p.AccountDate
                            },

                            AmountFact = p.FACTTERRACCACCIDs.Any(r => r.TotalAmount > 0 && r.PaymentStatus == 2 && r.PaymentDate >= beginDate && r.PaymentDate <= endDate) ? p.FACTTERRACCACCIDs.Where(r => r.PaymentStatus == 2 && r.PaymentDate >= beginDate && r.PaymentDate <= endDate).Sum(r => r.TotalAmount) : 0m,
                            AmountPayable = p.FACTECONPAYDTERRACCs.Any(r => r.AmountPayable > 0) ? p.FACTECONPAYDTERRACCs.Sum(r => r.AmountPayable) : 0m,
                            AmountRefuse = p.FACTECONREFTERRACCs.Any(r => r.RefuseTotalAmount > 0 && r.RefuseDate >= beginDate && r.RefuseDate <= endDate) ? p.FACTECONREFTERRACCs.Where(r => r.RefuseDate >= beginDate && r.RefuseDate <= endDate).Sum(r => r.RefuseTotalAmount) : 0m,
                        }));

                    
                    foreach (ReportReviseCustomModel model in data2)
                    {
                        ReportReviseCustomModel localModel = model;
                        var currAccount = db.GetTableQuery<FactTerritoryAccount>()
                                .Where(p => p.TotalPaymentAmount.HasValue && p.TerritoryAccountId == localModel.CurrentBalance.AccountId)
                                .Select(p => new
                                {
                                    p.AccountNumber,
                                    p.EconomicDate,
                                    p.AcceptPrice,
                                    p.AccountDate,
                                    FactAmount = p.FACTTERRACCACCIDs.Where(r => r.PaymentDate <= endDate && r.PaymentStatus == 2).Sum(r => r.TotalAmount)
                                })
                                .FirstOrDefault();

                        if (currAccount == null)
                        {
                            model.EndBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(model.CurrentBalance);
                            model.EndBalance.Amount = model.AmountPayable;
                            continue;
                        }

                        if (currAccount.AcceptPrice > (currAccount.FactAmount??0))
                        {
                            model.EndBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(currAccount);
                            model.EndBalance.Amount = currAccount.AcceptPrice - (currAccount.FactAmount ?? 0);
                        }
                    }

                    var beginAccount2 = db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p =>  
                            (p.TotalPaymentAmount??0) < p.AcceptPrice && 
                            beginDate > p.EconomicDate && 
                            p.Source == territory)
                        .Select(p => new
                        {
                            p.AccountNumber, 
                            p.AccountDate,
                            Amount = (p.AcceptPrice??0) - (p.TotalPaymentAmount??0)

                        }).ToList();

                    for (var i = 0; i < beginAccount2.Count; i++)
                    {
                        if (data2.Count > i)
                        {
                            data2[i].BeginBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(beginAccount2[i]);
                        }
                        else
                        {
                            data2.Add(new ReportReviseCustomModel
                            {
                                BeginBalance = Map.ObjectToObject<ReportReviseBalanceCustomModel>(beginAccount2[i])
                            });
                        }
                    }

                    outResult.Data = Tuple.Create(data, data2, territorySrc, territoryDst);
                }
            }
            catch (Exception exception)
            {
                outResult.AddError(exception);            
            }

            return outResult;
        }

        public OperationResult<string>TerritoryAccountSummary(int accountId, int version)
        {
            //TODO Оптимизировать запросы к БД
            var result = new OperationResult<string>();
            
            var html = new HtmlBuilder();
            try
            {
                using (var db = CreateContext())
                {
                   
                    var accountDb = db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == accountId);
                    if (accountDb != null)
                    {
                        var exchangeDb = db.GetTableQuery<FactExchange>().FirstOrDefault(p => p.AccountId == accountId);
                        if (!Constants.ZterritoryVersion.Contains(version))
                        {
                            //TODO доделать по новому запросы и вывод
                            var totalAmountDb = db.GetTableQuery<FactEconomicAccount>().FirstOrDefault(p => p.AccountId == accountId);

                            var totalAmount = totalAmountDb != null ? totalAmountDb.TotalAmount : default(decimal?);

                            html.BeginBold()
                                .Header("Итого:", 3)
                                .Header("Фактически:", 4)
                                .EndBold()
                                .Text("Пациентов - {0}",
                                    db.GetTableQuery<FactPatient>().Count(p => p.AccountId == accountId))
                                .Text("Случаев МП - {0}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Count(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId))
                                .Text("Услуг - {0}",
                                    db.GetTableQuery<FactMedicalServices>()
                                        .Count(
                                            p =>
                                                p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.AccountId ==
                                                accountId))
                                .Text("Сумма - {0:C2}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                        .Sum(p => p.Price))
                                .Text("Сумма принятая к оплате - {0:C2}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                        .Sum(p => p.AcceptPrice))
                                .Header("Штрафные санкции:", 4)
                                .Text("Не принято решение об оплате - {0}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Count(
                                            p =>
                                                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                p.PaymentStatus == 1))
                                .Text("Полностью принятых к оплате - {0}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Count(
                                            p =>
                                                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                p.PaymentStatus == 2))
                                .Text("Полностью отказанных в оплате  - {0}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Count(
                                            p =>
                                                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                p.PaymentStatus == 3))
                                .Text("Частично отказанных в оплате - {0}",
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Count(
                                            p =>
                                                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                p.PaymentStatus == 4))
                                .Text("МЭК - {0}", db.GetTableQuery<FactMedicalEvent>()
                                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                    .Sum(p => p.MEC))
                                .Text("МЭЭ - {0}", db.GetTableQuery<FactMedicalEvent>()
                                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                    .Sum(p => p.MEE))
                                .Text("ЭКМП - {0}", db.GetTableQuery<FactMedicalEvent>()
                                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                    .Sum(p => p.EQMA))
                                .Header("Экономический отдел:", 4)
                                .Text("Оплачено фактически - {0}",
                                    totalAmount != null ? totalAmount.Value.ToString("C2") : (0).ToString("C2"))
                                .Header("По условиям оказания МП:", 4);







                            for (int i = 1; i < 5; i++)
                            {
                                if (
                                    db.GetTableQuery<FactMedicalEvent>()
                                        .Any(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                  p.AssistanceConditions == i))
                                {
                                    html.Header(
                                        "{0}".F(db.GetTableQuery<V006>().FirstOrDefault(p => p.id == i).UMPNAME), 5)
                                        .Text("Случаев МП - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.AssistanceConditions == i)
                                                .Count())
                                        .Text("&nbsp;&nbsp;Услуг - {0}",
                                            db.GetTableQuery<FactMedicalServices>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.AccountId ==
                                                        accountId &&
                                                        p.FACTMEDIFMSMEIDFACTMEDI.AssistanceConditions == i)
                                                .Count());
                                    var price =
                                        db.GetTableQuery<FactMedicalEvent>()
                                            .Where(
                                                p =>
                                                    p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                    p.AssistanceConditions == i)
                                            .Sum(p => p.Price);
                                    html.Text("Сумма - {0}",
                                        price.HasValue ? price.Value.ToString("C2") : "0");
                                    var acceptPrice =
                                        db.GetTableQuery<FactMedicalEvent>()
                                            .Where(
                                                p =>
                                                    p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                    p.AssistanceConditions == i)
                                            .Sum(p => p.AcceptPrice);
                                    html.Text("Сумма принятая к оплате - {0}",
                                        acceptPrice.HasValue ? acceptPrice.Value.ToString("C2") : "0")
                                        .Header("Штрафные санкции:", 5)
                                        .Text("&nbsp;&nbsp;Не принято решение об оплате - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.PaymentStatus == 1 &&
                                                        p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Полностью принятых к оплате - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.PaymentStatus == 2 &&
                                                        p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Полностью отказанных в оплате - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.PaymentStatus == 3 &&
                                                        p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Частично отказанных в оплате - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.PaymentStatus == 4 &&
                                                        p.AssistanceConditions == i)
                                                .Count())
                                        .Text("МЭК - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.AssistanceConditions == i)
                                                .Sum(p => p.MEC))
                                        .Text("МЭЭ - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.AssistanceConditions == i)
                                                .Sum(p => p.MEE))
                                        .Text("ЭКМП - {0}",
                                            db.GetTableQuery<FactMedicalEvent>()
                                                .Where(
                                                    p =>
                                                        p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                                        p.AssistanceConditions == i)
                                                .Sum(p => p.EQMA));

                                    html.Header("Экономический отдел:", 4);
                                    decimal totalAmountType = 0;
                                    if (totalAmountDb != null)
                                    {
                                        totalAmountType =
                                            db.GetTableQuery<FactEconomicPayment>()
                                                .Where(
                                                    p =>
                                                        p.EconomicAccountId == totalAmountDb.EconomicAccountId &&
                                                        p.AssistanceConditionsId == i)
                                                .Sum(p => p.Amount);

                                    }
                                    html.Text("Оплачено фактически - {0}",
                                        totalAmountType.ToString("C2"));
                                }
                            }

                            html.Header("В записи счета:", 4)
                                .Text("Сумма - {0}", accountDb.Price.HasValue
                                    ? accountDb.Price.Value.ToString("C2")
                                    : "0")
                                .Text("Сумма принятая к оплате - {0}", accountDb.AcceptPrice.HasValue
                                    ? accountDb.AcceptPrice.Value.ToString("C2")
                                    : "0")
                                .Text("МЭК - {0}", accountDb.MECPenalties.HasValue
                                    ? accountDb.MECPenalties.Value.ToString("C2")
                                    : "0")
                                .Header("Журнал обмена:", 4);

                            if (exchangeDb != null)
                            {
                                html.Text("№ и имя файла - {0}", exchangeDb.FileName);
                                html.Text("Дата загрузки файла - {0}", exchangeDb.ActionDate.ToString("dd.MM.yyyy"));
                            }
                        }
                        else
                        {
                            var zslFactMedicalEvent = db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId);

                            html.BeginBold()
                                    .Header("Итого:", 3)
                                    .Header("Фактически:", 4)
                                    .EndBold()
                                    .Text("Пациентов - {0}", db.GetTableQuery<FactPatient>().Count(p => p.AccountId == accountId))
                                    .Text("Законченных случаев МП - {0}", zslFactMedicalEvent.Count())
                                    .Text("Cлучаев МП - {0}", db.GetTableQuery<ZFactMedicalEvent>()
                                            .Count(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId))
                                    .Text("Услуг - {0}", db.GetTableQuery<ZFactMedicalServices>()
                                            .Count(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId))
                                    .Text("Сумма - {0:C2}", zslFactMedicalEvent.Sum(p => p.Price))
                                    .Text("Сумма принятая к оплате - {0:C2}", zslFactMedicalEvent.Sum(p => p.AcceptPrice))
                                    .Header("Штрафные санкции:", 4).Text("Не принято решение об оплате - {0}", zslFactMedicalEvent.Count(p => p.PaymentStatus == 1))
                                    .Text("Полностью принятых к оплате - {0}", zslFactMedicalEvent.Count(p => p.PaymentStatus == 2))
                                    .Text("Полностью отказанных в оплате  - {0}", zslFactMedicalEvent.Count(p => p.PaymentStatus == 3))
                                    .Text("Частично отказанных в оплате - {0}", zslFactMedicalEvent.Count(p => p.PaymentStatus == 4));
                            for (int i = 1; i < 5; i++)
                            {
                                if (
                                    zslFactMedicalEvent.Any(p => p.AssistanceConditions == i))
                                {
                                    html.Header(
                                        "{0}".F(db.GetTableQuery<V006>().FirstOrDefault(p => p.id == i).UMPNAME), 5)
                                        .Text("Законченных случаев МП - {0}",
                                            zslFactMedicalEvent.Where(p => p.AssistanceConditions == i).Count())
                                        .Text("&nbsp;&nbsp;Услуг - {0}",
                                            db.GetTableQuery<ZFactMedicalServices>()
                                                .Where(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId ==
                                                        accountId && p.ZFactMedicalEvent.ZslFactMedicalEvent.AssistanceConditions == i)
                                                .Count());
                                    var price =
                                        zslFactMedicalEvent
                                            .Where(p => p.AssistanceConditions == i)
                                            .Sum(p => p.Price);
                                    html.Text("Сумма - {0}",
                                        price.HasValue ? price.Value.ToString("C2") : "0");
                                    var acceptPrice =
                                        zslFactMedicalEvent
                                            .Where(p => p.AssistanceConditions == i)
                                            .Sum(p => p.AcceptPrice);
                                    html.Text("Сумма принятая к оплате - {0}",
                                        acceptPrice.HasValue ? acceptPrice.Value.ToString("C2") : "0")
                                        .Header("Штрафные санкции:", 5)
                                        .Text("&nbsp;&nbsp;Не принято решение об оплате - {0}",
                                            zslFactMedicalEvent
                                                .Where(p =>
                                                    p.PaymentStatus == 1 &&
                                                    p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Полностью принятых к оплате - {0}",
                                            zslFactMedicalEvent
                                                .Where(
                                                    p => p.PaymentStatus == 2 &&
                                                         p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Полностью отказанных в оплате - {0}",
                                            zslFactMedicalEvent
                                                .Where(p =>
                                                    p.PaymentStatus == 3 &&
                                                    p.AssistanceConditions == i)
                                                .Count())
                                        .Text("Частично отказанных в оплате - {0}",
                                            zslFactMedicalEvent
                                                .Where(p =>
                                                    p.PaymentStatus == 4 &&
                                                    p.AssistanceConditions == i)
                                                .Count());
                                        //.Text("МЭК - {0}",
                                        //    zslFactMedicalEvent
                                        //        .Where(
                                        //            p =>p.AssistanceConditions == i)
                                        //        .Sum(p => p.MEC))
                                        //.Text("МЭЭ - {0}",
                                        //    db.GetTableQuery<FactMedicalEvent>()
                                        //        .Where(
                                        //            p =>
                                        //                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                        //                p.AssistanceConditions == i)
                                        //        .Sum(p => p.MEE))
                                        //.Text("ЭКМП - {0}",
                                        //    db.GetTableQuery<FactMedicalEvent>()
                                        //        .Where(
                                        //            p =>
                                        //                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                        //                p.AssistanceConditions == i)
                                        //        .Sum(p => p.EQMA));

                                   

                                }
                            }
                        }
                    }
                   
                }
                result.Data = html.Value();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }


        public OperationResult<string> DoRegisterEStats<T>(T data) where T : IRegister
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр успешно выгружен</b><br>");
                sb.AppendLine("Выгружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Select(r => r.InnerServiceCollection.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
            
        }

        public OperationResult<string> DoZRegisterEStats<T>(T data) where T : IZRegister
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр успешно выгружен</b><br>");
                sb.AppendLine("Выгружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Законченных случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Count()).Sum()));
                sb.AppendLine("Cлучаев оказанния МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Select(r=>r.InnerMeventCollection.Count()).Sum()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(               data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Select(r => r.InnerMeventCollection.Select(y=>y.InnerServiceCollection.Count()).Sum()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;

        }

        public OperationResult<string> DoRegisterEAnswerStats<T>(T data) where T : IRegisterAnswer
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр успешно выгружен</b><br>");
                sb.AppendLine("Выгружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoZRegisterEAnswerStats<T>(T data) where T : IZRegisterAnswer
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр успешно выгружен</b><br>");
                sb.AppendLine("Выгружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Законченных случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        /// <summary>
        /// Сведения о платежном поручении Ashurkova
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperationResult<string> DoRegisterPlAnswerStats<T>(T data) where T : IRegisterPlAnswer
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Сведения об оплате успешно выгружены</b><br>");
                sb.AppendLine("Выгружено:<br>");
                sb.AppendLine("Всего к оплате - {0}<br>".F(
                                            data.InnerAccount.Total.HasValue
                                                ? data.InnerAccount.Total.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Номер платежного поручения - {0}<br>".F(data.InnerAccount.PaymentOrderNumber));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;

        }

        public OperationResult<string> DoRegisterDLoadStats(AccountRegisterD data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр МО успешно загружен в базу данных</b><br>");
                sb.AppendLine("Загружено из xml:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.RecordsCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Select(r => r.Service.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(data.Account.Price.HasValue
                                                ? data.Account.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(data.Account.AcceptPrice.HasValue
                                                ? data.Account.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("МЭК - {0}<br>".F(data.Account.MECPenalties.HasValue
                    ? data.Account.MECPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("МЭЭ - {0}<br>".F(data.Account.MEEPenalties.HasValue
                    ? data.Account.MEEPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("ЭКМП - {0}<br>".F(data.Account.EQMAPenalties.HasValue
                    ? data.Account.EQMAPenalties.Value.ToString("C2")
                    : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterDLoadStats(v31K1.D.AccountRegisterD data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр МО успешно загружен в базу данных</b><br>");
                sb.AppendLine("Загружено из xml:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.RecordsCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Select(r => r.Event.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(data.Account.Price.HasValue
                                                ? data.Account.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(data.Account.AcceptPrice.HasValue
                                                ? data.Account.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("МЭК - {0}<br>".F(data.Account.MECPenalties.HasValue
                    ? data.Account.MECPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("МЭЭ - {0}<br>".F(data.Account.MEEPenalties.HasValue
                    ? data.Account.MEEPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("ЭКМП - {0}<br>".F(data.Account.EQMAPenalties.HasValue
                    ? data.Account.EQMAPenalties.Value.ToString("C2")
                    : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterDLoadStats(v31K1.DV.AccountRegisterD data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр МО успешно загружен в базу данных</b><br>");
                sb.AppendLine("Загружено из xml:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.RecordsCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Select(r => r.Event.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(data.Account.Price.HasValue
                                                ? data.Account.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(data.Account.AcceptPrice.HasValue
                                                ? data.Account.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("МЭК - {0}<br>".F(data.Account.MECPenalties.HasValue
                    ? data.Account.MECPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("МЭЭ - {0}<br>".F(data.Account.MEEPenalties.HasValue
                    ? data.Account.MEEPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("ЭКМП - {0}<br>".F(data.Account.EQMAPenalties.HasValue
                    ? data.Account.EQMAPenalties.Value.ToString("C2")
                    : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterDLoadStats(v32K1.D.AccountRegisterD data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр МО успешно загружен в базу данных</b><br>");
                sb.AppendLine("Загружено из xml:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.RecordsCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Select(r => r.Event.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(data.Account.Price.HasValue
                                                ? data.Account.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(data.Account.AcceptPrice.HasValue
                                                ? data.Account.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("МЭК - {0}<br>".F(data.Account.MECPenalties.HasValue
                    ? data.Account.MECPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("МЭЭ - {0}<br>".F(data.Account.MEEPenalties.HasValue
                    ? data.Account.MEEPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("ЭКМП - {0}<br>".F(data.Account.EQMAPenalties.HasValue
                    ? data.Account.EQMAPenalties.Value.ToString("C2")
                    : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }
        public OperationResult<string> DoRegisterDLoadStats(v30K1.D.AccountRegisterD data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<b>Реестр МО успешно загружен в базу данных</b><br>");
                sb.AppendLine("Загружено из xml:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.RecordsCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.RecordsCollection.Select(p => p.EventCollection.Select(r => r.Event.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(data.Account.Price.HasValue
                                                ? data.Account.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(data.Account.AcceptPrice.HasValue
                                                ? data.Account.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("МЭК - {0}<br>".F(data.Account.MECPenalties.HasValue
                    ? data.Account.MECPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("МЭЭ - {0}<br>".F(data.Account.MEEPenalties.HasValue
                    ? data.Account.MEEPenalties.Value.ToString("C2")
                    : "0"));
                sb.AppendLine("ЭКМП - {0}<br>".F(data.Account.EQMAPenalties.HasValue
                    ? data.Account.EQMAPenalties.Value.ToString("C2")
                    : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterELoadStats(IRegister data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Загружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Select(r => r.InnerServiceCollection.Count()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterELoadStats(IZRegister data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Загружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Законченных случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Count()).Sum()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Select(r=>r.InnerMeventCollection.Count()).Sum()).Sum()));
                sb.AppendLine("Услуг - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerZslEventCollection.Select(r => r.InnerMeventCollection.Select(y => y.InnerServiceCollection.Count()).Sum()).Sum()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterELoadStats(IRegisterAnswer data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Загружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> DoRegisterELoadStats(IZRegisterAnswer data)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Загружено:<br>");
                sb.AppendLine("Пациентов - {0}<br>".F(data.InnerRecordCollection.Count()));
                sb.AppendLine("Законченных случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Count()).Sum()));
                sb.AppendLine("Случаев оказанной МП - {0}<br>".F(data.InnerRecordCollection.Select(p => p.InnerEventCollection.Select(x=>x.InnerEventCollection.Count()).Count()).Sum()));
                sb.AppendLine("Сумма - {0}<br>".F(
                                            data.InnerAccount.Price.HasValue
                                                ? data.InnerAccount.Price.Value.ToString("C2")
                                                : "0"));
                sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(
                                            data.InnerAccount.AcceptPrice.HasValue
                                                ? data.InnerAccount.AcceptPrice.Value.ToString("C2")
                                                : "0"));
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }



        public OperationResult<string> DoRegisterEDbStats(int id)
        {
            var result = new OperationResult<string>();
            try
            {
                using (var db = CreateContext())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("В базе данных:<br>");
                    sb.AppendLine("Пациентов - {0}<br>".F(db.FactPatient.Count(p=>p.AccountId == id)));
                    sb.AppendLine("Случаев оказанной МП - {0}<br>".F(db.FactMedicalEvent.Count(p=>p.FACTMEDIPATIENTIDFACTPATI.AccountId == id)));
                    sb.AppendLine("Услуг - {0}<br>".F(db.FactMedicalServices.Count(p=>p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.AccountId == id)));
                    sb.AppendLine("Сумма - {0}<br>".F(db.FactMedicalEvent.Where(p=>p.FACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p=>p.Price)));
                    sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(db.FactMedicalEvent.Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p => p.AcceptPrice)));
                    result.Data = sb.ToString();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;           
        }

        public OperationResult<string> DoZRegisterEDbStats(int id)
        {
            var result = new OperationResult<string>();
            try
            {
                using (var db = CreateContext())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("В базе данных:<br>");
                    sb.AppendLine("Пациентов - {0}<br>".F(db.FactPatient.Count(p => p.AccountId == id)));
                    sb.AppendLine("Законченных случаев оказанной МП - {0}<br>".F(db.ZslFactMedicalEvent.Count(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id)));
                    sb.AppendLine("Случаев оказанной МП - {0}<br>".F(db.ZFactMedicalEvent.Count(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id)));
                    sb.AppendLine("Услуг - {0}<br>".F(db.ZFactMedicalServices.Count(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id)));
                    sb.AppendLine("Сумма - {0}<br>".F(db.ZslFactMedicalEvent.Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p => p.Price)));
                    sb.AppendLine("Сумма принятая к оплате - {0}<br>".F(db.ZslFactMedicalEvent.Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p => p.AcceptPrice)));
                    result.Data = sb.ToString();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<string> MedicalAccountSummary(int medicalAccountId, int version)
        {
             var result = new OperationResult<string>();
            string ResultText = string.Empty;
            try
            {
                using (var db = CreateContext())
                {
                    var accountDb = db.GetTableQuery<FactMedicalAccount>().FirstOrDefault(p => p.MedicalAccountId == medicalAccountId);
                    if (accountDb != null)
                    {
                        var exchangeDb = db.FactExchange.FirstOrDefault(p => p.AccountId == medicalAccountId && p.Type == 5);
                        ResultText += string.Format("<b>Итого:</b><br>");
                        ResultText += string.Format("<b>Фактически:</b><br>");

                        if (Constants.ZmedicalVersion.Contains(version))
                        {
                            ResultText += string.Format("Пациентов - {0}<br>", db.GetTableQuery<FactPatient>().Where(p => p.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Законченных случаев МП - {0}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Случаев МП - {0}<br>", db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Услуг - {0}<br>", db.GetTableQuery<ZFactMedicalServices>().Where(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Сумма - {0:C2}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.Price) ?? 0);
                            ResultText += string.Format("Сумма принятая к оплате - {0:C2}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.AcceptPrice) ?? 0);
                            ResultText += string.Format("<b>Штрафные санкции:</b><br>");
                            ResultText += string.Format("Не принято решение об оплате - {0}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 1).Count());
                            ResultText += string.Format("Полностью принятых к оплате - {0}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 2).Count());
                            ResultText += string.Format("Полностью отказанных в оплате - {0}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 3).Count());
                            ResultText += string.Format("Частично отказанных в оплате - {0}<br>", db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 4).Count());
                            ResultText += string.Format("МЭК - {0:C2}<br>", db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.MEC) ?? 0);
                            ResultText += string.Format("МЭЭ - {0:C2}<br>", db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.MEE) ?? 0);
                            ResultText += string.Format("ЭКМП - {0:C2}<br>", db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.EQMA) ?? 0);

                        }
                        else
                        {
                            ResultText += string.Format("Пациентов - {0}<br>", db.GetTableQuery<FactPatient>().Where(p => p.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Случаев МП - {0}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Услуг - {0}<br>", db.GetTableQuery<FactMedicalServices>().Where(p => p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Count());
                            ResultText += string.Format("Сумма - {0:C2}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.Price) ?? 0);
                            ResultText += string.Format("Сумма принятая к оплате - {0:C2}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.AcceptPrice) ?? 0);
                            ResultText += string.Format("<b>Штрафные санкции:</b><br>");
                            ResultText += string.Format("Не принято решение об оплате - {0}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 1).Count());
                            ResultText += string.Format("Полностью принятых к оплате - {0}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 2).Count());
                            ResultText += string.Format("Полностью отказанных в оплате - {0}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 3).Count());
                            ResultText += string.Format("Частично отказанных в оплате - {0}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId && p.PaymentStatus == 4).Count());
                            ResultText += string.Format("МЭК - {0:C2}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.MEC) ?? 0);
                            ResultText += string.Format("МЭЭ - {0:C2}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.MEE) ?? 0);
                            ResultText += string.Format("ЭКМП - {0:C2}<br>", db.GetTableQuery<FactMedicalEvent>().Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId).Sum(p => p.EQMA) ?? 0);

                             
                        }
                        ResultText += string.Format("<b>В записи счета:</b><br>");
                        ResultText += string.Format("Сумма - {0}<br>", accountDb.Price.HasValue
                                                ? accountDb.Price.Value.ToString("C2")
                                                : "0");
                        ResultText += string.Format("Сумма принятая к оплате - {0}<br>", accountDb.AcceptPrice.HasValue
                                                ? accountDb.AcceptPrice.Value.ToString("C2")
                                                : "0");
                        ResultText += string.Format("МЭК - {0}<br>", accountDb.MECPenalties.HasValue
                                                ? accountDb.MECPenalties.Value.ToString("C2")
                                                : "0");
                        ResultText += string.Format("МЭЭ - {0}<br>", accountDb.MEEPenalties.HasValue
                                               ? accountDb.MEEPenalties.Value.ToString("C2")
                                               : "0");
                        ResultText += string.Format("ЭКМП - {0}<br>", accountDb.EQMAPenalties.HasValue
                                               ? accountDb.EQMAPenalties.Value.ToString("C2")
                                               : "0");

                        ResultText += string.Format("<b>Журнал обмена:</b><br>");
                        if (exchangeDb != null)
                        {
                            ResultText += string.Format("№ и имя файла - {0}<br>", exchangeDb.FileName);
                            ResultText += string.Format("Дата загрузки файла - {0}<br>", exchangeDb.ActionDate.ToString("dd.MM.yyyy"));
                        }

                    }
                }
                result.Data = ResultText;
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
                

            
        }
    }
}
