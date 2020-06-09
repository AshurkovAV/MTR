using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Core.Helpers;

namespace Medical.DataCore.v10PL.PL
{
    [XmlRoot("PL_S", IsNullable = true, Namespace = "")]
    public class RegisterPlAnswer : IRegisterPlAnswer
    {

        [XmlIgnore]
        public InformationPlAnswer InnerAccount { get; set; }

        /// <summary>
        /// Имя файла со сведениями об оплате
        /// </summary>
        [XmlElement(ElementName = "FNAME")]
        public string FileName { get; set; }

       [XmlElement(ElementName = "SVED")]
        public InformationPlAnswer Account
        {
            get { return InnerAccount as InformationPlAnswer; }
            set { InnerAccount = value; }
        }
    }

    public class InformationPlAnswer : IInformationPlAnswer
    {
        public InformationPlAnswer()
        {
            _innerAccountCollection = new List<AccountPlAnswer>();
        }

        [XmlIgnore]
        private List<AccountPlAnswer> _innerAccountCollection { get; set; }

        [XmlIgnore]
        public RecipientInformationPlAnswer InnerRecipient { get; set; }

        [XmlIgnore]
        public PayerInformationPlAnswer InnerPayer { get; set; }
        
        /// <summary>
        /// Номер платежного поручения
        /// </summary>
        [XmlElement(ElementName = "N_PLPR")]
        public string PaymentOrderNumber { get; set; }

        [XmlIgnore]
        public DateTime? DatePaymentOrder { get; set; }

        /// <summary>
        /// Дата платежного поручения
        /// </summary>
        [XmlElement(ElementName = "D_PLPR")]
        public string DatePaymentOrderXml
        {
            get { return DatePaymentOrder != null ? DatePaymentOrder.Value.ToString("yyyy-MM-dd") : null; }
            set { DatePaymentOrder = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        }

        [XmlElement(ElementName = "KOL_SCH")]
        public string NumberAccount { get; set; }
        
        [XmlElement(ElementName = "SCH")]
        public List<AccountPlAnswer> Account
        {
            get { return _innerAccountCollection; }
            set { _innerAccountCollection = value; }
        }

        [XmlElement(ElementName = "ITOG")]
        public decimal? Total { get; set; }

        [XmlElement(ElementName = "PRED")]
        public string SubjectOfPayment { get; set; }

        [XmlElement(ElementName = "POL")]
        public RecipientInformationPlAnswer Recipient
        {
            get { return InnerRecipient as RecipientInformationPlAnswer; }
            set { InnerRecipient = value; }
        }

        [XmlElement(ElementName = "PLAT")]
        public PayerInformationPlAnswer Payer
        {
            get { return InnerPayer as PayerInformationPlAnswer; }
            set { InnerPayer = value; }
        }
    }

    public class AccountPlAnswer : IAccountPlAnswer
    {
        [XmlElement(ElementName = "N_SCH")]
        public string AccountNumber { get; set; }

        [XmlElement(ElementName = "D_SCH")]
        public string AccountDateXml
        {
            get { return AccountDate != null ? AccountDate.Value.ToString("yyyy-MM-dd") : null; }
            set { AccountDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        }
        /// <summary>
        /// Имя исходного реестра счета
        /// </summary>
        [XmlElement(ElementName = "FNAME_I")]
        public string NameIshodReestr { get; set; }

        //[XmlElement(ElementName = "SUM_SCH")]
        //public string SuumAccountXml {
        //    get { return SuumAccount?.ToString(); }
        //    set { SuumAccount = SafeConvert.ToDecimal(value); }
        //}

        [XmlIgnore]
        public DateTime? AccountDate { get; set; }

        [XmlElement(ElementName = "SUM_SCH")]
        public decimal? SuumAccount { get; set; }

        [XmlElement(ElementName = "SL_SCH")]
        public string NumberSlAccountXml
        {
            get { return NumberSlAccount?.ToString(); }
            set { NumberSlAccount = SafeConvert.ToInt32(value); }
        }

        [XmlIgnore]
        public int? NumberSlAccount { get; set; }
    }

    public class RecipientInformationPlAnswer : IRecipientInformationPlAnswer
    {
        [XmlElement(ElementName = "L_NAIM")]
        public string NameRecipient { get; set; }

        [XmlElement(ElementName = "L_A")]
        public string AdressRecipient { get; set; }

        [XmlElement(ElementName = "L_B")]
        public string BankRecipient { get; set; }

        [XmlElement(ElementName = "L_RS")]
        public string RsRecipient { get; set; }

        [XmlElement(ElementName = "L_BIC")]
        public string BicRecipient { get; set; }

        [XmlElement(ElementName = "L_IN")]
        public string InnRecipient { get; set; }

        [XmlElement(ElementName = "L_KP")]
        public string KppRecipient { get; set; }

        [XmlElement(ElementName = "L_KB")]
        public string KbkRecipient { get; set; }

        [XmlElement(ElementName = "L_OKTMO")]
        public string OktmoRecipient { get; set; }

       
    }

    public class PayerInformationPlAnswer : IPayerInformationPlAnswer
    {
        [XmlElement(ElementName = "T_NAIM")]
        public string NamePayer { get; set; }

        [XmlElement(ElementName = "T_A")]
        public string AdressPayer { get; set; }

        [XmlElement(ElementName = "T_B")]
        public string BankPayer { get; set; }

        [XmlElement(ElementName = "T_RS")]
        public string RsPayer { get; set; }

        [XmlElement(ElementName = "T_BIC")]
        public string BicPayer { get; set; }

        [XmlElement(ElementName = "T_IN")]
        public string InnPayer { get; set; }

        [XmlElement(ElementName = "T_KP")]
        public string KppPayer { get; set; }
        
        [XmlElement(ElementName = "T_OKTMO")]
        public string OktmoPayer { get; set; }

    }




}
