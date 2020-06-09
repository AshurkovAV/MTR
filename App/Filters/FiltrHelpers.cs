using System;
using System.IO;
using System.Text;
using Autofac;
using Core.Services;
using Core.Utils;
using DevExpress.Xpf.Grid;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Filters
{
    public class FiltrHelpers
    {
        private readonly IMedicineRepository _repository;
        private readonly int _userId;
        private IUserService _userService;
        public FiltrHelpers()
        {
            _userService = Di.I.Resolve<IUserService>();
            _repository = Di.I.Resolve<IMedicineRepository>();
            _userId = _userService.UserId;

        }

        public void SaveFilter(MemoryStream stream, TypeFiler type)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                
                var str = reader.ReadToEnd();

                var user = _repository.GetUserById(_userId);
                if (user.Success)
                {
                    var copyuser = user.Data;
                    if (type == TypeFiler.Account)
                    {
                        copyuser.CurrentFilter = str;
                    }
                    else
                    {
                        copyuser.CurrentFilterEvent = str;
                    }
                    var savefiltrresult = _repository.Update(copyuser);
                    if (savefiltrresult.HasError)
                    {
                        ControlResourcesLoger.LogDedug(savefiltrresult.LastError.Message);
                    }
                }
            }
        }

        public MemoryStream LoadFilter(GridControl grid, TypeFiler type)
        {
            var user = _repository.GetUserById(_userId);
            if (user.Success)
            {

                string str = String.Empty;
                if (type == TypeFiler.Account)
                {
                    str = user.Data.CurrentFilter;
                }
                else
                {
                    str = user.Data.CurrentFilterEvent;
                }
                if (str != String.Empty)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(str);
                    using (var stream = new MemoryStream(byteArray))
                    {
                        grid.RestoreLayoutFromStream(stream);
                    }
                }
                
            }
            return null;
        }
    }

    public enum TypeFiler
    {
        Account = 1,
        Event = 2
    }
}
