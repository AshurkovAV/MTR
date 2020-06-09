using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Medical.DataCore.Validate
{
    public class SchemaValidateLambda : ISchemaValidate
    {
        public SchemaValidateLambda()
        {
            Errors = new List<string>();
        }

        public ICollection<string> Errors { get; set; }

        public void Validate(Stream xmlStream, Stream schemaStream)
        {
            try
            {
                var schemas = new XmlSchemaSet();

                if (schemaStream != null)
                {
                    schemas.Add("", XmlReader.Create(schemaStream));
                    XDocument registerDocument = XDocument.Load(xmlStream);
                    registerDocument
                        .Validate(schemas, (o, e) => Errors.Add(string.Format("Ошибка валидации xml {0}",
                                                                              e.Message)));
                }
                else
                {
                    Errors.Add(string.Format("Не найдена схема xml"));
                }
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("Исключение при валидации {0}", exception.Message));
            }
        }

        public void Validate(XmlTextReader xmlStream, Stream schemaStream)
        {
            throw new NotImplementedException();
        }
    }
}