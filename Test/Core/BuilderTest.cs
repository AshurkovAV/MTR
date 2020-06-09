using System;
using Core.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Core
{
    [TestClass]
    public class BuilderTest
    {
        [TestMethod]
        public void HtmlBuilderTest()
        {
            var html = new HtmlBuilder();

            var result = html.BeginBold()
                    .Header("Итого:", 3)
                        .Header("Фактически:", 4)
                      .EndBold()
                      .Text("Пациентов - {0}", 10)
            .Value();
            Assert.AreEqual("<b><h3>Итого:</h3><h4>Фактически:</h4></b>Пациентов - 10<br/>", result);

        }
    }
}
