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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VirtualRequestMaintain : PageBase
    {

        VirtualRequestQueryFacade queryFacade;
        VirtualRequestQueryVM queryVM;
        VirtualRequestQueryFilter queryFilter;

        List<VirtualRequestInventoryInfoVM> inventoryInfoVM;

        private int? ProductSysNo
        {
            get
            {
                int? no = null;
                if (Request != null && !string.IsNullOrEmpty(Request.Param))
                {
                    int sysNo;
                    if (int.TryParse(Request.Param, out sysNo))
                    {
                        no = sysNo;
                    }
                }
                return no;
            }    
        }

        public VirtualRequestMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryVM = new VirtualRequestQueryVM();
            inventoryInfoVM = new List<VirtualRequestInventoryInfoVM>();
            queryFilter = new VirtualRequestQueryFilter();
            queryFacade = new VirtualRequestQueryFacade(this);
            this.DataContext = queryVM;
            BindComboBoxData();
            if (ProductSysNo.HasValue)
            {
                //queryVM.ProductSysNo = ProductSysNo.ToString();
                queryVM.ProductSysNo = ProductSysNo;
                btnSearchInventory_Click(null, null);
            }
        }

        private void BindComboBoxData()
        {
            // 虚库类型:
            CodeNamePairHelper.GetList("Inventory", "VirtualRequestType", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.cmbVirtualTypeList.ItemsSource = args.Result;
                this.cmbVirtualTypeList.SelectedIndex = 0;
            });
        }

        private void linkBtnNoExpiredDate_Click(object sender, RoutedEventArgs e)
        {
            //永久（一直有效，需认为操作结束） 
            SetBeginAndEndTimeForInventoryInfo(null);
        }

        private void LinkBtnExpiredDate7Day_Click(object sender, RoutedEventArgs e)
        {
            //七天（从当前时间起至168小时后结束） 
            SetBeginAndEndTimeForInventoryInfo(7);
        }

        private void linkBtnExpiredDate3Day_Click(object sender, RoutedEventArgs e)
        {
            //三天（从当前时间起至72小时后结束） 
            SetBeginAndEndTimeForInventoryInfo(3);
        }

        private void dgVirtualRequestInfo_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            //if (!string.IsNullOrEmpty(queryVM.ProductSysNo))
            if (queryVM.ProductSysNo.HasValue)
            {
                queryFilter.RequestStatus = VirtualRequestStatus.Origin;
                //查询审核虚库变更Items：
                queryFacade.QueryModifiedVirtualRequestList(queryFilter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    var getList = args.Result.Rows;
                    int totalCount = args.Result.TotalCount;
                    this.dgVirtualRequestInfo.ItemsSource = DynamicConverter<VirtualRequestVM>.ConvertToVMList(getList);
                });
            }
        }

        private void dgProductInventoryInfo_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            //查询库存信息:
            queryFacade.QueryVirtualInventoryInfoByStock(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var getList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                inventoryInfoVM = DynamicConverter<VirtualRequestInventoryInfoVM>.ConvertToVMList(getList);
                this.dgProductInventoryInfo.ItemsSource = inventoryInfoVM;
            });
        }

        private void chbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.dgProductInventoryInfo.ItemsSource)
                {
                    foreach (var item in this.dgProductInventoryInfo.ItemsSource)
                    {
                        if (item is VirtualRequestInventoryInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((VirtualRequestInventoryInfoVM)item).IsChecked)
                                {
                                    ((VirtualRequestInventoryInfoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((VirtualRequestInventoryInfoVM)item).IsChecked)
                                {
                                    ((VirtualRequestInventoryInfoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void btnSearchInventory_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(queryVM.ProductSysNo))
            if (!queryVM.ProductSysNo.HasValue)
            {
                Window.Alert("请选择商品!");
                return;
            }
            queryFilter = EntityConverter<VirtualRequestQueryVM, VirtualRequestQueryFilter>.Convert(queryVM);
            this.dgVirtualRequestInfo.Bind();
            this.dgProductInventoryInfo.Bind();
            GetLastVerifiedRequest();
        }

        private void btnSubmitRequest_Click(object sender, RoutedEventArgs e)
        {
            //提交申请:
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            #region [Check操作]
            List<VirtualRequestInventoryInfoVM> getSelectedList = GetSelectedInventoryInfoVMList();
            if (null == getSelectedList || getSelectedList.Count <= 0)
            {
                Window.Alert("请至少选择一条数据！");
                return;
            }

            bool checkErrorOnVirtualQty = false;
            getSelectedList.ForEach(x =>
            {
                if (string.IsNullOrEmpty(x.SetVirtualQry))
                {
                    checkErrorOnVirtualQty = true;
                }
            });
            if (checkErrorOnVirtualQty)
            {
                Window.Alert("请设定已选择的虚库数量！");
                return;
            }
            if (string.IsNullOrEmpty(this.tbRequestReason.Text.Trim()))
            {
                Window.Alert("PM申请理由不能为空！");
                return;
            }
            #endregion

            int productSysNo = 0;
            //if (!string.IsNullOrEmpty(queryVM.ProductSysNo))
            if (queryVM.ProductSysNo.HasValue)
            {
                //int sysNo;
                //if (int.TryParse(queryVM.ProductSysNo, out sysNo))
                //{
                //    productSysNo = sysNo;
                //}
                productSysNo = (int)queryVM.ProductSysNo;
            }
            List<VirtualRequestVM> requestList = new List<VirtualRequestVM>();
            getSelectedList.ForEach(item =>
            {
                requestList.Add(new VirtualRequestVM
                {
                    StartDate = string.IsNullOrEmpty(item.BeginDate) ? (DateTime?)null : Convert.ToDateTime(item.BeginDate),
                    EndDate = string.IsNullOrEmpty(item.EndDate) ? (DateTime?)null : Convert.ToDateTime(item.EndDate),
                    VirtualQuantity = item.SetVirtualQry,
                    RequestNote = this.tbRequestReason.Text.Trim(),
                    VirtualType = Convert.ToInt32(this.cmbVirtualTypeList.SelectedValue.ToString()),
                    StockSysNo = item.StockSysNo,
                    ProductSysNo = productSysNo

                });
            });
            if (requestList == null || requestList.Count == 0)
            {
                Window.Alert("请提供创建虚库的商品信息!");
                return;
            }
            new VirtualRequestMaintainFacade(this).ApplyRequest(requestList, (vmList) =>
            {
                Window.Alert("提示", "操作成功!", MessageType.Information, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.Cancel)
                    {
                        ResetWhenSubmit();
                        GetLastVerifiedRequest();
                        btnSearchInventory_Click(null, null);
                    }
                });
            });
        }


        private void ResetWhenSubmit()
        {
            this.tbRequestReason.Text = string.Empty;
            if (null != inventoryInfoVM && inventoryInfoVM.Count > 0)
            {
                inventoryInfoVM.ForEach(x =>
                {
                    x.IsChecked = false;
                    x.SetVirtualQry = null;
                    x.BeginDate = null;
                    x.EndDate = null;
                });
            }
        }
        private void GetLastVerifiedRequest()
        {
            VirtualRequestQueryFilter filter = new VirtualRequestQueryFilter()
            {
                ProductSysNo = Convert.ToInt32(this.queryVM.ProductSysNo)
            };
            queryFacade.QueryVirtualInventoryLastVerifiedRequest(filter, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    return;
                }
                if (null != args2.Result.Rows)
                {
                    string alertString = string.Empty;
                    foreach (var item in args2.Result.Rows)
                    {
                        alertString += string.Format("此产品在\"{0}\"最后一次虚库申请被拒绝的理由：{1}。", item["StockName"], item["AuditNote"]) + Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(alertString))
                    {
                        this.lblLastRefuseResult.Text = alertString;
                    }
                }
            });
        }
        private List<VirtualRequestInventoryInfoVM> GetSelectedInventoryInfoVMList()
        {
            if (null != inventoryInfoVM && inventoryInfoVM.Count > 0)
            {
                return inventoryInfoVM.Where(x => x.IsChecked == true).ToList();
            }
            else
            {
                return null;
            }
        }

        private void SetBeginAndEndTimeForInventoryInfo(int? dayInterval)
        {
            List<VirtualRequestInventoryInfoVM> getSelectedList = GetSelectedInventoryInfoVMList();
            if (null == getSelectedList || getSelectedList.Count <= 0)
            {
                Window.Alert("请至少选择一条数据！");
                return;
            }

            if (dayInterval.HasValue)
            {
                getSelectedList.ForEach(x =>
                {
                    x.BeginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    x.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1 + dayInterval.Value).Subtract(new TimeSpan(0, 0, 1)).ToString("yyyy-MM-dd HH:mm:ss"); ;
                });
            }
            else
            {
                getSelectedList.ForEach(x =>
                {
                    x.BeginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    x.EndDate = null;
                });
            }
        }

        private void hpkView_Click(object sender, RoutedEventArgs e)
        {
            VirtualRequestVM getSelectedItem = this.dgVirtualRequestInfo.SelectedItem as VirtualRequestVM;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.Inventory/VirtualRequestAudit/{0}", getSelectedItem.SysNo), null, true);
                return;
            }
        }

    }
}
