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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.BizEntity.ExternalSYS;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UserRoleAdd : UserControl
    {
        int roleSysNo;

        string roleName;

        bool IsUpdate { get; set; }

        ValidStatus roleStatus { get; set; }


        VendorFacade m_facade;

        public UserRoleAdd(int roleSysNo, string roleName, ValidStatus status,bool isUpdate)
        {
            this.roleSysNo = roleSysNo;
            this.roleName = roleName;
            this.roleStatus = status;
            this.IsUpdate = isUpdate;

           

            InitializeComponent();
            IntitalControl();
            this.Loaded+=new RoutedEventHandler(UserRoleAdd_Load);
        }

        void IntitalControl()
        {
            this.btnVoid.IsEnabled = false;
            this.btnValid.IsEnabled = false;

            if (IsUpdate)
            {
                this.RoleSysNo.Text = this.roleSysNo.ToString();
                this.RoleName.Text = this.roleName;

                if (roleStatus == ValidStatus.Active)
                    this.btnVoid.IsEnabled = true;
                else
                    this.btnValid.IsEnabled = true;


            }
 
        }

        void UserRoleAdd_Load(object sender, RoutedEventArgs e)
        {
            
            m_facade = new VendorFacade();
            List<Node> data = new List<Node>();

            //加载权限
            m_facade.GetPrivilegeList((o, rst) => {
                if (!rst.FaultsHandle() && rst.Result!=null)
                {
                    data = TransferToNodes(rst.Result);
                    this.tvSource.IsSpeardToParent = true;
                    this.tvSource.Nodes = data;
                    this.tvSource.BuildTreeByData();

                }
            });

            //加载角色权限
            if (IsUpdate)
            {
                m_facade.GetPrivilegeListByRoleSysNo(this.roleSysNo, (o, arg) => {

                    data = TransferToNodes(arg.Result);

                    this.tvTarget.Nodes = data;
                    this.tvTarget.BuildTreeByData();
                });
            }
        }
        //转换数据
        private List<Node> TransferToNodes(List<PrivilegeEntity> list)
        {
            List<Node> data = new List<Node>();

            foreach (PrivilegeEntity p in list)
            {
                data.Add(new Node { 
                     Name=p.Memo
                     ,Value=p.SysNo
                     ,ParentValue=p.ParentSysNo
                     
                });
            }

            return data;
        }

        public IDialog Dialog { get; set; }
        //保存
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_RoleAdd_Save))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }

            if (string.IsNullOrEmpty(this.RoleName.Text))
            {
                this.RoleName.SetValidation("角色名不能为空");
                this.RoleName.RaiseValidationError();
                return;
            }

            List<Node> data = this.tvTarget.SelectData(x => (true));

            if (data == null || data.Count == 0)
            {
                
            }

            Role role=new Role();

            role.RoleName=this.RoleName.Text;
            
            role.CompanyCode= CPApplication.Current.CompanyCode;
            role.InUser=CPApplication.Current.LoginUser.DisplayName;
            role.EditUser=CPApplication.Current.LoginUser.DisplayName;
            role.PrivilegeSysNoList=data.Select(x=>x.Value).ToList();

            if (IsUpdate)
            {
                role.SysNo = this.roleSysNo;

                m_facade.UpdateRole(role, (o, arg) => {
                    if (!arg.FaultsHandle())
                    {
                        this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        this.Dialog.Close(true);

                    }
                });

            }
            else
            {
                role.Status = "D";
                m_facade.CreateRole(role, (o, arg) =>
                {
                    if (!arg.FaultsHandle())
                    {
                        this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        this.Dialog.Close(true);

                    }
                });
            }

        }
        //箭头=>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            List<Node> addlist = this.tvSource.SelectData(x => (x.IsSelected));

            foreach (Node node in addlist)
            {
                var result = this.tvTarget.SelectData(x => x.Value == node.Value);

                if ((result == null || result.Count == 0) && node.Value != 0)
                {
                    if (this.tvTarget.Nodes == null)
                        this.tvTarget.Nodes = new List<Node>();
                    this.tvTarget.Nodes.Add(node);
                }
            }

            //this.tvTarget.Nodes = this.tvSource.SelectData(x => (x.IsSelected));
            this.tvTarget.BuildTreeByData();
        }
        //箭头<=
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            List<Node> deleteList = this.tvTarget.SelectData(x => (x.IsSelected));

            foreach (Node node in deleteList)
            {
                var result= this.tvTarget.Nodes.FirstOrDefault(x=>(x.Value==node.Value));

                this.tvTarget.Nodes.Remove(result);
            }
            //this.tvTarget.Nodes.Remove;


            this.tvTarget.BuildTreeByData();

        }
        //作废
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_RoleAdd_Abandon))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            Role role=new Role();
            role.SysNo = this.roleSysNo;
            role.RoleName=this.RoleName.Text;
            role.Status="D";
            role.CompanyCode= CPApplication.Current.CompanyCode;
            role.InUser=CPApplication.Current.LoginUser.DisplayName;
            role.EditUser=CPApplication.Current.LoginUser.DisplayName;

            m_facade.UpdateRoleStatus(role, (o, arg) => {

                if (!arg.FaultsHandle())
                {
                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.Close(true);

                }

            });


        }
        //生效
        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_RoleAdd_Effect))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            Role role = new Role();

            role.SysNo = this.roleSysNo;
            role.RoleName = this.RoleName.Text;
            role.Status = "A";
            role.CompanyCode = CPApplication.Current.CompanyCode;
            role.InUser = CPApplication.Current.LoginUser.DisplayName;
            role.EditUser = CPApplication.Current.LoginUser.DisplayName;

            m_facade.UpdateRoleStatus(role, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.Close(true);
                }

            });
        }
    }
}
