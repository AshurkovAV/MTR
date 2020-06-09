using System.ServiceModel.Channels;
using Core;
using Core.Infrastructure;

namespace Medical.AppLayer.Services
{
    public interface IRemoteService
    {
        string BindingTypeToString(BindingType protocol);
        OperationResult Test(string uri, Binding binding);
        string ConstructDataServiceConnectionString(object value);
        bool IsDataServiceConnectionString(object obj);
        OperationResult<Binding> CreateBinding(BindingType serviceProtocol);
        OperationResult<T> GetClient<T>(string configName) where T : class;
    }
}