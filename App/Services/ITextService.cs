using System;
using Core.Infrastructure;
using DataModel;

namespace Medical.AppLayer.Services
{
    /// <summary>
    /// Операции с текстом
    /// </summary>
    public interface ITextService
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
        [Obsolete("Используйте RichEditControl и HtmlBuilder")]
        OperationResult<string> ToRtf(string html);

        OperationResult<string> FormatXml(string xml);
        OperationResult IsValidXml(string xmlString);


        string Stringify(object data);
    }
}