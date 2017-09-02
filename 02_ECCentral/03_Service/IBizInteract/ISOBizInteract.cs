using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.IBizInteract
{
    public interface ISOBizInteract
    {
        /// <summary>
        /// 生成一个新的订单编号
        /// </summary>
        /// <returns></returns>
        int NewSOSysNo();

        /// <summary>
        /// 根据订单系统编号取得订单商品列表
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        List<SOItemInfo> GetSOItemList(int soSysNo);

        /// <summary>
        /// 根据订单编号取得订单主要信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        SOBaseInfo GetSOBaseInfo(int soSysNo);

        List<SOBaseInfo> GetSOBaseInfoList(List<int> soSysNoList);

        /// <summary>
        /// 根据订单编号取得订单所有信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        SOInfo GetSOInfo(int soSysNo);

        ///// <summary>
        ///// 根据订单编号取得订单状态
        ///// </summary>
        ///// <param name="soSysNo"></param>
        ///// <returns></returns>
        SOStatus GetSOStatus(int soSysNo);

        /// <summary>
        /// 添加订单投诉
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        SOComplaintCotentInfo AddComplain(SOComplaintCotentInfo info);

        /// <summary>
        /// 修改订单投诉
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        SOComplaintInfo ProcessComplain(SOComplaintInfo info);

        /// <summary>
        /// 根据订单编号取得订单收货人手机
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        string GetSOReceiverPhone(int soSysNo);

        ///// <summary>
        ///// 根据订单编号取得订单客户编号
        ///// </summary>
        ///// <param name="soSysNo"></param>
        ///// <returns></returns>
        //int GetSOCustomerSysNo(int soSysNo);

        /// <summary>
        /// 根据订单编号查检订单是否存在
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool ExistSO(int soSysNo);

        /// <summary>
        /// 根据订单编号取得订单的移仓单号
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        int GetShiftSysNoBySOSysNo(int soSysNo);


        /// <summary>
        /// 根据订单中的商品编号取得此商品的赠品列表.
        /// IPP3: [OverseaOrderManagement].[dbo].[UP_OM_GetGiftInfo] 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns>赠品列表</returns>
        List<SOItemInfo> GetGiftBySOProductSysNo(int soSysNo, int productSysNo);

        /// <summary>
        /// 根据订单编号取得订单拆分的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo);

        /// <summary>
        /// 将订单状态更改为物流拒收状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void UpdateSOStatusToReject(int soSysNo);

        /// <summary>
        /// 取得配送信息
        /// </summary>
        /// <param name="type">配送信息类型</param>
        /// <param name="orderSysNo">对应配送类型的单据编号</param>
        /// <returns></returns>
        DeliveryInfo GetDeliveryInfo(DeliveryType type, int orderSysNo, DeliveryStatus status);

        /// <summary>
        /// 根据优惠券编号，取得使用此优惠券的订单编号
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        int? GetSOSysNoByCouponSysNo(int couponSysNo);

        /// <summary>
        /// PO - 生成虚库采购单后将对应的订单标识为备货状态
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="vpoStatus"></param>
        void UpdateSOCheckShipping(int soSysNo, VirtualPurchaseOrderStatus vpoStatus);

        /// <summary>
        /// 此方法返回不完整的订单信息，只填充了订单的一些属性。
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <returns></returns>
        List<SOInfo> GetSimpleSOInfoList(List<int> soSysNoList);

        /// <summary>
        /// 查询当前itemSysNo已经创建的虚库采购单条数
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <returns></returns>
        int GetGeneratedSOVirtualCount(int soItemSysNo);

        /// <summary>
        /// 查询15分钟前待审核的秒杀订单
        /// </summary>
        /// <returns></returns>
        List<SOInfo> GetSOStatus();

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="soSysNo"></param>
        void MakeOpered(int soSysNo);
        /// <summary>
        /// 根据订单编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo);
    }
}
