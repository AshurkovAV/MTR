using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DataCore.v10PL.PL
{
    internal interface IRecipientInformationPlAnswer
    {
        string NameRecipient { get; set; }
        string AdressRecipient { get; set; }
        string BankRecipient { get; set; }
        string RsRecipient { get; set; }
        string BicRecipient { get; set; }
        string InnRecipient { get; set; }
        string KppRecipient { get; set; }
        string KbkRecipient { get; set; }
        string OktmoRecipient { get; set; }
    }
}
