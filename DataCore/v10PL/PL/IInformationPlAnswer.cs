using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DataCore.v10PL.PL
{
    public interface IInformationPlAnswer
    {
        string PaymentOrderNumber { get; set; }
        DateTime? DatePaymentOrder { get; set; }
        decimal? Total { get; set; }
        string SubjectOfPayment { get; set; }
        RecipientInformationPlAnswer InnerRecipient { get; set; }
        PayerInformationPlAnswer InnerPayer { get; set; }
    }
}
