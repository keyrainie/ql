using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.PO.Restful.ResponseMsg;
using ECCentral.Service.PO.AppService;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 关闭佣金
        /// </summary>
        /// <param name="commissionMaster"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Commission/CloseVendorCommission", Method = "PUT")]
        public CommissionMaster CloseCommission(CommissionMaster commissionMaster)
        {
            return ObjectFactory<CommissionAppService>.Instance.CloseCommission(commissionMaster);
        }

        /// <summary>
        /// 批量关闭佣金
        /// </summary>
        /// <param name="commissionSysNos"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Commission/BatchCloseVendorCommissions", Method = "PUT")]
        public int BatchCloseCommission(string commissionSysNos)
        {
            return ObjectFactory<CommissionAppService>.Instance.BatchCloseCommission(commissionSysNos);
        }

        /// <summary>
        /// 创建新的佣金规则
        /// </summary>
        /// <param name="newCommissionRule"></param>
        /// <returns></returns>
        public CommissionRule CreateCommission(CommissionRule newCommissionRule)
        {
            return newCommissionRule;
        }

        [WebInvoke(UriTemplate = "/Commission/QueryVendorCommissions", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public VendorCommissionRsp QueryCommissions(CommissionQueryFilter queryFilter)
        {
            int totalCount = 0;
            decimal totalAmt = 0;
            VendorCommissionRsp responseMsg = new VendorCommissionRsp()
            {
                Data = ObjectFactory<ICommissionQueryDA>.Instance.QueryCommission(queryFilter, out totalCount, out totalAmt)
            };
            responseMsg.TotalCount = totalCount;
            responseMsg.TotalAmt = totalAmt;
            return responseMsg;
        }

        [WebInvoke(UriTemplate = "/Commission/QueryVendorCommissionsTotalCount", Method = "POST")]
        public decimal QueryVendorCommissionsTotalCount(CommissionQueryFilter queryFilter)
        {
            return ObjectFactory<ICommissionQueryDA>.Instance.QueryCommissionTotalAmt(queryFilter);
        }
        /// <summary>
        /// 加载佣金信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Commission/LoadCommission/{sysNo}", Method = "GET")]
        public CommissionMaster LoadCommissionInfo(string sysNo)
        {
            int commissionSysNo = int.TryParse(sysNo, out commissionSysNo) ? commissionSysNo : 0;
            return ObjectFactory<CommissionAppService>.Instance.LoadCommissionMaseterInfo(commissionSysNo);
        }

        /// <summary>
        /// 创建结算
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Commission/CreateSettleCommission", Method = "PUT")]
        public CommissionMaster CreateSettleCommission(CommissionMaster req)
        {
            return ObjectFactory<CommissionAppService>.Instance.CreateSettleCommission(req);
        }

        //根据供应商获取未结算的Item
        /// <summary>
        /// 根据供应商获取未结算的Item
        /// </summary>
        /// <param name="merchantSysNo">结算供应商编号</param>
        /// <returns>待结算的项</returns>
        [WebInvoke(UriTemplate = "/Commission/GetManualCommissionMaster", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public CommissionMaster GetManualCommissionMaster(CommissionMaster req)
        {
            return ObjectFactory<CommissionAppService>.Instance.GetManualCommissionMaster(req);
        }

    }
}
