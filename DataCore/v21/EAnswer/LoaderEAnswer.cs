namespace Medical.DataCore.v21.EAnswer
{
    public class LoaderEAnswer : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v21.Xsd.E3.xsd"; }
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