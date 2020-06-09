using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Medical.CoreLayer.TestService;
using Medical.DatabaseCore.Services.Database;
using Newtonsoft.Json.Linq;
using Binding = System.ServiceModel.Channels.Binding;

namespace Medical.AppLayer.Services
{
    public class RemoteService : IRemoteService
    {
        private readonly IAppRemoteSettings _remoteSettings;

        public RemoteService(IAppRemoteSettings remoteSettings)
        {
            _remoteSettings = remoteSettings;
        }

        public string BindingTypeToString(BindingType protocol)
        {
            switch (protocol)
            {
                case BindingType.BasicHttpBinding:
                    return "http";
                case BindingType.NetTcpBinding:
                    return "net.tcp";
                default:
                    return "http";
            }
        }

        public OperationResult Test(string uri, Binding binding)
        {
            var result = new OperationResult();
            try
            {
                var client = new DataServiceClient(binding, new EndpointAddress(uri));
                var checkResult = client.CheckConfig();
                if (checkResult != "ok")
                {
                    result.AddError(new Exception("Ошибка при проверке подключения к сервису данных"));
                }
                client.Close();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public string ConstructDataServiceConnectionString(object value)
        {
            var result = string.Empty;
            try
            {
                dynamic tmp = value;
                result = string.Format("{0}://{1}:{2}",
                        BindingTypeToString((BindingType)tmp.ServiceProtocol),
                        tmp.RemoteAddress,
                        tmp.Port);

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
 
        public bool IsDataServiceConnectionString(object obj)
        {
            try
            {
                var checkList = new List<string>
                {
                    "RemoteAddress",
                    "Port",
                    "ServiceProtocol"
                };

                var tmp = JObject.FromObject(obj);
                return checkList.All(name => tmp.GetValue(name) != null);
            }
            catch (Exception)
            {
                return false;
            }

        }

        public OperationResult<Binding> CreateBinding(BindingType serviceProtocol)
        {
            var result = new OperationResult<Binding>();
            try
            {
                switch (serviceProtocol)
                {
                    case BindingType.BasicHttpBinding:
                        result.Data = new BasicHttpBinding
                        {
                            MaxReceivedMessageSize = 2097152,
                            MaxBufferSize = 2097152
                           
                        };
                        break;
                    case BindingType.NetTcpBinding:
                        result.Data = new NetTcpBinding
                        {
                            MaxReceivedMessageSize = 2097152,
                            MaxBufferSize = 2097152
                        };
                        break;
                    default:
                        result.Data = new BasicHttpBinding
                        {
                            MaxReceivedMessageSize = 2097152,
                            MaxBufferSize = 2097152
                        };
                        break;
                }
                 
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            
            return result;
        }

        public OperationResult<T> GetClient<T>(string configName) where T : class 
        {
            var result = new OperationResult<T>();
            try
            {
                object settings = _remoteSettings.Get(configName);
                if (settings.IsNull() || settings.IsNullAsDynamic())
                {
                    result.AddError(new Exception("DataService config {0} doesn't exists".F(configName)));
                    return result;
                }
                var uri = ConstructDataServiceConnectionString(settings);
                var bindingResult = CreateBinding((BindingType)(settings as dynamic).ServiceProtocol);
                if (!bindingResult.Success)
                {
                    result.AddError(bindingResult.LastError);
                    return result;
                }
                result.Data = Activator.CreateInstance(typeof (T), new object[]
                {
                    bindingResult.Data,
                    new EndpointAddress(uri)
                }) as T;
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }
    }
}
