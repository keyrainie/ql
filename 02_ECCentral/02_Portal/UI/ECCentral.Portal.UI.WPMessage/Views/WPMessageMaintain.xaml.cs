using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.WPMessage.Facades;
using ECCentral.Portal.UI.WPMessage.Models;
using ECCentral.Service.WPMessage.Restful.RequestMsg;
using ECCentral.Portal.UI.WPMessage.UserControls;

using Newegg.Oversea.Silverlight.Utilities;
using ECCentral.WPMessage.BizEntity;

namespace ECCentral.Portal.UI.WPMessage.Views
{
    [View]
    public partial class WPMessageMaintain : PageBase
    {
        WPMessagFacade facade;
        private List<SystemRoleVM> bindVM;
        private List<SystemRoleVM> oldBindVM;

        private List<WPMessageCategoryVM> bindCategory;
        private List<WPMessageCategoryVM> OldBindCategory;

        public WPMessageMaintain()
        {
            InitializeComponent();
            OnLoad += new EventHandler(WPMessageMaintain_OnLoad);
        }

        void WPMessageMaintain_OnLoad(object sender, EventArgs e)
        {
            OnLoad -= new EventHandler(WPMessageMaintain_OnLoad);
            if (CPApplication.Current.CurrentPage.Context.Request.URL.IndexOf("localhost") > -1)
                btnShow.Visibility = System.Windows.Visibility.Visible;

            facade = new WPMessagFacade(this);
            SystemRoleQueryFilter filter = new SystemRoleQueryFilter();
            filter.RoleName = string.Empty;
            filter.RoleStatus = AuthCenterStatus.Activated;


            facade.QueryAllRoles(filter, 10000, 1, string.Empty, (s2, args2) =>
            {
                if (args2.FaultsHandle())
                    return;
                bindVM = DynamicConverter<SystemRoleVM>.ConvertToVMList<List<SystemRoleVM>>(args2.Result.Rows);
                oldBindVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone(bindVM);

                roleList.ItemsSource = bindVM;

                facade.GetAllCategory((s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    List<WPMessageCategory> list = args.Result;
                    cbmMessageType.ItemsSource = list;

                    cbmMessageType2.ItemsSource = bindVM;
                    bindCategory = list.Convert<WPMessageCategory, WPMessageCategoryVM>();// DynamicConverter<WPMessageCategoryVM>.ConvertToVMList<List<WPMessageCategoryVM>>(list);
                    OldBindCategory = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone(bindCategory);

                    cateList.ItemsSource = bindCategory;
                    //if (bindVM.Count > 0)
                    //    UpdateTheRolesList(bindVM[0].SysNo.Value.ToString());
                });
            });


        }

        #region 为待办事项添加角色


        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            UCWPMessage usercontrol = new UCWPMessage();
            usercontrol.Dialog = Window.ShowDialog("待办事项", usercontrol, (obj, args) =>
            {
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool canUpdate = false;
            for (int i = 0; i < bindVM.Count; i++)
            {
                if (bindVM[i].IsChecked != oldBindVM[i].IsChecked)
                    canUpdate = true;
            }
            if (canUpdate)
            {
                UpdateWPMessageCategoryRoleReq req = new UpdateWPMessageCategoryRoleReq();
                req.CategorySysNo = Convert.ToInt32(cbmMessageType.SelectedValue);
                List<int> rs = new List<int>();
                foreach (var item in bindVM)
                {
                    if (item.IsChecked)
                        rs.Add(item.SysNo.Value);
                }
                req.RoleSysNoList = rs;
                facade.UpdateCategoryRole(req, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功！", MessageType.Information);
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert("请更新角色！", MessageType.Information);
        }

        private void cbmMessageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbmMessageType.SelectedValue != null)
            {
                string sysNo = cbmMessageType.SelectedValue.ToString();
                UpdateTheRolesList(sysNo);
            }
        }

        private void UpdateTheRolesList(string sysNo)
        {
            List<int> roles;
            facade.GetCategoryRole(sysNo, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                roles = args.Result;
                foreach (var item in bindVM)
                {
                    item.IsChecked = false;
                }
                foreach (var r in roles)
                {
                    bindVM.SingleOrDefault(a => a.SysNo == r).IsChecked = true;
                }
                roleList.ItemsSource = bindVM;
            });
        }



        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox xb = sender as CheckBox;
            if (xb.IsChecked.HasValue && xb.IsChecked.Value)
            {
                foreach (var item in bindVM)
                {
                    item.IsChecked = true;
                }
            }
            else
            {
                foreach (var item in bindVM)
                {
                    item.IsChecked = false;
                }
            }
        }

        #endregion

        #region 为角色添加待办事项类别

        private void cbmMessageType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbmMessageType2.SelectedValue != null)
            {
                string sysNo = cbmMessageType2.SelectedValue.ToString();
                UpdateTheCategoryList(sysNo);
            }
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            bool canUpdate = false;
            for (int i = 0; i < bindCategory.Count; i++)
            {
                if (bindCategory[i].IsChecked != OldBindCategory[i].IsChecked)
                    canUpdate = true;
            }
            if (canUpdate)
            {
                UpdateWPMessageCategoryRoleByRoleSysNoReq req = new UpdateWPMessageCategoryRoleByRoleSysNoReq();
                req.RoleSysNo = Convert.ToInt32(cbmMessageType2.SelectedValue);
                List<int> rs = new List<int>();
                foreach (var item in bindCategory)
                {
                    if (item.IsChecked)
                        rs.Add(item.SysNo.Value);
                }
                req.CategorySysNoList = rs;
                facade.UpdateRoleCategory(req, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功！", MessageType.Information);
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert("请更新待办事项类别！", MessageType.Information);
        }


        private void UpdateTheCategoryList(string sysNo)
        {
            List<int> cates;
            facade.GetRoleCategory(sysNo, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                cates = args.Result;
                foreach (var item in bindCategory)
                {
                    item.IsChecked = false;
                }
                foreach (var r in cates)
                {
                    bindCategory.SingleOrDefault(a => a.SysNo == r).IsChecked = true;
                }
                cateList.ItemsSource = bindCategory;
            });
        }


        private void CheckBox2_Click(object sender, RoutedEventArgs e)
        {
            CheckBox xb = sender as CheckBox;
            if (xb.IsChecked.HasValue && xb.IsChecked.Value)
            {
                foreach (var item in bindCategory)
                {
                    item.IsChecked = true;
                }
            }
            else
            {
                foreach (var item in bindCategory)
                {
                    item.IsChecked = false;
                }
            }
        }

        #endregion

    }
}

