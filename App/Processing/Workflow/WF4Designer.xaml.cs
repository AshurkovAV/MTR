using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Medical.AppLayer.Processing.Workflow
{
    /// <summary>
    /// Interaction logic for WF4Designer.xaml
    /// </summary>
    public partial class WF4Designer : UserControl
    {
        private WorkflowDesigner wd;
        private string workflowFilePathName = "temp.xaml";

        public WF4Designer()
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
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Sequence)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(DoWhile)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(ForEach<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(If)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Parallel)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(ParallelForEach<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Pick)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(PickBranch)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Switch<>)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(While)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Flowchart");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Flowchart)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(FlowDecision)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(FlowSwitch<>)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("Primitives");
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(WriteLine)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Assign)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(Delay)));
            toolboxCategory.Add(new ToolboxItemWrapper(typeof(InvokeMethod)));
            toolboxControl.Categories.Add(toolboxCategory);

            toolboxCategory = new ToolboxCategory("DataService");
            //toolboxCategory.Add(new ToolboxItemWrapper(typeof(Activity1)));
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

        private void MenuItem_Click_NewWorkflow(object sender,
                                                RoutedEventArgs e)
        {
            workflowFilePathName = @"WFTemplate.xaml";
            LoadWorkflowFromFile(Path.Combine(Path.GetDirectoryName(typeof(WF4Designer).Assembly.Location), workflowFilePathName));
            workflowFilePathName = "temp.xaml";
        }

        private void MenuItem_Click_LoadWorkflow(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
            {
                workflowFilePathName = openFileDialog.FileName;
                LoadWorkflowFromFile(workflowFilePathName);
            }
        }


        private void Save()
        {
            if (workflowFilePathName == "temp.xaml")
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog =
                new Microsoft.Win32.SaveFileDialog();
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
        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private void MenuItem_Click_SaveAs(object sender, RoutedEventArgs
        e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog =
            new Microsoft.Win32.SaveFileDialog();
            if (saveFileDialog.ShowDialog().Value)
            {
                workflowFilePathName = saveFileDialog.FileName;
                wd.Save(workflowFilePathName);
                MessageBox.Show("Save Ok");
                
            }
        }

        Activity GetActivity()
        {
            wd.Flush();
            System.IO.StringReader stringReader = new System.IO.StringReader(wd.Text);
            Activity root =
            System.Activities.XamlIntegration.ActivityXamlServices.Load(stringReader) as Activity;
            return root;
        }

        private void MenuItem_Click_RunWorkflow(object sender,
        RoutedEventArgs e)
        {
            Save();
            ConsoleManager.Show();
            Activity activity = GetActivity();
            WorkflowApplication wfApp = new WorkflowApplication(activity);
            wfApp.Run();
        }



        private void TabItem_GotFocus_RefreshXamlBox(object sender,RoutedEventArgs e)
        {
            if (wd.Text != null)
            {
                wd.Flush();
                xamlTextBox.Text = wd.Text;
            }
        }
    }
}