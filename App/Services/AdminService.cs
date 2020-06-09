using System;
using Core.Helpers;
using Core.Infrastructure;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Services
{
    public class AdminService : IAdminService
    {
        private readonly IMedicineRepository _repository;

        public AdminService(IMedicineRepository repository)
        {
            _repository = repository;
        }

        public OperationResult CreateUser(localUser user)
        {
            var result = new OperationResult();
            var salt = CryptoHelpers.CreateRandom(16);
            user.Salt = Convert.ToBase64String(salt);
            user.Pass = Convert.ToBase64String(CryptoHelpers.Hash(user.Pass, salt));
            var insertResult = _repository.InsertWithIdentity(user);
            if (insertResult.HasError)
            {
                result.AddError(insertResult.LastError);
            }
            return result;
        }

        public OperationResult<Tuple<string, string>> GenerateNewPassword(string p)
        {
            var result = new OperationResult<Tuple<string, string>>();
            var random = CryptoHelpers.CreateRandom(16);
            var salt = Convert.ToBase64String(random);
            var pass = Convert.ToBase64String(CryptoHelpers.Hash(p, random));
            result.Data = Tuple.Create(pass, salt);
            return result;
        }
    }
}
