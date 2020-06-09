using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Core.Extensions;
using Core.Infrastructure;
using Core.MarkupConverter;
using Newtonsoft.Json;

namespace Medical.AppLayer.Services
{
    /// <summary>
    /// Операции с текстом, имплементация ITextService
    /// </summary>
    public class TextService : ITextService
    {
        /// <summary>
        /// Конвертация html в rtf формат
        /// </summary>
        /// <param name="html">Строка в html с использованием разрешенных элементов/атрибутов</param>
        /// <returns>Строка в rtf формате</returns>
        /// <remarks>  
        /// Разрешенные элементы и атрибуты:
        /// b,i,u,color,backcolor,size,br,
        /// </remarks>
        public OperationResult<string> ToRtf(string html)
        {
            var result = new OperationResult<string>();
            try
            {
                result.Data = new MarkupConverter().ConvertHtmlToRtf(html);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        //TODO add declaration as parameter
        /// <summary>
        /// Форматирование XML в читабельный вид
        /// </summary>
        public OperationResult<string> FormatXml(string xml)
        {
            var result = new OperationResult<string>();
            try
            {
                var sb = new StringBuilder();
                XDocument doc = XDocument.Parse(xml);
                
                sb.AppendLine(doc.Declaration.ToString());
                sb.AppendLine(doc.ToString());
                result.Data = sb.ToString();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        /// <summary>
        /// Проверка XML на корректность
        /// </summary>
        public OperationResult IsValidXml(string xmlString)
        {
            var result = new OperationResult();
            var tagsWithData = new Regex("<\\w+>[^<]+</\\w+>");

            if (xmlString.IsNullOrEmpty() || tagsWithData.IsMatch(xmlString) == false)
            {
                result.AddError(new Exception("Проверка XML с помощью регулярного выражения не пройдена"));
            }
            else
            {
                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlString);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;

        }

        public string Stringify(object data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception exception)
            {
                //TODO add log
            }

            return null;
        }
    }
}
