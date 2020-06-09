using System;
using DataModel;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Models
{
    public class EventDetailView : ViewModelBase 
    {
        private EventShortView _eventShortView;
        private FactTerritoryAccount _account;

        public EventDetailView(int eventId)
        {
            /*using (var db = new DatabaseIns())
            {
                _eventShortView = db.GetTableQuery<EventShortView>().FirstOrDefault(p => p.EventId == eventId);
                var accountList = db.GetTableQuery<FactMedicalEvent>().Where(p => p.MedicalEventId == eventId).Select(p => p.FACTMEDIPATIENTIDFACTPATI.FACTTERACCOUNTID).ToList();
                if (accountList.Count > 0)
                {
                    _account = accountList.First();
                }
            }*/
        }

        public int? PatientId
        {
            get
            {
                return _eventShortView.PatientId;
            }
        }

        public int EventId
        {
            get
            {
                return _eventShortView.EventId;
            }
        }

        public string Surname
        {
            get
            {
                return _eventShortView.Surname;
            }
        }

        public string Name
        {
            get
            {
                return _eventShortView.Name;
            }
        }

        public string Patronymic
        {
            get
            {
                return _eventShortView.Patronymic;
            }
        }

        public string Sex
        {
            get
            {
                return _eventShortView.Sex;
            }
        }

        public DateTime? Birthday
        {
            get
            {
                return _eventShortView.Birthday;
            }
        }

        public string INP
        {
            get
            {
                return _eventShortView.INP;
            }
        }

        public string InsuranceDocNumber
        {
            get
            {
                return _eventShortView.InsuranceDocNumber;
            }
        }

        public string InsuranceDocSeries
        {
            get
            {
                return _eventShortView.InsuranceDocSeries;
            }
        }

        public string Insurance
        {
            get
            {
                return _eventShortView.Insurance;
            }
        }

        public DateTime? EventBegin
        {
            get
            {
                return _eventShortView.EventBegin;
            }
        }

        public DateTime? EventEnd
        {
            get
            {
                return _eventShortView.EventEnd;
            }
        }

        public string Diagnosis
        {
            get
            {
                return _eventShortView.Diagnosis;
            }
        }

        public decimal? Rate
        {
            get
            {
                return _eventShortView.Rate;
            }
        }

        public decimal? Quantity
        {
            get
            {
                return _eventShortView.Quantity;
            }
        }

        public decimal? Price
        {
            get
            {
                return _eventShortView.Price;
            }
        }

        public decimal? MEC
        {
            get
            {
                return _eventShortView.MEC;
            }
        }

        public decimal? MEE
        {
            get
            {
                return _eventShortView.MEE;
            }
        }

        public decimal? EQMA
        {
            get
            {
                return _eventShortView.EQMA;
            }
        }

        public decimal? AcceptPrice
        {
            get
            {
                return _eventShortView.AcceptPrice;
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
