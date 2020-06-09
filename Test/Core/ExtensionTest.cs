using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Extensions;
using Medical.AppLayer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void IfNotNullTest()
        {
            var user = new User
            {
                Inner = new UserInternal()
            };
            var username = user.IfNotNull(ctx => ctx.Inner.IfNotNull(inner => inner.Inner2.IfNotNull(iden => iden.Name)));
            Assert.IsNull(username);
            var user2 = new User
            {
                Inner = new UserInternal
                {
                    Inner2 = new UserInternal2
                    {
                        Id = 1,
                        Name = "Name"
                    }
                }
            };
            var username2 = user2.IfNotNull(ctx => ctx.Inner.IfNotNull(inner => inner.Inner2.IfNotNull(iden => iden.Name)));
            Assert.AreEqual("Name", username2);
        }

        [TestMethod]
        public void ToEnumFlagTest()
        {
            IEnumerable<object> list = new List<object>
            {
                2,
                4,
                8
            };

            var result = list.ToEnumFlag<ProcessingOperations>();
            
            Assert.IsFalse(result.Has(ProcessingOperations.PrimaryCheck));
            Assert.IsTrue(result.Has(ProcessingOperations.PolicyCheck));
            Assert.IsTrue(result.Has(ProcessingOperations.TortillaPolicyCheck));
            Assert.IsTrue(result.Has(ProcessingOperations.DuplicateCheck));
        }
    }

    [DebuggerDisplay("This class is : {OutputClass()}")]
    public class User
    {
        public UserInternal Inner { get; set; }
        private string OutputClass()
        {
            return "Whatever you want here...";
        }
    }

    public class UserInternal
    {
        public UserInternal2 Inner2 { get; set; }
    }

    public class UserInternal2
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
    }
}
