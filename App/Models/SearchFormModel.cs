using System;

namespace Medical.AppLayer.Models
{
    public class SearchFormModel
    {
        public string Name { get; set; }
        public object SelectedObject { get; set; }
        public Action<SearchFormModel> Handler { get; set; }
    }
}
