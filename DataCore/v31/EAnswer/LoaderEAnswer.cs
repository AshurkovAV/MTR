﻿namespace Medical.DataCore.v31.EAnswer
{
    public class LoaderEAnswer : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v30.Xsd.E3.xsd"; }
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