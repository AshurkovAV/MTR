using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RegisterDTests
    {
        public RegisterDTests()
        {
            /*UnityService.Instance.RegisterType<IMessageServiceCore, MessageServiceXtra>(
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
            manager.LoadClassifier<M001>();*/
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
        public void TestSerializeD()
        {
            /*var serializer = new XmlSerializer(typeof (AccountRegisterD));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterD.xml"))
            {
                Assert.IsNotNull(stream);
                var register = (AccountRegisterD) serializer.Deserialize(stream);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestSerializeDPatient()
        {
           /* var serializer = new XmlSerializer(typeof (PersonalRegisterD));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.PatientD.xml"))
            {
                Assert.IsNotNull(stream);
                var register = (PersonalRegisterD) serializer.Deserialize(stream);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestLoaderD()
        {
           /* Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterD.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderD();
                var result = loader.Load<AccountRegisterD>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }

        [TestMethod]
        public void TestLoaderPersonalD()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.PatientD.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderPersonalD();
                var result = loader.Load<PersonalRegisterD>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }

        [TestMethod]
        public void TestLoaderDZip()
        {
            /*var loader = new LoaderD();
            List<string> files = loader.Unpack<RegisterDInfo>(Path.GetFullPath("Data/HM460003T462011121.zip"));
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            var result = loader.Load<AccountRegisterD>(files[0]);
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            Assert.IsNotNull(result);*/
        }

        [TestMethod]
        public void TestLoaderPersonalDZip()
        {
            /*var loader = new LoaderPersonalD();
            List<string> files = loader.Unpack<RegisterDInfo>(Path.GetFullPath("Data/LM460003T462011121.zip"));
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            var result = loader.Load<PersonalRegisterD>(files[0]);
            Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
            Assert.IsNotNull(result);*/
        }


        [TestMethod]
        public void TestSerializeAnswerD()
        {
            /*var serializer = new XmlSerializer(typeof (RegisterAnswerD));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.AnswerD.xml"))
            {
                Assert.IsNotNull(stream);
                var register = (RegisterAnswerD) serializer.Deserialize(stream);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestLoaderAnswerD()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.AnswerD.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderDAnswer();
                var result = loader.Load<RegisterAnswerD>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }

        [TestMethod]
        public void TestLoaderAnswerDEmpty()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.AnswerD2.xml"))
            {
                Assert.IsNotNull(stream);
                var loader = new LoaderDAnswer();
                var result = loader.Load<RegisterAnswerD>(stream);
                Assert.IsTrue(loader.ErrorCount == 0, loader.ErrorsAggregate);
                Assert.IsNotNull(result);
            }*/
        }
    }
}