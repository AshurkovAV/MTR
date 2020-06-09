using System;
using DataModel;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Models
{
    public class PatientDetailView : ViewModelBase 
    {
        private PatientShortView _patientShortView;
        private FactTerritoryAccount _account;

        public PatientDetailView(int patientId)
        {
           /* using (var db = new DatabaseIns())
            {
                _patientShortView = db.GetTableQuery<PatientShortView>().FirstOrDefault(p => p.PatientId == patientId);
                var accountList = db.GetTableQuery<FactPatient>().Where(p => p.PatientId == patientId).Select(p => p.FACTTERACCOUNTID).ToList();
                if (accountList.Count > 0)
                {
                    _account = accountList.First();
                }
            }*/
        }

        public int PatientId
        {
            get
            {
                return _patientShortView.PatientId;
            }
        }

        public string Surname
        {
            get
            {
                return _patientShortView.Surname;
            }
        }

        public string Name
        {
            get
            {
                return _patientShortView.Name;
            }
        }

        public string Patronymic
        {
            get
            {
                return _patientShortView.Patronymic;
            }
        }

        public string Sex
        {
            get
            {
                return _patientShortView.Sex;
            }
        }

        public DateTime? Birthday
        {
            get
            {
                return _patientShortView.Birthday;
            }
        }

        public string INP
        {
            get
            {
                return _patientShortView.INP;
            }
        }

        public string InsuranceDocNumber
        {
            get
            {
                return _patientShortView.InsuranceDocNumber;
            }
        }

        public string InsuranceDocSeries
        {
            get
            {
                return _patientShortView.InsuranceDocSeries;
            }
        }

        public string Insurance
        {
            get
            {
                return _patientShortView.Insurance;
            }
        }

        public int? EventCount
        {
            get
            {
                return _patientShortView.EventCount;
            }
        }

        public string Source
        {
            get
            {
                return _account.Source;
            }
        }

        public string Destination
        {
            get
            {
                return _account.Destination;
            }
        }

        public int? Direction
        {
            get
            {
                return _account.Direction;
            }
        }

        public string AccountNumber
        {
            get
            {
                return _account.AccountNumber;
            }
        }

        public int? Type
        {
            get
            {
                return _account.Type;
            }
        }

        public int? TerritoryAccountId
        {
            get
            {
                return _account.TerritoryAccountId;
            }
        }
        
    }
}
