using Core.Attributes;
using Core.Infrastructure;

namespace Medical.DataCore
{
    [Module(Name = "Обработка данных", ClassType = typeof(InitCommand), Priority = ModulePriority.Normal, Order = 3)]
    public class InitCommand : IModuleActivator
    {
        public void Run()
        {
            
        }
    }
}
