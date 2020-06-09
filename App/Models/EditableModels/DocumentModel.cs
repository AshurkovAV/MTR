using System;
using System.Linq;
using BLToolkit.EditableObjects;
using Core.Extensions;
using DataModel;

namespace Medical.AppLayer.Models.EditableModels
{
    public abstract class DocumentModel : EditableObject<DocumentModel>
    {
        // Any abstract property becomes editable.
        //
        public abstract int DocumentId { get; set; }
        public abstract int DocumentPersonId { get; set; }
        public abstract int? DocType { get; set; }
        public abstract string DocSeries { get; set; }
        public abstract string DocNum { get; set; }
        public abstract DateTime? DocDate { get; set; }
        public abstract string DocOrg { get; set; }
        public abstract string IssueName { get; set; }
        // This field is not editable.
        //
        public string SerialNumber
        {
            get { return string.Format("{0} {1}", DocSeries, DocNum); }
        }

        public FactDocument Update(FactDocument document)
        {
            
            var patientInfo = document.GetType().GetProperties();
            var dirtyMembers = GetDirtyMembers();
            foreach (var info in dirtyMembers)
            {
                var value = info.GetValue(this, null);
                var dest = patientInfo.FirstOrDefault(p => p.Name == info.Name);
                if (dest.IsNotNull())
                {
                    dest.SetValue(document, value, null);
                }
            }
            return document;
        }
    }
}
