using System;
using System.Collections.ObjectModel;
using Medical.AppLayer.Models;

namespace Medical.AppLayer.SampleData
{
    public class MigrationObservableCollectionData : ObservableCollection<MigrationModel>
    {
        public MigrationObservableCollectionData()
        {
            Add(new MigrationModel { Name = "test text 1", Applied = true, Date = DateTime.Now });
            Add(new MigrationModel {Name = "test text 3", Applied = false, Date = DateTime.Now});
            throw new Exception("bilyat");
            
        }
    }
}
