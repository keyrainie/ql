using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Service.SO.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// SOService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public SOFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        ///  计算(价格、费用)
        /// </summary>
        /// <param name="soViewModel"></param>
        /// <param name="callback"></param>
        public void CalculateSO(SOVM soViewModel, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/Calculate";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            restClient.Query<SOInfo>(relativeUrl, soInfo, callback);
        }

        /// <summary>
        ///  数量变化 重新计算(价格、费用)
        /// </summary>
        /// <param name="soViewModel"></param>
        /// <param name="callback"></param>
        public void ProductQtyChange(SOVM soViewModel, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/ProductQtyChange";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            restClient.Query<SOInfo>(relativeUrl, soInfo, callback);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="data">创建订单所需数据</param>
        /// <param name="callback"></param>
        public void CreateSO(SOVM soViewModel, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/Create";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            restClient.Create<SOInfo>(relativeUrl, soInfo, callback);
        }

        public void CloneSO(SOVM so, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/CloneSO";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(so);
            restClient.Create<SOInfo>(relativeUrl, soInfo, callback);
        }

        /// <summary>
        /// 创建赠品订单
        /// </summary>
        /// <param name="soViewModel"></param>
        /// <param name="masterSOSysNo"></param>
        /// <param name="callback"></param>
        public void CreateGiftSO(SOVM soViewModel, int masterSOSysNo, Action<SOVM> callback)
        {
            string relativeUrl = "/SOService/SO/Create/Gift";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            SOCreateGiftReq request = new SOCreateGiftReq { SOInfo = soInfo, MasterSOSysNo = masterSOSysNo };
            restClient.Create<SOInfo>(relativeUrl, request, (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    SOVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertTOSOVMFromSOInfo(args.Result);
                    }
                    callback(vm);
                }
            });
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateSO(SOVM soViewModel, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/Update";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            restClient.Update<SOInfo>(relativeUrl, soInfo, callback);
        }

        /// <summary>
        /// 订单出库后 普票改增票
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void SetSOVATInvoiveWhenSOOutStock(SOVM soVM, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            string relativeUrl = "/SOService/SO/SetSOVATInvoiveWhenSOOutStock";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soVM);
            restClient.Update<SOInfo>(relativeUrl, soInfo, callback);
        }

        /// <summary>
        /// 设置恶意用户
        /// </summary>
        /// <param name="customerSysNo">客户SysNo</param>
        /// <param name="isMalice">是否恶意用户</param>
        /// <param name="memo">备注</param>
        /// <param name="soSysNo">对应的订单编号</param>
        /// <param name="callback"></param>
        public void SetMaliceCustomer(int customerSysNo, string memo, int soSysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string customerRestUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Customer, ConstValue.Key_ServiceBaseUrl);
            RestClient customeRestClient = new RestClient(customerRestUrl);
            customeRestClient.Update<object>(string.Format("/CustomerService/Customer/SetMaliceCustomer/{0}/{1}/{2}", customerSysNo, memo, soSysNo), null, callback);
        }

        /// <summary>
        /// 根据订单编号查询BackOrder
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="callback">回调函数</param>
        public void QueryBackOrderItem(int soSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(string.Format("/SOService/SO/QuerySOBackOrderItem/{0}", soSysNo), callback);
        }

        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info"></param>
        public void WHUpdateStock(SOWHUpdateInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            restClient.Update("/SOService/SO/WHUpdateStock", info, callback);
        }

        public void GetIsAllSubSONotOutStock(int soSysNo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            restClient.Query<bool>("/SOService/IsAllSubSONotOutStockList/"+soSysNo.ToString(), callback);
        }       

        public void GetOnlinePayTypeSysNos(EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/SOService/SO/GetOnlinePayTypeSysNos";
            restClient.Query<List<int>>(relativeUrl,callback);
        }

        #region 数据转换

        /// <summary>
        /// SOViewMode  转换为 SOInfo  
        /// </summary>
        /// <param name="soViewModel"></param>
        /// <returns></returns>
        public static SOInfo ConvertTOSOInfoFromSOVM(SOVM soViewModel)
        {
            SOInfo soInfo = new SOInfo();
            soInfo.CouponCode = soViewModel.CouponCode;
            soInfo.BaseInfo = soViewModel.BaseInfoVM.ConvertVM<SOBaseInfoVM, SOBaseInfo>();
            if (soViewModel.CustomerAuthentication != null)
            {
                soInfo.CustomerAuthentication = soViewModel.CustomerAuthentication.ConvertVM<SOCustomerAuthenticationVM, SOCustomerAuthentication>();
            }
            soInfo.SysNo = soInfo.BaseInfo.SysNo;
            soInfo.CompanyCode = soInfo.BaseInfo.CompanyCode;
            soInfo.Merchant = soInfo.BaseInfo.Merchant;
            soInfo.CompanyCode = soInfo.BaseInfo.CompanyCode;
            soInfo.ShippingInfo = soViewModel.ShippingInfoVM.ConvertVM<SOShippingInfoVM, SOShippingInfo>();
            soInfo.InvoiceInfo = soViewModel.InvoiceInfoVM.ConvertVM<SOInvoiceInfoVM, SOInvoiceInfo>();
            soInfo.InvoiceInfo.VATInvoiceInfo = soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.ConvertVM<SOVATInvoiceInfoVM, SOVATInvoiceInfo>();
            soInfo.FPInfo = soViewModel.FPInfoVM.ConvertVM<SOFPInfoVM, SOFPInfo>();
            foreach (var item in soViewModel.SOInterceptInfoVMList)
            {
                soInfo.SOInterceptInfoList.Add(item.ConvertVM<SOInterceptInfoVM, SOInterceptInfo>());
            }
            foreach (var item in soViewModel.ItemsVM)
            {
                item.ProductType = item.ProductType == null ? SOProductType.Product : item.ProductType;
                soInfo.Items.Add(item.ConvertVM<SOItemInfoVM, SOItemInfo>());
            }
            foreach (var promotionVM in soViewModel.PromotionsVM)
            {
                SOPromotionInfo promotion = promotionVM.ConvertVM<SOPromotionInfoVM, SOPromotionInfo>();

                if (promotionVM.GiftListVM != null)
                {
                    List<SOPromotionInfo.GiftInfo> gifts = new List<SOPromotionInfo.GiftInfo>();
                    promotionVM.GiftListVM.ForEach(x => {
                        gifts.Add(x.ConvertVM<GiftInfoVM, SOPromotionInfo.GiftInfo>((s, t) => {
                            t.ProductSysNo = s.SysNo;
                            t.Quantity = s.Quantity;
                        }));
                        
                    });
                    promotion.GiftList = gifts;
                }

                if (promotionVM.MasterListVM != null)
                {
                    List<SOPromotionInfo.MasterInfo> masters = new List<SOPromotionInfo.MasterInfo>();
                    promotionVM.MasterListVM.ForEach(x=>{
                        masters.Add(x.ConvertVM<MasterInfoVM,SOPromotionInfo.MasterInfo>());
                    });

                    promotion.MasterList = masters;
                }

                //if (promotionVM.PromotionDetailsVM != null)
                //{
                //    List<SOPromotionDetailInfo> details = new List<SOPromotionDetailInfo>();
                //    promotionVM.PromotionDetailsVM.ForEach(x => {
                //        SOPromotionDetailInfo d = new SOPromotionDetailInfo();
                //        d = x.ConvertVM<SOPromotionDetailInfoVM, SOPromotionDetailInfo>();

                //        if(x.GiftListVM!=null)
                //            d.GiftList=x.GiftListVM;

                //        details.Add(d);
                //    });

                //    promotion.SOPromotionDetails = details;
                //}

                soInfo.SOPromotions.Add(promotion);

            }
            if (soViewModel.GiftCardRedeemLogListVM != null && soViewModel.GiftCardRedeemLogListVM.Count > 0)
            {
                soInfo.SOGiftCardList = new List<GiftCardRedeemLog>();
                foreach (var item in soViewModel.GiftCardRedeemLogListVM)
                {
                    soInfo.SOGiftCardList.Add(item.ConvertVM<GiftCardRedeemLogVM, GiftCardRedeemLog>());
                }
            }
            // ---此处 转换比较特殊  主要用于 页面上 数据 为 GiftCardInfo 需要传到后台的 GiftCardRedeemLog中
            if (soViewModel.GiftCardListVM != null && soViewModel.GiftCardListVM.Count > 0)
            {
                soInfo.SOGiftCardList = new List<GiftCardRedeemLog>();
                foreach (var item in soViewModel.GiftCardListVM)
                {
                    GiftCardRedeemLog giftCardRedeemLog = new GiftCardRedeemLog();
                    giftCardRedeemLog.ActionSysNo = soViewModel.SysNo;
                    giftCardRedeemLog.ActionType = ActionType.SO;
                    giftCardRedeemLog.Amount = item.Amount;
                    giftCardRedeemLog.AvailAmount = item.AvailAmount.HasValue ? item.AvailAmount.Value : 0;
                    giftCardRedeemLog.TotalAmount = item.TotalAmount.HasValue ? item.TotalAmount.Value : 0;
                    switch (item.Status)
                    {
                        case GiftCardStatus.Valid:
                            giftCardRedeemLog.Status = ECCentral.BizEntity.IM.ValidStatus.Active;
                            break;
                        case GiftCardStatus.InValid:
                            giftCardRedeemLog.Status = ECCentral.BizEntity.IM.ValidStatus.DeActive;
                            break;
                        case GiftCardStatus.ManualActive:
                            giftCardRedeemLog.Status = ECCentral.BizEntity.IM.ValidStatus.Active;
                            break;
                        default:
                            giftCardRedeemLog.Status = ECCentral.BizEntity.IM.ValidStatus.DeActive;
                            break;
                    }
                    giftCardRedeemLog.CustomerSysNo = soViewModel.BaseInfoVM.CustomerSysNo;
                    giftCardRedeemLog.Code = item.CardCode;
                    giftCardRedeemLog.Type = CardMaterialType.Physical;
                    soInfo.SOGiftCardList.Add(giftCardRedeemLog);
                }

            }
            foreach (var item in soViewModel.StatusChangeInfoListVM)
            {
                soInfo.StatusChangeInfoList.Add(item.ConvertVM<SOStatusChangeInfoVM, SOStatusChangeInfo>());
            }
            #region ReceiveInfo 属性定义不一致 需要单独转换
            soInfo.ReceiverInfo = new SOReceiverInfo();
            soInfo.ReceiverInfo.Address = soViewModel.ReceiverInfoVM.ReceiveAddress;
            soInfo.ReceiverInfo.AreaSysNo = soViewModel.ReceiverInfoVM.ReceiveAreaSysNo;
            soInfo.ReceiverInfo.MobilePhone = soViewModel.ReceiverInfoVM.ReceiveCellPhone;
            soInfo.ReceiverInfo.Phone = soViewModel.ReceiverInfoVM.ReceivePhone;
            soInfo.ReceiverInfo.Name = soViewModel.ReceiverInfoVM.ReceiveContact;
            soInfo.ReceiverInfo.Zip = soViewModel.ReceiverInfoVM.ReceiveZip;
            #endregion
            return soInfo;
        }

        /// <summary>
        /// SOInfo 转换为 SOViewMode     
        /// </summary>
        /// <param name="soViewModel"></param>
        /// <returns></returns>
        public static SOVM ConvertTOSOVMFromSOInfo(SOInfo soInfo)
        {
            SOVM soViewModel = new SOVM();
            if (soInfo != null)
            {
                soViewModel.BaseInfoVM = soInfo.BaseInfo.Convert<SOBaseInfo, SOBaseInfoVM>();
                if (soInfo.CustomerAuthentication != null)
                {
                    soViewModel.CustomerAuthentication = soInfo.CustomerAuthentication.Convert<SOCustomerAuthentication, SOCustomerAuthenticationVM>();
                }
                soViewModel.BaseInfoVM.IsUseGiftCard = soInfo.BaseInfo.GiftCardPay > 0;
                soViewModel.BaseInfoVM.OrderTime = soInfo.BaseInfo.CreateTime;
                soViewModel.SysNo = soInfo.SysNo;
                soViewModel.WebChannel = soInfo.WebChannel;
                soViewModel.CompanyCode = soInfo.CompanyCode;
                soViewModel.Merchant = soInfo.Merchant;
                soViewModel.ShippingInfoVM = soInfo.ShippingInfo.Convert<SOShippingInfo, SOShippingInfoVM>();
                soViewModel.InvoiceInfoVM = soInfo.InvoiceInfo.Convert<SOInvoiceInfo, SOInvoiceInfoVM>();
                soViewModel.InvoiceInfoVM.VATInvoiceInfoVM = soInfo.InvoiceInfo.VATInvoiceInfo.Convert<SOVATInvoiceInfo, SOVATInvoiceInfoVM>();
                 soViewModel.FPInfoVM = soInfo.FPInfo.Convert<SOFPInfo, SOFPInfoVM>();
                foreach (var item in soInfo.SOInterceptInfoList)
                {
                    soViewModel.SOInterceptInfoVMList.Add(item.Convert<SOInterceptInfo, SOInterceptInfoVM>());
                }
                foreach (var item in soInfo.Items)
                {
                    var itemVM = item.Convert<SOItemInfo, SOItemInfoVM>();
                    itemVM.SOVM = soViewModel;
                    soViewModel.ItemsVM.Add(itemVM);
                }
                

                foreach (var item in soInfo.SOPromotions)
                {

                    SOPromotionInfoVM pvm = item.Convert<SOPromotionInfo, SOPromotionInfoVM>();

                    //转化赠品
                    if (item.GiftList != null)
                    {
                        item.GiftList.ForEach(x => {
                            pvm.GiftListVM.Add(
                                x.Convert<SOPromotionInfo.GiftInfo, GiftInfoVM>((t,s) => {
                                    s.SysNo = t.ProductSysNo;
                                    s.Quantity = t.Quantity;
                                    s.QtyPreTime = t.QtyPreTime;
                                })
                            );
                        });
                    }

                    //转换主商品
                    if (item.MasterList != null)
                    {
                        item.MasterList.ForEach(x => {
                            pvm.MasterListVM.Add(
                                x.Convert<SOPromotionInfo.MasterInfo, MasterInfoVM>()
                            );
                        });
                    }


                    soViewModel.PromotionsVM.Add(pvm);
                }
                soViewModel.BaseInfoVM.CouponAmount = Math.Abs(soViewModel.BaseInfoVM.CouponAmount.HasValue ? soViewModel.BaseInfoVM.CouponAmount.Value : 0.00M);
                
                decimal? otherCouponAmount = 0.0M;

                List<SOItemInfoVM> itemVMList = soViewModel.ItemsVM.Where(item => item.ProductType == SOProductType.Product).ToList();
                for (int i = 0; i < itemVMList.Count; i++)
                {
                    SOItemInfoVM item = itemVMList[i];
                    if (i < itemVMList.Count - 1)
                    {
                        item.ShowCouponDiscount = item.CouponAverageDiscount * item.Quantity;
                        if (itemVMList[i].ShowCouponDiscount.HasValue)
                        {
                            otherCouponAmount += Math.Round(item.ShowCouponDiscount.Value, 2);
                        }
                    }
                    else
                    {
                        if (soViewModel.BaseInfoVM.CouponAmount.HasValue)
                        {
                            item.ShowCouponDiscount = Math.Round(soViewModel.BaseInfoVM.CouponAmount.Value, 2) - otherCouponAmount;
                        }
                    }
                }

                foreach (var item in soInfo.SOGiftCardList)
                {
                    soViewModel.GiftCardRedeemLogListVM.Add(item.Convert<GiftCardRedeemLog, GiftCardRedeemLogVM>());
                    GiftCardInfoVM giftCardInfoVM = new GiftCardInfoVM();
                    giftCardInfoVM.CardCode = item.Code;
                    giftCardInfoVM.TotalAmount = item.TotalAmount;
                    giftCardInfoVM.AvailAmount = item.AvailAmount;
                    giftCardInfoVM.Amount = item.Amount;
                    giftCardInfoVM.CustomerSysNo = item.CustomerSysNo;
                    giftCardInfoVM.BindingCustomer.SysNo = item.CustomerSysNo;
                    switch (item.Status)
                    {
                        case ECCentral.BizEntity.IM.ValidStatus.Active:
                            giftCardInfoVM.Status = GiftCardStatus.Valid;
                            break;
                        case ECCentral.BizEntity.IM.ValidStatus.DeActive:
                            giftCardInfoVM.Status = GiftCardStatus.InValid;
                            break;
                        default:
                            giftCardInfoVM.Status = GiftCardStatus.InValid;
                            break;
                    }
                    soViewModel.GiftCardListVM.Add(giftCardInfoVM);
                }
                foreach (var item in soInfo.StatusChangeInfoList)
                {
                    soViewModel.StatusChangeInfoListVM.Add(item.Convert<SOStatusChangeInfo, SOStatusChangeInfoVM>());
                }
                if (!soViewModel.BaseInfoVM.KFCStatus.HasValue)
                {
                    soViewModel.BaseInfoVM.KFCStatus = 0;
                }
                if (soViewModel.BaseInfoVM.CustomerChannel != null && string.IsNullOrEmpty(soViewModel.BaseInfoVM.CustomerChannel.ChannelID))
                {
                    soViewModel.BaseInfoVM.CustomerChannel.ChannelID = "1";
                }
                #region ReceiveInfo 属性定义不一致 需要单独转换
                soViewModel.ReceiverInfoVM.ReceiveAddress = soInfo.ReceiverInfo.Address;
                soViewModel.ReceiverInfoVM.ReceiveAreaSysNo = soInfo.ReceiverInfo.AreaSysNo == 0 ? null : soInfo.ReceiverInfo.AreaSysNo;
                soViewModel.ReceiverInfoVM.ReceiveCellPhone = soInfo.ReceiverInfo.MobilePhone;
                soViewModel.ReceiverInfoVM.ReceivePhone = soInfo.ReceiverInfo.Phone;
                soViewModel.ReceiverInfoVM.ReceiveContact = soInfo.ReceiverInfo.Name;
                soViewModel.ReceiverInfoVM.ReceiveZip = soInfo.ReceiverInfo.Zip;
                soViewModel.ReceiverInfoVM.CustomerSysNo = soViewModel.BaseInfoVM.CustomerSysNo;
                #endregion
            }
            return soViewModel;
        }        

        #endregion

        #region  订单操作

        /// <summary>
        /// 审核订单,单个订单审核和批量审核都用此方法
        /// </summary>
        private void AuditSO(List<int> soSysNoList, bool isForce, bool isManagerAudit, bool isAuditNetPay, Action<SOVM> callback)
        {
            SOAuditReq request = new SOAuditReq
            {
                IsForce = isForce,
                SOSysNoList = soSysNoList,
                IsManagerAudit = isManagerAudit,
                IsAuditNetPay = isAuditNetPay
            };
            restClient.Update<SOInfo>("/SOService/SO/Audit", request, (sender, e) =>
            {
                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(ConvertTOSOVMFromSOInfo(e.Result));
                    }
                }
            });
        }

        /// <summary>
        /// 审核订单,单个订单审核和批量审核都用此方法
        /// </summary>
        public void AuditSO(SOActionValidator soValidator, bool isForce, Action<SOVM> callback)
        {
            string message = null;
            List<SOActionValidator.SOInfo> soList = soValidator.Validate_Audit(out message);
            if (string.IsNullOrEmpty(message))
            {
                AuditSO(SOActionValidator.GetSOSysNoList(soList), isForce, false, false, callback);
            }
            else if (viewPage != null)
            {
                viewPage.Context.Window.Alert(message);
            }
        }

        /// <summary>
        /// 审核订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="isForce">是否强制审核,默认为false</param>
        /// <param name="isAuditNetPay">是否同时审核网上支付,默认为false</param>
        /// <param name="callback"></param>
        public void AuditSO(int soSysNo, bool isForce, bool isAuditNetPay, Action<SOVM> callback)
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.Add(soSysNo);
            AuditSO(soSysNoList, isForce, false, isAuditNetPay, callback);
        }

        /// <summary>
        /// 审核订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="isForce">是否强制审核</param>
        /// <param name="callback"></param>
        public void AuditSO(int soSysNo, bool isForce, Action<SOVM> callback)
        {
            AuditSO(soSysNo, isForce, false, callback);
        }

        /// <summary>
        /// 审核订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="callback"></param>
        public void AuditNetPayAndSO(int soSysNo, Action<SOVM> callback)
        {
            AuditSO(soSysNo, false, true, callback);
        }

        public void ManagerAuditSO(SOActionValidator soValidator, bool isForce, Action<SOVM> callback)
        {
            string message = null;
            List<SOActionValidator.SOInfo> soList = soValidator.Validate_ManagerAudit(out message);
            if (string.IsNullOrEmpty(message))
            {
                AuditSO(SOActionValidator.GetSOSysNoList(soList), isForce, true, false, callback);
            }
            else if (viewPage != null)
            {
                viewPage.Context.Window.Alert(message);
            }
        }

        public void ManagerAuditSO(int soSysNo, bool isForce, Action<SOVM> callback)
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.Add(soSysNo);
            AuditSO(soSysNoList, isForce, true, false, callback);
        }

        public void CancelAuditSO(int soSysNo, Action<SOVM> callback)
        {
            restClient.Update<SOInfo>("/SOService/SO/CAudit", soSysNo, (sender, e) =>
            {
                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(ConvertTOSOVMFromSOInfo(e.Result));
                    }
                }
            });
        }

        private void AbandonSO(List<int> soSysNoList, bool immediatelyReturnInventory, bool isCreateAO, RefundInfoVM refundVM, Action<SOVM> callback)
        {
            ECCentral.BizEntity.Invoice.SOIncomeRefundInfo refundInfo = null;
            if (isCreateAO && refundVM != null)
            {
                refundInfo = new BizEntity.Invoice.SOIncomeRefundInfo
                  {
                      BankName = refundVM.BankName,
                      BranchBankName = refundVM.BranchBankName,
                      CardNumber = refundVM.CardNumber,
                      CardOwnerName = refundVM.CardOwnerName,
                      PostAddress = refundVM.PostAddress,
                      PostCode = refundVM.PostCode,
                      ReceiverName = refundVM.CashReceiver,
                      Note = refundVM.RefundMemo,
                      RefundPayType = refundVM.RefundPayType,
                      RefundReason = refundVM.RefundReason,
                      RefundCashAmt = decimal.Parse(refundVM.RefundCashAmt),
                      SOSysNo = soSysNoList[0]
                  };
            }
            else
            {
                isCreateAO = false;
            }
            SOAbandonReq request = new SOAbandonReq
            {
                ImmediatelyReturnInventory = immediatelyReturnInventory,
                SOSysNoList = soSysNoList,
                IsCreateAO = isCreateAO,
                RefundInfo = refundInfo
            };
            restClient.Update<SOInfo>("/SOService/SO/Abandon", request, (sender, e) =>
            {
                SOVM vm = null;
                if (!e.FaultsHandle() && callback != null)
                {
                    if (e.Result != null)
                    {
                        vm = ConvertTOSOVMFromSOInfo(e.Result);
                    }
                }
                callback(vm);
            });
        }

        /// <summary>
        /// 作废订单，单个订单作废和批量作废都可用此方法
        /// </summary>
        public void AbandonSO(List<int> soSysNoList, bool immediatelyReturnInventory, Action<SOVM> callback)
        {
            AbandonSO(soSysNoList, immediatelyReturnInventory, false, null, callback);
        }
        /// <summary>
        /// 作废单个订单作废
        /// </summary>
        public void CreateAOAndAbandonSO(int soSysNo, bool immediatelyReturnInventory, RefundInfoVM refundVM, Action<SOVM> callback)
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.Add(soSysNo);
            AbandonSO(soSysNoList, immediatelyReturnInventory, true, refundVM, callback);
        }

        /// <summary>
        /// 作废单个订单作废
        /// </summary>
        public void AbandonSO(int soSysNo, bool immediatelyReturnInventory, Action<SOVM> callback)
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.Add(soSysNo);
            AbandonSO(soSysNoList, immediatelyReturnInventory, false, null, callback);
        }

        public void HoldSO(int soSysNo, string reason, Action<SOVM> callback)
        {
            SOHoldReq request = new SOHoldReq
            {
                SOSysNo = soSysNo,
                Note = reason
            };
            restClient.Update<SOInfo>("/SOService/SO/Hold", request, (sender, e) =>
            {
                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(ConvertTOSOVMFromSOInfo(e.Result));
                    }
                }
            });
        }

        public void UnholdSO(int soSysNo, string reason, Action<SOVM> callback)
        {
            SOHoldReq request = new SOHoldReq
            {
                SOSysNo = soSysNo,
                Note = reason
            };
            restClient.Update<SOInfo>("/SOService/SO/Unhold", request, (sender, e) =>
            {
                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(ConvertTOSOVMFromSOInfo(e.Result));
                    }
                }
            });
        }

        /// <summary>
        /// 设置订单的赠值发票已开具
        /// </summary>
        public void SOVATPrinted(List<int> soSysNoList, Action callback)
        {
            restClient.Update("/SOService/SO/VATPrinted", soSysNoList, (sender, e) =>
            {
                if (!e.FaultsHandle() && callback != null)
                {
                    callback();
                }
            });
        }

        /// <summary>
        /// 拆分订单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callBack">将拆分后的子订单编号列表作为参数传入</param>
        public void SplitSO(int soSysNo, Action<List<SOVM>> callBack)
        {
            restClient.Update<List<SOInfo>>("/SOService/SO/Split", soSysNo, (sender, e) =>
            {
                List<SOVM> vmList = new List<SOVM>();
                if (!e.FaultsHandle())
                {
                    if (e.Result != null)
                    {
                        e.Result.ForEach(info =>
                            {
                                vmList.Add(ConvertTOSOVMFromSOInfo(info));
                            });
                    }
                    callBack(vmList);
                }
            });
        }

        /// <summary>
        /// 拆分订单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callBack"></param>
        public void CancelSplitSO(int soSysNo, Action<SOVM> callback)
        {
            restClient.Update<SOInfo>("/SOService/SO/CSplit", soSysNo, (sender, e) =>
            {
                SOVM vm = null;
                if (!e.FaultsHandle() && callback != null)
                {
                    if (e.Result != null)
                    {
                        vm = ConvertTOSOVMFromSOInfo(e.Result);
                    }
                    callback(vm);
                }
            });
        }

        /// <summary>
        /// 拆分发票
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="soCompanyCode">订单的公司编号</param>
        /// <param name="invoiceProductList">拆分后的商品信息</param>
        /// <param name="callback"></param>
        public void SplitInvoice(int soSysNo, string soCompanyCode, List<InvoiceProductVM> invoiceProductList, Action callback)
        {
            SOSpliteInvoiceReq request = new SOSpliteInvoiceReq
            {
                SOSysNo = soSysNo,
                InvoiceItems = (from item in invoiceProductList
                                select new ECCentral.BizEntity.Invoice.SubInvoiceInfo
                                {
                                    StockSysNo = item.StockSysNo,
                                    InvoiceSeq = item.InvoiceNo,
                                    SplitQty = item.InvoiceQuantity,
                                    ProductSysNo = item.ProductSysNo,
                                    SOSysNo = soSysNo,
                                    CompanyCode = soCompanyCode,
                                    IsExtendWarrantyItem = item.IsExtendWarrantyItem,
                                    MasterProductSysNo = item.MasterProductSysNo
                                }).ToList()
            };
            restClient.Update("/SOService/SO/SplitInvoice", request, (sender, e) =>
            {
                if (!e.FaultsHandle() && callback != null)
                {
                    callback();
                }
            });
        }

        /// <summary>
        /// 取消拆分发票
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void CancelSplitInvoice(int soSysNo, Action callback)
        {
            restClient.Update("/SOService/SO/CSplitInvoice", soSysNo, (sender, e) =>
            {
                if (!e.FaultsHandle() && callback != null)
                {
                    callback();
                }
            });
        }

        /// <summary>
        /// 取得订单的拆分后的发票列表
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void GetSOInvoiceList(int soSysNo, List<InvoiceProductVM> originalProductList, Action<bool, List<InvoiceProductVM>> callback)
        {
            restClient.Query<List<ECCentral.BizEntity.Invoice.SubInvoiceInfo>>(String.Format("/InvoiceService/SubInvoice/Load/{0}", soSysNo), (sender, e) =>
            {
                if (!e.FaultsHandle() && callback != null)
                {
                    List<ECCentral.BizEntity.Invoice.SubInvoiceInfo> invoiceList = e.Result;
                    bool isSplited = false;
                    List<InvoiceProductVM> invoiceProductList = originalProductList;
                    if (e.Result != null && e.Result.Count > 0)
                    {
                        isSplited = true;
                        invoiceProductList = invoiceList.Select<ECCentral.BizEntity.Invoice.SubInvoiceInfo, InvoiceProductVM>((item) =>
                            {

                                InvoiceProductVM proudct = null;
                                if (item.IsExtendWarrantyItem.Value && item.MasterProductSysNo != null)
                                {
                                    proudct = originalProductList.FirstOrDefault(p =>
                                    {
                                        bool r = p.ProductSysNo == item.ProductSysNo;
                                        if (r && p.MasterProductSysNo != null && item.MasterProductSysNo.Count == p.MasterProductSysNo.Count)
                                        {
                                            item.MasterProductSysNo.Sort();
                                            p.MasterProductSysNo.Sort();
                                            for (int i = 0; i < item.MasterProductSysNo.Count; i++)
                                            {
                                                if (p.MasterProductSysNo[i] != item.MasterProductSysNo[i])
                                                {
                                                    r = false;
                                                    break;
                                                }
                                            }
                                        }
                                        return r;
                                    });
                                }
                                if (proudct == null)
                                {
                                    proudct = originalProductList.FirstOrDefault(p => p.ProductSysNo == item.ProductSysNo);
                                }
                                return new InvoiceProductVM
                                {
                                    InvoiceNo = item.InvoiceSeq.Value,
                                    InvoiceQuantity = item.SplitQty.Value,
                                    ProductSysNo = item.ProductSysNo.Value,
                                    StockSysNo = item.StockSysNo,
                                    Price = proudct.Price,
                                    ProductID = proudct.ProductID,
                                    Quantity = proudct.Quantity,
                                    ProductName = proudct.ProductName,
                                    StockName = proudct.StockName,
                                    IsExtendWarrantyItem = item.IsExtendWarrantyItem,
                                    MasterProductSysNo = item.MasterProductSysNo,
                                    IsSplited = true
                                };
                            }).ToList();
                    }
                    callback(isSplited, invoiceProductList);
                }
            });
        }

        /// <summary>
        /// 创建客户加积分申请单
        /// </summary>
        /// <param name="soVM">订单信息</param>
        /// <param name="totalPoint">积分</param>
        /// <param name="note">申请备注</param>
        /// <param name="logList">涉及到的商品</param>
        /// <param name="callback"></param> 
        public void CreateCustomerPointsAddRequest(SOVM soVM, int totalPoint, string note, List<PriceChangeLogInfo> logList, EventHandler<RestClientEventArgs<CustomerPointsAddRequest>> callback)
        {
            #region 创建补偿积分请求

            //根据订单模式记录不同NewEggAccount
            string accountInfo = "PM-价保积分";
            if (soVM.ShippingInfoVM.StockType == ECCentral.BizEntity.Invoice.StockType.MET)
            {
                accountInfo = "Seller-Depreciation";
            }
            #region 数据初始
            CustomerPointsAddRequest request = new CustomerPointsAddRequest();
            request.CustomerSysNo = soVM.BaseInfoVM.CustomerSysNo;
            request.SOSysNo = soVM.BaseInfoVM.SysNo;
            request.Point = totalPoint;
            request.PointType = (int)AdjustPointType.ProductPriceAdjustPoint;
            request.Note = note;
            request.NewEggAccount = accountInfo;
            request.Source = "SO";
            request.Memo = "产品调价补偿";
            request.OperationType = AdjustPointOperationType.AddOrReduce;
            List<CustomerPointsAddRequestItem> PointsItemList = new List<CustomerPointsAddRequestItem>();
            foreach (var item in soVM.ItemsVM)
            {
                CustomerPointsAddRequestItem requestItem = new BizEntity.Customer.CustomerPointsAddRequestItem();
                requestItem.Point = item.CompensationPoint;
                requestItem.ProductID = item.ProductID;
                requestItem.ProductSysNo = item.ProductSysNo;
                requestItem.Quantity = item.Quantity;
                requestItem.SOSysNo = soVM.BaseInfoVM.SysNo;
                PointsItemList.Add(requestItem);
            }
            request.PointsItemList = PointsItemList;
            #endregion
            string customerRestUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Customer, ConstValue.Key_ServiceBaseUrl);
            RestClient customeRestClient = new RestClient(customerRestUrl);
            customeRestClient.Update<CustomerPointsAddRequest>("/CustomerService/Point/CreateCustomerPointsAddRequest", request, callback);
            #endregion
        }

        public void ConfirmOperationSubSO(SOVM soVM, Action confirmOperationAllSubSO, Action operationOne)
        {
            if (soVM.BaseInfoVM.SplitType == SOSplitType.SubSO)
            {
                GetIsAllSubSONotOutStock(soVM.SysNo.Value, (subSOSender, subSOargs) =>
                {
                    if (!subSOargs.FaultsHandle())
                    {
                        if (subSOargs.Result)
                        {
                            confirmOperationAllSubSO();
                        }
                        else
                        {
                            operationOne();
                        }
                    }
                });
            }
            else
            {
                operationOne();
            }
        }

        #endregion

        #region SendEmail

        public void SendEmail(SendEmailReq req, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            restClient.Create("/SOService/SO/SendEmail", req, callback);
        }
        #endregion

        public void BatchDealItemInFile(byte[] fileStream, EventHandler<RestClientEventArgs<List<SOItemInfo>>> callback)
        {
            restClient.Query<List<SOItemInfo>>("/SOService/SO/BatchDealItemInFile", fileStream, callback);
        }


        /// <summary>
        /// 审核订单,单个订单审核和批量审核都用此方法
        /// </summary>
        public void BatchReportedSo(List<int> soSysNoList, Action<string> callback)
        {

            restClient.Update<bool>("/SOService/SO/BatchReportedSo", soSysNoList, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        public string GetError(RestServiceError error)
        {
            StringBuilder build = new StringBuilder();
            foreach (Error item in error.Faults)
            {
                build.Append(string.Format("{0}", item.ErrorDescription));
            }
            return build.ToString();
        }

        /// <summary>
        /// 根据订单编号查询支付流水
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="callback">回调函数</param>
        public void QueryPayFlow(int SoSysNo, Action<SOPayFlowQueryVM> callback)
        {
            restClient.Query<TransactionQueryBill>(string.Format("/SOService/SO/QueryBill/{0}", SoSysNo), (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    SOPayFlowQueryVM vm = null;
                    if (args.Result != null)
                    {
                        vm = EntityConverter<TransactionQueryBill, SOPayFlowQueryVM>.Convert(args.Result);
                    }
                    callback(vm);
                }
            });
        }

        /// <summary>
        /// 修改订单状态为 已申报待通关
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void UpdateSOStatusToReported(int soSysNo, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/UpdateSOStatusToReported", soSysNo, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        /// <summary>
        /// 修改订单状态为 申报失败订单作废
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void UpdateSOStatusToReject(int soSysNo, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/UpdateSOStatusToReject", soSysNo, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }


        /// <summary>
        /// 修改订单状态为 已通关发往顾客
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void UpdateSOStatusToCustomsPass(int soSysNo, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/UpdateSOStatusToCustomsPass", soSysNo, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }


        /// <summary>
        /// 修改订单状态为 通关失败订单作废
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void UpdateSOStatusToCustomsReject(int soSysNo, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/UpdateSOStatusToCustomsReject", soSysNo, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        /// <summary>
        /// 批量设置 申报失败订单作废
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <param name="callback"></param>
        public void BatchOperationUpdateSOStatusToReject(List<int> soSysNoList, Action<string> callback)
        {

            restClient.Update<bool>("/SOService/SO/BatchOperationUpdateSOStatusToReject", soSysNoList, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        /// <summary>
        /// 批量设置 已通关发往顾客
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <param name="callback"></param>
        public void BatchOperationUpdateSOStatusToCustomsPass(List<int> soSysNoList, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/BatchOperationUpdateSOStatusToCustomsPass", soSysNoList, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        /// <summary>
        /// 批量设置 通关失败订单作废
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <param name="callback"></param>
        public void BatchOperationUpdateSOStatusToCustomsReject(List<int> soSysNoList, Action<string> callback)
        {
            restClient.Update<bool>("/SOService/SO/BatchOperationUpdateSOStatusToCustomsReject", soSysNoList, (sender, e) =>
            {
                if (e.Error != null)
                {
                    string error = GetError(e.Error);
                    callback(error);
                    return;
                }

                if (!e.FaultsHandle() && e.Result != null && callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void SOMaintainUpdateNote(SOVM soViewModel, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/SOService/SO/SOMaintainUpdateNote";
            SOInfo soInfo = ConvertTOSOInfoFromSOVM(soViewModel);
            restClient.Update(relativeUrl, soInfo, callback);
        }
    }

    

    /// <summary>
    /// 订单操作验证类，将订单在传到Service端前的验证统一放到这里。这里只已取得的数据作验证，如果需要到Service端取数据再验证，则请不要放到这里验证，请直接放到Service端的操作中验证，以减少调用Service的次数。
    /// 可以在类中直接加验证订单用到的属性
    /// </summary>
    public class SOActionValidator
    {
        public class SOInfo
        {
            public int SOSysNo
            {
                get;
                set;
            }

            public SOType? SOType
            {
                get;
                set;
            }

            public SOStatus? SOStatus
            {
                get;
                set;
            }
        }
        public SOActionValidator()
        {
            SOList = new List<SOInfo>();
        }

        public List<SOActionValidator.SOInfo> SOList { get; set; }

        private bool SOListIsNullOrEmpty(out string message)
        {
            message = null;
            if (SOList == null || SOList.Count < 1)
            {
                message = ResSO.Msg_SOIsNull;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 订单的审核验证
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        private bool Validate_Audit(SOActionValidator.SOInfo soInfo, out string message)
        {
            message = null;
            if (soInfo.SOStatus.HasValue && soInfo.SOStatus.Value != BizEntity.SO.SOStatus.Origin)
            {
                message = String.Format(ResSO.Msg_SOStatusIsNotOriginal, soInfo.SOSysNo);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 订单主管审核验证
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        private bool Validate_ManagerAudit(SOActionValidator.SOInfo soInfo, out string message)
        {
            message = null;
            if (soInfo.SOStatus.HasValue && soInfo.SOStatus.Value != BizEntity.SO.SOStatus.WaitingManagerAudit)
            {
                message = String.Format(ResSO.Msg_SOStatusIsNotWaitingManagerAudit, soInfo.SOSysNo);
                return false;
            }
            return true;
        }

        public List<SOActionValidator.SOInfo> Validate_Audit(out string message)
        {
            message = string.Empty;
            List<SOActionValidator.SOInfo> validationSOList = new List<SOInfo>();
            if (SOListIsNullOrEmpty(out message))
            {
                StringBuilder msgBuilder = new StringBuilder();
                foreach (SOActionValidator.SOInfo soInfo in SOList)
                {
                    string msg;
                    if (Validate_Audit(soInfo, out msg))
                    {
                        validationSOList.Add(soInfo);
                        continue;
                    }
                    msgBuilder.AppendLine(msg);
                }
                message = msgBuilder.ToString();
            }
            return validationSOList;
        }

        public List<SOActionValidator.SOInfo> Validate_ManagerAudit(out string message)
        {
            message = null;
            List<SOActionValidator.SOInfo> validationSOList = new List<SOInfo>();
            if (SOListIsNullOrEmpty(out message))
            {
                StringBuilder msgBuilder = new StringBuilder();
                foreach (SOActionValidator.SOInfo soInfo in SOList)
                {
                    string msg;
                    if (Validate_ManagerAudit(soInfo, out msg))
                    {
                        validationSOList.Add(soInfo);
                        continue;
                    }
                    msgBuilder.AppendLine(msg);
                }
                message = msgBuilder.ToString();
            }
            return validationSOList;
        }

        public static List<int> GetSOSysNoList(List<SOActionValidator.SOInfo> soInfoList)
        {
            return soInfoList == null ? null : (from soInfo in soInfoList select soInfo.SOSysNo).ToList();
        }
    }
}
