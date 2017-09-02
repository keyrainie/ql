using System;
using System.Windows;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class VendorUserSearch : PageBase
    {
        VendorUserQueryFilter m_query;
        VendorFacade m_facade;
        List<ValidationEntity> validationVendorSysno;
        List<ValidationEntity> validationSerialNum;

        public VendorUserSearch()
        {
            InitializeComponent();
            SeachBuilder.DataContext = m_query = new VendorUserQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            m_facade = new VendorFacade();

            this.cmbConsignType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.All);
            this.cmbConsignType.SelectedIndex = 0;

            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;

            this.cmbRank.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRank>(EnumConverter.EnumAppendItemType.All);
            this.cmbRank.SelectedIndex = 0;

            this.cmbUserStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.ExternalSYS.ValidStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbUserStatus.SelectedIndex = 0;

            CodeNamePairHelper.GetList(ConstValue.DomainName_PO
                , new string[] { ConstValue.Key_POVendorAgentLevel }
                , CodeNamePairAppendItemType.All, (o, p) =>
                {
                    this.cmbAgentLevel.ItemsSource = p.Result[ConstValue.Key_POVendorAgentLevel];
                    this.cmbAgentLevel.SelectedIndex = 0;
                });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            validationVendorSysno = new List<ValidationEntity>();
            validationVendorSysno.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResVendorInfo.Msg_VerderSysNo_Check));
            validationSerialNum = new List<ValidationEntity>();
            if (this.txtSerialNum.Text != string.Empty)
                validationSerialNum.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^\d+-\d+$", ResVendorInfo.Msg_SerialNum_Check));
            if (!ValidationHelper.Validation(this.txtVendorSysNo, validationVendorSysno)
                || !ValidationHelper.Validation(this.txtSerialNum, validationSerialNum))
                return;
            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = dataGrid.PageSize,
                PageIndex = dataGrid.PageIndex,
                SortBy = e.SortField
            };
            m_query.PagingInfo.SortBy = e.SortField;
            m_query.CompanyCode = CPApplication.Current.CompanyCode;
            //if (!Regex.IsMatch(m_query.SerialNum, @"^\d+-\d+$"))
            //{
            //    m_query.SerialNum = string.Empty;
            //}
            m_facade.QueryUser(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var list = args.Result.Rows.ToList("IsCheck", false);
                foreach (var item in list)
                {
                    item["SerialNum"] = string.Format("{0}-{1:0000}", item["VendorSysNo"], item["UserNum"]);
                }
                dataGrid.TotalCount = args.Result.TotalCount;
                dataGrid.ItemsSource = list;
            });
        }

        private void btnChooseRole_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtRoleSysNo.Text))
                this.txtRoleSysNo.Text = string.Empty;
            UCVendorRoleQuery selectDialog = new UCVendorRoleQuery(m_query.RoleSysNo);
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResVendorInfo.Header_RoleList, selectDialog, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    DynamicXml getSelectedManufacturer = args.Data as DynamicXml;
                    if (null != getSelectedManufacturer)
                    {
                        txtRoleName.Text = getSelectedManufacturer["RoleName"].ToString();
                        txtRoleSysNo.Text = getSelectedManufacturer["SysNo"].ToString();
                        return;
                    }
                }
                //其他情况默认情况
                txtRoleName.Text = txtRoleSysNo.Text = string.Empty;
            }, new Size(700, 500));
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_UserMgmt_Add))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            UserProcessor newRequestCtrl = new UserProcessor(0);
            newRequestCtrl.Dialog = Window.ShowDialog(
                        ResVendorInfo.Header_CreateVendorUser
                        , newRequestCtrl
                        , (s, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                dataGrid.PageIndex = 0;
                                dataGrid.SelectedIndex = -1;
                                dataGrid.Bind();
                            }
                        }
                        , new Size(850, 550)
                );
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_UserMgmt_Edit))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            DynamicXml selectedModel = this.dataGrid.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                UserProcessor newRequestCtrl = new UserProcessor((int)selectedModel["SysNo"]);
                newRequestCtrl.Dialog = Window.ShowDialog(
                           ResVendorInfo.Header_EditVendorUser
                           , newRequestCtrl
                           , (s, args) =>
                           {
                               if (args.DialogResult == DialogResultType.OK)
                               {
                                   dataGrid.Bind();
                               }
                           }
                           , new Size(850, 550)
                    );
            }
        }

        private void btnBatchPass_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_UserMgmt_BatchEffect))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            List<int> list = new List<int>();
            if (LoadSelect(list))
            {
                this.Window.Confirm(ResVendorInfo.Msg_ConfirmPassUser, (o, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        //设置无效数据
                        m_facade.BatchPassUser(list, (up, ar) =>
                        {
                            if (!ar.FaultsHandle())
                            {
                                Window.Alert(ResVendorInfo.Msg_PassSuccess, MessageType.Information);
                                dataGrid.Bind();
                            }
                        });
                    }
                });
            }
        }

        private void btnBatchInvalid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_UserMgmt_BatchAbandon))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            List<int> list = new List<int>();
            if (LoadSelect(list))
            {
                this.Window.Confirm(ResVendorInfo.Msg_ConfirmInvalidUser, (o, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        //设置无效数据
                        m_facade.BatchInvalidUser(list, (up, ar) =>
                        {
                            if (!ar.FaultsHandle())
                            {
                                Window.Alert(ResVendorInfo.Msg_InvalidSuccess, MessageType.Information);
                                dataGrid.Bind();
                            }
                        });
                    }
                });
            }
        }

        private bool LoadSelect(List<int> list)
        {
            var dynamic = this.dataGrid.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add((int)item["SysNo"]);
                    }
                }
            }

            if (list.Count == 0)
            {
                this.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return false;
            }
            return true;
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dataGrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }

        //清空选择角色
        private void txtRoleSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtRoleSysNo.Text))
            {
                m_query.RoleSysNo = null;
                this.txtRoleName.Text = string.Empty;
            }
        }
    }
}
