using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DatabaseCore.EntityDataModel
{
    public partial class EconomicPartner
    {

        private int _EconomicPartnerId;

        private string _Version;

        private System.Nullable<System.DateTime> _Date;

        private string _Name;

        private string _Adr;

        private string _Bank;

        private string _Rs;

        private string _Bic;

        private string _Inn;

        private string _Kpp;

        private string _Kbk;

        private string _Oktmo;

        private string _Okato;

        private string _Ogrn;

        public EconomicPartner()
        {
        }

        public int EconomicPartnerId
        {
            get
            {
                return this._EconomicPartnerId;
            }
            set
            {
                if ((this._EconomicPartnerId != value))
                {
                    this._EconomicPartnerId = value;
                }
            }
        }

        public string Version
        {
            get
            {
                return this._Version;
            }
            set
            {
                if ((this._Version != value))
                {
                    this._Version = value;
                }
            }
        }

        public System.Nullable<System.DateTime> Date
        {
            get
            {
                return this._Date;
            }
            set
            {
                if ((this._Date != value))
                {
                    this._Date = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value))
                {
                    this._Name = value;
                }
            }
        }

        public string Adr
        {
            get
            {
                return this._Adr;
            }
            set
            {
                if ((this._Adr != value))
                {
                    this._Adr = value;
                }
            }
        }

        public string Bank
        {
            get
            {
                return this._Bank;
            }
            set
            {
                if ((this._Bank != value))
                {
                    this._Bank = value;
                }
            }
        }

        public string Rs
        {
            get
            {
                return this._Rs;
            }
            set
            {
                if ((this._Rs != value))
                {
                    this._Rs = value;
                }
            }
        }

        public string Bic
        {
            get
            {
                return this._Bic;
            }
            set
            {
                if ((this._Bic != value))
                {
                    this._Bic = value;
                }
            }
        }

        public string Inn
        {
            get
            {
                return this._Inn;
            }
            set
            {
                if ((this._Inn != value))
                {
                    this._Inn = value;
                }
            }
        }

        public string Kpp
        {
            get
            {
                return this._Kpp;
            }
            set
            {
                if ((this._Kpp != value))
                {
                    this._Kpp = value;
                }
            }
        }

        public string Kbk
        {
            get
            {
                return this._Kbk;
            }
            set
            {
                if ((this._Kbk != value))
                {
                    this._Kbk = value;
                }
            }
        }

        public string Oktmo
        {
            get
            {
                return this._Oktmo;
            }
            set
            {
                if ((this._Oktmo != value))
                {
                    this._Oktmo = value;
                }
            }
        }

        public string Okato
        {
            get
            {
                return this._Okato;
            }
            set
            {
                if ((this._Okato != value))
                {
                    this._Okato = value;
                }
            }
        }

        public string Ogrn
        {
            get
            {
                return this._Ogrn;
            }
            set
            {
                if ((this._Ogrn != value))
                {
                    this._Ogrn = value;
                }
            }
        }
    }
}
