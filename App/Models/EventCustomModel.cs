using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Autofac;
using Core.Services;
using DataModel;
using DevExpress.XtraEditors.DXErrorProvider;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models
{
    public class EventCustomModel : BaseEntity, IDXDataErrorInfo
    {
        private static readonly ICacheRepository _cache;

        static EventCustomModel()
        {
            _cache = Di.I.Resolve<ICacheRepository>();
        }

        public EventCustomModel()
        {


        }

        [DisplayName(@"ID случая")]
        public int EventId { get; set; }

        [DisplayName(@"Позиция в счете")]
        public int? ExternalId { get; set; }

        [DisplayName(@"ID врача")]
        public string DoctorId { get; set; }

        [DisplayName(@"Код МО")]
        public string MedicalOrganizationCode { get; set; }

        [DisplayName(@"Стоимость")]
        public decimal? Price { get; set; }

        [DisplayName(@"Принятая к оплате")]
        public decimal? AcceptPrice { get; set; }

        [DisplayName(@"Диагноз основной")]
        public string DiagnosisGeneral { get; set; }

        [DisplayName(@"Диагноз основной")]
        public string DiagnosisSecondary { get; set; }

        [DisplayName(@"№ Истории болезни/талон")]
        public string History { get; set; }

        [DisplayName(@"Дата начала случая")]
        public DateTime? EventBegin { get; set; }

        [DisplayName(@"Код Профиля МП")]
        public int? ProfileCodeId  { get; set; }

        [DisplayName(@"Дата окончания случая")]
        public DateTime? EventEnd { get; set; }

        [DisplayName(@"Кол-во единиц оплаты")]
        public decimal? Quantity { get; set; }

        [Required(ErrorMessage = @"Поле 'Код отказа' обязательно для заполнения")]
        [DisplayName(@"Код отказа")]
        public int ReasonId { get; set; }

        [DisplayName(@"Сумма отказа")]
        public decimal? RefusalPrice { get; set; }

        [DisplayName(@"Штраф")]
        public decimal? PenaltyPrice { get; set; }
        
        [DisplayName(@"Исход")]
        public int? Outcome { get; set; }

        [DisplayName(@"Диагноз осложнения")]
        public string DiagnosisComplication { get; set; }

        [DisplayName(@"Позиция в счета и дата лечения")]
        public string ExternalIdDate
        {
            get { return string.Format("№{0}   {1}-{2}", ExternalId, EventBegin.HasValue ? EventBegin.Value.ToString("d") : "отсутствует", EventEnd.HasValue ? EventEnd.Value.ToString("d") : "отсутствует"); }
        }

        [DisplayName(@"Код отказа")]
        public string ReasonCode {
            get
            {
                return _cache.Get(CacheRepository.F014aCache).GetString(ReasonId);
            } 
        }

        [DisplayName(@"Профиль МП")]
        public string Profile
        {
            get
            {
                return _cache.Get(CacheRepository.V002Cache).GetString(ProfileCodeId);
            }
        }

        public void GetPropertyError(string propertyName, ErrorInfo info)
        {
            switch (propertyName)
            {
                case "ReasonId":
                    if (ReasonId == 0)
                    {
                        SetErrorInfo(info, "Поле 'Код отказа' обязательно для заполнения!", ErrorType.Critical);
                    }
                    break;
            }
        }

        public void GetError(ErrorInfo info)
        {
            //if (true)
            //    SetErrorInfo(info, "аЙ АЙ", ErrorType.Information);
        }

        private void SetErrorInfo(ErrorInfo info, string errorText, ErrorType errorType)
        {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }





        
    }
}