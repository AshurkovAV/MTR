using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RegisterETests
    {
        public RegisterETests()
        {
           /* UnityService.Instance.RegisterType<IMessageServiceCore, MessageServiceXtra>(
                new ContainerControlledLifetimeManager(), new InjectionConstructor());
            
            UnityService.Instance.RegisterType<IDatabase, DatabaseIns>(new ContainerControlledLifetimeManager(),
                                                                       new InjectionConstructor());
            UnityService.Instance.RegisterType<IClassifierService, ClassifierService>(
                new ContainerControlledLifetimeManager(), new InjectionConstructor());
            var manager = UnityService.Instance.Resolve<ClassifierManager>();
            manager.LoadClassifier<F001>();
            manager.LoadClassifier<F002>();
            manager.LoadClassifier<F003>();
            manager.LoadClassifier<F005>();
            manager.LoadClassifier<F008>();
            manager.LoadClassifier<F009>();
            manager.LoadClassifier<F010>();
            manager.LoadClassifier<F011>();
            manager.LoadClassifier<F014>();
            manager.LoadClassifier<V002>();
            manager.LoadClassifier<V003>();
            manager.LoadClassifier<V004>();
            manager.LoadClassifier<V005>();
            manager.LoadClassifier<V006>();
            manager.LoadClassifier<V008>();
            manager.LoadClassifier<V009>();
            manager.LoadClassifier<V010>();
            manager.LoadClassifier<V012>();
            manager.LoadClassifier<M001>();
            new IdcOptimize();*/
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestSerializeE()
        {
            /*var serializer = new XmlSerializer(typeof (RegisterE));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterE.xml"))
            {
                Assert.IsNotNull(stream);
                var register = (RegisterE) serializer.Deserialize(stream);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestLoaderE()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterE.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderE();
                var result = loader.Load<RegisterE>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }

        [TestMethod]
        public void TestLoaderBadE()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterEBad.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderE();
                loader.Validate<RegisterE>(stream);
                Assert.IsTrue(loader.ErrorCount > 0, loader.ErrorsAggregate);
                loader.Errors.ToList().ForEach(Console.WriteLine);
            }*/
        }

        [TestMethod]
        public void TestLoaderEOms()
        {
            /*var loader = new LoaderE();
            var files = loader.Unpack<RegisterEInfo>(Path.GetFullPath("Data/R7846110001.oms"));
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            var result = loader.Load<RegisterE>(files[0]);
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            Assert.IsNotNull(result);*/
        }


        [TestMethod]
        public void TestSerializeEAnswer()
        {
            /*var serializer = new XmlSerializer(typeof (RegisterEAnswer));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterEAnswer.xml"))
            {
                Assert.IsNotNull(stream);
                var register = (RegisterEAnswer) serializer.Deserialize(stream);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestLoaderEAnswer()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterEAnswer.xml"))
            {
                var loader = new LoaderEAnswer();
                var result = loader.Load<RegisterEAnswer>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }

        [TestMethod]
        public void TestLoaderEAnswerOms()
        {
            /*var loader = new LoaderEAnswer();
            var files = loader.Unpack<RegisterEInfo>(Path.GetFullPath("Data/A4678110001.oms"));
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            var result = loader.Load<RegisterEAnswer>(files[0]);
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            Assert.IsNotNull(result);*/
        }
    }
}