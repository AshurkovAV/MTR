using System.Collections.Generic;

namespace Medical.AppLayer.Economic.Models.Reports
{
    /// <summary> 
    /// Модель данных для отчета "Форма2"
    /// </summary>
    public class ReportForm2CustomModel
    {
        public ReportForm2CustomModel()
        {
            Entry = new List<ReportForm2Entry>();
        }

        // <summary> 
        /// Номер строки
        /// </summary> 
        public int Row { get; set; }
        /// <summary> 
        /// Данные для округа
        /// </summary> 
        public List<ReportForm2Entry> Entry { get; set; }

        /// <summary> 
        /// Код округа 
        /// </summary> 
        public int DistrictCode { get; set; }

        /// <summary> 
        /// Наименование округа 
        /// </summary> 
        public string DistrictName { get; set; }

        /// <summary> 
        /// Данные для округа
        /// </summary> 
        public ReportForm2Entry DistrictEntry { get; set; }
    }

    public class ReportForm2Entry
    {
        /// <summary> 
        /// Номер строки
        /// </summary> 
        public int Row { get; set; }
        /// <summary> 
        /// Наименование региона 
        /// </summary> 
        public string Region { get; set; }
        /// <summary> 
        /// ОКАТО территории 
        /// </summary> 
        public string TerritoryOkato { get; set; }
        /// <summary> 
        /// Наименование территории 
        /// </summary> 
        public string TerritoryName { get; set; }
        /// <summary> 
        /// Дебиторская задолженность на начало отчетного периода Всего
        /// </summary> 
        public decimal BeginDebtTotal { get; set; }
        /// <summary> 
        /// Дебиторская задолженность на начало отчетного периода Cвыше 25 дней
        /// </summary> 
        public decimal BeginDebt25Days { get; set; }
        /// <summary> 
        /// предъявленно в отчетном периоде
        /// </summary> 
        public decimal AmountPayable { get; set; }
        /// <summary> 
        /// Отказы Всего
        /// </summary>
        public decimal AmountRefuseTotal { get; set; }
        /// <summary> 
        /// Отказы По счетам отчетного периода
        /// </summary>
        public decimal AmountRefuseCurrentPeriod { get; set; }
        /// <summary> 
        /// Перечисленно Всего
        /// </summary>
        public decimal AmountFactTotal { get; set; }
        /// <summary> 
        /// Перечисленно по счетам отчетного периода 
        /// </summary>
        public decimal AmountFactCurrentPeriod { get; set; }
        /// <summary> 
        /// Перечисленно за предыдущ.годы (доплаты прошлых лет)
        /// </summary>
        public decimal AmountFactLastYears { get; set; }
        /// <summary> 
        /// дебит.зад.на конц отч. периода всего на конец отчетного периода
        /// </summary>
        public decimal EndDebtTotal { get; set; }
        /// <summary> 
        /// дебит.зад.на конц отч. периода свыше 25 раб дней
        /// </summary>
        public decimal EndDebt25Days { get; set; }
    }
}
