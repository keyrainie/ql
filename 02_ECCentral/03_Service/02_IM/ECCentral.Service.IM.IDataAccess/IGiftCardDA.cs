using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using System.Data;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IGiftCardDA
    {
        List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType);

        GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type);

        List<GiftCardInfo> GetGiftCardsByCodeList(List<string> codeList);

        List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType);

        string OperateGiftCard(string xmlMsg);

        /// <summary>
        /// 操作礼品卡的状态
        /// </summary>
        /// <param name="action"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string OperateGiftCardStatus(string action, ECCentral.BizEntity.IM.GiftCardInfo item);

        /// <summary>
        /// 加载礼品卡
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        GiftCardInfo LoadGiftCardInfo(int sysNo);

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        List<GiftCardOperateLog> GetGiftCardOperateLogByCode(string code);
        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        List<GiftCardRedeemLog> GetGiftCardRedeemLogJoinSOMaster(string code);

        /// <summary>
        /// GetItemExchangeRate
        /// </summary>
        /// <param name="currencySysNo"></param>
        /// <returns></returns>
        decimal GetItemExchangeRate(int currencySysNo);

        /// <summary>
        /// GetItemExchangeRate
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        int GetItemPayTypeSysNo(int vendorSysNo);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="exchangeRate"></param>
        /// <param name="iPayPeriodType"></param>
        /// <returns></returns>
        List<GiftCardFabrication> GetGiftCardFabricationItem(int sysNo, decimal exchangeRate, int iPayPeriodType);

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        DataTable GetGiftCardFabricationItemSum(int sysNo);

        /// <summary>
        /// 更新主体信息
        /// </summary>
        /// <param name="item"></param>
        void UpdateGiftCardFabricationMaster(GiftCardFabricationMaster item);

        #region GiftCardFabricationMaster

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        void DeleteGiftCardFabrication(int sysNo);

        /// <summary>
        /// 创建,事实已经存在该GiftCardFabricationMaster对象 
        /// </summary>
        /// <param name="item"></param>
        void CreatePOGiftCardFabrication(GiftCardFabricationMaster item);

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="item"></param>
        int InsertGiftCardFabricationMaster(GiftCardFabricationMaster item);
        #endregion

        #region GiftCardFabrication
        /// <summary>
        /// 新建礼品卡子项，对应礼品卡具体金额
        /// </summary>
        /// <param name="item"></param>
        void InsertGiftCardFabricationItem(GiftCardFabrication item);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="item"></param>
        void UpdateGiftCardFabricationItem(GiftCardFabrication item);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        decimal GetAddGiftCardInfoList(int sysNo);

        /// <summary>
        /// 导出制卡后改变礼品卡制作单的状态
        /// </summary>
        /// <param name="sysNO"></param>
        void UpdateGiftCardInfoStatus(int sysNO);

        /// <summary>
        /// 获取当前生成的需要导出的礼品卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        DataTable GetGiftCardInfoByGiftCardFabricationSysNo(int sysNo);
        #endregion

        #region GiftVoucherProduct

        int SaveGiftVoucherProduct(GiftVoucherProduct item);

        void UpdateVoucherProduct(GiftVoucherProduct item);

        GiftVoucherProduct GetVoucherProductBySysNo(int sysNo);

        List<GiftVoucherProductRelation> GetVoucherProductRelationByVoucher(int sysNo);

        GiftVoucherProductRelation GetVoucherProductRelationByRequest(int sysNo);

        List<GiftVoucherProductRelationRequest> GetGiftVoucherProductRelationRequestByRelation(int sysNo);

        GiftVoucherProductRelationRequest GetGiftVoucherProductRelationRequestBySysNo(int sysNo);

        int SaveGiftVoucherProductRelation(GiftVoucherProductRelation item);

        void SaveGiftVoucherProductRelationRequest(GiftVoucherProductRelationRequest item);

        void DeleteGiftVoucherProductRelationByRelation(GiftVoucherProductRelation item);

        void DeleteGiftVoucherProductReleationBySysNo(GiftVoucherProductRelation item);

        void UpdateGiftVoucherProductRelationStatus(GiftVoucherProductRelation item);

        void UpdateGiftVoucherProductRelationRequestStatus(GiftVoucherProductRelationRequest item);

        bool IsExistsSameProduct(GiftVoucherProduct item);

        bool IsExistSameGiftVoucherPrice(GiftVoucherProduct item);

        void UpdateProductGiftVoucherType(int productSysNo, GiftVoucherType voucherType);

        bool IsExistsSameRelationProduct(int productSysNo);

        #endregion
    }
}
