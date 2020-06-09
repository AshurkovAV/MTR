using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.XtraEditors.DXErrorProvider;
using Medical.DatabaseCore.Services.Classifiers;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;

namespace Medical.AppLayer.Economic.Models
{
    public class PaymentCustomModel : IDXDataErrorInfo
    {

        public ItemCollection V006Items
        {
            get { return new V006ItemsSource().GetValues(); }
        }

        public int EconomicPaymentId { get; set; }
        public int EconomicAccountId { get; set; }

        public bool IsSelected { get; set; }

        [Required(ErrorMessage = @"Поле 'Условия оказания МП' обязательно для заполнения")]
        [DisplayName(@"Условия оказания МП")]
        public int AssistanceConditionsId { get; set; }

        [Required(ErrorMessage = @"Поле 'Сумма' обязательно для заполнения")]
        [DisplayName(@"Сумма")]
        public decimal Amount { get; set; }


        public void GetPropertyError(string propertyName, ErrorInfo info)
        {
            
        }

        public void GetError(ErrorInfo info)
        {
            
        }

        private void SetErrorInfo(ErrorInfo info, string errorText, ErrorType errorType)
        {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }
    }
}
