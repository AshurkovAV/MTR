using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.LayoutControl;
using Medical.CoreLayer.View;

namespace Medical.CoreLayer.Helpers
{
    public class OptimizerHelpers
    {
        public static void RunTypeInitializers(Assembly a)
        {
            Type[] types = a.GetExportedTypes();
            foreach (var t in types)
            {
                RuntimeHelpers.RunClassConstructor(t.TypeHandle);
            }
        }

        public static void PreloadDevexpress()
        {
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();

            PreloadViewWindow(new DatabaseConfigView());

            RunTypeInitializers(Assembly.GetAssembly(typeof(LayoutControl)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(TextEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(TreeListControl)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(GridControl)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(BarManager)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(CheckEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(MemoEdit)));

            RunTypeInitializers(Assembly.GetAssembly(typeof(LayoutItem)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(ComboBoxEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(LookUpEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(LayoutGroup)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(DateEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(ButtonEdit)));

            ThemeManager.SetThemeName(new TextEdit(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new TreeListControl(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new GridControl(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new BarManager(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new CheckEdit(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new MemoEdit(), Theme.DeepBlueName);

            ThemeManager.SetThemeName(new LayoutItem(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new ComboBoxEdit(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new LookUpEdit(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new LayoutGroup(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new DateEdit(), Theme.DeepBlueName);
            ThemeManager.SetThemeName(new ButtonEdit(), Theme.DeepBlueName);
        }

        private static void PreloadViewUserControl(UserControl control)
        {
            var view = new Window
            {
                Content = control,
                WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
                Left = int.MaxValue,
                ShowInTaskbar = false
            };
            view.Show();
            view.Hide();
            view.Close();
        }

        private static void PreloadViewWindow(Window view)
        {
            view.WindowStartupLocation = WindowStartupLocation.Manual;
            view.Left = int.MaxValue;
            view.ShowInTaskbar = false;
            view.Show();
            view.Hide();
            view.Close();
        }
    }
}
