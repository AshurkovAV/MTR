using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Medical.DataCore.Validate
{
    public class SchemaValidateHandler : ISchemaValidate
    {
        public SchemaValidateHandler()
        {
            Errors = new List<string>();
        }

        public ICollection<string> Errors { get; set; }

        public void Validate(Stream xmlStream, Stream schemaStream)
        {
            try
            {
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                XmlSchema schema = XmlSchema.Read(schemaStream, ValidationEventHandler);
                //settings.Schemas.Add(schema);
                //XmlReader reader = XmlReader.Create(xmlStream, settings);
                //while (reader.Read())
                //{
                    
                //}
                var document = new XmlDocument();
                document.Load(xmlStream);
                document.Schemas.Add(schema);

                var eventHandler = new ValidationEventHandler(ValidationEventHandler);
                // the following call to Validate succeeds.
                document.Validate(eventHandler);
            }
            catch( XmlSchemaValidationException exception)
            {
                Errors.Add(string.Format("Исключение при валидации {0}, строка {1}, позиция {2}", exception.Message,exception.LineNumber,exception.LinePosition));
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("Исключение при валидации {0}", exception.Message));
            }
        }

        public void Validate(XmlTextReader xmlStream, Stream schemaStream)
        {
            try
            {
                var xsc = new XmlSchemaSet();
                xsc.Add(null, new XmlTextReader(schemaStream));

                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema
                };
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.Schemas.Add(xsc);
                settings.ValidationEventHandler += ValidationEventHandler;
                
                var vr = XmlReader.Create(xmlStream, settings);

                while (vr.Read())
                {
                    if (Errors.Count > 50)
                    {
                        Errors.Add(string.Format("Слишком много ошибок"));
                        return;
                    }
                }

            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("Исключение при валидации {0}", exception.Message));
            }
        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
#if DEBUG
            Console.WriteLine("***DEBUG***");
            Console.WriteLine("*Validation error*");
            Console.WriteLine("\tSeverity:{0}", e.Severity);
            Console.WriteLine("\tMessage:{0}", e.Message);
            Console.WriteLine("\tLine:{0} {1}", e.Exception.LineNumber, e.Exception.LinePosition);
#endif
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Errors.Add(string.Format("Ошибка {0} Строка: {1}, позиция:{2}", e.Message, e.Exception.LineNumber, e.Exception.LinePosition));
                    break;
                case XmlSeverityType.Warning:
                    Errors.Add(string.Format("Предупреждение {0} Строка: {1}, позиция:{2}", e.Message, e.Exception.LineNumber, e.Exception.LinePosition));
                    break;
            }

        }
    }
}