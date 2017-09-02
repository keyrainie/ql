using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.BizEntity.ExternalSYS;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using System.Threading;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using UserStatus = ECCentral.BizEntity.ExternalSYS.ValidStatus;
using APIStatus = ECCentral.BizEntity.ExternalSYS.ValidStatus;
using ECCentral.QueryFilter.ExternalSYS;
using System;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UserProcessor : UserControl
    {
        #region 定义及初始化
        int m_sysNo;

        List<VendorAgentInfoVM> vendorAgentList;

        List<VendorUserMappingVM> vendorUserMappingList;

        List<PrivilegeVM> privilegeList;

        VendorFacade m_facade;

        VendorUserVM m_vm;

        bool IsEdit
        {
            get
            {
                return m_sysNo > 0;
            }
        }

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

        public UserProcessor(int sysNo)
        {
            m_sysNo = sysNo;
            InitializeComponent();
            this.DataContext = m_vm = new VendorUserVM();

            Loaded += new RoutedEventHandler(UserProcessor_Loaded);

        }
        #endregion

        void UserProcessor_Loaded(object sender, RoutedEventArgs e)
        {
            m_facade = new VendorFacade();


            this.cmbVendorStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStatus>(EnumConverter.EnumAppendItemType.Select);
            this.cmbVendorStatus.SelectedIndex = 0;

            this.cmbRank.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRank>(EnumConverter.EnumAppendItemType.Select);
            this.cmbRank.SelectedIndex = 0;

            if (IsEdit)
            {
                //加载数据
                m_facade.QueryUserBySysNo(m_sysNo, (o, arg) =>
                {
                    if (!arg.FaultsHandle())
                    {
                        m_vm = arg.Result.Convert<VendorUser, VendorUserVM>();
                        m_vm.IsEdit = IsEdit;
                        m_vm.IsSelectAPI = m_vm.APIStatus == UserStatus.Active;
                        this.ucVendor.IsEnabled = false;
                        this.DataContext = m_vm;
                        this.dataGrid.Bind();
                    }
                    else
                    {
                        this.DataContext = m_vm;
                    }
                });
            }
            else
            {
                dataGrid_LoadingRow(null, null);
                m_vm.UserID = string.Empty;
                m_vm.UserName = string.Empty;
            }
        }

        //保存按钮事件（创建/编辑）
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!m_vm.HasValidationErrors)
            {
                if (m_vm.IsSelectAPI)
                    m_vm.APIStatus = APIStatus.Active;
                else
                    m_vm.APIStatus = APIStatus.DeActive;
                if (!IsEdit)
                {
                    m_vm.Status = UserStatus.DeActive;
                    if (string.IsNullOrEmpty(this.txtEmailAddress.Text))
                        m_vm.Email = string.Empty;
                    m_facade.CreateVendorUser(m_vm, (obj, args) =>
                        {
                            if (args.FaultsHandle()) return;
                            if (args.Result != null)
                            {
                                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_CreateSuccess);
                                if (Dialog != null)
                                {
                                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                    Dialog.Close();
                                }
                            }
                        });
                }
                else
                {
                    m_facade.UpdateVendorUser(m_vm, (obj, args) =>
                        {
                            if (args.FaultsHandle()) return;
                            if (args.Result)
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
            }
        }

        //生效按钮事件
        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNos = new List<int>();
            sysNos.Add(m_sysNo);
            m_facade.BatchPassUser(sysNos, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PassSuccess, MessageType.Information);
                        if (Dialog != null)
                        {
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        }
                    }
                });
        }

        //作废按钮事件
        private void btnInvalid_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNos = new List<int>();
            sysNos.Add(m_sysNo);
            m_facade.BatchInvalidUser(sysNos, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_InvalidSuccess, MessageType.Information);
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                }
            });
        }

        //关闭按钮事件
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (m_vm.VendorSysNo != null && m_vm.VendorSysNo != 0)
            {
                m_facade.GetVendorAgentInfoList(m_vm.VendorSysNo.ToString(), (m, s) =>
                {
                    if (s.FaultsHandle()) return;
                    vendorAgentList = s.Result.Convert<VendorAgentInfo, VendorAgentInfoVM>();
                    List<int> ManufacturerSysNoList = new List<int>();
                    for (int i = 0; i < vendorAgentList.Count; i++)
                    {
                        if (vendorAgentList[i].ManufacturerInfo != null && vendorAgentList[i].ManufacturerInfo.SysNo != null)
                        {
                            bool hasChangePricePrivileg = false;
                            GetRoleListByVendorEx(vendorAgentList[i].AgentSysNo.Value, m_sysNo, i, hasChangePricePrivileg);
                            vendorAgentList[i].HasChangePrice = hasChangePricePrivileg;
                            ManufacturerSysNoList.Add(vendorAgentList[i].ManufacturerInfo.SysNo.Value);
                        }
                    }
                    if (ManufacturerSysNoList != null && ManufacturerSysNoList.Count > 0)
                    {
                        m_vm.ManufacturerSysNoList = new List<int>();
                        foreach (var item in ManufacturerSysNoList)
                        {
                            if (!m_vm.ManufacturerSysNoList.Contains(item))
                            {
                                m_vm.ManufacturerSysNoList.Add(item);
                            }
                        }
                    }
                });
            }
        }

        #region 异步加载处理
        private void GetRoleListByVendorEx(int agentSysNo, int userSysNo, int i, bool hasChangePricePrivileg)
        {
            m_facade.GetRoleListByVendorEx(agentSysNo.ToString(), userSysNo.ToString(), (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result == null || args.Result.Count < 1)
                {
                    GetRoleListByUserSysNo(userSysNo, i, hasChangePricePrivileg);
                }
                else
                {
                    vendorUserMappingList = args.Result.Convert<VendorUserMapping, VendorUserMappingVM>();
                    if (vendorUserMappingList != null
                            && vendorUserMappingList.Count > 0)
                    {
                        foreach (var role in vendorUserMappingList)
                        {
                            vendorAgentList[i].RoleString += "[" + role.RoleName + "]";
                            #region 调价权限检测
                            if (vendorAgentList[i].SettleType != null
                                && !string.IsNullOrEmpty(vendorAgentList[i].SettleType.Value.ToString())
                                && vendorAgentList[i].SettleType == SettleType.P
                                && !hasChangePricePrivileg)
                            {
                                GetPrivilegeListByRoleSysNo(role.RoleSysNo.Value, i, hasChangePricePrivileg);
                            }
                            #endregion
                        }
                    }
                    this.dataGrid.ItemsSource = vendorAgentList;
                }
            });
        }

        private void GetRoleListByUserSysNo(int userSysNo, int i, bool hasChangePricePrivileg)
        {
            m_facade.GetRoleListByUserSysNo(userSysNo.ToString(), (v, n) =>
            {
                if (n.FaultsHandle()) return;
                vendorUserMappingList = n.Result.Convert<VendorUserMapping, VendorUserMappingVM>();
                if (vendorUserMappingList != null
                            && vendorUserMappingList.Count > 0)
                {
                    foreach (var role in vendorUserMappingList)
                    {
                        vendorAgentList[i].RoleString += "[" + role.RoleName + "]";
                        #region 调价权限检测
                        if (vendorAgentList[i].SettleType != null
                            && !string.IsNullOrEmpty(vendorAgentList[i].SettleType.Value.ToString())
                            && vendorAgentList[i].SettleType == SettleType.P
                            && !hasChangePricePrivileg)
                        {
                            GetPrivilegeListByRoleSysNo(role.RoleSysNo.Value, i, hasChangePricePrivileg);
                        }
                        #endregion
                    }
                }
                this.dataGrid.ItemsSource = vendorAgentList;
            });
        }

        private void GetPrivilegeListByRoleSysNo(int roleSysNo, int i, bool hasChangePricePrivileg)
        {
            m_facade.GetPrivilegeListByRoleSysNo(roleSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    privilegeList = args.Result.Convert<PrivilegeEntity, PrivilegeVM>();
                    if (privilegeList != null
                        && privilegeList.Count > 0)
                    {
                        foreach (var item in privilegeList)
                        {
                            if (item.PrivilegeName == "HasChangePrice")
                            {
                                hasChangePricePrivileg = true;
                                break;
                            }
                        }
                        vendorAgentList[i].HasChangePrice = hasChangePricePrivileg;
                    }
                    this.dataGrid.ItemsSource = vendorAgentList;
                });
        }
        #endregion

        /// <summary>
        /// 供应商控件选择完成后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCVendorPicker_VendorSelected(object sender, Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs e)
        {
            var vendorInfo = e.SelectedVendorInfo;
            m_vm.VendorStatus = vendorInfo.VendorBasicInfo.VendorStatus;
            m_vm.Rank = vendorInfo.VendorBasicInfo.VendorRank.ToString();
            this.hlbtnVendorInfo.IsEnabled = true;
            this.dataGrid.Bind();
        }

        //查看供应商详细信息
        private void hlbtnVendorInfo_Click(object sender, RoutedEventArgs e)
        {
            if (m_vm.VendorSysNo != null
                && m_vm.VendorSysNo > 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.ExternalSYS_VendorInfoUrlFormat, m_vm.VendorSysNo), null, true);
            }
        }

        private void hlbtnRoleEdit_Click(object sender, RoutedEventArgs e)
        {
            UCRoleCheck roleCheck = new UCRoleCheck(null, m_sysNo);
            roleCheck.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResVendorInfo.Info_RoleListEdit, roleCheck, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        dataGrid.Bind();
                    }
                }, new Size(560, 450));
        }

        private void hlbtnProductListEdit_Click(object sender, RoutedEventArgs e)
        {
            VendorAgentInfoVM agentView = this.dataGrid.SelectedItem as VendorAgentInfoVM;
            VendorProductQueryFilter filter = new VendorProductQueryFilter()
            {
                VendorManufacturerSysNo = agentView.ManufacturerInfo.SysNo,
                UserSysNo = m_sysNo,
                C2SysNo = int.Parse(agentView.C2SysNo),
                C3SysNo = agentView.C3SysNo,
                VendorSysNo = m_vm.VendorSysNo,
                ManufacturerSysNo = agentView.AgentSysNo,
            };
            Vendor_ExVM vm = new Vendor_ExVM()
            {
                VendorSysNo = filter.VendorSysNo.Value,
                InvoiceType = VendorInvoiceType.NEG,
                ShippingType = VendorShippingType.MET,//ShippingType = VendorShippingType.NEG,
                StockType = VendorStockType.NEG
            };
            if (filter.VendorSysNo != null)
            {
                m_facade.QueryByStockShippingeInvoic(vm, (m, n) =>
                    {
                        if (n.FaultsHandle()) return;
                        if (n.Result.Count > 0)
                        {
                            filter.VendorSysNo = 1;
                        }
                        m_facade.GetIsAuto(filter, (o, s) =>
                        {
                            if (s.FaultsHandle()) return;
                            filter.IsAuto = (s.Result == 1) ? true : false;
                            UCProductCheck proCheck = new UCProductCheck(filter);
                            proCheck.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResVendorInfo.Info_ProductListEdit, proCheck, (obj, args) =>
                            {
                                if (args.DialogResult == DialogResultType.OK)
                                {
                                    dataGrid.Bind();
                                }
                            }, new Size(800, 700));
                        });
                    });
            }

        }

        private void hlbtnRoleListEdit_Click(object sender, RoutedEventArgs e)
        {
            VendorAgentInfoVM view = this.dataGrid.SelectedItem as VendorAgentInfoVM;
            UCRoleCheck roleCheck = new UCRoleCheck(view.AgentSysNo, m_sysNo);
            roleCheck.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResVendorInfo.Info_RoleListEdit, roleCheck, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dataGrid.Bind();
                }
            }, new Size(560, 450));
        }

        /// <summary>
        /// 新增时不显示的列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (!IsEdit)
            {
                this.dataGrid.Columns[3].Visibility =
                this.dataGrid.Columns[4].Visibility =
                this.dataGrid.Columns[5].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        
        //2015.8.19为重置代码添加 John
        /// <summary>
        /// 重置密码操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ResetPassWord_Click(object sender, RoutedEventArgs e)
        { 
            Random rdm=new Random();
            int rdmNumber = rdm.Next(100000, 999999);
            m_vm.Pwd = rdmNumber.ToString();
            m_facade.UpdateVendorUser(m_vm, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_ResetPasswordSucess + m_vm.Pwd);
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                }
            });
        }
    }
}
