using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.IDataAccess;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        /// <summary>
        /// 查询补偿退款单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/Query", Method = "POST")]
        public QueryResult RefundAdjustQuery(RefundAdjustQueryFilter filter)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IRefundAdjustQueryDA>.Instance.RefundAdjustQuery(filter, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 节能补贴查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/EnergySubsidyQuery", Method = "POST")]
        public QueryResult EnergySubsidyQuery(RefundAdjustQueryFilter filter)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IRefundAdjustQueryDA>.Instance.QueryEnergySubsidy(filter, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 节能补贴Excel导出
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/EnergySubsidyExport", Method = "POST")]
        public QueryResult EnergySubsidyExportQuery(RefundAdjustQueryFilter filter)
        {
            return new QueryResult()
            {
                Data = ObjectFactory<IRefundAdjustQueryDA>.Instance.QueryEnergySubsidyExport(filter),
                //仅用于导出，不代表实际数量
                TotalCount = 1
            };
        }

        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/Create", Method = "POST")]
        public void RefundAdjustCreate(RefundAdjustInfo entity)
        {
            ObjectFactory<RefundAdjustAppService>.Instance.RefundAdjustCreate(entity);
        }

        /// <summary>
        /// 更新补偿退款单
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/Update", Method = "PUT")]
        public Boolean RefundAdjustUpdate(string refundSysNo)
        {
            int RefundSysNo = int.TryParse(refundSysNo, out RefundSysNo) ? RefundSysNo : 0;
            return true;
        }

        /// <summary>
        /// 修改补偿退款单的状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/RefundAdjustUpdateStatus", Method = "PUT")]
        public Boolean RefundAdjustUpdateStatus(RefundAdjustInfo entity)
        {
            return true;
        }

        /// <summary>
        /// 批量修改补偿退款单的状态
        /// </summary>
        /// <param name="entitys"></param>
        [WebInvoke(UriTemplate = "/RefundAdjust/BatchUpdateRefundAdjustStatus", Method = "PUT")]
        public void BatchUpdateRefundAdjustStatus(List<RefundAdjustInfo> entitys)
        {
            ObjectFactory<RefundAdjustAppService>.Instance.BatchUpdateRefundAdjustStatus(entitys);
        }

        /// <summary>
        /// 根据订单号获取补偿退款单相关信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/GetRefundAdjustBySoSysNo", Method = "POST")]
        public RefundAdjustInfo GetRefundAdjustByRequestID(RefundAdjustInfo entity)
        {
            return ObjectFactory<RefundAdjustAppService>.Instance.GetInfoBySOSysNo(entity);
        }

        /// <summary>
        /// 获取节能补贴详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RefundAdjust/GetEnergySubsidyDetails", Method = "POST")]
        public List<EnergySubsidyInfo> GetEnergySubsidyDetailst(EnergySubsidyInfo entity)
        {
            return ObjectFactory<RefundAdjustAppService>.Instance.GetEnergySubsidyDetails(entity);
        }
    }
}
