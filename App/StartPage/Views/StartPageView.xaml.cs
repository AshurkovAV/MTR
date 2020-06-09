namespace Medical.AppLayer.StartPage.Views
{
    /// <summary>
    /// Interaction logic for StartPageView.xaml
    /// </summary>
    public partial class StartPageView
    {
        public StartPageView()
        {
            InitializeComponent();
        }

        public void LoadLayout()
        {
            //var path = Path.GetDirectoryName(typeof(StartPageView).Assembly.Location);
            //if (!string.IsNullOrEmpty(path))
            //{
            //    if (File.Exists(Path.Combine(path, "layout.xml")))
            //    {
            //        using (var stream = new FileStream(Path.Combine(path, "layout.xml"), FileMode.Open))
            //        {
            //            using (var reader = XmlReader.Create(stream))
            //            {
            //                TileControl.ReadFromXML(reader);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        SaveLayout();
            //    }
            //}
        }

        private void SaveLayout()
        {
            //var path = Path.GetDirectoryName(typeof(StartPageView).Assembly.Location);
            //if(!string.IsNullOrEmpty(path))
            //{
            //    using (var stream = new FileStream(Path.Combine(path, "layout.xml"), FileMode.Create))
            //    {
            //        using (var writer = XmlWriter.Create(stream))
            //        {
            //            TileControl.WriteToXML(writer);
            //        }
            //    }
            //}
        }
    }
}