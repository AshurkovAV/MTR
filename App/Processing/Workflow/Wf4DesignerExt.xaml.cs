using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using Microsoft.Win32;

namespace Medical.AppLayer.Processing.Workflow
{
    /// <summary>
    /// Interaction logic for Wf4DesignerExt.xaml
    /// </summary>
    public partial class Wf4DesignerExt : UserControl
    {
        private WorkflowDesigner wd;
        private string workflowFilePathName = "temp.xaml";

        public Wf4DesignerExt()
        {
            InitializeComponent();
            //(new DesignerMetadata()).Register(); //Registers the runtime metadata.
            AddDesigner();
            AddToolBox();
            AddPropertyInspector();
        }

        private void AddDesigner()
        {
            wd = new WorkflowDesigner();
            workflowDesignerPanel.Content = wd.View;
        }

        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            toolboxPanel.Content = tc;
        }

        private ToolboxControl GetToolboxControl()
        {
            var toolboxControl = new ToolboxControl();
            ToolboxCategory toolboxCategory;

            //"System.Activities.Statements.Sequence","System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",null, "Sequence"


            toolboxCategory = new ToolboxCategory("Flow control");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Sequence), "Sequence"));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (DoWhile)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (ForEach<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (If), "If/Else"));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Parallel)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (ParallelForEach<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Pick)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (PickBranch)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Switch<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (While)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Flowchart");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Flowchart)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (FlowDecision)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (FlowSwitch<>)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Primitives");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (WriteLine)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Assign)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Delay)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (InvokeMethod)));
            toolboxControl.Categories.Add(toolboxCategory);

            /*toolboxCategory = new ToolboxCategory("Messaging");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (CorrelationScope)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (InitializeCorrelation)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Send)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Receive)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (TransactedReceiveScope)));
            toolboxControl.Categories.Add(toolboxCategory);*/

            toolboxCategory = new ToolboxCategory("Runtime");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Persist)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (TerminateWorkflow)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Transaction");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (CancellationScope)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (CompensableActivity)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Compensate)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Confirm)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (TransactionScope)));
            toolboxControl.Categories.Add(toolboxCategory);

            //toolboxCategory = new ToolboxCategory("Data");
            //toolboxCategory.Add(new ToolboxItemWrapper(typeof(ExecuteSqlQuery)));
            //toolboxCategory.Add(new ToolboxItemWrapper(typeof(ExecuteSqlQuery<>)));
            //toolboxCategory.Add(new ToolboxItemWrapper(typeof(ExecuteSqlNonQuery)));
            //toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Collection");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (AddToCollection<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (ExistsInCollection<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (ClearCollection<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (RemoveFromCollection<>)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Error handling");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Rethrow)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Throw)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (TryCatch)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Error handling");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Rethrow)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (Throw)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof (TryCatch)));
            toolboxControl.Categories.Add(toolboxCategory);

            return toolboxControl;
        }

        private void AddPropertyInspector()
        {
            if (wd == null)
                return;
            WorkflowPropertyPanel.Content = wd.PropertyInspectorView;
        }

        private void LoadWorkflowFromFile(string fileName)
        {
            workflowDesignerPanel.Content = null;
            WorkflowPropertyPanel.Content = null;
            wd = new WorkflowDesigner();
            wd.Load(fileName);
            //var designerView = wd.Context.Services.GetService<DesignerView>();
            //designerView.WorkflowShellBarItemVisibility = ShellBarItemVisibility.All;
            //designerView.WorkflowShellBarItemVisibility =
            //    ShellBarItemVisibility.Arguments |
            //    ShellBarItemVisibility.Imports |
            //    ShellBarItemVisibility.MiniMap |
            //    ShellBarItemVisibility.Variables |
            //    ShellBarItemVisibility.Zoom;
            workflowDesignerPanel.Content = wd.View;
            WorkflowPropertyPanel.Content = wd.PropertyInspectorView;
        }

        private void Save()
        {
            if (workflowFilePathName == "temp.xaml")
            {
                var saveFileDialog =
                    new SaveFileDialog();
                if (saveFileDialog.ShowDialog().Value)
                {
                    workflowFilePathName = saveFileDialog.FileName;
                    wd.Save(workflowFilePathName);
                    MessageBox.Show("Save Ok");
                }
                else
                {
                    return;
                }
            }
            else
            {
                wd.Save(workflowFilePathName);
                MessageBox.Show("Save Ok");
            }
            LoadWorkflowFromFile(workflowFilePathName);
        }


        private Activity GetActivity()
        {
            wd.Flush();
            var stringReader = new StringReader(wd.Text);
            Activity root =
                ActivityXamlServices.Load(stringReader);
            return root;
        }


        private void bSaveAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            var saveFileDialog =
                new SaveFileDialog();
            if (saveFileDialog.ShowDialog().Value)
            {
                workflowFilePathName = saveFileDialog.FileName;
                wd.Save(workflowFilePathName);
                MessageBox.Show("Save Ok");
            }
        }

        private void bOpen_ItemClick(object sender, ItemClickEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
            {
                workflowFilePathName = openFileDialog.FileName;
                LoadWorkflowFromFile(workflowFilePathName);
            }
        }

        private void bNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            workflowFilePathName = @"WFTemplate.xaml";
            LoadWorkflowFromFile(Path.Combine(Path.GetDirectoryName(typeof (WF4Designer).Assembly.Location),
                                              workflowFilePathName));
            workflowFilePathName = "temp.xaml";
        }

        private void bRun_ItemClick(object sender, ItemClickEventArgs e)
        {
            Save();
            //ConsoleManager.Show();
            Activity activity = GetActivity();
            var wfApp = new WorkflowApplication(activity);
            wfApp.Run();
        }

        private void bSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            Save();
        }

        //private void dockLayoutManager1_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (wd.Text != null)
        //    {
        //        wd.Flush();
        //        //xamlTextBox.Text = wd.Text;
        //    }
        //}
    }
}