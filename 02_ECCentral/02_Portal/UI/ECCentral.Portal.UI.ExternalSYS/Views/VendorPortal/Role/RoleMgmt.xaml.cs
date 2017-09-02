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
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class RoleMgmt : PageBase
    {
        VendorRoleQueryFilter m_query;
        VendorFacade m_facade;

        RoleMgmtVM vm;

        List<Node> privilegeData = new List<Node>();

        public RoleMgmt()
        {
            InitializeComponent();
            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            SeachBuilder.DataContext = m_query = new VendorRoleQueryFilter();

            m_facade = new VendorFacade();
            vm = new RoleMgmtVM();

            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.ExternalSYS.ValidStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;

            this.ResultGrid.DataContext = vm;


            m_facade.GetPrivilegeList((o, rst) =>
            {
                if (!rst.FaultsHandle() && rst.Result != null)
                {
                    privilegeData = TransferToNodes(rst.Result);

                }
            });
        }

        //转换数据
        private List<Node> TransferToNodes(List<PrivilegeEntity> list)
        {
            List<Node> data = new List<Node>();

            foreach (PrivilegeEntity p in list)
            {
                data.Add(new Node
                {
                    Name = p.Memo
                    ,
                    Value = p.SysNo
                    ,
                    ParentValue = p.ParentSysNo
                });
            }

            return data;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ResultGrid.Bind();
        }
        //查询
        private void ResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ResultGrid.PageSize,
                PageIndex = ResultGrid.PageIndex,
                SortBy = e.SortField
            };
            m_query.PagingInfo.SortBy = e.SortField;


            m_facade.QueryVendorRoleVM(m_query, (results, count) => {


                vm.TotalCount = count;
                vm.Result = results;

            });

        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (vm != null && vm.Result!=null && checkBox!=null && checkBox.IsChecked.HasValue)
            {
                bool ischecked = checkBox.IsChecked.Value;
                foreach(RoleMgmtSearchResultVM item in vm.Result)
                {
                    if (item.IsChecked != ischecked)
                        item.IsChecked = ischecked;
                }
            }
        }
        //编辑
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_Role_Edit))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            RoleMgmtSearchResultVM item = this.ResultGrid.SelectedItem as RoleMgmtSearchResultVM;

            UserRoleAdd roleAddCtrl = new UserRoleAdd(item.SysNo.Value, item.RoleName,item.Status,true);
            roleAddCtrl.Dialog = Window.ShowDialog(
                        "编辑角色"
                        , roleAddCtrl
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                ResultGrid.PageIndex = 0;
                                ResultGrid.SelectedIndex = -1;
                                ResultGrid.Bind();
                            }
                        }
                        , new Size(850, 600)
                );
        }
        //新建
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_Role_Add))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            UserRoleAdd roleAddCtrl = new UserRoleAdd(0, "", ValidStatus.DeActive,false);
            roleAddCtrl.Dialog = Window.ShowDialog(
                        "新建角色"
                        , roleAddCtrl
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                ResultGrid.PageIndex = 0;
                                ResultGrid.SelectedIndex = -1;
                                ResultGrid.Bind();
                            }
                        }
                        , new Size(850, 600)
                );
        }
        //
        private List<Role> GetRoleList(string status)
        {
            List<RoleMgmtSearchResultVM> selectedItem = new List<RoleMgmtSearchResultVM>();
            if (vm.Result != null)
            {
                selectedItem = vm.Result.Where(x => x.IsChecked).ToList();
            }

            List<Role> roles = new List<Role>();
            if (selectedItem != null 
                && selectedItem.Count != 0)
            {
                foreach (RoleMgmtSearchResultVM item in selectedItem)
                {

                    Role role = new Role
                    {
                        SysNo = item.SysNo.Value
                         ,Status = status
                         ,EditUser=CPApplication.Current.LoginUser.DisplayName
                         ,CompanyCode=CPApplication.Current.CompanyCode
                    };
                    roles.Add(role);
                }
            }

            return roles;

 
        }

        private void btnBatchValid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_Role_BatchEffect))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            List<Role> list = GetRoleList("A");
            if (list == null
               || list.Count == 0)
            {
                Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }

            m_facade.UpdateRoleStatusBatch(list, (o, arg) => {
                if (!arg.FaultsHandle())
                {
                    Window.Alert("生效成功");

                    ResultGrid.PageIndex = 0;
                    ResultGrid.SelectedIndex = -1;
                    ResultGrid.Bind();
                    
                }

            });
            
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_Role_BatchAbandon))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            List<Role> list = GetRoleList("D");
            if (list == null
                || list.Count == 0)
            {
                Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }

            m_facade.UpdateRoleStatusBatch(list, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    Window.Alert("作废成功");

                    ResultGrid.PageIndex = 0;
                    ResultGrid.SelectedIndex = -1;
                    ResultGrid.Bind();

                }

            });



        }

        private void btnLoadTree_Click(object sender, RoutedEventArgs e)
        {
            UCTreeViewSelection treeSelection = new UCTreeViewSelection(privilegeData);
            treeSelection.HorizontalAlignment = HorizontalAlignment.Center;
            
            treeSelection.Dialog=Window.ShowDialog(
                        "选择权限"
                        , treeSelection
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                this.txtPrivilege.Text = args.Data.ToString();
                            }
                        }
                        , new Size(550, 600)
                );
        }


    }
}
