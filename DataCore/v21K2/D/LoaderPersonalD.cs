namespace Medical.DataCore.v21K2.D
{
    public class LoaderPersonalD : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v21K2.Xsd.D2.xsd"; }
        }

        public override int FileCount
        {
            get { return 2; }
        }

        public override string FileExtension
        {
            get { return "xml"; }
        }
    }
}