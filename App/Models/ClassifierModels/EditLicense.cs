using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.AppLayer.Attributes;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
using System.Linq;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditLicense : ClassifierBaseVm<globalLicense>
    {
        private IEnumerable<EditLicenseEntry> licenseEntry;
        public EditLicense(globalLicense classifier)
            : base(classifier)
        {
            licenseEntry = Classifier.dbo_globalLicenseEntrydbo_globalLicenseLicenseLicenseIds.Select(p => new EditLicenseEntry(p));
        }

        public EditLicense()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.LicenseId; }
            set { Classifier.LicenseId = value; }
        }

        [Display(Name = "Код медицинской организации")]
        public string MedicalOrganization
        {
            get { return Classifier.MedicalOrganization; }
            set { Classifier.MedicalOrganization = value; }
        }

        [Display(Name = "Дата начала")]
        public DateTime? DateBegin
        {
            get { return Classifier.DateBegin; }
            set { Classifier.DateBegin = value; }
        }

        [Display(Name = "Дата окончания")]
        public DateTime? DateEnd
        {
            get { return Classifier.DateEnd; }
            set { Classifier.DateEnd = value; }
        }

        [Display(Name = "Дата остановки")]
        public DateTime? DateStop
        {
            get { return Classifier.DateStop; }
            set { Classifier.DateStop = value; }
        }

        [Display(Name = "№ лицензии")]
        public string LicenseNumber
        {
            get { return Classifier.LicenseNumber; }
            set { Classifier.LicenseNumber = value; }
        }

        [Display(Name = "Содержимое лицензии")]
        public IEnumerable<EditLicenseEntry> Details
        {
            get { return licenseEntry; }
        }

    }
}
