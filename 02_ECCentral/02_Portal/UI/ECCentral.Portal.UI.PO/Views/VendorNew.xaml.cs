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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VendorNew : PageBase
    {

        public VendorInfoVM vendorInfoVM;
        public VendorFacade serviceFacade;
        public VendorPayTermsFacade payTermsServiceFacade;
        public List<KeyValuePair<VendorConsignFlag?, string>> vendorConsignFlagList;
        public List<EPortEntity> EPortList;
        public string getLoadVendorSysNo;

        public VendorNew()
        {
            InitializeComponent();
            InitializeComboBoxData();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            vendorInfoVM = new VendorInfoVM();
            serviceFacade = new VendorFacade(this);
            payTermsServiceFacade = new VendorPayTermsFacade(this);

            //账期类型,初始化默认为:经销:
            BindPayTermsData(VendorConsignFlag.Sell);
            //默认送货分仓:
            CodeNamePairHelper.GetList("PO", "VendorDefaultShippingStock", (obj, args) =>
            {
                this.cmbDefaultStock.ItemsSource = args.Result;
            });
            vendorInfoVM.VendorBasicInfo.PaySettleCompany = PaySettleCompany.SH;
            this.DataContext = vendorInfoVM;
            //this.cmbEPort.SelectedEPort = 0;
            //SetAccessControl();
            //this.cmbEPort.ItemsSource = EnumConverter.GetKeyValuePairs<EPortStatusENUM>(EnumConverter.EnumAppendItemType.Select);
        }

        private void SetAccessControl()
        {
            //创建供应商:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditAndCreateVendor))
            {
                this.btnCreate.IsEnabled = false;
            }
            //显示账期类型:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_ShowAllPayPeriodType))
            {
                this.cmbSettlePeriodType.ItemsSource = null;
                //(this.cmbSettlePeriodType.ItemsSource as List<KeyValuePair<VendorSettlePeriodType?, string>>).Clear();
            }
        }


        private void InitializeComboBoxData()
        {
            //供应商属性:
            vendorConsignFlagList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.None);
            vendorConsignFlagList.RemoveAll(item => item.Key == VendorConsignFlag.Consign);//隐藏代销
            this.cmbConsignFlag.ItemsSource = vendorConsignFlagList;
            this.cmbConsignFlag.SelectedIndex = 0;
            //开票方式:
            this.cmbInvoiceType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorInvoiceType>(EnumConverter.EnumAppendItemType.None);
            this.cmbInvoiceType.SelectedIndex = 0;
            //仓储方式:
            this.cmbStockType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStockType>(EnumConverter.EnumAppendItemType.None);
            this.cmbStockType.SelectedIndex = 0;
            //配送方式:
            this.cmbShippingType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorShippingType>(EnumConverter.EnumAppendItemType.None);
            this.cmbShippingType.SelectedIndex = 0;

            //付款结算公司:
            //this.cmbPaySettleCompany.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleCompany>(EnumConverter.EnumAppendItemType.None);
            //this.cmbPaySettleCompany.SelectedIndex = 0;
            //供应商等级:
            this.cmbRank.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRank>(EnumConverter.EnumAppendItemType.None);
            this.cmbRank.SelectedIndex = 0;
            //结算方式 ：
            this.cmbSettlePeriodType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorSettlePeriodType>(EnumConverter.EnumAppendItemType.None);
            this.cmbSettlePeriodType.SelectedIndex = 0;

        }

        private void BindPayTermsData(VendorConsignFlag consignFlag)
        {
            //财务 - 账期类型(调用Invoice接口获取LIST):
            if (null != payTermsServiceFacade)
            {
                payTermsServiceFacade.QueryVendorPayTermsList(CPApplication.Current.CompanyCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<VendorPayTermsItemInfo> listItems = args.Result.Where(x => x.IsConsignment == (int)consignFlag).OrderBy(j => j.PayTermsNo).ToList();
                    listItems.RemoveAll(item => item.PayTermsNo != 19);//只留账期类型为月结的
                    this.cmbVendorPayTermsType.ItemsSource = listItems;
                    if (null == this.cmbVendorPayTermsType.ItemsSource || 0 >= listItems.Count)
                    {
                        List<VendorPayTermsItemInfo> list = new List<VendorPayTermsItemInfo>();
                        list.Add(new VendorPayTermsItemInfo() { PayTermsNo = null, PayTermsName = null });
                        this.cmbVendorPayTermsType.ItemsSource = list;
                    }
                    this.cmbVendorPayTermsType.SelectedIndex = 0;
                });
            }
        }

        private void SetIsShowStore()
        {
            if (null != this.cmbInvoiceType.SelectedValue && null != this.cmbShippingType.SelectedValue && null != this.cmbStockType.SelectedValue)
            {

                //if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString() && this.cmbStockType.SelectedValue.ToString() == VendorStockType.NEG.ToString() && this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.NEG.ToString())
                //{
                //    this.chkIsShowStore.IsChecked = false;
                //    this.chkIsShowStore.IsEnabled = false;
                //}
                //else
                //{
                    this.chkIsShowStore.IsEnabled = true;
                //}
            }
        }

        private void SetControlStatus()
        {
            //if (null != this.cmbInvoiceType.SelectedValue && null != this.cmbShippingType.SelectedValue && null != this.cmbStockType.SelectedValue)
            //{
            //    if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
            //    {
            //        if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString())
            //        {
            //            this.cmbConsignFlag.SelectedIndex = 1;
            //            this.cmbConsignFlag.IsEnabled = false;
            //        }

            //    }
            //    else
            //    {
            //        this.cmbConsignFlag.SelectedIndex = 2;
            //        this.cmbConsignFlag.IsEnabled = false;
            //    }

            //}
        }

        #region [Events]

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            #region [UI 验证]

            if (!ValidationManager.Validate(this.Root))
            {
                return;
            }

            this.tabVendorAdvancedInfo.SelectedIndex = 1;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.tabVendorAdvancedInfo))
            {
                return;
            }
            VendorPayTermsItemInfo getSelectedItem = this.cmbVendorPayTermsType.SelectedItem as VendorPayTermsItemInfo;

            if (getSelectedItem.PayTermsNo == null)
            {
                Window.Alert(ResVendorNew.Msg_PayTermsNull);
                return;
            }
            if (string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.EmailAddress))
            {
                Window.Alert(ResVendorNew.Msg_VendorMailAddressNull);
                return;
            }

            this.tabVendorAdvancedInfo.SelectedIndex = 2;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.tabVendorAdvancedInfo))
            {
                return;
            }
            this.tabVendorAdvancedInfo.SelectedIndex = 0;

            #endregion

            //新建供应商信息:

            //默认分仓:
            this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.DefaultStock = (this.cmbDefaultStock.Visibility == Visibility.Collapsed ? (int?)null : this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.DefaultStock);

            this.vendorInfoVM.VendorFinanceInfo.PayPeriodType.PayTermsNo = int.Parse(this.cmbVendorPayTermsType.SelectedValue.ToString());
            this.vendorInfoVM.VendorFinanceInfo.SettlePeriodType = cmbSettlePeriodType.Visibility == Visibility.Collapsed ? null : ((KeyValuePair<Nullable<VendorSettlePeriodType>, string>)this.cmbSettlePeriodType.SelectedItem).Key;
            this.vendorInfoVM.VendorBasicInfo.VendorIsCooperate = VendorIsCooperate.Yes;
            this.vendorInfoVM.VendorBasicInfo.BuyWeekDayVendor = BuildSelectVendorBuyWeekDayString();
            this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.CurrencyCode = VendorCurrencyCode.CNY.ToString();
            this.vendorInfoVM.VendorBasicInfo.ConsignFlag = VendorConsignFlag.Sell;
            serviceFacade.CreateVendor(this.vendorInfoVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                string getCreatedVendorSysNo = args.Result.SysNo.Value.ToString();
                Window.Alert(ResVendorNew.AlertMsg_AlertTitle, ResVendorNew.AlertMsg_CreateVendorSuc, MessageType.Information, (obj2, args2) =>
                {
                    if (args2.DialogResult == DialogResultType.Cancel)
                    {
                        Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/VendorMaintain/{0}", getCreatedVendorSysNo), null, false);
                    }
                });

            });
        }

        private void btnNewAgentInfo_Click(object sender, RoutedEventArgs e)
        {
            //新建代理信息
            VendorAgentInfoMaintain agentInfo = new VendorAgentInfoMaintain(this.vendorInfoVM.VendorBasicInfo);
            agentInfo.Dialog = Window.ShowDialog(string.Empty, agentInfo, (obj, args) =>
            {
                if (DialogResultType.OK == args.DialogResult)
                {
                    VendorAgentInfoVM getNewAgentInfoVM = args.Data as VendorAgentInfoVM;
                    if (null != getNewAgentInfoVM)
                    {
                        this.vendorInfoVM.VendorAgentInfoList.Add(getNewAgentInfoVM);
                        this.dataGrid_VendorAgentInfo.Bind();
                        UpdateTotalCommissionFees();
                    }
                }
            });
        }

        /// <summary>
        /// 开票方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbInvoiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetIsShowStore();
            /*
           if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
           {
               //泰隆优选开票
               if (null != this.cmbStockType.SelectedValue)
               {
                   this.cmbConsignFlag.ItemsSource = vendorConsignFlagList;

                   if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.NEG.ToString())
                   {
                       this.cmbConsignFlag.SelectedIndex = 0;
                       this.cmbConsignFlag.IsEnabled = true;
                   }
                   else if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString())
                   {
                       this.cmbConsignFlag.SelectedIndex = 0;
                       this.cmbConsignFlag.IsEnabled = false;
                   }
               }
               SwitchBuyWeekDayVisibility();
           }
               
           else
           {
               //商家开票
               //业务模式4
               if ((this.cmbStockType.SelectedValue!=null && this.cmbStockType.SelectedValue.ToString() == VendorStockType.NEG.ToString())
                   &&
                   (this.cmbShippingType.SelectedValue != null && this.cmbShippingType.SelectedValue.ToString() == VendorStockType.NEG.ToString())
                   )
               {
                   this.cmbConsignFlag.ItemsSource = vendorConsignFlagList;
                   this.cmbConsignFlag.SelectedIndex = 0;
                   this.cmbConsignFlag.IsEnabled = false;
               }
               else
               {
                   //商家属性:
                   var source = vendorConsignFlagList;
                   if (this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.MET.ToString())
                   {
                       source.Add(vendorConsignFlagList.FirstOrDefault(p => p.Key == VendorConsignFlag.GroupBuying));
                       this.cmbConsignFlag.IsEnabled = true;
                   }
                   else
                   {
                       this.cmbConsignFlag.IsEnabled = false;
                   }
                   this.cmbConsignFlag.ItemsSource = source;

                   this.cmbConsignFlag.SelectedIndex = 0;                    
               }
               SwitchBuyWeekDayVisibility();
           }*/
        }

        /// <summary>
        /// 仓储方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbStockType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetIsShowStore();
            /*
            //泰隆优选仓储
            if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.NEG.ToString())
            {
                // 泰隆优选开票
                if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
                {
                    this.cmbConsignFlag.IsEnabled = true;
                }
                //商家开票
                else 
                {
                    //业务模式4
                    if (
                     (this.cmbShippingType.SelectedValue != null && this.cmbShippingType.SelectedValue.ToString() == VendorStockType.NEG.ToString())
                     )
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(3, 1);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = false;
                    }
                    else
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(2, 1);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = false;
                    }
                }
                this.lblDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                this.cmbDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                SwitchBuyWeekDayVisibility();
            }
            // 商家仓储
            else
            {

                if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
                {
                    this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(1, 1);
                    this.cmbConsignFlag.SelectedIndex = 0;
                    this.cmbConsignFlag.IsEnabled = false;
                }
                else
                {
                    var source = vendorConsignFlagList.GetRange(2, 1);
                    if (this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.MET.ToString())
                    {
                        this.cmbConsignFlag.IsEnabled = true;
                        source.Add(vendorConsignFlagList.FirstOrDefault(p => p.Key == VendorConsignFlag.GroupBuying));
                    }
                    else
                    {
                        this.cmbConsignFlag.IsEnabled = false;
                    }
                    this.cmbConsignFlag.ItemsSource = source;
                    this.cmbConsignFlag.SelectedIndex = 0;                    
                }
                if (this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.NEG.ToString())
                {
                    this.lblDefaultStock.Visibility = System.Windows.Visibility.Visible;
                    this.cmbDefaultStock.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.lblDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                    this.cmbDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                }
                SwitchBuyWeekDayVisibility();
            }*/
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbShippingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetIsShowStore();
            /*
            //泰隆优选配送
            if (this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.NEG.ToString())
            {
                //商家仓储
                if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString())
                {
                    this.lblDefaultStock.Visibility = System.Windows.Visibility.Visible;
                    this.cmbDefaultStock.Visibility = System.Windows.Visibility.Visible;
                    
                    //代销
                    if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(1, 1);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = false;
                    }
                    //经销
                    else
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(0, 1);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = false;
                    }

                }
                //泰隆优选仓储
                else
                {
                    //业务模式4
                    if (
                     ( this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.MET.ToString())
                     )
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(3, 1);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = false;
                    }
                    else
                    {
                        this.cmbConsignFlag.ItemsSource = vendorConsignFlagList.GetRange(0, 2);
                        this.cmbConsignFlag.SelectedIndex = 0;
                        this.cmbConsignFlag.IsEnabled = true;
                    }
                    this.lblDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                    this.cmbDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                }
                SwitchBuyWeekDayVisibility();
            }
            //商家配送
            else
            {
                if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.MET.ToString()
                    && this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString())
                {
                    var source = vendorConsignFlagList.GetRange(2, 1);
                    this.cmbConsignFlag.IsEnabled = true;
                    source.Add(vendorConsignFlagList.FirstOrDefault(p => p.Key == VendorConsignFlag.GroupBuying));
                    this.cmbConsignFlag.ItemsSource = source;
                    this.cmbConsignFlag.SelectedIndex = 0;
                }
                this.lblDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                this.cmbDefaultStock.Visibility = System.Windows.Visibility.Collapsed;
                SwitchBuyWeekDayVisibility();

            }
            */
        }

        private void cmbDefaultStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //默认送货分仓:
        }

        /// <summary>
        /// 供应商属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbConsignFlag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.cmbConsignFlag.SelectedItem)
            {
                tabItemVendorStore.Visibility = Visibility.Collapsed;
                tabItemCS.Visibility = System.Windows.Visibility.Visible;

                KeyValuePair<VendorConsignFlag?, string> getSelectedFlag = (KeyValuePair<VendorConsignFlag?, string>)this.cmbConsignFlag.SelectedItem;
                if (null != getSelectedFlag.Key)
                {
                    BusinessModeGroup.Visibility = System.Windows.Visibility.Visible;
                    //需根据当前所选属性，重新绑定财务-》账期类型的数据:
                    BindPayTermsData(getSelectedFlag.Key.Value);
                    if (getSelectedFlag.Key == VendorConsignFlag.Consign)
                    {
                        //代销:
                        this.dataGrid_VendorAgentInfo.Columns[7].Visibility = Visibility.Visible;
                        this.dataGrid_VendorAgentInfo.Columns[8].Visibility = Visibility.Visible;
                        this.lblAutoAudit.Visibility = Visibility.Collapsed;
                        this.txtAutoAudit.Visibility = Visibility.Collapsed;
                        this.lblSettlePeriodType.Visibility = Visibility.Visible;
                        this.cmbSettlePeriodType.Visibility = Visibility.Visible;
                    }
                    else if (getSelectedFlag.Key == VendorConsignFlag.GroupBuying)
                    {
                        tabItemVendorStore.Style = Application.Current.Resources["MiniLastTabItemStyle"] as Style;
                        tabItemVendorStore.Visibility = Visibility.Visible;
                        tabItemCS.Visibility = System.Windows.Visibility.Collapsed;
                        BusinessModeGroup.Visibility = System.Windows.Visibility.Collapsed;                        
                    }
                    else
                    {
                        //非代销:
                        this.dataGrid_VendorAgentInfo.Columns[7].Visibility = Visibility.Collapsed;
                        this.dataGrid_VendorAgentInfo.Columns[8].Visibility = Visibility.Collapsed;
                        this.lblAutoAudit.Visibility = Visibility.Visible;
                        this.txtAutoAudit.Visibility = Visibility.Visible;
                        this.lblSettlePeriodType.Visibility = Visibility.Collapsed;
                        this.cmbSettlePeriodType.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void hpl_AgentInfoEdit_Click(object sender, RoutedEventArgs e)
        {
            //编辑代理信息:
            VendorAgentInfoVM getSelectAgentVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
            if (null != getSelectAgentVM)
            {
                VendorAgentInfoMaintain maintainUC = new VendorAgentInfoMaintain(getSelectAgentVM, this.vendorInfoVM.VendorBasicInfo, false);
                maintainUC.Dialog = Window.ShowDialog(ResVendorNew.AlertMsg_EditTitle, maintainUC, (obj, args) =>
                {
                    getSelectAgentVM = (VendorAgentInfoVM)args.Data;
                    this.dataGrid_VendorAgentInfo.Bind();
                    UpdateTotalCommissionFees();
                });
            }
        }

        private void hpl_AgentInfoDelete_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResVendorNew.AlertMsg_ConfirmDelete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorAgentInfoVM getSelectAgentVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
                    if (null != getSelectAgentVM)
                    {
                        this.vendorInfoVM.VendorAgentInfoList.Remove(getSelectAgentVM);
                        this.dataGrid_VendorAgentInfo.Bind();
                        UpdateTotalCommissionFees();
                    }
                }
            });
        }

        private void dataGrid_VendorAgentInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.dataGrid_VendorAgentInfo.ItemsSource = vendorInfoVM.VendorAgentInfoList;
        }

        private void cmbVendorPayTermsType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //账期类型下拉框改变，更新计算公式TextBlock:
            VendorPayTermsItemInfo info = this.cmbVendorPayTermsType.SelectedItem as VendorPayTermsItemInfo;
            if (null != info && !string.IsNullOrEmpty(info.DiscribComputer))
            {
                this.txtPayTypeComputeText.Text = info.DiscribComputer.ToString().Replace("</br>", Environment.NewLine);
                //自动审核:
                if (info.PayTermsNo == 29 || info.PayTermsNo == 30 || info.PayTermsNo == 31)
                {
                    vendorInfoVM.VendorFinanceInfo.IsAutoAudit = true;
                }
                else
                {
                    vendorInfoVM.VendorFinanceInfo.IsAutoAudit = null;
                }
            }
            else
            {
                this.txtPayTypeComputeText.Text = string.Empty;
            }
        }

        #endregion

        //private void SwitchBuyWeekDayVisibility()
        //{
        //    if (null != this.cmbInvoiceType.SelectedValue && null != this.cmbStockType.SelectedValue && null != this.cmbShippingType.SelectedValue)
        //    {
        //        if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.MET.ToString() || this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString() || this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.MET.ToString())
        //        {
        //            this.txtBuyWeekDay.Visibility = Visibility.Collapsed;
        //            this.spBuyWeekDay.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            this.txtBuyWeekDay.Visibility = Visibility.Visible;
        //            this.spBuyWeekDay.Visibility = Visibility.Visible;
        //        }
        //    }
        //}

        /// <summary>
        /// 构建下单日期 - CheckBoxList
        /// </summary>
        /// <returns></returns>
        private string BuildSelectVendorBuyWeekDayString()
        {
            string returnStr = string.Empty;

            //foreach (var chkItem in this.spBuyWeekDay.Children)
            //{
            //    CheckBox chkBox = chkItem as CheckBox;
            //    if (null != chkBox && chkBox.IsChecked.HasValue && chkBox.IsChecked.Value)
            //    {
            //        returnStr += string.Format("{0};", chkBox.Tag.ToString());
            //    }
            //}

            return (!string.IsNullOrEmpty(returnStr) ? returnStr.TrimEnd(';') : returnStr);

        }

        /// <summary>
        /// 更新"店租佣金合计"
        /// </summary>
        private void UpdateTotalCommissionFees()
        {
            if (null != vendorInfoVM.VendorAgentInfoList && vendorInfoVM.VendorAgentInfoList.Count > 0)
            {
                decimal totalRentFees = 0m;
                vendorInfoVM.VendorAgentInfoList.ForEach(x =>
                {

                    totalRentFees += x.VendorCommissionInfo.RentFee.ToDecimal();
                });
                this.txtTotalRent.Text = totalRentFees.ToString("f2");
            }
        }

        private void btnEditVendorMailAddress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //编辑供应商:
            VendorMailAddressMaintain vendorAddressUC = new VendorMailAddressMaintain(BuildVendorMailAddresList());
            vendorAddressUC.Dialog = Window.ShowDialog("维护电子邮件", vendorAddressUC, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    List<string> getVendorMailAddressList = args.Data as List<string>;
                    if (null != getVendorMailAddressList)
                    {
                        BindVendorMailAddressList(getVendorMailAddressList);
                    }
                }
            }, new Size(480, 260));
        }

        private void BindVendorMailAddressList(List<string> vendorMailAddressList)
        {
            this.cmbVendorMailAddress.Items.Clear();
            vendorMailAddressList.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x))
                {
                    this.cmbVendorMailAddress.Items.Add(x);
                }
            });
            if (vendorMailAddressList.Count > 0)
            {
                this.cmbVendorMailAddress.SelectedIndex = 0;
            }
            this.vendorInfoVM.VendorBasicInfo.EmailAddress = vendorMailAddressList.Join(";");
            this.vendorInfoVM.VendorBasicInfo.EmailAddress = vendorInfoVM.VendorBasicInfo.EmailAddress.Length <= 0 ? null : vendorInfoVM.VendorBasicInfo.EmailAddress;
        }

        private List<string> BuildVendorMailAddresList()
        {
            List<string> vendorMailList = new List<string>();
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.EmailAddress))
            {
                if (vendorInfoVM.VendorBasicInfo.EmailAddress.IndexOf(';') >= 0)
                {
                    string[] list = vendorInfoVM.VendorBasicInfo.EmailAddress.Split(';');
                    foreach (var item in list)
                    {
                        vendorMailList.Add(item.Trim());
                    }
                }
                else
                {
                    vendorMailList.Add(vendorInfoVM.VendorBasicInfo.EmailAddress);
                }
            }
            return vendorMailList;
        }
    }

}
