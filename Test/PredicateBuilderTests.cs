using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class PredicateBuilderTests
    {
        /*
        Nesting Predicates
            Consider the following predicate:
                p => p.Price > 100 &&
                p.Price < 1000 &&
                (p.Description.Contains ("foo") || p.Description.Contains ("far"))
            was 
                var inner = PredicateBuilder.False<Product>();
                inner = inner.Or (p => p.Description.Contains ("foo"));
                inner = inner.Or (p => p.Description.Contains ("far"));
                var outer = PredicateBuilder.True<Product>();
                outer = outer.And (p => p.Price > 100);
                outer = outer.And (p => p.Price < 1000);
                outer = outer.And (inner);
         
         */
        [TestMethod]
        public void PredicateBuilderTest()
        {
            var list = new List<Product>
                           {
                               new Product()
                                   {
                                       Description = "345a",
                                       Discontinued = false
                                   },
                               new Product
                                   {
                                       Description = "1245b",
                                       Discontinued = false
                                   },
                               new Product
                                   {
                                       Description = "1235c",
                                       Discontinued = true
                                   },
                                new Product
                                    {
                                        Description = "35d",
                                       Discontinued = false
                                    }
                           };
            var predicate = Product.ContainsInDescription("1");
            predicate = predicate.And(Product.ContainsInDescription("2"));
            predicate = predicate.And(Product.IsSelling());

            var query =
              from p in list.AsQueryable().Where(predicate)
              select p;
            
            foreach (var product in query)
            {
                Console.WriteLine(product.Description);
            }
            Console.WriteLine("***");
            predicate = PredicateBuilder.True<Product>();
            predicate = predicate.And(p => !p.Description.Contains("1"));
            predicate = predicate.And(p => !p.Description.Contains("4"));

            query =
              from p in list.AsQueryable().Where(predicate)
              select p;
            foreach (var product in query)
            {
                Console.WriteLine(product.Description);
            }
            Console.WriteLine("***");
            predicate = PredicateBuilder.False<Product>();
            predicate = predicate.Or(p => p.Description.Contains("a"));
            predicate = predicate.Or(p => p.Description.Contains("d"));

            query =
              from p in list.AsQueryable().Where(predicate)
              select p;
            foreach (var product in query)
            {
                Console.WriteLine(product.Description);
            }
            Console.WriteLine("***");
            //string s = "The quick brown fox jumps over the lazy dog";
            //var inner = PredicateBuilder.False<Product>();
            //inner = inner.Or(p => p.Description.Contains("fox"));
            //inner = inner.Or(p => p.Description.Contains("over"));

            //var outer = PredicateBuilder.True<Product>();
            //outer = outer.And(p => p.Description.Length > 10);
            //outer = outer.And(p => p.Description.Length < 100);
            //outer = outer.And(inner);


        }

        public partial class Product
        {
            public string Description { get; set; }
            public bool Discontinued { get; set; }

            public static Expression<Func<Product, bool>> ContainsInDescription(
                                                          params string[] keywords)
            {
                var predicate = PredicateBuilder.False<Product>();
                foreach (string keyword in keywords)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.Description.Contains(temp));
                }
                return predicate;
            }
        }

        public partial class Product
        {
            public static Expression<Func<Product, bool>> IsSelling()
            {
                return p => !p.Discontinued;
            }
        }
    }
}
