using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.SO;
using System.ComponentModel.Composition;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.SO.AppService
{
    /// <summary>
    /// 订单服务
    /// </summary>
    [Export(typeof(SOBizInteractAppService))]
    [VersionExport(typeof(ISOBizInteract))]
    public class SOBizInteractAppService : ISOBizInteract
    {
        public List<SOItemInfo> GetSOItemList(int soSysNo)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetSOItemsBySOSysNo(soSysNo);
        }

        public SOBaseInfo GetSOBaseInfo(int soSysNo)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetSOBaseInfoBySOSysNo(soSysNo);
        }

        public List<SOBaseInfo> GetSOBaseInfoList(List<int> soSysNoList)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetSOBaseInfoBySOSysNoList(soSysNoList);
        }
        public SOInfo GetSOInfo(int soSysNo)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetSOBySOSysNo(soSysNo);
        }

        //public SOStatus GetSOStatus(int soSysNo)
        //{
        //    throw new NotImplementedException();
        //}

        public SOComplaintCotentInfo AddComplain(SOComplaintCotentInfo info)
        {
            return ObjectFactory<SOComplainProcessor>.Instance.Create(info).ComplaintCotentInfo;
        }

        public SOComplaintInfo ProcessComplain(SOComplaintInfo info)
        {
            return ObjectFactory<SOComplainProcessor>.Instance.Update(info);
        }

        public string GetSOReceiverPhone(int soSysNo)
        {
            List<SOInfo> soList = GetSimpleSOInfoList(new List<int> { { soSysNo } });
            if (soList != null && soList.Count > 0)
            {
                return soList[0].ReceiverInfo.MobilePhone == null || soList[0].ReceiverInfo.MobilePhone.Trim() == string.Empty ? soList[0].ReceiverInfo.Phone : soList[0].ReceiverInfo.MobilePhone;
            }
            return string.Empty;
        }

        //public int GetSOCustomerSysNo(int soSysNo)
        //{
        //    throw new NotImplementedException();
        //}

        public bool ExistSO(int soSysNo)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetSOBaseInfoBySOSysNo(soSysNo) != null;
        }

        public int GetShiftSysNoBySOSysNo(int soSysNo)
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.GetShiftSysNoBySOSysNo(soSysNo);
        }

        public int NewSOSysNo()
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            return processor.NewSOSysNo();
        }


        /// <summary>
        /// 根据订单中的商品编号取得此商品的赠品列表 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns>赠品列表</returns>
        public List<SOItemInfo> GetGiftBySOProductSysNo(int soSysNo, int productSysNo) //IPP3:OverseaOrderManagement].[dbo].[UP_OM_GetGiftInfo]
        {
            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;
            SOBaseInfo soBaseInfo = processor.GetSOBaseInfoBySOSysNo(soSysNo);
            List<SOItemInfo> giftList = new List<SOItemInfo>();
            if (soBaseInfo.SOType != SOType.Gift)
            {
                List<SOInfo> soList = null;
                List<int> soSysNoList = new List<int>();
                if (soBaseInfo.Status == SOStatus.Split)
                {
                    soList = processor.GetSubSOByMasterSOSysNo(soSysNo);
                }
                else
                {
                    soSysNoList.Add(soSysNo);
                }
                soList = soList ?? new List<SOInfo>();

                // 如果订单的赠品(指购买订单，送给客户的商品，但此商品并没有一起同订单发给客户，请与赠品规则的赠品，厂商赠品，附件相区分。)，且赠品已经下单，则取得订单的赠品的订单编号
                List<SOLogInfo> logList = new List<SOLogInfo>();
                if (soList.Count > 0)
                {
                    soList.ForEach(so =>
                    {
                        List<SOLogInfo> t = ObjectFactory<SOLogProcessor>.Instance.GetSOLogBySOSysNoAndLogType(soSysNo, BizEntity.Common.BizLogType.Sale_SO_CreateGiftSO);
                        if (t != null && t.Count > 0)
                        {
                            logList.AddRange(t);
                        }
                    });
                }

                if (logList.Count > 0)
                {
                    logList.ForEach(l =>
                    {
                        int no = int.TryParse(l.Note, out no) ? no : 0;
                        if (no != 0)
                        {
                            soSysNoList.Add(no);
                        }
                    });
                }
                var tso = processor.GetSOBySOSysNoList(soSysNoList);
                if (tso != null && tso.Count > 0)
                {
                    soList.AddRange(tso);
                }
                soList.ForEach(so =>
                {
                    List<SOItemInfo> tgList = null;
                    if (so.BaseInfo.SOType == SOType.Gift)
                    {
                        tgList = (from item in so.Items
                                  where String.Format(",{0},", item.MasterProductSysNo).IndexOf(String.Format(",{0},", productSysNo)) >= 0
                                  select item).ToList();
                    }
                    else
                    {
                        List<SOPromotionInfo.GiftInfo> gList = (from p in so.SOPromotions
                                                                from pd in p.SOPromotionDetails
                                                                from g in p.GiftList
                                                                where pd.MasterProductSysNo == productSysNo && (p.PromotionType == SOPromotionType.Accessory || p.PromotionType == SOPromotionType.SelfGift || p.PromotionType == SOPromotionType.VendorGift)
                                                                select g).ToList();
                        tgList = (from item in so.Items
                                  join g in gList on item.ProductSysNo equals g.ProductSysNo
                                  select item).ToList();
                    }
                    if (tgList != null && tgList.Count > 0)
                    {
                        giftList.AddRange(tgList);
                    }
                });
            }
            return giftList;
        }

        public List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo)
        {
            return ObjectFactory<SOPriceProcessor>.Instance.GetSOPriceBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 将订单状态更改为物流拒收状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void UpdateSOStatusToReject(int soSysNo)
        {
            ObjectFactory<SOProcessor>.Instance.UpdateSOStatusToReject(soSysNo);
        }

        /// <summary>
        /// 取得配送信息
        /// </summary>
        /// <param name="type">配送信息类型</param>
        /// <param name="orderSysNo">对应配送类型的单据编号</param>
        /// <returns></returns>
        [Obsolete("商城没有自有物流，因而不能取得配送任务")]
        public DeliveryInfo GetDeliveryInfo(DeliveryType type, int orderSysNo, DeliveryStatus status)
        {
            //return ObjectFactory<DeliveryProcessor>.Instance.GetDeliveryInfo(type, orderSysNo, status);
            return null;
        }
        /// <summary>
        /// 根据优惠券编号，取得使用此优惠券的订单编号
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        public int? GetSOSysNoByCouponSysNo(int couponSysNo)
        {
            return ObjectFactory<SOProcessor>.Instance.GetSOSysNoByCouponSysNo(couponSysNo);
        }

        /// <summary>
        /// PO - 作废虚库采购单后将对应的订单标识为未备货状态
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="vpoStatus"></param>
        public void UpdateSOCheckShipping(int soSysNo, BizEntity.PO.VirtualPurchaseOrderStatus vpoStatus)
        {
            ObjectFactory<SOProcessor>.Instance.UpdateSOCheckShippingVPOStatus(soSysNo, vpoStatus);
        }

        public List<SOInfo> GetSimpleSOInfoList(List<int> soSysNoList)
        {
            return ObjectFactory<SOProcessor>.Instance.GetSimpleSOInfoList(soSysNoList);
        }

        /// <summary>
        /// 查询当前itemSysNo已经创建的虚库采购单条数
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <returns></returns>
        public int GetGeneratedSOVirtualCount(int soItemSysNo)
        {
            return ObjectFactory<SOProcessor>.Instance.GetGeneratedSOVirtualCount(soItemSysNo);
        }



        public SOStatus GetSOStatus(int soSysNo)
        {
            throw new NotImplementedException();
        }

        public List<SOInfo> GetSOStatus()
        {
            throw new NotImplementedException();
        }

        public void MakeOpered(int soSysNo)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 根据订单编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo)
        {
            return ObjectFactory<SOProcessor>.Instance.LoadVendorCustomsInfo(soSysNo);
        }
    }
}
