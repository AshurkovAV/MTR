using System;
using Core.Infrastructure;
using DataModel;

namespace Medical.AppLayer.Services
{
    public interface IAdminService
    {
        OperationResult CreateUser(localUser user);
        OperationResult<Tuple<string,string>> GenerateNewPassword(string p);

    }
}