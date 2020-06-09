using System;
using System.Xml.Linq;
using Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class XmlHelperTest
    {
        [TestMethod]
        public void TestReplaceComma()
        {
            string str =
            @"<?xml version=""1.0""?>
            <!-- comment at the root level -->
            <Root>
                <тест>0.1</тест>
                <Child>0,1</Child>
                <Child></Child>
                <Child>    </Child>
            </Root>";
            XDocument doc = XDocument.Parse(str);
            Console.WriteLine(doc);
            doc = XmlHelpers.RemoveEmptyOrNull(doc);
            Console.WriteLine(@"RemoveEmptyOrNull");
            Console.WriteLine(doc);
            doc = XmlHelpers.ToUpperCase(doc);
            Console.WriteLine(@"ToUpperCase");
            Console.WriteLine(doc);
        }
    }
}
