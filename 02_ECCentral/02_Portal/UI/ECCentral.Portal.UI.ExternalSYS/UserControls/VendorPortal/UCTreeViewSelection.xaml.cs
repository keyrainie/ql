using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UCTreeViewSelection : UserControl
    {
        

        public IDialog Dialog { get; set; }

        public UCTreeViewSelection(List<Node> nodes)
        {
            this.Nodes = nodes;

            InitializeComponent();


            this.Loaded += new RoutedEventHandler(UCTreeViewSelection_Load);
        }

        void UCTreeViewSelection_Load(object sender, RoutedEventArgs e)
        {
            BuildTreeByData();
        }



        //private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    HyperlinkButton btn = sender as HyperlinkButton;

        //    this.Dialog.ResultArgs.Data = btn.CommandParameter;
        //    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
        //    this.Dialog.Close(true);
        //}

        public void BuildTreeByData()
        {
            if (Nodes != null)
            {
                TreeNode = new Node { SonNodes = new List<Node>(), Value = 0 };
                GetTreeNodes(TreeNode, null);

                this.tvNodeTree.ItemsSource = TreeNode.SonNodes;
               

            }
        }

        //Node数据
        private List<Node> Nodes { get; set; }
        //树
        public Node TreeNode { get; set; }
        //数据转成树
        protected void GetTreeNodes(Node startNode, Node father)
        {
            //if (startNode.Value == 0)
            //    startNode.SonNodes = Nodes.Where(x => !x.ParentValue.HasValue).ToList();
            //else
            //{
            startNode.ParentNode = father;
            startNode.SonNodes = Nodes.Where(x => x.ParentValue.HasValue && x.ParentValue.Value == startNode.Value).ToList();
            //}

            foreach (Node node in startNode.SonNodes)
            {
                GetTreeNodes(node, startNode);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Node node = this.tvNodeTree.SelectedItem as Node;

            if (node != null)
            {
                this.Dialog.ResultArgs.Data = node.Name;
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            }
            else
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            }
            this.Dialog.Close(true);

        }
    }
    
}
