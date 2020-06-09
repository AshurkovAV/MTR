using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class LinqTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IEnumerable<string> list = new List<string> { "foo", "bar", "baz" };
            Test(list);
            
        }

        private void Test<T>(T t) 
        {
            List<string> list = new List<string> { "foo", "bar", "baz" };
            
            ParameterExpression p = Expression.Parameter(typeof(string), "item");
            Delegate predicate = Expression.Lambda(
            Expression.Call(
                p,
                typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                Expression.Constant("b")),
            p).Compile();

            Expression anyCall = Linq.CallAny(Expression.Constant(list), predicate);
            Func<bool> a = (Func<bool>)Expression.Lambda(anyCall).Compile();
            Console.WriteLine("Found? {0}", a());
            
            //Assert.IsTrue(t.GetType().IsIEnumerable());
        }
    }
}
