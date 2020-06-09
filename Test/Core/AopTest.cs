using System;
using System.ComponentModel;
using Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Core
{
    [TestClass]
    public class AopTest
    {
        [TestMethod]
        public void ApplyDefaultValuesTest()
        {
            var t = new Test();
            Assert.IsNull(t.TestString);
            Assert.AreEqual(0,t.TestInt);
            t.ApplyDefaultValues();
            Assert.AreEqual(13, t.TestInt);
            Assert.AreEqual("Test", t.TestString);
            t.TestString = "TestChange";
            t.ResetDefaultValues();
            Assert.AreEqual("Test", t.TestString);
        }
    }

    public class Test
    {
        [DefaultValue("Test")]
        public string TestString { get; set; }
        [DefaultValue(13)]
        public int TestInt { get; set; }
    }
}
