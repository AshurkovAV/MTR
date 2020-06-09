namespace Medical.CoreLayer.Models
{
    public class HospitalizationModel
    {
        private decimal _id;
        private string _name;

        public HospitalizationModel(decimal id, string name)
        {
            _id = id;
            _name = name;
        }

        public decimal Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }
    }
}
