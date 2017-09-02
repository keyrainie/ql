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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ConvertRequestMaintain : PageBase
    {
        ConvertRequestVM requestVM;
        ConvertRequestVM RequestVM
        {
            get
            {
                return requestVM;
            }
            set
            {
                requestVM = value;
                requestVM = requestVM ?? new ConvertRequestVM();
                requestVM.ConvertItemInfoList.ForEach(x =>
                {
                    x.RequestStatus = requestVM.RequestStatus;
                });
                SetDataContext();
            }
        }
        Facades.ConvertRequestMaintainFacade MaintainFacade;

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

        #region 初始化加载

        public ConvertRequestMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, System.EventArgs e)
        {
            base.OnPageLoad(sender, e);

            ucSourceProductList.Page = this;
            ucTargetProductList.Page = this;
            MaintainFacade = new ConvertRequestMaintainFacade();
            if (RequestSysNo.HasValue)
            {
                MaintainFacade.GetConvertRequestInfoBySysNo(RequestSysNo.Value, (vm) =>
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
                RequestVM = new ConvertRequestVM();
            }
        }

        private void SetDataContext()
        {
            RequestVM.ConvertItemInfoList = RequestVM.ConvertItemInfoList ?? new List<ConvertRequestItemVM>();
            ucSourceProductList.RequestVM = RequestVM;
            ucTargetProductList.RequestVM = RequestVM;
            List<ConvertRequestItemVM> sourseItems = new List<ConvertRequestItemVM>();
            List<ConvertRequestItemVM> targerItems = new List<ConvertRequestItemVM>();
            if (RequestVM.ConvertItemInfoList.Count > 0)
            {
                List<int> productSysNoList = new List<int>();
                RequestVM.ConvertItemInfoList.ForEach(item =>
                {
                    productSysNoList.Add(item.ProductSysNo.Value);
                    //item.DeleteButtonVisibility = RequestVM.RequestStatus == ConvertRequestStatus.Origin ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    if (item.ConvertType == ucSourceProductList.ConvertType)
                    {
                        sourseItems.Add(item);
                    }
                    else if (item.ConvertType == ucTargetProductList.ConvertType)
                    {
                        targerItems.Add(item);
                    }
                });
                //OtherDomainDataFacade OtherDomainDataFacade = new Facades.OtherDomainDataFacade(this);
                //OtherDomainDataFacade.GetProductInfoByProductSysNoList(productSysNoList,
                //    (productList) =>
                //    {
                //        if (productList != null)
                //        {
                //            RequestVM.ConvertItemInfoList.ForEach(item =>
                //            {
                //                ECCentral.BizEntity.IM.ProductInfo product = productList.FirstOrDefault(p => p.SysNo == item.ProductSysNo);
                //                if (product != null)
                //                {
                //                    item.ProductID = product.ProductID;
                //                    item.ProductName = product.ProductName;
                //                }
                //            });
                //        }
                //    });
            }
            ucSourceProductList.ItemList = sourseItems;
            ucTargetProductList.ItemList = targerItems;
            this.DataContext = RequestVM;
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
                    case ConvertRequestStatus.Origin:
                        btnAbandon.IsEnabled = btnAudit.IsEnabled = true;
                        break;
                    case ConvertRequestStatus.Verified:
                        btnCancelAudit.IsEnabled = btnOutStock.IsEnabled = true;
                        break;
                    case ConvertRequestStatus.Abandon:
                        btnCancelAbandon.IsEnabled = true;
                        break;
                }
            }
            
            SetConvertRight();
        }

        //设置按钮权限点
        private void SetConvertRight()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_Create))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_Reset))
            {
                btnReset.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }         
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_OutStock))
            {
                btnOutStock.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequest_Print))
            {
                btnPrint.IsEnabled = false;
            }         
        }

        #endregion


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (PreSaveValidate())
            {
                InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                List<ProductPMLine> productLine = new List<ProductPMLine>();
                //bool isSameLine = true;
                if (RequestVM.ConvertItemInfoList.Count > 1)
                {
                    queryFilter.ProductSysNos = new List<int>();
                    foreach (var item in RequestVM.ConvertItemInfoList)
                    {
                        queryFilter.ProductSysNos.Add(item.ProductSysNo.Value);
                    }
                    new InventoryQueryFacade(this).GetProductLineSysNoByProductList(queryFilter, (Innerogj, innerArgs) =>
                    {
                        if (!innerArgs.FaultsHandle())
                        {
                            productLine = innerArgs.Result;
                            //    if (productLine != null && productLine.Count > 0)
                            //    {
                            //        for (int i = 0; i < productLine.Count; i++)
                            //        {
                            //            for (int a = 1; a < productLine.Count; a++)
                            //            {
                            //                if (productLine[i].ProductLineSysNo != productLine[a].ProductLineSysNo)
                            //                {
                            //                    isSameLine = false;
                            //                    break;
                            //                }
                            //            }
                            //            if (isSameLine == false)
                            //            {
                            //                break;
                            //            }
                            //        }
                            //    }
                        }

                        //if (isSameLine == false)
                        //{
                        //    Window.Alert("添加的商品与列表中的商品不在同一产品线");
                        //    return;
                        //}
                        //else
                        //{

                        RequestVM.ProductLineSysno = productLine[0].ProductLineSysNo.HasValue ? productLine[0].ProductLineSysNo.Value.ToString() : "";
                        SaveConvertRequest();
                        //}
                    });
                }
                else
                {
                    MaintainFacade.GetProductLineSysNoBySysNo(RequestVM.ConvertItemInfoList[0].ProductSysNo.Value, vm =>
                    {
                        if (vm != null)
                        {
                            RequestVM.ProductLineSysno = vm.ProductLineSysno;
                        }
                        SaveConvertRequest();
                    });
                }
            }
        }

        private void SaveConvertRequest()
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
            MaintainFacade.CreateRequest(RequestVM, (vm) =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    RequestSysNo = RequestVM.SysNo;
                    Window.Alert("创建转换单成功");
                }
            });
        }

        private void innerUpdateRequest()
        {
            MaintainFacade.UpdateRequest(RequestVM, (vm) =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    Window.Alert("转换单已修改");
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
                RequestVM = new ConvertRequestVM();
            }           
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (PreSaveValidate())
            {
                Window.Confirm("确认要进行作废操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.AbandonRequest(RequestVM, (vm) =>
                        {
                            if (vm != null)
                            {
                                Window.Alert("作废成功");
                                RequestVM = vm;
                            }
                        });
                    }
                });                
            }
        }

        private void btnCancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (PreSaveValidate())
            {
                Window.Confirm("确认要进行取消作废操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.CancelAbandonRequest(RequestVM, (vm) =>
                        {
                            if (vm != null)
                            {
                                Window.Alert("取消作废成功");
                                RequestVM = vm;
                            }
                        });
                    }
                });                
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (PreSaveValidate())
            {
                Window.Confirm("确认要进行审核操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.AuditRequest(RequestVM, (vm) =>
                        {
                            if (vm != null)
                            {
                                Window.Alert("审核成功");
                                RequestVM = vm;
                            }
                        });
                    }
                });                
            }
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {

            if (PreSaveValidate())
            {
                Window.Confirm("确认要进行取消审核操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.CancelAuditRequest(RequestVM, (vm) =>
                        {
                            if (vm != null)
                            {
                                Window.Alert("取消审核成功");
                                RequestVM = vm;
                            }
                        });
                    }
                });                
            }
        }

        private void btnOutStock_Click(object sender, RoutedEventArgs e)
        {
            if (PreSaveValidate())
            {
                Window.Confirm("确认要进行出库操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.OutStockRequest(RequestVM, (vm) =>
                        {
                            if (vm != null)
                            {
                                Window.Alert("出库成功");
                                RequestVM = vm;
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
                HtmlViewHelper.WebPrintPreview("Inventory", "ConvertRequest", t);
            }
        }

        private bool PreSaveValidate()
        {
            if (!RequestVM.StockSysNo.HasValue)
            {
                Window.Alert("请选择转换商品所在仓库。");
                return false;
            }

            if (RequestVM.ConvertItemInfoList != null && RequestVM.ConvertItemInfoList.Count > 0 && ValidationManager.Validate(this))
            {
                var sourceItem = RequestVM.ConvertItemInfoList.Where(p => p.ConvertType == ConvertProductType.Source);
                if (sourceItem == null || sourceItem.GetCount() <= 0)
                {
                    Window.Alert("请选择要转换的源商品！");
                    return false;
                }
                var targetItem = RequestVM.ConvertItemInfoList.Where(p => p.ConvertType == ConvertProductType.Target);
                if (targetItem == null || targetItem.GetCount() <= 0)
                {
                    Window.Alert("请选择要转换的目标商品！");
                    return false;
                }

                //
                //创建转换单时不需要验证总成本
                //if (ucSourceProductList.TotalCost != ucTargetProductList.TotalCost)
                //{
                //    Window.Alert("转换前后的成本价格不一样，必需保证转换前后的成本总价一样。");
                //    return false;
                //}
                return true;
            }
            else
            {
                Window.Alert("请选择要转换的源商品和目标商品。");
            }
            return false;
        }

    }
}
