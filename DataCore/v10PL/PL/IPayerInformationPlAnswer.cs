using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DataCore.v10PL.PL
{
    interface IPayerInformationPlAnswer
    {
        string NamePayer { get; set; }
        string AdressPayer { get; set; }
        string BankPayer { get; set; }
        string RsPayer { get; set; }
        string BicPayer { get; set; }
        string InnPayer { get; set; }
        string KppPayer { get; set; }
        string OktmoPayer { get; set; }
    }
}
