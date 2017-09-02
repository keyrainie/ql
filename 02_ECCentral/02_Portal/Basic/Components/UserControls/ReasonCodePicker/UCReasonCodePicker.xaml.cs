using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker
{
    public partial class UCReasonCodePicker : UserControl
    {
        private const int MaxNodeLevel = 15;
        private const int REASONCODETREE_ROOT_PARENTREASONCODESYSNO = 0;

        public IDialog Dialog { get; set; }
        public ReasonCodeType ReasonCodeType { get; set; }
        public ReasonCodeTreeShowType ShowType { get; set; }
        private List<ReasonCodeEntity> reasonCodeList;

        public UCReasonCodePicker()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCReasonCodePicker_Loaded);
        }

        void UCReasonCodePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCReasonCodePicker_Loaded);
           
            new ReasonCodeFacade(CPApplication.Current.CurrentPage).GetReasonCodeList(MaxNodeLevel, (obj, args) =>
            {
                reasonCodeList = args.Result;

                if (ShowType == ReasonCodeTreeShowType.Active)
                {
                    reasonCodeList = reasonCodeList.Where(p => p.Status == 1).ToList();
                }
                ReasonCodeEntity dataNode = reasonCodeList.SingleOrDefault(p => p.ReasonCodeID.Trim() == this.ReasonCodeType.ToString());
                TreeViewItem treeNode = new TreeViewItem();
                treeNode.IsExpanded = true;
                if (dataNode != null)
                {
                    treeNode.Header = dataNode.Description;
                    treeNode.Tag = dataNode.SysNo;
                }
                AddChildNode(treeNode, dataNode, ref reasonCodeList);
                this.tvReasonCode.Items.Add(treeNode);
            });           
        }      

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var item = this.tvReasonCode.SelectedItem as TreeViewItem;
            this.Dialog.ResultArgs = new ResultEventArgs
            {
                DialogResult = DialogResultType.OK,
            };

            if (item != null && item.Tag != null)
            {
                string path = GetNodePath((int)item.Tag, ref reasonCodeList);
                KeyValuePair<string, string> kv = new KeyValuePair<string, string>(item.Tag.ToString(), path);
                this.Dialog.ResultArgs.Data = kv;
            }

            this.Dialog.Close();
        }

        private void AddChildNode(TreeViewItem treeNode, ReasonCodeEntity dataNode, ref List<ReasonCodeEntity> allList)
        {            
            List<ReasonCodeEntity> childNodes = allList.Where(p => p.ParentNodeSysNo == dataNode.SysNo).ToList();
            foreach (ReasonCodeEntity currentDataNote in childNodes)
            {

                TreeViewItem currentNode = new TreeViewItem();
                currentNode.Tag = currentDataNote.SysNo;
                //if (currentDataNote.Status == 1)
                //{
                //    currentNode.Checked = true;
                //}
                currentNode.Header = currentDataNote.Description;                
                AddChildNode(currentNode, currentDataNote, ref allList);
                treeNode.Items.Add(currentNode);
            }
        }

        private string GetNodePath(int sysNo, ref List<ReasonCodeEntity> allList)
        {
            string result = string.Empty;

            ReasonCodeEntity nodeData = allList.SingleOrDefault(p => p.SysNo == sysNo);
            Stack<string> stackPath = new Stack<string>();

            stackPath.Push(nodeData.Description);
            while (nodeData.ParentNodeSysNo != REASONCODETREE_ROOT_PARENTREASONCODESYSNO)
            {
                nodeData = allList.Single(p => p.SysNo == nodeData.ParentNodeSysNo);
                stackPath.Push(nodeData.Description);
            }

            StringBuilder sb = new StringBuilder();

            while (stackPath.Count > 0)
            {
                sb.Append(stackPath.Pop());
                sb.Append(">");
            }
            result = sb.ToString();
            if (result.EndsWith(">"))
            {
                result = result.Remove(sb.ToString().Length - 1, 1);
            }

            return result;
        }
    }    
}
