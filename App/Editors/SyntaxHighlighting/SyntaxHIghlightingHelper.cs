using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Medical.AppLayer.Editors.SyntaxHighlighting
{
    class SyntaxHIghlightingHelper
    {
        //TODO
        /// <summary>
        ///  Load syntax highlightind from embedded resources
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="textEditor"></param>
        /// <example>LoadFromEmbedded("MyHighlighting.xshd", typeof(AnyClassOfAssemblyWithSyntax, textEditor) );</example>
        public void LoadFromEmbedded(string path, Type type, ICSharpCode.AvalonEdit.TextEditor textEditor)
        {
            Type t = typeof(SyntaxHIghlightingHelper);
            var myAssembly = Assembly.GetAssembly(type);
            using (Stream s = myAssembly.GetManifestResourceStream(path))
            {
                using (var reader = new XmlTextReader(s))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
