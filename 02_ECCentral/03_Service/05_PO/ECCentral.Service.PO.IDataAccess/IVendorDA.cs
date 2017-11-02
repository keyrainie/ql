using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

using System.Data;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IVendorDA
    {
        /// <summary>
        /// 加载供应商信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorInfo LoadVendorInfo(int vendorSysNo);

        VendorInfo LoadVendorInfoByVendorName(string vendorName);

        /// <summary>
        /// 加载供应商扩展信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorExtendInfo LoadVendorExtendInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 加载供应商附件信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorAttachInfo LoadVendorAttachmentsInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 加载供应商历史信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        List<VendorHistoryLog> LoadVendorHistoryLog(VendorInfo vendorInfo);

        /// <summary>
        /// 加载供应商代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        List<VendorAgentInfo> LoadVendorAgentInfoList(VendorInfo vendorInfo);

        /// <summary>
        /// 加载供应商代理信息(根据供应商编号和商品编号)
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        VendorAgentInfo LoadVendorAgentInfoByVendorAndProductID(int vendorSysNo, int productSysNo);

        /// <summary>
        /// 保存(更新)供应商代理信息
        /// </summary>
        /// <param name="vendorAgentInfo"></param>
        /// <returns></returns>
        VendorAgentInfo SaveVendorAgentInfo(VendorAgentInfo vendorAgentInfo);

        /// <summary>
        /// 保存供应商基本信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorInfo SaveVendorBasicInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 创建供应商信息(所有新增代理信息均需审核,不直接新增)
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorInfo CreateVendorInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 新建/更新 供应商附件信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorInfo CreateOrUpdateVendorAttachInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 创建/更新 供应商扩展信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorInfo CreateOrUpdateVendorExtendInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 创建供应商代理信息(厂商信息)
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorAgentInfo CreateVendorManufacturerInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo);

        /// <summary>
        /// 创建供应商代理信息(佣金信息)
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorAgentInfo CreateVendorCommissionInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo, string createUserName);

        /// <summary>
        ///  创建供应商代理信息(佣金信息)y>
        /// <param name="VendorAgentInfo"></param>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorAgentInfo CreateVMVendorCommissionInfo(int createUserSysNo, VendorAgentInfo VendorAgentInfo);

        /// <summary>
        /// 作废代理商佣金信息
        /// </summary>
        /// <param name="agentInfoSysNo"></param>
        /// <returns></returns>
        int AbandonVMCommissionRule(int agentInfoSysNo);

        /// <summary>
        /// 创建/更新供应商附件信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorAttachInfo SaveVendorAttachmentInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 新建供应商修改请求
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        VendorModifyRequestInfo CreateModifyRequest(VendorModifyRequestInfo requestInfo);

        /// <summary>
        /// 新建供应商修改请求(财务)
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        VendorModifyRequestInfo CreateVendorModifyRequest(VendorModifyRequestInfo requestInfo);

        /// <summary>
        /// 更新供应商修改申请请求的状态
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        VendorModifyRequestInfo UpdateVendormodifyRequestStatus(VendorModifyRequestInfo requestInfo);

        /// <summary>
        /// 更新供应商下单日期（审核通过后.)
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        void UpdateVendorBuyWeekDay(VendorModifyRequestInfo modifyRequestInfo);

        /// <summary>
        /// 获取Check状态的供应商代理信息:
        /// </summary>
        /// <param name="vendorManufacturerSysNo"></param>
        /// <returns></returns>
        List<VendorAgentInfo> GetCheckVendorManufacturerInfo(int vendorManufacturerSysNo);

        /// <summary>
        /// 获取厂商信息
        /// </summary>
        /// <param name="manufacturersysno"></param>
        /// <returns></returns>
        string GetManufacturerName(int manufacturersysno);

        /// <summary>
        /// 获取供应商代理厂商信息
        /// </summary>
        /// <param name="vendorManufacturerSysNo"></param>
        /// <returns></returns>
        VendorAgentInfo GetVendorManufacturerBySysNo(int vendorManufacturerSysNo);

        /// <summary>
        /// 更新供应商EMail地址
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="newMailAddress"></param>
        void UpdateVendorEmailAddress(int vendorSysNo, string newMailAddress);

        /// <summary>
        /// 创建供应商历史日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        VendorHistoryLog CreateVendorHistoryLog(VendorHistoryLog log);

        /// <summary>
        /// 加载供应商修改请求
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<VendorModifyRequestInfo> LoadVendorModifyRequests(VendorModifyRequestInfo info);

        /// <summary>
        ///  删除供应商代理信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        int DeleteVendorManufacturer(VendorModifyRequestInfo info);

        /// <summary>
        /// 取消审核  - 供应商修改申请
        /// </summary>
        /// <param name="info"></param>
        void CancelVendorModifyRequest(VendorModifyRequestInfo info);

        /// <summary>
        /// 根据系统编号获取供应商修改申请
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        VendorModifyRequestInfo LoadVendorModifyRequest(int sysNo, VendorModifyRequestStatus status);

        /// <summary>
        /// 审核通过 - 供应商修改申请
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        VendorModifyRequestInfo ApproveVendorModifyRequest(VendorModifyRequestInfo info);

        /// <summary>
        ///  审核未通过/撤销审核  供应商修改申请
        /// </summary>
        /// <param name="info"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        VendorModifyRequestInfo DeclineWithdrawVendorModifyRequest(VendorModifyRequestInfo info, VendorModifyRequestStatus status);

        /// <summary>
        /// 根据新的合作日期,重新计算累计金额
        /// </summary>
        /// <param name="vendorSysNo"></param>
        void CalcTotalPOMoney(int vendorSysNo);

        /// <summary>
        ///  锁定/解锁 供应商
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isHold"></param>
        /// <param name="holdReason"></param>
        void HoldOrUnholdVendor(int userSysNo, int vendorSysNo, bool isHold, string holdReason);

        /// <summary>
        /// 编辑供应商信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        VendorInfo EditVendorInfo(VendorInfo vendorInfo);

        /// <summary>
        /// 更新供应商等级信息
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        int UpdateVendorRank(VendorModifyRequestInfo requestInfo);

        /// <summary>
        /// 更新供应商代理信息
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        int UpdateVendorManufacturer(VendorModifyRequestInfo requestInfo);

        /// <summary>
        /// 获取代理产品数量
        /// </summary>
        /// <param name="vendorManufacturerSysNo"></param>
        /// <returns></returns>
        int GetItemCountByVendorManufacturerSysNo(int vendorManufacturerSysNo);

        /// <summary>
        /// 获取供应商账期类型
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        int LoadVendorPayPeriodType(int? vendorSysNo);

        /// <summary>
        /// 获取供应商财务审核申请信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorModifyRequestInfo GetApplyVendorFinanceModifyRequest(int vendorSysNo);

        /// <summary>
        /// 获取当前最大的SellerID
        /// </summary>
        /// <param name="vendorType"></param>
        /// <returns></returns>
        string GetMaxSellerID(int vendorType, string companyCode);

        /// <summary>
        /// 根据类型获取商家List:
        /// </summary>
        /// <param name="vendorType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<KeyValuePair<int, string>> GetVendorNameListByVendorType(VendorType vendorType, string companyCode);

        /// <summary>
        /// 获取供应商邮件信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        DataTable GetVendorMailInfo(int sysNo);

        bool CheckVendorFiananceAccountAndConsignExists(string account, VendorConsignFlag consignFlag, int? vendorSysNo, string companyCode);


        /// <summary>
        /// 获取已经锁定的PM列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<VendorHoldPMInfo> GetVendorPMHoldInfoByVendorSysNo(int vendorSysNo, string companyCode);

        /// <summary>
        /// 插入VendorPM Information
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        VendorHoldPMInfo CreateVendorPMHoldInfo(VendorHoldPMInfo info, string companyCode);

        /// <summary>
        /// 更新VendorPM Information
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        int EditVendorPMHoldInfo(int vendorSysNo, int holdMark, string editUser, string pMSysNoIn, string companyCode);

        #region For Invoice

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByVendorSysNo(int vendorSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByPOSysNo(int poSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo);

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorPMByPOSysNo(int poSysNo);

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo);

        #endregion For Invoice

        bool UpdateHoldReasonVendorPM(int sysNo, bool isHold, int holdUser, string holdReason);
        void UpdateVendorStatus(VendorApproveInfo vendorApproveInfo);
        void CreateEmptyVendorCustomsInfo(int? merchantSysNo, string userName);
        VendorCustomsInfo GetVendorCustomsInfo(int merchantSysNo);

        void CreateOrUpdateVendorCustomsInfo(VendorInfo info);

        /// <summary>
        /// 创建跨境供应商仓库
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        int CreateVendorStock(StockInfoForKJ stockInfo);

        List<AttachmentForApplyFor> LoadAttachmentForApplyForInfo(BizEntity.PO.VendorInfo vendorInfo);

        List<ApplyInfo> LoadApplyInfo(BizEntity.PO.VendorInfo vendorInfo);
    }
}
