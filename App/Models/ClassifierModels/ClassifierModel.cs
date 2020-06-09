using System.ComponentModel.DataAnnotations;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class ClassifierModel
    {
        [Display(Name = @"Id справочника")]
        public int Id { get; set; }

        [Display(Name = @"Название группы справочников")]
        public string GroupName { get; set; }

        [Display(Name = @"Название справочника")]
        public string Name { get; set; }

        [Display(Name = @"Название таблицы")]
        public string Table { get; set; }
        
        [Display(Name = @"Название таблицы для импорта")]
        public string ImportTable { get; set; }

        [Display(Name = @"Разрешено редактировать справочник вручную")]
        public bool Editable { get; set; }

        [Display(Name = @"Тип ключа")]
        public bool IsManualIdentity { get; set; }
    }
}
