using Core.Attributes;
using Core.Infrastructure;

namespace Core
{
    [Module(Name = "Общая библиотека", ClassType = typeof(InitCommand), Priority = ModulePriority.Normal, Order = 0)]
    public class InitCommand : IModuleActivator
    {
        public void Run()
        { 
        }
    }
}
