namespace Core.Helpers
{
    public class UnityHelpers
    {

        public UnityHelpers()
        {
            // install a named string that holds the connection string to use
            //container.RegisterInstance<string>("MyConnectionString", connectionString, new ContainerControlledLifetimeManager());

            // register the class that will use the connection string
            //container.RegisterType<MyNamespace.MyObjectContext, MyNamespace.MyObjectContext>(new InjectionConstructor(new ResolvedParameter<string>("MyConnectionString")));

            //var context = container.Resolve<MyNamespace.MyObjectContext>();
        }
    }
}
