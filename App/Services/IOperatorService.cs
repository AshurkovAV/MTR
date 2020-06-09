using System.Collections.Generic;
using Core;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Services
{
    public interface IOperatorService
    {
        IEnumerable<RefusalModel> LoadRefusal(int id, RefusalType type, int? source = null);
    }

    public interface IZOperatorService
    {
        IEnumerable<ZRefusalModel> LoadRefusal(int id, RefusalType type, TypeSank typeSank, int? source = null);
        IEnumerable<ZSlkoefModel> LoadSlkoef(int id);
    }
}