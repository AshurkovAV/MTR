using System;
using System.Text.RegularExpressions;
using Core;
using Core.Extensions;
using Medical.AppLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class FlagExtensionTest
    {
        [TestMethod]
        public void TestFlagExtension()
        {
            RegexOptions options = RegexOptions.None;

            options = options.Add(RegexOptions.IgnoreCase);
            options = options.Add(RegexOptions.Multiline | RegexOptions.Singleline);
            options = options.Remove(RegexOptions.IgnoreCase);
            Assert.IsTrue(options.Has(RegexOptions.Multiline)); 
            Assert.IsTrue(options.Missing(RegexOptions.IgnoreCase)); 

        }

        [TestMethod]
        public void TestAddIfTrue()
        {
            RegexOptions options = RegexOptions.None;

            options = options.AddIfTrue(RegexOptions.Multiline,true);
            options = options.AddIfTrue(RegexOptions.IgnoreCase,false);
            Assert.IsTrue(options.Has(RegexOptions.Multiline)); 
            Assert.IsTrue(options.Missing(RegexOptions.IgnoreCase)); 

        }

        [TestMethod]
        public void TestToInt32()
        {
            Assert.AreEqual(1, SexEnum.Male.ToInt32());
            Assert.AreEqual(2, SexEnum.Female.ToInt32());
        }
    }
}
