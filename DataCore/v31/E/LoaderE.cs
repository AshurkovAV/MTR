﻿namespace Medical.DataCore.v31.E
{
    public class LoaderE : LoaderBase
    {
        public override string SchemePath
        {
            get { return "Medical.DataCore.v31.Xsd.E2.xsd"; }
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