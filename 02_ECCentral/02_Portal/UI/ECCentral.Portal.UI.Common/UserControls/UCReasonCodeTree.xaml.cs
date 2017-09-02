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
using System.ComponentModel;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCReasonCodeTree : UserControl
    {
        public UCReasonCodeTree()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCReasonCodeTree_Load);
        }

        void UCReasonCodeTree_Load(object sender, RoutedEventArgs e)
        {

        }
        //节点数据
        public List<ReasonCodeNode> NodeData { get; set; }
        //节点树
        public ReasonCodeNode NodeTree { get; set; }
        //节点数据转成树
        public void BuildTreeByData()
        {
            if (NodeData != null)
            {
                //RootNode
                NodeTree = new ReasonCodeNode { SonNodes = new List<ReasonCodeNode>(),SysNo = 0 };
                //BuildTree
                GetTreeNodes(NodeTree, null);
                this.tvNodeTree.ItemsSource = NodeTree.SonNodes;
            }
        }
        //递归遍历
        protected void GetTreeNodes(ReasonCodeNode startNode, ReasonCodeNode father)
        {
            startNode.ParentNode = father;
            startNode.SonNodes = NodeData.Where(x => x.ParentSysNo.HasValue && x.ParentSysNo.Value == startNode.SysNo).ToList();

            foreach (ReasonCodeNode node in startNode.SonNodes)
            {
                GetTreeNodes(node, startNode);
            }
        }
        //选中节点
        public ReasonCodeNode SelectedNode
        {
            get { return this.tvNodeTree.SelectedItem as ReasonCodeNode; }
        }
        //选中节点的Path
        public string ReasonCodePath
        {
            get 
            {
               return GetReasonCodePath(SelectedNode, "");
            }
        }
        //Select数据
        public List<ReasonCodeNode> Select(Func<ReasonCodeNode, bool> predicate)
        {
            return NodeData.Where(predicate).ToList();
        }
        //Query数据
        public List<ReasonCodeNode> Query(Func<ReasonCodeNode, bool> predicate)
        {
            List<ReasonCodeNode> list=new List<ReasonCodeNode>();
            if (NodeTree != null)
            {
                FindNodes(predicate, NodeTree, list);
                
            }
            return list;
        }
        //查找数据
        private void FindNodes(Func<ReasonCodeNode, bool> predicate,ReasonCodeNode node,List<ReasonCodeNode> result)
        {
            if(predicate(node))
            {
                result.Add(node);
            }

            if(node.SonNodes!=null)
            {
                foreach(ReasonCodeNode son in node.SonNodes)
                {
                    FindNodes(predicate, son, result);
                }
            }
        }
        //遍历获取Path
        private string GetReasonCodePath(ReasonCodeNode node,string path)
        {
            if (node == null)
                return string.Empty;

            string sonpath= string.IsNullOrEmpty(path)?"":">"+path;
            string currentPath = node.Description + sonpath;

            if (node.ParentNode != null)
            {
                currentPath = GetReasonCodePath(node.ParentNode, currentPath);
            }
            
            return currentPath;

        }
    }
    //节点
    public class ReasonCodeNode : INotifyPropertyChanged
    {

        public ReasonCodeNode ParentNode { get; set; }

        public List<ReasonCodeNode> SonNodes { get; set; }

        public int SysNo { get; set; }

        public int ReasonCodeType { get; set; }

        public int? ParentSysNo { get; set; }

        public string Description { get; set; }

        private int status;
        public int Status
        {
            get { return status; }
            set {
                status = value;

                if (status == 0)
                {
                    this.NodeFontColor = new SolidColorBrush(Colors.Gray);

                    if(this.IsChecked)
                        this.IsChecked = false;
                }
                else
                    this.NodeFontColor = new SolidColorBrush(Colors.Blue);
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {

                isExpanded = value;
                onPropertyChanged(this, "IsExpanded");
            }
        }

        private bool isChecked;
        public bool IsChecked 
        {
            get { return isChecked; }
            set {

                isChecked = value;
                onPropertyChanged(this, "IsChecked");


                if (!Spread.HasValue || !isChecked)
                    Spread = true;

                SetParent(isChecked);
                SetChildren(isChecked);

            }
        }


        private Brush nodeFontColor;
        public Brush NodeFontColor
        {
            get { return nodeFontColor; }
            set {
                nodeFontColor = value;
                onPropertyChanged(this, "NodeFontColor");
            }
 

        }

        //通知事件
        public event PropertyChangedEventHandler PropertyChanged;
        //通知
        private void onPropertyChanged(object sender, string propertName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertName));
            }
        }

        //是否扩散到子节点
        public bool? Spread { get; set; }

        //设置子节点
        public void SetChildren(bool isChecked)
        {
            if (Spread.HasValue && Spread.Value)
            {
                if (isChecked && this.SonNodes != null)
                {
                    foreach (ReasonCodeNode sonNode in this.SonNodes)
                    {
                        sonNode.IsChecked = true;
                    }
                }
                else if (!isChecked && this.SonNodes != null)
                {
                    foreach (ReasonCodeNode sonNode in this.SonNodes)
                    {
                        sonNode.IsChecked = false;
                    }
                }
            }
        }
        //设置父节点
        public void SetParent(bool isChecked)
        {
            if (isChecked)
            {
                if (this.ParentNode != null && this.ParentNode.IsChecked == false)
                {
                    this.ParentNode.Spread = false;
                    this.ParentNode.IsChecked = true;
                }
            }
        }

    }



}
