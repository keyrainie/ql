using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface ICommissionDA
    {

        /// <summary>
        /// 加载佣金主信息
        /// </summary>
        /// <param name="commissionSysNo"></param>
        /// <returns></returns>
        CommissionMaster LoadCommissionMaster(int commissionSysNo);

        /// <summary>
        /// 加载佣金Item信息
        /// </summary>
        /// <param name="commissionSysNo"></param>
        /// <returns></returns>
        List<CommissionItem> LoadCommissionItems(int commissionSysNo);

        //加载佣金详细信息
        List<CommissionItemDetail> LoadCommissionItemDetails(int commissionSysNo, VendorCommissionItemType type);

        /// <summary>
        /// 创建新的佣金规则
        /// </summary>
        /// <param name="newCommissionRule"></param>
        /// <returns></returns>
        CommissionRule CreateCommission(CommissionRule newCommissionRule);

        /// <summary>
        /// 添加佣金主表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        void InsertCommissionMaster(CommissionMaster req);

        /// <summary>
        /// 添加佣金项
        /// </summary>
        /// <param name="req">主体请求体</param>
        void InsertCommissionItems(CommissionMaster req);

        /// <summary>
        /// 添加到详细信息数据
        /// </summary>
        /// <param name="req"></param>
        void InsertCommissionDetail(CommissionItemDetail req, string companyCode, VendorCommissionItemType type);

        /// <summary>
        /// 关闭佣金规则
        /// </summary>
        /// <param name="commissionMaster"></param>
        /// <returns></returns>
        int CloseCommission(CommissionMaster commissionMaster);

        List<CommissionItemDetail> QueryCommissionItemDetails(int vendorSysNo, DateTime startDate, DateTime endDate, string companyCode);

        List<VendorCommissionInfo> QueryCommissionRuleByMerchantSysNo(int vendorManufacturerSysNo);

        CommissionItem QueryVendorManufacturerBySysNo(int vendorManufacturerSysNo);
    }
}
