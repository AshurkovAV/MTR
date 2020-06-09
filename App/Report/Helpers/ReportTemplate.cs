using System;
using System.IO;
using System.Xml.Linq;

namespace Medical.AppLayer.Report.Helpers
{
    public class ReportTemplate
    {
        public static Stream GetDefaultReport()
        {
            var memory = new MemoryStream();
            var template = new XElement("Report",
                    new XAttribute("ScriptLanguage", "CSharp"),
                    new XAttribute("ReportInfo.Created", DateTime.Now),
                    new XAttribute("ReportInfo.Modified", DateTime.Now),
                    new XAttribute("CreatorVersion", "1.8.1.0"),
                    new XElement("Dictionary",
                        new XElement("MsSqlDataConnection",
                            new XAttribute("Name", "Connection"),
                            new XAttribute("ConnectionString", "rijcmlqM3/HbiZANEYP3Y6oNtE7mrUmT0ytl94C6DYS4DXvgc8JqhVjVQGe7Sc2pOMNI3eDZu4y6+BLf6/PCErp5V5iYO+NhUsLGHVpiKUoEc7fdRAKM3dC4G3zKvHq63vBaaJuNWt1RaEUDxyLLh6IMbjf+Iqjj5s+Su4iyajqSNb5u2mTlxcVInKupOBFFxa/QOyK")
                            ),
                        new XElement("Parameter",
                            new XAttribute("Name", "MedicalAccountId"),
                            new XAttribute("DataType", "System.Int32")
                        )),
                    new XElement("ReportPage",
                        new XAttribute("Name", "Page1"),
                        new XAttribute("Landscape", "true"),
                        new XAttribute("PaperWidth", "297"),
                        new XAttribute("PaperHeight", "210"),
                        new XAttribute("RawPaperSize", "9"),
                        new XElement("ReportTitleBand",
                            new XAttribute("Name", "ReportTitle1"),
                            new XAttribute("Width", "1047.06"),
                            new XAttribute("Height", "50.00"))));

            var templateReport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), template);
            templateReport.Save(memory);
            memory.Position = 0;
            return memory;
        }
    }
}
