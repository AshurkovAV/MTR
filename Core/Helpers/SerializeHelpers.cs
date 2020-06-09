using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Helpers
{
    public class SerializeHelpers
    {

        public static string CleanEmptyTags(String xml)
        {
            var regex = new Regex(@"(\s)*<(\w)*(\s)*/>");
            return regex.Replace(xml, string.Empty);
        }

        public static string Serialize(XmlSerializer serializer,
                               Encoding encoding,
                               XmlSerializerNamespaces ns,
                               bool omitDeclaration,
                               object objectToSerialize)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = omitDeclaration;
            settings.Encoding = encoding;
            XmlWriter writer = XmlWriter.Create(ms, settings);
            serializer.Serialize(writer, objectToSerialize, ns);
            string xml = encoding.GetString(ms.ToArray());
            xml = CleanEmptyTags(xml);
            return xml;
        }

        public static string DeSerialize(XmlSerializer serializer,
                               Encoding encoding,
                               XmlSerializerNamespaces ns,
                               bool omitDeclaration,
                               object objectToSerialize)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = omitDeclaration;
            settings.Encoding = encoding;
            XmlWriter writer = XmlWriter.Create(ms, settings);
            serializer.Serialize(writer, objectToSerialize, ns);
            string xml = encoding.GetString(ms.ToArray());
            xml = CleanEmptyTags(xml);
            return xml;
        }

        
    }
}
