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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Common.UserControls;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ReasonCode : PageBase
    {
        ReasonCodeFacade m_facade;

        List<ReasonCodeNode> ReasonCodelist { get; set; }

        public ReasonCode()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);


            this.cmbType.ItemsSource = EnumConverter.GetKeyValuePairs<ReasonCodeType>();
            this.cmbType.SelectedIndex = 0;

            m_facade = new ReasonCodeFacade(this);

            BindDataFromDB();
        }

        private void ResetCtrl()
        {
            this.chxShowAva.IsChecked = false;
            this.tbxDesc.Text = string.Empty;
            this.cmbType.SelectedIndex = 0;
        }

        private void BindDataFromDB()
        {
            m_facade.GetReasonCodeList(999, (obj, args) =>
            {
                List<ReasonCodeEntity> list = args.Result;
                ReasonCodelist = TransferToNodes(list);

                this.tvReasonCode.NodeData = ReasonCodelist;
                this.tvReasonCode.BuildTreeByData();
            });
        }


        private List<ReasonCodeNode> TransferToNodes(List<ReasonCodeEntity> nodes)
        {

            List<ReasonCodeNode> list = new List<ReasonCodeNode>();

            if (nodes != null)
            {
                foreach(ReasonCodeEntity node in nodes )
                {
                    list.Add(new ReasonCodeNode
                    {
                        SysNo = node.SysNo.Value
                        ,
                        Description = node.Description
                        ,
                        ParentSysNo = node.ParentNodeSysNo
                        ,
                        ReasonCodeType = node.ReasonCodeType.HasValue ? node.ReasonCodeType.Value : 0
                        ,
                        Status = node.Status.Value
                        ,
                        IsChecked = node.Status.HasValue ? node.Status.Value == 1 ? true : false : true
                        ,
                        IsExpanded = node.ParentNodeSysNo == 0 ? true : false
                    });
                }
            }
            return list;
        }
        //查询
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string desc = this.tbxDesc.Text;
            int type = (int)this.cmbType.SelectedValue;

            if (string.IsNullOrEmpty(desc) && type == 0)
            {
                Window.Alert("请选择至少一个条件");
                return;
            }

            InitialAll(this.tvReasonCode.NodeTree);

            List<ReasonCodeNode> list = this.tvReasonCode.Query(node =>{
                
                
                bool cond1=string.IsNullOrEmpty(desc)?true:string.IsNullOrEmpty(node.Description)?false:node.Description.Contains(desc);

                bool cond2=type==0?true:node.ReasonCodeType == type;


                return cond1 && cond2;

            });

            list.ForEach(x =>
            {
                x.NodeFontColor = new SolidColorBrush(Colors.Red);
                ExpandFather(x);
            });


        }
        //添加
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ReasonCode_Add))
            {
                Window.Alert("您没有此功能的操作权限");
                return;
            }
            ReasonCodeNode node= this.tvReasonCode.SelectedNode;

            if (node == null)
            {
                Window.Alert("请选择一个节点");
                return;
            }

            string path=this.tvReasonCode.ReasonCodePath;

            UCReasonCodeEdit reasonCodeCtrl = new UCReasonCodeEdit(node.SysNo,node.ParentSysNo, node.Description, path, false);
            reasonCodeCtrl.Dialog = Window.ShowDialog(
                        "添加节点"
                        , reasonCodeCtrl
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                BindDataFromDB();
                            }
                        }
                        , new Size(550, 300)
                );
            

        }
        //编辑
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ReasonCode_Edit))
            {
                Window.Alert("您没有此功能的操作权限");
                return;
            }
            ReasonCodeNode node = this.tvReasonCode.SelectedNode;

            if (node == null)
            {
                Window.Alert("请选择一个节点");
                return;
            }
            else if (node.ParentSysNo == 0 || node.ParentSysNo == 1)
            {
                Window.Alert("不能编辑系统根节点");
                return;
 
            }

            string path = this.tvReasonCode.ReasonCodePath;

            UCReasonCodeEdit reasonCodeCtrl = new UCReasonCodeEdit(node.SysNo, node.ParentSysNo, node.Description, path, true);
            reasonCodeCtrl.Dialog = Window.ShowDialog(
                        "编辑节点"
                        , reasonCodeCtrl
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                BindDataFromDB();
                            }
                        }
                        , new Size(550, 300)
                );
        }


        private void ExpandFather(ReasonCodeNode node)
        {
            if (node != null && node.ParentNode!=null)
            {
                if (node.ParentNode.IsExpanded == false)
                    node.ParentNode.IsExpanded = true;

                ExpandFather(node.ParentNode);
            }
        }


        private void InitialAll(ReasonCodeNode node)
        {
            if (node != null)
            {

                if (node.ParentSysNo == 0)
                    node.IsExpanded = true;
                else
                    node.IsExpanded = false;

                if(node.Status==0)
                    node.NodeFontColor = new SolidColorBrush(Colors.Gray);
                else
                    node.NodeFontColor = new SolidColorBrush(Colors.Blue);

                

                if (node.SonNodes != null)
                {
                    foreach (ReasonCodeNode son in node.SonNodes)
                    {
                        InitialAll(son);
                    }
                }
            }
        }

        //保存所有激活状态
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ReasonCode_Save))
            {
                Window.Alert("您没有此功能的操作权限");
                return;
            }

            List<ReasonCodeNode> voiddata = this.tvReasonCode.Select(x => (x.Status == 1 && !x.IsChecked));
            List<ReasonCodeNode> avadata = this.tvReasonCode.Select(x => (x.Status == 0 && x.IsChecked));
            List<ReasonCodeEntity> dataList = new List<ReasonCodeEntity>();

            if ((voiddata == null || voiddata.Count == 0)
                && (avadata == null || avadata.Count == 0))
            {
                Window.Alert("没有需要更新的数据");
                return;
            }
            else
            {
                voiddata.ForEach(x =>
                {
                    dataList.Add(new ReasonCodeEntity
                    {
                        SysNo = x.SysNo
                        ,
                        EditUser = CPApplication.Current.LoginUser.DisplayName
                        ,
                        Status = 0
                    });
                });
                avadata.ForEach(x =>
                {
                    dataList.Add(new ReasonCodeEntity
                    {
                        SysNo = x.SysNo
                        ,
                        EditUser = CPApplication.Current.LoginUser.DisplayName
                        ,
                        Status = 1
                    });
                });
            }

            m_facade.UpdateReasonStatusList(dataList, (o, arg) => {

                if (arg.FaultsHandle())
                   return;

                BindDataFromDB();
            });

            ResetCtrl();
        }


        private void chxShowAva_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chbx = sender as CheckBox;

            InitialAll(this.tvReasonCode.NodeTree);

            if (chbx != null)
            {
                if (chbx.IsChecked.Value)
                {
                    this.tvReasonCode.NodeData = this.tvReasonCode.NodeData.Where(x => x.Status == 1).ToList();
                    this.tvReasonCode.BuildTreeByData();
                }
                else
                {
                    this.tvReasonCode.NodeData = ReasonCodelist;
                    this.tvReasonCode.BuildTreeByData();

                }
            }
        }

       
     

    }
}
