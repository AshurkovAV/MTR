namespace Medical.DataCore.v10.E
{
    public class LoaderE : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v10.Xsd.E2.xsd"; }
        }

        public override int FileCount
        {
            get { return 1; }
        }

        public override string FileExtension
        {
            get { return "xml"; }
        }
    }
}