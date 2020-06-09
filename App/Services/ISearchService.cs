using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core;
using DataModel;

namespace Medical.AppLayer.Services
{
    public interface ISearchService
    {
        IEnumerable<EventShortView> SearchByParameters(IDictionary<SearchParameters, object> parameters);
        IEnumerable<GeneralEventShortView> SearchGeneralByParameters(IDictionary<SearchParameters, object> parameters);
    }
}
