using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.IDataAccess
{
    public partial interface ISODA
    {
        #region 团购订单相关

        /// <summary>
        /// 根据团购编号取得所有订单中的团购商品列表
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <returns></returns>
        List<SOItemInfo> GetGroupBuySOItemByGroupBuySysNo(int groupBuySysNo);

        /// <summary>
        /// 根据团购编号修改所有团购订单的商品处理状态
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        void UpdateGroupBuySOItemSettlementStatusByGroupBuySysNo(int groupBuySysNo, SettlementStatus settlementStatus);

        /// <summary>
        /// 根据团购编号修改所有团购订单的处理状态
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        void UpdateGroupBuySOSettlementStatusByGroupBuySysNo(int groupBuySysNo, SettlementStatus settlementStatus);

        /// <summary>
        /// 修改团购订单的价格信息
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        void UpdateGroupBuySOAmount(SOBaseInfo baseInfo);
        /// <summary>
        /// 修改团购商品价格
        /// </summary>
        void UpdateGroupBuyProduct(SOItemInfo product);

        /// <summary>
        /// 根据订单编号和团购商品编号修改团购订单和其团购商品处理状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="productSysNo">订单中的团购商品编号</param>
        /// <param name="settlementStatus">处理状态</param>
        void UpdateGroupBuySOAndItemSettlementStatus(int soSysNo, int productSysNo, SettlementStatus settlementStatus);

        /// <summary>
        /// 根据订单编号修改团购订单处理状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="settlementStatus"></param>
        void UpdateGroupBuySOSettlementStatusBySOSysNo(int soSysNo, SettlementStatus settlementStatus);

        /// <summary>
        /// 取得无效的团购订单编号
        /// </summary>
        /// <returns></returns>
        List<int> GetInvalidGroupBuySOSysNoList(string companyCode);
        /// <summary>
        /// 取得48小时内没有支付的团购订单
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<int> GetNotPayGroupBuySOSysNoList(string companyCode);
        #endregion

        #region FPCheck

        /// <summary>
        /// 获取待检验的FP列表
        /// </summary>
        /// <param name="totalCount">最大获取数据行数</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>FPCheck订单列表</returns>
        List<SOInfo> GetFPCheckSOList(int totalCount, string companyCode);

        /// <summary>
        /// 获取一段时间内物流拒收的订单
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns></returns>
        List<SOInfo> GetAutoRMASOInfoListInTime(string companyCode);

        /// <summary>
        /// 是否恶意用户
        /// </summary>
        /// <param name="CustomerSysNo">用户编号</param>
        /// <returns>是恶意用户返回真，否则返回假</returns>
        bool IsSpiteCustomer(int customerSysNo, string companyCode);

        /// <summary>
        /// 是否是新用户
        /// </summary>
        /// <param name="customerSysNo">用户编号</param>
        /// <returns>是返回真，旧用户返回假</returns>
        bool IsNewCustom(int customerSysNo);

        /// <summary>
        /// 判断是否是拒收用户
        /// </summary>
        /// <param name="customSysNo">用户编号</param>
        /// <param name="addr">地址</param>
        /// <param name="cellPhone">手机</param>
        /// <param name="phone">座机</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>是拒收用户返回真，失败返回假</returns>
        bool IsRejectionCustomer(int? customSysNo, string addr, string cellPhone, string phone, string companyCode);

        /// <summary>
        /// 判断是否有过恶意占库存记录
        /// </summary>
        /// <param name="customSysNo">用户编号</param>
        /// <param name="addr">地址</param>
        /// <param name="cellPhone">手机</param>
        /// <param name="phone">座机</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>是恶意占用用户返回真，失败返回假</returns>
        bool IsOccupyStockCustomer(int? customSysNo, string addr, string cellPhone, string phone, string companyCode);

        /// <summary>
        /// 是否是新用户拒收或者是用户拒收订单的比例超过限度
        /// </summary>
        /// <param name="CustomerSysNo">用户编号</param>
        /// <returns>有数据返回假，无返回真</returns>
        bool IsNewRejectionCustomerB(int CustomerSysNo);

        /// <summary>
        /// 统计一天内统一用户不同订单地址
        /// </summary>
        /// <param name="customerSysNo">用户编号</param>
        /// <returns>不同订单地址</returns>
        int GetSOCount4OneDay(int customerSysNo);

        bool IsNewOccupyStockCustomerA(int CustomerSysNo);

        bool IsNewOccupyStockCustomerB(int CustomerSysNo);

        /// <summary>
        /// 根据ProductSysNo获取串货SO订单列表
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="customerIPAddress">用户IP地址</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>串货SO订单列表</returns>
        List<SOInfo> GetChuanHuoSOListByProduct(int productSysNo, string customerIPAddress, DateTime createTime, string companyCode);

        /// <summary>
        /// 根据C3SysNo获取串货SO订单列表
        /// </summary>
        /// <param name="c3No">3级分类编号</param>
        /// <param name="customerIPAddress">用户IP地址</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>串货SO订单列表</returns>
        List<SOInfo> GetChuanHuoSOListByC3(int c3No, string customerIPAddress, DateTime createTime, string companyCode);

        /// <summary>
        /// 根据商品编号获取SO重复订单列表
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="customerSysNo">用户编号</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="CompanyCode">公司编码</param>
        /// <returns>SO重复订单列表</returns>
        List<SOInfo> GetDuplicatSOList(int productSysNo, int customerSysNo, DateTime createTime, string companyCode);

        /// <summary>
        /// 更新IsDuplicateOrder字段
        /// </summary>
        /// <param name="duplicateSOSysNo">重复订单编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="soSysNos">要更新的订单编号</param>
        void UpdateMarkException(string duplicateSOSysNo, int productSysNo, string soSysNos);

        /// <summary>
        /// 获取炒货订单列表
        /// </summary>
        /// <param name="receiveCellPhone">收货人手机</param>
        /// <param name="receivePhone">收货人电话</param>
        /// <param name="hours"></param>
        /// <param name="orderDatetime">订单创建时间</param>
        /// <param name="pointPromotionFlag"></param>
        /// <param name="shipPriceFlag"></param>
        /// <param name="isVATFlag"></param>
        /// <param name="companyCode"></param>
        /// <returns>炒货订单列表</returns>
        List<SOInfo> GetChaoHuoSOList(string receiveCellPhone, string receivePhone, int hours, DateTime orderDatetime, int? pointPromotionFlag, int? shipPriceFlag, int? isVATFlag, string companyCode);

        /// <summary>
        /// 更新FP状态
        /// </summary>
        /// <param name="soSysNos">多个字符串形式订单编号</param>
        /// <param name="isFPSO">是否是FP订单</param>
        /// <param name="fpReason">FP原因</param>
        /// <param name="isMarkRed">是否需要编辑为红色</param>
        void UpdateMarkFPStatus(string soSysNos, int isFPSO, string fpReason, bool isMarkRed);

        /// <summary>
        /// 统计不在本地仓的SOItem
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="localWH">本地仓库编码</param>
        /// <returns>统计数</returns>
        int CountNotLocalWHSOItem(int soSysNo, string localWH);

        /// <summary>
        /// 标记LocalWH
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="localWH">本地仓库编号</param>
        /// <param name="targetStatus">仓库状态</param>
        void UpdateLocalWHMark(int soSysNo, string localWH, int targetStatus);

        #endregion

        #region 审核订单通过发送邮件和短信以及更新数据库

        //审核通过后发送邮件和短信
        void AuditSendMailAndUpdateSO(int SOSysNo);

        //电子卡订单出库发送shipping消息
        void CreateEGiftCardOrderInvoice(int soSysNo);

        //更新操作
        int UpdateSOPassAutoAuditSendMessage(int soSysNo);

        #endregion

        #region CPSSend
        
        /// <summary>
        /// 获取CPS相关订单
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>订单列表</returns>
        List<SOInfo> GetCPSList(string companyCode);

        /// <summary>
        /// 新增数据到CPSSendLog
        /// </summary>
        /// <param name="soNumber">订单编号</param>
        /// <param name="targetUrl">目标值</param>
        /// <param name="returnMsg">返回信息</param>
        /// <param name="returnReceived">返回的状态描述</param>
        void InsertCPSLog(int soNumber, string targetUrl, string returnMsg, int returnReceived);

        #endregion

        #region InternalMemoReport
        
        /// <summary>
        /// 查询订单追踪创建者人员列表
        /// </summary>
        /// <param name="startDate">查询开始时间</param>
        /// <param name="endDate">查询结束时间</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns>订单追踪创建者人员列表</returns>
        List<SOInternalMemoInfo> GetInternalMemoReportList(DateTime startDate, DateTime endDate, string companyCode);

        #endregion

        #region AutoAudioSO

        /// <summary>
        /// 获取自动审核单
        /// </summary>
        /// <param name="topCount">最大获取数</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns>自动审核单</returns>
        List<SOInfo> GetSOList4Audit(int topCount, string companyCode);

        /// <summary>
        /// 更新自动审单审单类型
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="auditType">审核类型：手动，自动</param>
        /// <param name="autoAuditMemo">备注</param>
        void UpdateCheckShippingAuditTypeBySysNo(int soSysNo, AuditType auditType, string autoAuditMemo);

        /// <summary>
        /// 更新审单人
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="userSysNo">审核人编号</param>
        void UpdateSO4AuditUserInfo(int soSysNo, int userSysNo);

        /// <summary>
        /// 更新审单人（订单状态为待审核）
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="userSysNo">审核人编号</param>
        /// <returns>成功返回真，否则返回假</returns>
        bool UpdateSO4PassAutoAudit(int soSysNo, int userSysNo);

        #endregion
    }
}
