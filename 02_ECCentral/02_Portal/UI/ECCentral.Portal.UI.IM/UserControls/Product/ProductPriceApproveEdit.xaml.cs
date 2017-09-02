using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Linq;
using System.Windows.Input;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductPriceApproveEdit : UserControl
    {
        #region 属性
        private ProductPriceRequestFacade _facade;
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        public String ProductID { get; set; }
        public int ProductSysNo { get; set; }

        private ProductPriceRequestVM _vm;
        /// <summary>
        /// 提交通过的状态
        /// </summary>
        public ProductPriceRequestStatus PassStatus { get; set; }
        #endregion

        #region 初始化加载
        public ProductPriceApproveEdit()
        {
            InitializeComponent();
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindPage();
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {

            if (SysNo > 0)
            {
                txtProductID.Text = ProductID;
                _facade = new ProductPriceRequestFacade();
                _facade.GetNeweggProductPriceRequestBySysNo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("没有获得商品价格审核对象");
                        return;
                    }

                    _vm = new ProductPriceRequestVM()
                    {
                        Category = args.Result.PriceRequestMsg.Category,
                        AlipayVipPrice = args.Result.PriceRequestMsg.AlipayVipPrice,
                        AuditTime = args.Result.PriceRequestMsg.AuditTime,
                        AuditType = args.Result.PriceRequestMsg.AuditType,
                        AuditUser = args.Result.PriceRequestMsg.AuditUser,
                        AvailableQty = args.Result.PriceRequestMsg.AvailableQty,
                        BasicPrice = args.Result.PriceRequestMsg.BasicPrice,
                        CashRebate = args.Result.PriceRequestMsg.CashRebate,
                        ConsignQty = args.Result.PriceRequestMsg.ConsignQty,
                        CreateUser = args.Result.PriceRequestMsg.CreateUser,
                        CurrentPrice = args.Result.PriceRequestMsg.CurrentPrice,
                        DenyReason = args.Result.PriceRequestMsg.DenyReason,
                        DiscountAmount = args.Result.PriceRequestMsg.DiscountAmount,
                        FinalAuditUser = args.Result.PriceRequestMsg.FinalAuditUser,
                        FinalAuditTime = args.Result.PriceRequestMsg.FinalAuditTime,
                        IsExistRankPrice = args.Result.PriceRequestMsg.IsExistRankPrice,
                        IsUseAlipayVipPrice = args.Result.PriceRequestMsg.IsUseAlipayVipPrice,
                        IsWholeSale = args.Result.PriceRequestMsg.IsWholeSale,
                        LastInTime = args.Result.PriceRequestMsg.LastInTime,
                        LastUpdateTime = args.Result.PriceRequestMsg.LastUpdateTime,
                        Margin = args.Result.PriceRequestMsg.Margin,
                        MarginAmount = args.Result.PriceRequestMsg.MarginAmount,
                        MaxCountPerDay = args.Result.PriceRequestMsg.MaxCountPerDay,
                        MinCountPerOrder = args.Result.PriceRequestMsg.MinCountPerOrder,
                        NewMargin = args.Result.PriceRequestMsg.NewMargin,
                        NewMarginAmount = args.Result.PriceRequestMsg.NewMarginAmount,
                        PayType = args.Result.PriceRequestMsg.PayType,
                        PMDMemo = args.Result.PriceRequestMsg.PMDMemo,
                        PMMemo = args.Result.PriceRequestMsg.PMMemo,
                        Point = args.Result.PriceRequestMsg.Point,
                        ProductRankPrice = args.Result.PriceRequestMsg.ProductRankPrice,
                        ProductWholeSalePriceInfo = args.Result.PriceRequestMsg.ProductWholeSalePriceInfo,
                        RequestStatus = args.Result.PriceRequestMsg.RequestStatus,
                        SysNo = args.Result.PriceRequestMsg.SysNo,
                        UnitCost = args.Result.PriceRequestMsg.UnitCost,
                        UnitCostWithoutTax = args.Result.PriceRequestMsg.UnitCostWithoutTax,
                        VirtualPrice = args.Result.PriceRequestMsg.VirtualPrice,
                        TLMemo = args.Result.PriceRequestMsg.TLMemo,
                        GiftSysNo = args.Result.PriceRequestMsg.GiftSysNo,
                        CouponSysNo = args.Result.PriceRequestMsg.CouponSysNo
                    };
                    _vm.OldPrice = args.Result.PriceRequestMsg.OldPrice.Convert<ProductPriceInfo, ProductPriceInfoVM>();
                    DataContext = _vm;
                    BindOptionalAccessories(args.Result.PromotionMsg);
                    if (_vm.RequestStatus == ProductPriceRequestStatus.Origin)
                    {
                        tb_PMDMemo.IsReadOnly = true;
                    }
                    else if (_vm.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove)
                    {
                        tb_TLMemo.IsReadOnly = true;
                    }
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有获得商品价格审核对象");
                return;
            }
        }

        private void BindOptionalAccessories(List<ProductPromotionMsg> msg)
        {
            if (msg != null && msg.Count > 0)
            {
                var source = msg.Where(p => p.PromotionType == PromotionType.OptionalAccessories).ToList();
                if (source != null && source.Count > 0)
                {
                    source.ForEach(v =>
                                       {
                                           var userContrl = new ProductPriceOptionalAccessories();
                                           v.Discount = -v.Discount;
                                           userContrl.DataBind(v);
                                           Panel_OptionalAccessories.Children.Add(userContrl);
                                       });
                }
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件


        private ProductPriceRequestInfo GetProductPriceRequestInfo(ProductPriceRequestStatus status)
        {
            if (_vm == null)
            {
                return new ProductPriceRequestInfo();
            }
            var entity = new ProductPriceRequestInfo
                             {
                                 SysNo = SysNo,
                                 RequestStatus = status,
                                 TLMemo = _vm.TLMemo,
                                 PMDMemo = _vm.PMDMemo,
                                 HasAdvancedAuditPricePermission = _vm.HasAdvancedAuditPricePermission,
                                 HasPrimaryAuditPricePermission = _vm.HasPrimaryAuditPricePermission
                             };
            return entity;
        }

        private void btnPass_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkResult = CheckDemo();
            if (!checkResult) return;
            var entity = GetProductPriceRequestInfo(PassStatus);
            //初级毛利率大于申请后毛利率需要确认
            if (_vm.Category.CategorySetting.PrimaryMargin >= _vm.Margin)
            {
                var info = string.Format(ResProductPriceApproveEdit.MarginMessage, _vm.NewMargin.ToString("P"),
                                         _vm.NewMarginAmount);
                CPApplication.Current.CurrentPage.Context.Window.Confirm(info, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        Save(entity);
                    }
                });
            }
            else
            {
                Save(entity);
            }

        }

        private void btnNoPass_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkResult = CheckDemo();
            if (!checkResult) return;
            var entity = GetProductPriceRequestInfo(ProductPriceRequestStatus.Deny);
            _facade = new ProductPriceRequestFacade();
            _facade.AuditProductPriceRequest(entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功");
                CloseDialog(DialogResultType.OK);
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.OK);
        }

        private void btnOnePass_Click(object sender, RoutedEventArgs e)
        {
            var checkResult = CheckDemo();
            if (!checkResult) return;
            var entity = GetProductPriceRequestInfo(ProductPriceRequestStatus.Approved);
            entity.IsOnePass = true;
            //初级毛利率大于申请后毛利率需要确认
            if (_vm.Category.CategorySetting.PrimaryMargin >= _vm.Margin)
            {
                var info = string.Format(ResProductPriceApproveEdit.MarginMessage, _vm.NewMargin.ToString("P"),
                                         _vm.NewMarginAmount);
                CPApplication.Current.CurrentPage.Context.Window.Confirm(info, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        Save(entity);
                    }
                });
            }
            else
            {
                Save(entity);
            }

        }

        private void Save(ProductPriceRequestInfo entity)
        {

            _facade = new ProductPriceRequestFacade();
            _facade.AuditProductPriceRequest(entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功");
                CloseDialog(DialogResultType.OK);
            });
        }
        /// <summary>
        /// 检查审核理由是否填写
        /// </summary>
        private bool CheckDemo()
        {
            var demo = "";
            var result = true;
            if (_vm.RequestStatus == ProductPriceRequestStatus.Origin)
            {
                demo = (_vm.TLMemo ?? "").Trim();
                if (string.IsNullOrEmpty(demo))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("TL审核理由不能为空");
                    result = false;
                }
            }
            else if (_vm.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove)
            {
                demo = (_vm.PMDMemo ?? "").Trim();
                if (string.IsNullOrEmpty(demo))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("PMD审核理由不能为空");
                    result = false;
                }
            }
            return result;
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }


        private void hlbProductLink_Click(object sender, RoutedEventArgs e)
        {
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(urlFormat, ProductSysNo), null, true);
            CloseDialog(DialogResultType.Cancel);
        }

        private void hlbLastInTimeLink_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Format(ConstValue.IM_ProductPurchaseHistoryUrlFormat, ProductSysNo + "|" + ProductID);
            CPApplication.Current.CurrentPage.Context.Window.Navigate(url, null, true);
            CloseDialog(DialogResultType.Cancel);
        }
        #endregion


        #endregion

        #region 跳转
        #endregion


    }
}
