using System;
using Autofac;
using Core.Attributes;
using Core.Infrastructure;
using Core.Infrastructure.Compiler;
using Core.Services;
using Medical.CoreLayer.Models.Config;
using Medical.CoreLayer.Service;

namespace Medical.CoreLayer
{
    [Module(Name = "Ядро приложения", ClassType = typeof(InitCommand), Priority = ModulePriority.Normal, Order = 1)]
    public class InitCommand : IModuleActivator
    {
        public void Run()
        {
            try
            {
                var builder = new ContainerBuilder();

                //Регистрация сервиса хранения общих настроек
                builder.RegisterType<AppShareSettings>().As<IAppShareSettings>().SingleInstance();
                //Регистрация сервиса хранения локальных настроек
                builder.RegisterType<AppLocalSettings>().As<IAppLocalSettings>().SingleInstance();
                //Регистрация сервиса сообщений
                builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
                //Регистрация сервиса компиляции скриптов
                builder.RegisterType<ScriptCompiler>().As<IScriptCompiler>().SingleInstance();
                //Регистрация модели хранения настроек соединения с БД
                builder.RegisterType<DatabaseConfigModel>().AsSelf();

                Di.Update(builder);
            }
            catch (Exception)
            {
                
            }

        }
    }
}
