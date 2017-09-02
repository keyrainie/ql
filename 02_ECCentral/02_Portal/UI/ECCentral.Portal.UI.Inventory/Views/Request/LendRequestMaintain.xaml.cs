using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class LendRequestMaintain : PageBase
    {
        LendRequestVM requestVM;
        LendRequestVM RequestVM
        {
            get
            {
                return requestVM;
            }
            set
            {
                requestVM = value;
                requestVM = requestVM ?? new LendRequestVM();
                requestVM.LendItemInfoList.ForEach(x => {
                    x.RequestStatus = requestVM.RequestStatus;
                });
                SetDataContext();
            }
        }

        private List<LendRequestItemVM> RequestItemList
        {
            get
            {
                return (RequestVM.LendItemInfoList);
            }
        }

        LendRequestMaintainFacade MaintainFacade;
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

        public LendRequestMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            MaintainFacade = new LendRequestMaintainFacade(this);
            base.OnPageLoad(sender, e); if (RequestSysNo.HasValue)
            {
                MaintainFacade.GetLendRequestInfoBySysNo(RequestSysNo.Value, (vm) =>
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
                RequestVM = new LendRequestVM();
            }      
        }
 
        private void SetDataContext()
        {            
            this.DataContext = RequestVM;
            ucLendProductList.dgProductList.ItemsSource = new ObservableCollection<LendRequestItemVM>(RequestItemList);
            SetOpertionButton();
        }

        private void SetOpertionButton()
        {
            btnAbandon.IsEnabled = btnCancelAbandon.IsEnabled =
                btnAudit.IsEnabled = btnCancelAudit.IsEnabled =
                btnOutStock.IsEnabled = btnReturn.IsEnabled = false;
            if (requestVM.SysNo.HasValue)
            {
                switch (RequestVM.RequestStatus)
                {
                    case LendRequestStatus.Origin:
                        btnAbandon.IsEnabled = btnAudit.IsEnabled = btnSave.IsEnabled = true;
                        break;
                    case LendRequestStatus.Verified:
                        btnCancelAudit.IsEnabled = btnOutStock.IsEnabled = true;
                        break;
                    case LendRequestStatus.Abandon:
                        btnCancelAbandon.IsEnabled = true;
                        break;
                    case LendRequestStatus.OutStock:
                    case LendRequestStatus.ReturnPartly:
                        btnReturn.IsEnabled = true;
                        break;
                }
            }

            #region 权限设置

            SetLendRight();

            #endregion
        }

        private void SetLendRight()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Create))
            {
                btnSave.IsEnabled = false;
            }    
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Return))
            {
                btnReturn.IsEnabled = false;
            }     
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Reset))
            {
                btnReset.IsEnabled = false;
            }   
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }    
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_OutStock))
            {
                btnOutStock.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequest_Print))
            {
                btnPrint.IsEnabled = false;
            }
        }

        #region 借货商品操作     

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!PreCheckAddProduct(RequestVM))
            {
                return;
            }

            UCProductBatch batch = new UCProductBatch();
            batch.IsCreateMode = true;
            batch.StockSysNo = RequestVM.StockSysNo;
            batch.PType = Models.Request.PageType.Lend;
            batch.IsNotLend_Return = true;

            IDialog dialog = Window.ShowDialog("添加明细", batch, (obj, args) => 
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        LendRequestItemVM vm = RequestItemList.FirstOrDefault(item =>
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
                                            vm = new LendRequestItemVM
                                            {
                                                ProductSysNo = p.SysNo,
                                                LendQuantity = quantity,
                                                ProductName = p.ProductName,
                                                ProductID = p.ProductID,
                                                PMUserName = p.PMUserName,
                                                ReturnDateETA = productList.ReturnDate,
                                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                                IsHasBatch = p.IsHasBatch
                                            };
                                            RequestItemList.Add(vm);
                                            ucLendProductList.dgProductList.ItemsSource = RequestItemList;
                                        }
                                    }
                                });
                            }
                            else
                            {
                                vm = new LendRequestItemVM
                                {
                                    ProductSysNo = p.SysNo,
                                    LendQuantity = quantity,
                                    ProductName = p.ProductName,
                                    ProductID = p.ProductID,
                                    PMUserName = p.PMUserName,
                                    ReturnDateETA = productList.ReturnDate,
                                    BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                    IsHasBatch = p.IsHasBatch
                                };
                                RequestItemList.Add(vm);
                                ucLendProductList.dgProductList.ItemsSource = RequestItemList;
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

        #endregion 借货商品操作
        

        #region 借货单操作

        private bool SavePreValidate()
        {
            if (RequestVM.StockSysNo == null || RequestVM.StockSysNo <= 0)
            {
                Window.Alert("请选择渠道仓库.");
                return false;
            }

            if (RequestVM.LendUserSysNo == null || RequestVM.LendUserSysNo <= 0)
            {
                Window.Alert("请选择借货人.");
                return false;
            }

            if (RequestVM.LendItemInfoList == null || RequestVM.LendItemInfoList.Count < 1)
            {
                Window.Alert("请添加借货商品.");
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
                if (RequestVM.LendItemInfoList.Count > 1)
                {
                    queryFilter.ProductSysNos = new List<int>();
                    foreach (var item in RequestVM.LendItemInfoList)
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
                            RequestVM.ProductLineSysno = productLine[0].ProductLineSysNo.HasValue?productLine[0].ProductLineSysNo.Value.ToString():"";
                            SaveLendRequest();
                        }
                    });
                }
                else
                {
                    MaintainFacade.GetProductLineSysNoBySysNo(RequestVM.LendItemInfoList[0].ProductSysNo.Value, vm =>
                    {
                        if (vm != null)
                        {
                            RequestVM.ProductLineSysno = vm.ProductLineSysno;
                        }
                        SaveLendRequest();
                    });
                }
            }
        }

        private void SaveLendRequest()
        {
            if (RequestSysNo.HasValue)
            {
                InnerUpdateLendRequest();
              
            }
            else
            {
                InnerCreateLendRequest();
            }
        }

        private void InnerCreateLendRequest()
        {
            MaintainFacade.CreateRequest(RequestVM, vm =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    RequestSysNo = RequestVM.SysNo;
                    Window.Alert("借货单创建成功");
                }
            });
        }

        private void InnerUpdateLendRequest()
        {
            MaintainFacade.UpdateRequest(RequestVM, vm =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    Window.Alert("借货单修改成功");
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
                RequestVM = new LendRequestVM();
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

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行归还操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.ReturnRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                Window.Alert("归还成功");
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
                HtmlViewHelper.WebPrintPreview("Inventory", "LendRequest", t);
            }
        }        
     
        #endregion 页面内按钮处理事件     

        private bool PreCheckAddProduct(LendRequestVM request)
        {
            bool result = true;
            if (!request.StockSysNo.HasValue)
            {
                result = false;
                Window.Alert("请选择先仓库！");
            }

            return result;
        }
    }
}
