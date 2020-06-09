using System.Collections.Generic;
using Core;
using Core.DataStructure;
using DataModel;
using System;
using Medical.AppLayer.Economic.Models;

namespace Medical.AppLayer.Services
{
    public interface ICommonService
    {
        void StartWatch();
        double StopWatchAndGetSeconds();
        string ConstructDatabaseConnectionString(object value);
        bool IsDatabaseConnectionString(object obj);
        string GetTitleByIdForOperatorMode(int id, OperatorMode mode);
        string GetTitleByTerritoryAccountView(TerritoryAccountView account);
        string GetTitleByTerritoryAccountView(FactTerritoryAccount account);
        string GetTitleByEconomicAccountView(EconomicAccountCustomModel account);
        string GetTitleByMedicalAccountView(MedicalAccountView account);
        IEnumerable<CommonTuple> GetMonths();
        string GenerateAccountData(string accountNumber, int type, DateTime? accountDate, int direction, string sourceName, string destName);
        string GenerateAccountData(EventShortView data);
        string GenerateAccountData(GeneralEventShortView data);
    }
}