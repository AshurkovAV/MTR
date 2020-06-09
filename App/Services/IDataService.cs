using System;
using System.IO;
using Core;
using Core.Helpers;
using Core.Infrastructure;
using Medical.DataCore;
using Medical.DataCore.Interface;
using Medical.DataCore.v10PL.PL;
using v30K1 = Medical.DataCore.v30K1;
using v31K1 = Medical.DataCore.v31K1;
using v32K1 = Medical.DataCore.v32K1;
using v30 = Medical.DataCore.v30;
using v31 = Medical.DataCore.v31;
using v32 = Medical.DataCore.v32;

namespace Medical.AppLayer.Services
{
    /// <summary>
    /// Операции с данными информационного обмена (xml данными)
    /// </summary>
    public interface IDataService
    {
        int GetAccountType(int id);
        int GetPaymentStatus(decimal? acceptPrice, decimal? price);
        int GetRefusalSource(int id, OperatorMode mode);
        int GetRefusalSourceByScope(int id, int scope);
        OperationResult SerializeToFile<T>(string fileName, T data, XmlOperations operations = 0, string encoding = "windows-1251");
        OperationResult ExchangeCreateOrUpdate(string omsFilename, RegisterEInfo registerEInfo, int id, int count, DateTime date, int version, int type, int direction);
        OperationResult ExchangeCreateOrUpdate(string omsFilename, RegisterDInfo registerDInfo, int id, int count, DateTime date, int version, int type, int direction);
        OperationResult<Tuple<T1, RegisterEInfo, int>> Write<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IRecord, new()
            where T5 : IPatient, new()
            where T6 : IMEvent, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteZ<T1, T2, T3, T4, T5, T6>(int accountId, int version,
            int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteZOnk<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new()
            where T7 : IZEvent, new()
            where T8 : IDirectionOnkE, new()
            where T9 : IConsultationsOnk, new()
            where T10 : IEventOnk, new()
            where T11 : IDiagBlokOnk, new()
            where T12 : IContraindicationsOnk, new()
            where T13 : IServiceOnk, new()
            where T14 : IAnticancerDrugOnk, new()
            where T15 : IKsgKpg, new()
            where T16 : ISlKoef, new()
            where T17 : IRefusal, new()
            where T18 : IService, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswer<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type)
            where T1 : IRegisterAnswer, new()
            where T2 : IAccountAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswerZ<T1, T2, T3, T4, T5, T6, T7>(int accountId, int version, int type)
            where T1 : IZRegisterAnswer, new()
            where T2 : IAccountAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IZRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IZslMeventAnswer, new()
            where T7 : IZMeventAnswer, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswer1<T1, T2, T3, T4, T5, T6>(int accountId, int economicAccountId, int version, int type)
            where T1 : IRegisterPlAnswer, new()
            where T2 : InformationPlAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WritePaymentOrder<T1, T2, T3, T4, T5, T6>(int accountId, int economicAccountId, int version, int type)
            where T1 : IRegisterPlAnswer, new()
            where T2 : InformationPlAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBack<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IRecord, new()
            where T5 : IPatient, new()
            where T6 : IMEvent, new();
        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZ<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
           where T2 : IAccount, new()
           where T3 : IHeader, new()
           where T4 : IZRecord, new()
           where T5 : IPatient, new()
           where T6 : IZslEvent, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZOnk<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
           where T2 : IAccount, new()
           where T3 : IHeader, new()
           where T4 : IZRecord, new()
           where T5 : IPatient, new()
           where T6 : IZslEvent, new();

        OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZOnk<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new()
            where T7 : IZEvent, new()
            where T8 : IDirectionOnkE, new()
            where T9 : IConsultationsOnk, new()
            where T10 : IEventOnk, new()
            where T11 : IDiagBlokOnk, new()
            where T12 : IContraindicationsOnk, new()
            where T13 : IServiceOnk, new()
            where T14 : IAnticancerDrugOnk, new()
            where T15 : IKsgKpg, new()
            where T16 : ISlKoef, new()
            where T17 : IRefusal, new()
            where T18 : IService, new();

        OperationResult<int> Load(DataCore.v21K2.D.AccountRegisterD registerData, DataCore.v21K2.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> Load(v30K1.D.AccountRegisterD registerData, v30K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadZ(v30K1.D.AccountRegisterD registerData, v30K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadZ(v31K1.D.AccountRegisterD registerData, v31K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing);
        OperationResult<int> LoadZ(v31K1.DV.AccountRegisterD registerData, v31K1.DV.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing);
        OperationResult<int> LoadZ(v32K1.D.AccountRegisterD registerData, v32K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing);
        OperationResult<int> Load(DataCore.v21.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadZ(v30.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadZ(v31.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadZ(v32.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadBack(DataCore.v21.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadBackZ(v30.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadBackZ(v31.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadBackZ(v32.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadAnswer(DataCore.v21.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadAnswer(v30.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadAnswer(v31.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations);
        OperationResult<int> LoadAnswer(v32.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations);
        OperationResult<T> Deserialize<T>(Stream stream);

        OperationResult<int> GetOmsFileVersion(string fileName);
        OperationResult<LoaderBase> GetLoaderByVersion(int version);
        OperationResult<int> GetVersion(int version);
        OperationResult<LoaderBase> GetLoaderByVersionD(int version);
        OperationResult ValidateXml(int version, LoaderBase loader, string file);
        OperationResult ValidateDXml(int version, LoaderBase loader, string lFile, string hFile);
        OperationResult UpdatePacketNumber(int accountId, RegisterInfo info);

       
    }
}