using System;
using Autofac;

namespace Core.Services
{
    public class Di
    {
        private static IContainer _instance;
        private static readonly object SyncRoot = new Object();

        public static IContainer I
        {
            get
            {
                if (_instance == null)
                {
                    Init();
                }
                return _instance;
            }
        }

        private static void Init()
        {
            lock (SyncRoot)
            {
                if (_instance == null)
                {
                    var builder = new ContainerBuilder();
                    _instance = builder.Build();
                }
            }
        }

        public static void Update(ContainerBuilder builder)
        {
            lock (SyncRoot)
            {
                if (_instance == null)
                {
                    Init();
                }
                builder.Update(_instance);
            }
        }
    }
}
