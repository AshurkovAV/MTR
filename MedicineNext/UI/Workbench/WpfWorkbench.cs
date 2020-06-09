using System;

namespace MedicineNext.UI.Workbench
{
    public class WpfWorkbench
    {
        private static volatile WpfWorkbench _instance;
        private static readonly object SyncRoot = new Object();

        private WpfWorkbench() { }

        public static WpfWorkbench Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new WpfWorkbench();
                    }
                }

                return _instance;
            }
        }

        public static DevExpress.Xpf.Docking.DockLayoutManager DockManager { get; set; }   
    }
}
