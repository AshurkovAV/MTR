using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RegisterD21K2Tests
    {
        public RegisterD21K2Tests()
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
        public void TestValidateD()
        {
            /*LoaderBase loader = new v21K2.D.LoaderD();
            string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            loader.Validate<v21K2.D.AccountRegisterD>(new XmlTextReader(solution_dir + "\\DataTests\\Data\\RegisterD21K2.xml"), "Medical.DataCore.v21K2.Xsd.D1.xsd");
            loader.ErrorCount.Should().Be(0, "Validation errors must be 0");*/
        }

        [TestMethod]
        public void TestValidatePatientD()
        {
            /*LoaderBase loader = new v21K2.D.LoaderD();
            loader.Validate<v21K2.D.AccountRegisterD>(new XmlTextReader("Data\\PatientD21K_1.xml"), "Medical.DataCore.v21K2.Xsd.D2.xsd");
            loader.ErrorCount.Should().Be(0, "Validation errors must be 0");*/
        }

        
        [TestMethod]
        public void TestSerializeD()
        {
            /*var serializer = new XmlSerializer(typeof(v21K2.D.AccountRegisterD));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.RegisterD21K2_16MB.xml"))
            {
                Assert.IsNotNull(stream);
                var watch = new Stopwatch();
                watch.Start();
                var register = (v21K2.D.AccountRegisterD)serializer.Deserialize(stream);
                watch.Stop();
                Console.WriteLine("Elapsed: {0}",         watch.Elapsed);
                Console.WriteLine("In milliseconds: {0}", watch.ElapsedMilliseconds);
                Console.WriteLine("In timer ticks: {0}",  watch.ElapsedTicks);
                Assert.IsNotNull(register);
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestSerializeDPatient()
        {
            /*var serializer = new XmlSerializer(typeof(v21K2.D.PersonalRegisterD));
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("DataTests.Data.PatientD21K2.xml"))
            {
                Assert.IsNotNull(stream);
                try
                {
                    var register = (v21K2.D.PersonalRegisterD)serializer.Deserialize(stream);
                    Assert.IsNotNull(register);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                
                stream.Close();
            }*/
        }

        [TestMethod]
        public void TestLoaderD()
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
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
           /* var loader = new LoaderPersonalD();
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
            }*/}
    }
}