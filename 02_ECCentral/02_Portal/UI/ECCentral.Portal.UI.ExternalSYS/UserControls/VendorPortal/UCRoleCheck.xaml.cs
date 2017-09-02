using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.ExternalSYS;
using RoleStatus = ECCentral.BizEntity.ExternalSYS.ValidStatus;
using System.Collections.Generic;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.Basic.Utilities;
using System;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UCRoleCheck : UserControl
    {
        #region 初始化
        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        private VendorFacade m_facade;
        private VendorRoleQueryFilter filter;
        private List<RoleMgmtSearchResultVM> leftRoleList;
        private List<VendorUserMappingVM> rightRoleList;
        private VendorUserRoleListVM roleListVM;

        private int? agentSysNo;
        public int? AgentSysNo
        {
            get { return agentSysNo; }
            set { agentSysNo = value; }
        }
        private int userSysNo;
        public int UserSysNo
        {
            get { return userSysNo; }
            set { userSysNo = value; }
        }

        public UCRoleCheck(int? agentSysNo, int userSysNo)
        {
            InitializeComponent();
            this.AgentSysNo = agentSysNo;
            this.UserSysNo = userSysNo;
            Loaded += new RoutedEventHandler(UCRoleCheck_Loaded);
        }
        #endregion

        #region 页面加载事件
        //页面加载
        void UCRoleCheck_Loaded(object sender, RoutedEventArgs e)
        {
            m_facade = new VendorFacade();
            filter = new VendorRoleQueryFilter();
            roleListVM = new VendorUserRoleListVM();
            LoadAllActiveRoles();
            LoadSettedRoles(AgentSysNo, UserSysNo);
        }

        #region 加载可设置的角色列表
        private void LoadAllActiveRoles()
        {
            filter.Status = RoleStatus.Active;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                SortBy = string.Empty
            };
            m_facade.QueryVendorRole(filter, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    leftRoleList = DynamicConverter<RoleMgmtSearchResultVM>.ConvertToVMList(args.Result.Rows);
                    this.lbAllRoles.ItemsSource = leftRoleList;
                });
        }
        #endregion

        #region 加载已已设置的角色列表
        private void LoadSettedRoles(int? agentSysNo, int userSysno)
        {
            if (agentSysNo != null)
            {
                m_facade.GetRoleListByVendorEx(agentSysNo.ToString(), userSysno.ToString(), (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        rightRoleList = args.Result.Convert<VendorUserMapping, VendorUserMappingVM>();
                        if (rightRoleList == null || rightRoleList.Count < 1)
                        {
                            GetRoleListByUserSysNo(userSysno);
                        }
                        this.lbSetRoles.ItemsSource = rightRoleList;
                    });
            }
            else
            {
                GetRoleListByUserSysNo(userSysno);
            }
        }

        private void GetRoleListByUserSysNo(int userSysNo)
        {
            m_facade.GetRoleListByUserSysNo(userSysNo.ToString(), (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                rightRoleList = args.Result.Convert<VendorUserMapping, VendorUserMappingVM>();
                this.lbSetRoles.ItemsSource = rightRoleList;
            });
        }
        #endregion

        #endregion

        #region 按钮事件
        //添加授权
        private void btnToRight_Click(object sender, RoutedEventArgs e)
        {
            VendorUserMappingVM view;
            RoleMgmtSearchResultVM roleView;
            if (lbAllRoles.SelectedItems.Count > 0)
            {
                for (int i = 0; i < lbAllRoles.SelectedItems.Count; i++)
                {
                    int flag = 0;
                    roleView = lbAllRoles.SelectedItems[i] as RoleMgmtSearchResultVM;
                    view = new VendorUserMappingVM()
                    {
                        RoleSysNo = roleView.SysNo,
                        RoleName = roleView.RoleName,
                        UserSysNo = UserSysNo
                    };
                    for (int j = 0; j < rightRoleList.Count; j++)
                    {
                        if (view.RoleSysNo == rightRoleList[j].RoleSysNo)
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                        rightRoleList.Add(view);
                }
                this.lbSetRoles.ItemsSource = null;
                this.lbSetRoles.ItemsSource = rightRoleList;
                this.lbAllRoles.SelectedItems.Clear();
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
        }

        //取消授权
        private void btnToLeft_Click(object sender, RoutedEventArgs e)
        {
            VendorUserMappingVM view;
            if (lbSetRoles.SelectedItems.Count > 0)
            {
                for (int i = 0; i < lbSetRoles.SelectedItems.Count; i++)
                {
                    view = lbSetRoles.SelectedItems[i] as VendorUserMappingVM;
                    rightRoleList.Remove(view);
                }
                this.lbSetRoles.ItemsSource = null;
                this.lbSetRoles.ItemsSource = rightRoleList;
                this.lbSetRoles.SelectedItems.Clear();
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        //保存
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            roleListVM.UserSysNo = UserSysNo;
            roleListVM.ManufacturerSysNo = AgentSysNo;
            List<int> roleSysNoList = new List<int>();
            foreach (var list in rightRoleList)
            {
                roleSysNoList.Add(list.RoleSysNo.Value);
            }
            roleListVM.RoleSysNoList = roleSysNoList;
            m_facade.UpdateVendorUserRole(roleListVM, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                        if (Dialog != null)
                        {
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        }
                    }

                });
        }

        #endregion
    }
}
