using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Core.Extensions;

namespace Medical.AppLayer.Models.PolicySearch
{
    public class SearchPeopleByPolicyModel
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string Surname{ get; set; }
        public string Patronymic{ get; set; }
        public int? Sex{ get; set; }
        public DateTime? Birthday{ get; set; }
        public string Inp { get; set; }

        public string FullName {
            get { return "{0} {1} {2} {3} {4}".F(Surname, FirstName, Patronymic, Birthday.ToFormatString(), Sex.HasValue ? ((SexEnum)Sex).GetDisplayShortName() : string.Empty ); }
        }
    }
}
