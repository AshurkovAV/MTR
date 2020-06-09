using System;
using System.Collections.Generic;
using Core;
using Medical.AppLayer.Models.OperatorModels;
using Medical.CoreLayer.Attributes;

namespace Medical.AppLayer.Managers
{
    public interface IDockLayoutManager
    {
        object GetActiveWorkspaceDataContext();
        void ShowLogin();
        void ShowOperator(int id, OperatorMode mode, IEnumerable<OperatorAction> operatorActions = null);
        void ShowReport();
        void ShowPreparedReport();
        void ShowCriterion();
        void ShowEconomicReport();
        void Clear();
        void ShowOverlay();
        void ShowOverlay(string title, string message, double progress = double.NaN);
        void SetOverlayMessage(string message);
        void SetOverlayProgress(double percent);
        void SetOverlayProgress(double total, double progress);
        void HideOverlay();
        void ShowEconomicAccount(int? id = null);
        void ShowEconomicRefuse(int? id = null);
        void ShowEconomicSurcharge(int? id = null);

        void ShowTerritoryAccountCollection(List<int> idAccounts);
        void ShowActExpertiseCollection(int idAct, VidControls vidExpertise);
        void ShowEconomicJournal();
        void ShowDashboard();


        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Импорт реестров\nМО", "/Resource/Icons/64/go-bottom-out.png", 1, "#FF2d89ef", 0)]
        void ShowImportLocal();
        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Импорт реестров\nс территорий", "/Resource/Icons/64/go-bottom-in.png", 1, "#FF2d89ef", 0)]
        void ShowImportInterTerritorial();
        void ShowImportInterTerritorial(string fileName);
      
       
        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр счетов\nс территорий", "/Resource/Icons/64/text-x-changelog.png", 1, "#FF00C000", 0)]
        void ShowTerritoryAccount();
         [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр счетов\nМО", "/Resource/Icons/64/text-x-changelog.png", 1, "#FF00C000", 0)]
        void ShowMedicalAccount();

        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр журнала\nинф. обмена", "/Resource/Icons/64/text-x-log.png", 1, "#FFBA55D3", 2)]
        void ShowExchange();
        void ShowExchange(int? id);
        void ShowExchangeTerritory(int? id);
        void ShowExchangeLocal(int? id);
        void ExportVolume();
        void ShowSearch();
        void ShowReportDesigner();
        void ShowReportView();

        void ShowXmlEditor(Tuple<string, string> data);
        void ShowUsersManagement();

        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр файлов\nинф. обмена", "/Resource/Icons/64/drive-harddisk.png", 1, "#FF2b5797", 2)]
        void ShowFileView();
        void ShowProcessing();
        void ShowClassifier();

        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр актов\nэкспертиз МО", "/Resource/Icons/64/text-x-log.png", 1, "#FF00aba9", 3)]
        void ShowActsMo();
        [StartPageCommand(typeof(IDockLayoutManager), "Счета", "Просмотр актов\nэкспертиз для территорий", "/Resource/Icons/64/text-x-log.png", 1, "#FF00aba9", 3)]
        void ShowActsTerritory();
        //[StartPageCommand(typeof(IDockLayoutManager), "Счета", "Акты экспертиз МО", "/Resource/Icons/64/text-x-log.png", 1, "#FF00aba9", 3)]
        void ShowActsExpertiseMo(int? id);

        void ShowActs(int scope, int? id);
    }
}