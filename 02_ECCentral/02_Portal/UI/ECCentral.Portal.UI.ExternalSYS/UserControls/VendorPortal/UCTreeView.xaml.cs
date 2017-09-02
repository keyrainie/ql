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
using System.Collections;
using System.ComponentModel;
using Newegg.Oversea.Silverlight.Utilities;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UCTreeView : UserControl
    {
        public UCTreeView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCTreeView_Load);
        }

        void UCTreeView_Load(object sender,RoutedEventArgs e)
        {
 
        }

        public bool IsSpeardToParent { get; set; }

        public void BuildTreeByData()
        {

            if (Nodes != null)
            {
                TreeNode = new Node { SonNodes = new List<Node>(), Value = 0, SpreadToParent = this.IsSpeardToParent };
                GetTreeNodes(TreeNode, null);
                this.tvNodeTree.ItemsSource = TreeNode.SonNodes;
                this.tvNodeTree.ExpandAll();
                
            }
 
        }

        public void BuildTreeByNode()
        {
            if (this.TreeNode != null)
            {
                this.tvNodeTree.ItemsSource = null;
                this.tvNodeTree.ItemsSource = TreeNode.SonNodes;
                
            }
 
        }


        //Node数据
        public List<Node> Nodes { get; set; }
        //树
        public Node TreeNode { get; set; }
        //数据转成树
        protected void GetTreeNodes(Node startNode,Node father)
        {

            startNode.ParentNode = father;
            startNode.SpreadToParent = this.IsSpeardToParent;
            startNode.SonNodes = Nodes.Where(x => x.ParentValue.HasValue && x.ParentValue.Value == startNode.Value).ToList();

            foreach (Node node in startNode.SonNodes)
            {
                GetTreeNodes(node, startNode);
            }
        }

        //选择Clone
        public List<Node> SelectData(Func<Node, bool> predicate)
        {
            List<Node> list = new List<Node>();

            FindNode(predicate, TreeNode, list);

            return list;
        }
        private void FindNode(Func<Node, bool> predicate,Node tree, List<Node> list)
        {
            if (tree!=null  && predicate(tree))
            {
                list.Add(new Node { 
                     Value=tree.Value
                     ,Name=tree.Name
                     , ParentValue=tree.ParentValue
                     , IsSelected=false
                     , Spread=true
                    });
            }

            if (tree != null && tree.SonNodes != null)
            {
                foreach(Node node in tree.SonNodes)
                {
                    FindNode(predicate, node, list);
                }
            }
        }
        //删除未选中节点
        public void DeleteNodeNotSelected()
        {
            DeleteNodeNotSelected(this.TreeNode);
        }
        protected void DeleteNodeNotSelected(Node treeNode)
        {
            if (treeNode != null && treeNode.SonNodes != null)
            {
                for (int i = treeNode.SonNodes.Count - 1; i >= 0; i--)
                {
                    if (!treeNode.SonNodes[i].IsSelected)
                    {
                        treeNode.SonNodes.RemoveAt(i);
                    }
                    else
                    {
                        DeleteNodeNotSelected(treeNode.SonNodes[i]);
                    }
                }
            }
        }
        //删除选中节点
        public void DeleteNodeSelected()
        {
            DeleteNodeSelected(this.TreeNode);
        }
        protected void DeleteNodeSelected(Node treeNode)
        {
            if (treeNode != null && treeNode.SonNodes != null)
            {
                for (int i = treeNode.SonNodes.Count - 1; i >= 0; i--)
                {
                    if (treeNode.SonNodes[i].IsSelected)
                    {
                        treeNode.SonNodes.RemoveAt(i);
                    }
                    else
                    {
                        DeleteNodeSelected(treeNode.SonNodes[i]);
                    }
                }
            }
        }





        //得到选中的节点树
        public Node GetTreeNodesIsSelected()
        {
            var t = UtilityHelper.BinarySerialize(this.TreeNode);
            Node seletedNode = UtilityHelper.BinaryDeserialize<Node>(t);

            //Node seletedNode= UtilityHelper.DeepClone(TreeNode);

            if(seletedNode!=null)
            {
                DeleteNodeNotSelected(seletedNode);
            }

            return seletedNode;
        }
        

        //public void GetTreeNodeData(Node treeNode, List<Node> data)
        //{
        //    if (data == null)
        //        data = new List<Node>();

        //    if (treeNode.Value != 0)
        //        data.Add(treeNode);

        //    if (treeNode.SonNodes != null)
        //    {
        //        foreach (Node son in treeNode.SonNodes)
        //        {
        //            //data.Add(son);
        //            GetTreeNodeData(son, data);

        //        }

        //    }
        //}

        //public List<Node> GetSelectedNodeData()
        //{
        //    List<Node> data = new List<Node>();

        //    Node selectedNodes = GetTreeNodesIsSelected();

        //    GetTreeNodeData(selectedNodes, data);

        //    return data;
        //}

    }

    public class Node : INotifyPropertyChanged
    {   //父节点Key
        public int? ParentValue { get; set; }
        //节点名称
        public string Name { get; set; }
        //节点Key
        public int Value { get; set; }
        //是否选中
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            { 
                isSelected = value;
                onPropertyChanged(this, "IsSelected");

                if (!Spread.HasValue||!isSelected)
                    Spread = true;

                
                SetParent(isSelected && SpreadToParent);

                SetChildren(isSelected);
            }

        }
        //子节点
        public List<Node> SonNodes { get; set; }
        //父节点
        public Node ParentNode { get; set; }
        //是否扩散到子节点
        public bool? Spread { get; set; }
        //是否扩散到父节点
        public bool SpreadToParent { get; set; }
        //通知事件
        public event PropertyChangedEventHandler PropertyChanged;
        //通知
        private void onPropertyChanged(object sender, string propertName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender,new PropertyChangedEventArgs(propertName));
            }
        }
        //设置子节点
        public void SetChildren(bool isSelected)
        {
            if (Spread.HasValue && Spread.Value)
            {
                if (isSelected && this.SonNodes != null)
                {
                    foreach (Node sonNode in this.SonNodes)
                    {
                        sonNode.IsSelected = true;
                    }
                }
                else if (!IsSelected && this.SonNodes != null)
                {
                    foreach (Node sonNode in this.SonNodes)
                    {
                        sonNode.IsSelected = false;
                    }
                }
            }
        }
        //设置父节点
        public void SetParent(bool isSelected)
        {
            if (isSelected)
            {
                if (this.ParentNode != null && this.ParentNode.IsSelected == false)
                {
                    this.ParentNode.Spread = false;
                    this.ParentNode.IsSelected = true;
                }
            }
        }

    }
}
