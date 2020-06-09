using System;
using System.Collections.Generic;
using Core.Infrastructure;
using DataModel;

namespace Medical.DatabaseCore.Services.Database
{
    public interface ITortillaRepository
    {
        TransactionResult AddSrzQuery(SrzQueryModel model);
        TransactionResult<IEnumerable<ZP1errors>> GetSrzError(string guid);
        TransactionResult<IEnumerable<SrzAnswer>> GetSrzAnswer(string guid);
        TransactionResult<IEnumerable<ZP1msg>> GetSrzQueries(string surname, string name, string patronymic, DateTime? birthday, int? sex, int? docType = null, string docNum = null, string docSer = null, DateTime? formDate = null);
    }
}