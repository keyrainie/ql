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
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftRequestMaintain : PageBase
    {
        ShiftRequestVM requestVM;
        ShiftRequestVM RequestVM
        {
            get
            {
                txtTotalAmountInfo.Text = requestVM.ShiftItemInfoList.Sum(x => { return Math.Round(x.TotalCost.Value,2); }).ToString();
                return requestVM;
            }
            set
            {
                requestVM = value;
                requestVM = requestVM ?? new ShiftRequestVM();
                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList ?? null;
                requestVM.ShiftItemInfoList.ForEach(x =>
                {
                    x.RequestStatus = requestVM.RequestStatus;
                });
                txtTotalAmountInfo.Text = requestVM.ShiftItemInfoList.Sum(x => { return Math.Round(x.TotalCost.Value, 2); }).ToString();
                SetDataContext();
            }
        }

        List<CodeNamePair> shiftShippingTypeList;
        List<CodeNamePair> ShiftShippingTypeList
        {
            get
            {
                return shiftShippingTypeList;
            }
            set
            {
                shiftShippingTypeList = value;                
            }
        }
        ShiftRequestMaintainFacade MaintainFacade;
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
        public ShiftRequestMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            RequestVM = new ShiftRequestVM();
            MaintainFacade = new ShiftRequestMaintainFacade(this);
            base.OnPageLoad(sender, e);
            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, ConstValue.Key_ShiftShippingType, CodeNamePairAppendItemType.None,
                (obj, args) =>
                {
                    if (!args.FaultsHandle() && args.Result != null)
                    {
                        this.ShiftShippingTypeList = args.Result;
                        RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                    }
                });
            if (RequestSysNo.HasValue)
            {
                MaintainFacade.GetShiftRequestInfoBySysNo(RequestSysNo.Value, (vm) =>
                {
                    if (vm == null || vm.CompanyCode == null || vm.CompanyCode.Trim() != CPApplication.Current.CompanyCode)
                    {
                        vm = null;
                        Window.Alert("单据不存在，此单据可能已经被删除或请传入其它的单据编号重试。");
                    }
                    vm.ShiftShippingTypeList = RequestVM.ShiftShippingTypeList;
                    RequestVM = vm;
                });
             
                this.tbConsign.Visibility = Visibility.Visible;
                this.lblConsign.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbConsign.Visibility = Visibility.Collapsed;
                this.lblConsign.Visibility = Visibility.Collapsed;
            }
            
        }
        private void SetDataContext()
        {
            this.DataContext = RequestVM;
            SetOpertionButton();
        }

        private void SetOpertionButton()
        {
            btnAbandon.IsEnabled = btnCancelAbandon.IsEnabled =
                btnAudit.IsEnabled = btnCancelAudit.IsEnabled = btnInStock.IsEnabled =
                btnOutStock.IsEnabled = false;
            if (requestVM.SysNo.HasValue)
            {
                switch (RequestVM.RequestStatus)
                {
                    case BizEntity.Inventory.ShiftRequestStatus.Origin:
                        btnAbandon.IsEnabled = btnAudit.IsEnabled = true;
                        break;
                    case BizEntity.Inventory.ShiftRequestStatus.Verified:
                        btnCancelAudit.IsEnabled = btnOutStock.IsEnabled = true;
                        break;
                    case BizEntity.Inventory.ShiftRequestStatus.Abandon:
                        btnCancelAbandon.IsEnabled = true;
                        break;
                }
            }

            SetShiftRight();
        }

        private void SetShiftRight()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_Create))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_Reset))
            {
                btnReset.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_BatchAddGiftItem))
            {
                btnBatchAddGiftItem.IsEnabled = false;
            }            
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }    
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_Print))
            {
                btnPrint.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_PrintInvoice))
            {
                btnPrintInvoice.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_RequestMemo))
            {
                btnRequestMemoMaintain.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_SetSpecial))
            {
                btnSetSpecialShiftType.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequest_CancelSpecial))
            {
                btnCancelSpecialShiftType.IsEnabled = false;
            }
        }

        #region 移仓商品操作
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch content = new UCProductSearch();
            content.SelectionMode = SelectionMode.Multiple;
            IDialog dialog = Window.ShowDialog("添加商品", content, (obj, args) =>
            {
                List<ProductVM> productList = args.Data as List<ProductVM>;
                if (productList != null)
                {
                    productList.ForEach(p =>
                    {
                        ShiftRequestItemVM vm = RequestVM.ShiftItemInfoList.FirstOrDefault(item =>
                        {
                            return item.ProductSysNo == p.SysNo;
                        });
                        if (vm == null)
                        {
                            #region 只允许添加自己权限范围内可以访问商品
                            string errorMessage = "对不起，您没有权限访问{0}商品!";
                            InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                            queryFilter.ProductSysNo = p.SysNo;
                            queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
                            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                            //if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                            //{
                            //    queryFilter.PMQueryRightType = BizEntity.Common.PMQueryType.AllValid;
                            //}
                            //else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_IntermediatePM_Query))
                            //{
                            //    queryFilter.PMQueryRightType = BizEntity.Common.PMQueryType.Team;
                            //}
                            //else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_JuniorPM_Query))
                            //{
                            //    queryFilter.PMQueryRightType = BizEntity.Common.PMQueryType.Self;
                            //}
                            //else
                            //{
                            //    Window.Alert(string.Format(errorMessage, p.ProductID));
                            //    return;
                            //}
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
                                            vm = new ShiftRequestItemVM
                                            {
                                                ProductSysNo = p.SysNo,
                                                ShiftQuantity = 1,
                                                ShiftUnitCost = p.UnitCost,
                                                ProductName = p.ProductName,
                                                ProductID = p.ProductID,
                                                InStockQuantity = 0,
                                                ShiftUnitCostWithoutTax = p.UnitCostWithoutTax,
                                                Weight = p.Weight,
                                                ShippingCost = 0,
                                                RequestStatus = ShiftRequestStatus.Origin
                                            };
                                            RequestVM.ShiftItemInfoList.Add(vm);
                                            this.dgShiftRequestMaintainQueryResult.ItemsSource = RequestVM.ShiftItemInfoList;
                                        }
                                    }
                                });
                            }else 
                            {
                                vm = new ShiftRequestItemVM
                                {
                                    ProductSysNo = p.SysNo,
                                    ShiftQuantity = 1,
                                    ShiftUnitCost = p.UnitCost,
                                    ProductName = p.ProductName,
                                    ProductID = p.ProductID,
                                    InStockQuantity = 0,
                                    ShiftUnitCostWithoutTax = p.UnitCostWithoutTax,
                                    Weight = p.Weight,
                                    ShippingCost = 0,
                                    RequestStatus = ShiftRequestStatus.Origin
                                };
                                RequestVM.ShiftItemInfoList.Add(vm);
                                this.dgShiftRequestMaintainQueryResult.ItemsSource = RequestVM.ShiftItemInfoList;
                            }

                           
                            #endregion                            
                        }
                    });
                }
            });
            content.DialogHandler = dialog;
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            ConvertRequestItemVM vm = txt.DataContext as ConvertRequestItemVM;
            if (!Regex.IsMatch(txt.Text, @"^[1-9](\d{0,5})$"))
            {
                if (Regex.IsMatch(txt.Text, @"^\d+$"))
                {
                    txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                }
                else
                {
                    txt.Text = "0";
                }
            }
        }

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            ShiftRequestItemVM vm = btn.DataContext as ShiftRequestItemVM;
            RequestVM.ShiftItemInfoList.Remove(vm);
            this.dgShiftRequestMaintainQueryResult.ItemsSource = RequestVM.ShiftItemInfoList;
        }

        private void hlbtnProudct_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            ShiftRequestItemVM vm = btn.DataContext as ShiftRequestItemVM;
            Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, vm.ProductSysNo), null, true);
        }
        #endregion


        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {

        }       

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (RequestVM.SysNo != null)
            {
                OnPageLoad(sender, e);
            }
            else
            {
                RequestVM = new ShiftRequestVM();
                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
            }        
        }

        private bool SavePreValidate()
        {
           // List<string> productLineList = new List<string>();
            bool result = true;
            if (RequestVM.ShiftItemInfoList == null || RequestVM.ShiftItemInfoList.Count < 1)
            {
                Window.Alert("请添加移仓商品");
                result= false;
            }

            if (RequestVM.SourceStockSysNo == null || RequestVM.SourceStockSysNo <= 0)
            {
                Window.Alert("请选择移出仓");
                result= false;
            }

            if (RequestVM.TargetStockSysNo == null || RequestVM.TargetStockSysNo <= 0)
            {
                Window.Alert("请选择移入仓");
                result= false;
            }
            
            
            //if (RequestVM.ShiftItemInfoList.Count > 1)
            //{
            //    foreach (var item in RequestVM.ShiftItemInfoList)
            //    {
            //        MaintainFacade.GetProductLineSysNoBySysNo(item.ProductSysNo.Value, vm =>
            //        {
            //            if (vm != null)
            //            {
            //                RequestVM.ProductLineSysno = vm.ProductLineSysno;
            //                productLineList.Add(RequestVM.ProductLineSysno);
            //            }

            //        });
            //    }
            //    for (int i = 0; i < productLineList.Count; i++)
            //    {
            //        for (int a = 1; a < productLineList.Count; a++)
            //        {
            //            if (int.Parse(productLineList[i]) != int.Parse(productLineList[a]))
            //            {
            //                isSameLine = false;
            //                break;
            //            }
            //        }
            //        if (isSameLine == false)
            //        {
            //            break;
            //        }

            //    }
            //}


            return result;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                List<ProductPMLine> productLine = new List<ProductPMLine>();
                bool isSameLine = true;
                if (RequestVM.ShiftItemInfoList.Count > 1)
                {
                    queryFilter.ProductSysNos = new List<int>();
                    foreach (var item in RequestVM.ShiftItemInfoList)
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
                                        if (productLine[i].ProductLineSysNo.Value != productLine[a].ProductLineSysNo.Value)
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
                            SaveShiftRequest();
                        }
                    });
                }
                else
                {
                   MaintainFacade.GetProductLineSysNoBySysNo(RequestVM.ShiftItemInfoList[0].ProductSysNo.Value, vm =>
                    {
                        if (vm != null)
                        {
                            RequestVM.ProductLineSysno = vm.ProductLineSysno;
                        }
                        SaveShiftRequest();
                    });          
                }
            }
        }

        private void SaveShiftRequest()
        { 
            if (RequestSysNo.HasValue)
            {               
                InnerUpdateRequest();              
            }
            else
            {
              InnerCreateRequest();             
            }
        }

        private void InnerCreateRequest()
        {
            MaintainFacade.CreateRequest(RequestVM, vmList =>
            {
                if (vmList != null)
                {
                    RequestVM = vmList[0];
                    RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                    RequestSysNo = RequestVM.SysNo;
                    Window.Alert("移仓单创建成功");

                    //打开其他移仓单Tab
                    vmList.ForEach(x =>
                    {
                        if (x.SysNo != RequestVM.SysNo)
                        {
                            Window.Navigate(String.Format(ConstValue.Inventory_ShiftRequestMaintainUrlFormat, x.SysNo), null, true);
                        }
                    });
                }
            });
        }

        private void InnerUpdateRequest()
        {
            MaintainFacade.UpdateRequest(RequestVM, vm =>
            {
                if (vm != null)
                {
                    RequestVM = vm;
                    RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                    Window.Alert("移仓单修改成功");
                }
            });
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
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
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
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
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
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
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
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
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
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                                Window.Alert("出库成功");
                            }
                        });
                    }
                });                      
            }
        }

        private void btnInStock_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行入库操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        MaintainFacade.InStockRequest(RequestVM, vm =>
                        {
                            if (vm != null)
                            {
                                RequestVM = vm;
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                                Window.Alert("入库成功");
                            }
                        });
                    }
                });                       
            }
        }

        private void btnSetSpecialShiftType_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行设置特殊状态操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        List<int> requestSysNoList = new List<int>();
                        requestSysNoList.Add((int)RequestVM.SysNo);
                        MaintainFacade.SetSpecialShiftType(requestSysNoList, vmList =>
                        {
                            if (vmList != null && vmList.Count > 0)
                            {
                                RequestVM = vmList[0];
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                                Window.Alert("设置特殊状态成功");
                            }
                        });
                    }
                });
            }
        }

        private void btnCancelSpecialShiftType_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {
                Window.Confirm("确认要进行取消特殊状态操作？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        List<int> requestSysNoList = new List<int>();
                        requestSysNoList.Add((int)RequestVM.SysNo);
                        MaintainFacade.CancelSpecialShiftType(requestSysNoList, vmList =>
                        {
                            if (vmList != null && vmList.Count > 0)
                            {
                                RequestVM = vmList[0];
                                RequestVM.ShiftShippingTypeList = this.ShiftShippingTypeList;
                                Window.Alert("取消特殊状态成功");
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
                HtmlViewHelper.WebPrintPreview("Inventory", "ShiftRequest", t);
            }
        }

        /// <summary>
        /// 此功能点 不实现了（PM基本没有用过此功能点）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchAddGiftItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRequestMemoMaintain_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(String.Format(ConstValue.Inventory_ShiftRequestMemoMaintainUrlFormat, RequestSysNo.Value), null, true);
        }      

    }

}
