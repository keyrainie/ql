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
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using System.Text.RegularExpressions;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class AdjustRequestMaintain : PageBase
    {
        AdjustRequestVM requestVM;
        AdjustRequestVM RequestVM
        {
            get
            {
                return requestVM;
            }
            set
            {
                requestVM = value;
                requestVM = requestVM ?? new AdjustRequestVM();
                requestVM.AdjustItemInfoList.ForEach(x =>
                {
                    x.RequestStatus = requestVM.RequestStatus;
                });
                SetDataContext();
            }
        }
        AdjustRequestMaintainFacade MaintainFacade;
        private int? _requestSysNo;
        private int? RequestSysNo
        {
            get
            {
                if (!_requestSysNo.HasValue)
                {
                    int tSysNo = 0;
                    if (!string.IsNullOrEmpty(Request.Param) && int.TryParse(Request.Param, out tSysNo))
                    {
                        _requestSysNo = tSysNo;
                    }
                }
                return _requestSysNo;
            }
            set
            {
                _requestSysNo = value;
            }
        }
        public AdjustRequestMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            MaintainFacade = new AdjustRequestMaintainFacade(this);
            base.OnPageLoad(sender, e);
            if (RequestSysNo.HasValue)
            {
                MaintainFacade.GetAdjustRequestInfoBySysNo(RequestSysNo.Value, (vm) =>
                {
                    if (vm == null || vm.CompanyCode == null || vm.CompanyCode.Trim() != CPApplication.Current.CompanyCode)
                    {
                        vm = null;
                        Window.Alert("单据不存在，此单据可能已经被删除或请传入其它的单据编号重试。");
                    }
                    RequestVM = vm;
                });
            }
            else
            {
                RequestVM = new AdjustRequestVM();
            }
        }

        private void SetDataContext()
        {
            this.DataContext = RequestVM;
            // this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
            SetOpertionButton();
        }

        private void SetOpertionButton()
        {
            btnAbandon.IsEnabled = btnCancelAbandon.IsEnabled =
                btnAudit.IsEnabled = btnCancelAudit.IsEnabled =
                btnOutStock.IsEnabled = false;
            if (requestVM.SysNo.HasValue)
            {
                switch (RequestVM.RequestStatus)
                {
                    case AdjustRequestStatus.Origin:
                        btnAbandon.IsEnabled = btnAudit.IsEnabled = true;
                        break;
                    case AdjustRequestStatus.Verified:
                        btnCancelAudit.IsEnabled = btnOutStock.IsEnabled = true;
                        break;
                    case AdjustRequestStatus.Abandon:
                        btnCancelAbandon.IsEnabled = true;
                        break;
                }
            }
            SetAdjustRight();
        }
        private void SetAdjustRight()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Create))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Reset))
            {
                btnReset.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_OutStock))
            {
                btnOutStock.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Print))
            {
                btnPrint.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_Invoice))
            {
                btnInvoice.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequest_PrintInvoice))
            {
                btnPrintInvoice.IsEnabled = false;
            }
        }

        #region 损益商品操作

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (RequestVM.StockSysNo == null || RequestVM.StockSysNo <= 0)
            {
                Window.Alert("请选择渠道仓库");
                return;
            }
            if (RequestVM != null && RequestVM.AdjustItemInfoList != null && RequestVM.AdjustItemInfoList.Count >= 1)
            {
                Window.Alert("每张损益单只能添加一个商品");
                return;
            }

            UCProductBatch batch = new UCProductBatch();
            batch.IsCreateMode = true;
            batch.StockSysNo = RequestVM.StockSysNo;
            batch.PType = Models.Request.PageType.Adjust;
            batch.IsNotLend_Return = true;

            IDialog dialog = Window.ShowDialog("添加明细", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null && productList.ProductVM != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        AdjustRequestItemVM vm = RequestVM.AdjustItemInfoList.FirstOrDefault(item =>
                        {
                            return item.ProductSysNo == p.SysNo;
                        });
                        if (vm == null)
                        {
                            #region 只允许添加自己权限范围内可以访问商品
                            string errorMessage = "对不起，您没有权限访问{0}商品!";
                            InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                            queryFilter.ProductSysNo = p.SysNo;
                            queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                            queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;

                            int quantity = 1;
                            if (p.IsHasBatch == 1)
                            {
                                quantity = (from s in p.ProductBatchLst select s.Quantity).Sum();
                            }
                            else if (p.IsHasBatch == 0)
                            {
                                quantity = productList.Quantity;
                            }

                            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                            {
                                new InventoryQueryFacade(this).CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                                {
                                    if (!innerArgs.FaultsHandle())
                                    {
                                        if (!innerArgs.Result)
                                        {
                                            Window.Alert(string.Format(errorMessage, p.ProductID));
                                            return;
                                        }
                                        else
                                        {
                                            vm = new AdjustRequestItemVM
                                            {
                                                ProductSysNo = p.SysNo,
                                                AdjustQuantity = quantity,
                                                AdjustCost = p.UnitCost,
                                                ProductName = p.ProductName,
                                                ProductID = p.ProductID,
                                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                                IsHasBatch = p.IsHasBatch,
                                                RequestStatus = AdjustRequestStatus.Origin
                                            };
                                            RequestVM.AdjustItemInfoList.Add(vm);
                                            this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
                                        }
                                    }
                                });
                            }
                            else
                            {
                                vm = new AdjustRequestItemVM
                                {
                                    ProductSysNo = p.SysNo,
                                    AdjustQuantity = quantity,
                                    AdjustCost = p.UnitCost,
                                    ProductName = p.ProductName,
                                    ProductID = p.ProductID,
                                    BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                    IsHasBatch = p.IsHasBatch,
                                    RequestStatus = AdjustRequestStatus.Origin
                                };
                                RequestVM.AdjustItemInfoList.Add(vm);
                                this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
                            }

                            #endregion
                        }
                        else
                        {
                            Window.Alert(string.Format("已存在编号为{0}的商品.", p.SysNo));
                        }
                    });
                }
            });

            batch.DialogHandler = dialog;
        }

        private void txtAdjustQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            AdjustRequestItemVM vm = txt.DataContext as AdjustRequestItemVM;
            if (txt.Text != "-" && !Regex.IsMatch(txt.Text, @"^-?[1-9](\d{0,5})$"))
            {
                if (Regex.IsMatch(txt.Text, @"^-?\d+$"))
                {
                    txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                }
                else
                {
                    txt.Text = "0";
                    txt.SelectAll();
                }
            }
        }

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            AdjustRequestItemVM vm = btn.DataContext as AdjustRequestItemVM;
            RequestVM.AdjustItemInfoList.Remove(vm);
            this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            AdjustRequestItemVM selected = btn.DataContext as AdjustRequestItemVM;
            AdjustRequestItemVM seleced = RequestVM.AdjustItemInfoList.Where(p => p.ProductSysNo == selected.ProductSysNo).FirstOrDefault();

            UCProductBatch batch = new UCProductBatch(seleced.ProductSysNo.Value.ToString(), seleced.ProductID, seleced.HasBatchInfo, seleced.BatchDetailsInfoList);

            batch.IsCreateMode = false;
            batch.StockSysNo = RequestVM.StockSysNo;
            batch.ProductSysNo = selected.ProductSysNo.Value.ToString();
            batch.ProductID = selected.ProductID;
            batch.OperationQuantity = selected.AdjustQuantity;
            batch.IsNotLend_Return = true;
            batch.PType = Models.Request.PageType.Adjust;

            IDialog dialog = Window.ShowDialog("更新明细", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null && productList.ProductVM != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        AdjustRequestItemVM vm = null;

                        #region 只允许添加自己权限范围内可以访问商品
                        string errorMessage = "对不起，您没有权限访问{0}商品!";
                        InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                        queryFilter.ProductSysNo = p.SysNo;
                        queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                        queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                        queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;

                        int quantity = 1;
                        if (p.IsHasBatch == 1)
                        {
                            quantity = (from s in p.ProductBatchLst select s.Quantity).Sum();
                        }
                        else if (p.IsHasBatch == 0)
                        {
                            quantity = productList.Quantity;
                        }

                        if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                        {
                            new InventoryQueryFacade(this).CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                            {
                                if (!innerArgs.FaultsHandle())
                                {
                                    if (!innerArgs.Result)
                                    {
                                        Window.Alert(string.Format(errorMessage, p.ProductID));
                                        return;
                                    }
                                    else
                                    {
                                        vm = new AdjustRequestItemVM
                                        {
                                            ProductSysNo = p.SysNo,
                                            AdjustQuantity = quantity,
                                            AdjustCost = p.UnitCost,
                                            ProductName = p.ProductName,
                                            ProductID = p.ProductID,
                                            BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                            IsHasBatch = p.IsHasBatch,
                                            RequestStatus = AdjustRequestStatus.Origin
                                        };

                                        RequestVM.AdjustItemInfoList.Remove((AdjustRequestItemVM)this.dgAdjustProduct.SelectedItem);
                                        RequestVM.AdjustItemInfoList.Add(vm);
                                        this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
                                    }
                                }
                            });
                        }
                        else
                        {
                            vm = new AdjustRequestItemVM
                            {
                                ProductSysNo = p.SysNo,
                                AdjustQuantity = quantity,
                                AdjustCost = p.UnitCost,
                                ProductName = p.ProductName,
                                ProductID = p.ProductID,
                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                IsHasBatch = p.IsHasBatch,
                                RequestStatus = AdjustRequestStatus.Origin
                            };
                            RequestVM.AdjustItemInfoList.Remove((AdjustRequestItemVM)this.dgAdjustProduct.SelectedItem);
                            RequestVM.AdjustItemInfoList.Add(vm);
                            this.dgAdjustProduct.ItemsSource = RequestVM.AdjustItemInfoList;
                        }

                        #endregion

                    });
                }
            });

            batch.DialogHandler = dialog;
        }

        private void hlbtnProudct_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            AdjustRequestItemVM vm = btn.DataContext as AdjustRequestItemVM;
            Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, vm.ProductSysNo), null, true);
        }
        #endregion

        #region 损益单操作

        private bool SavePreValidate()
        {
            if (RequestVM.AdjustItemInfoList == null || RequestVM.AdjustItemInfoList.Count < 1)
            {
                Window.Alert("请添加损益商品");
                return false;
            }
            if (RequestVM.StockSysNo == null || RequestVM.StockSysNo <= 0)
            {
                Window.Alert("请选择渠道仓库");
                return false;
            }
            if (RequestVM.AdjustProperty == null)
            {
                Window.Alert("请选择损益类型");
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                List<ProductPMLine> productLine = new List<ProductPMLine>();
                bool isSameLine = true;
                if (RequestVM.AdjustItemInfoList.Count > 1)
                {
                    queryFilter.ProductSysNos = new List<int>();
                    foreach (var item in RequestVM.AdjustItemInfoList)
                    {
                        queryFilter.ProductSysNos.Add(item.ProductSysNo.Value);
                    }
                    new InventoryQueryFacade(this).GetProductLineSysNoByProductList(queryFilter, (Innerogj, innerArgs) =>
                    {
                        if (!innerArgs.FaultsHandle())
                        {
                            productLine = innerArgs.Result;
                            if (productLine != null && productLine.Count > 0)
                            {
                                for (int i = 0; i < productLine.Count; i++)
                                {
                                    for (int a = 1; a < productLine.Count; a++)
                                    {
                                        if (productLine[i].ProductLineSysNo != productLine[a].ProductLineSysNo)
                                        {
                                            isSameLine = false;
                                            break;
                                        }
                                    }
                                    if (isSameLine == false)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (isSameLine == false)
                        {
                            Window.Alert("添加的商品与列表中的商品不在同一产品线");
                            return;
                        }
                        else
                        {
                            RequestVM.ProductLineSysno = productLine[0].ProductLineSysNo.HasValue ? productLine[0].ProductLineSysNo.Value.ToString() : "";
                            SaveAdjustRequest();
                        }
                    });
                }
                else
                {
                    MaintainFacade.GetProductLineSysNoBySysNo(RequestVM.AdjustItemInfoList[0].ProductSysNo.Value, vm =>
                    {
                        if (vm != null)
                        {
                            RequestVM.ProductLineSysno = vm.ProductLineSysno;
                        }
                        SaveAdjustRequest();
                    });
                }
            }
        }

        private void SaveAdjustRequest()
        {
            if (RequestSysNo.HasValue)
            {
                innerUpdateRequest();
            }
            else
            {
                innerCreateRequest();

            }
        }
        private void innerCreateRequest()
        {
            MaintainFacade.CreateRequest(RequestVM, vmList =>
            {
                if (vmList != null)
                {
                    RequestVM = vmList[0];
                    RequestSysNo = RequestVM.SysNo;
                    Window.Alert("损益单创建成功");

                    //打开其他损益单Tab
                    vmList.ForEach(x =>
                    {
                        if (x.SysNo != RequestVM.SysNo)
                        {
                            Window.Navigate(String.Format(ConstValue.Inventory_AdjustRequestMaintainUrlFormat, x.SysNo), null, true);
                        }
                    });
                }
            });
        }

        private void innerUpdateRequest()
        {
            MaintainFacade.UpdateRequest(RequestVM, vm =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    Window.Alert("损益单修改成功");
                }
            });
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (RequestVM.SysNo != null)
            {
                OnPageLoad(sender, e);
            }
            else
            {
                RequestVM = new AdjustRequestVM();
            }
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行作废操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.AbandonRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("作废成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnCancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行取消作废操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.CancelAbandonRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("取消作废成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行审核操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.VerifyRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("审核成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行取消审核操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.CancelVerifyRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("取消审核成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnOutStock_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行出库操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.OutStockRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("出库成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (RequestSysNo.HasValue)
            {
                Dictionary<string, string> t = new Dictionary<string, string>();
                t.Add("SysNo", RequestSysNo.Value.ToString());
                HtmlViewHelper.WebPrintPreview("Inventory", "AdjustRequest", t);
            }
        }

        private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (RequestSysNo.HasValue)
            {
                Dictionary<string, string> t = new Dictionary<string, string>();
                t.Add("SysNo", RequestSysNo.Value.ToString());
                HtmlViewHelper.WebPrintPreview("Inventory", "AdjustRequestInvoice", t);
            }
        }

        private void btnInvoice_Click(object sender, RoutedEventArgs e)
        {
            AdjustRequestInvoice content = new AdjustRequestInvoice { Page = this, RequestVM = this.RequestVM };
            content.Dialog = Window.ShowDialog(String.Format("损益单:{0}的发票信息", RequestSysNo), content, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        var adjustRequestVM = args.Data as AdjustRequestVM;
                        this.RequestVM = adjustRequestVM;
                    }
                });
        }

        #endregion
    }

}
