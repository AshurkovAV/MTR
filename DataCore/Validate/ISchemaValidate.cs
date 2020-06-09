using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Medical.DataCore.Validate
{
    public interface ISchemaValidate
    {
        ICollection<string> Errors { get; set; }
        void Validate(Stream xmlStream, Stream schemaStream);
        void Validate(XmlTextReader xmlStream, Stream schemaStream);
    }
}