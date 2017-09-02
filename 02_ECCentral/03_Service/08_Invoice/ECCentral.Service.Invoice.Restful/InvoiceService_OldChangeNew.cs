using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        #region NoBizQuery
        /// <summary>
        /// 以旧换新补贴款查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <returns>结果集</returns>
        [WebInvoke(UriTemplate = "/Invoice/QueryOldChangeNew", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult OldChangeNewQuery(OldChangeNewQueryFilter filter)
        {
            int totalCount = 0;
            return new QueryResult()
            {
                Data = ObjectFactory<IOldChangeNewQueryDA>.Instance.OldChangeNewQuery(filter, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取以旧换新补贴款列表信息
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <returns>结果集</returns>
        [WebInvoke(UriTemplate = "/Invoice/GetOldChangeNewList", Method = "POST")]
        public List<OldChangeNewInfo> GetOldChangeNewList(OldChangeNewQueryFilter filter)
        {
            return ObjectFactory<IOldChangeNewQueryDA>.Instance.GetOldChangeNewList(filter);
        }

        /// <summary>
        /// Check以旧换新信息是否有效
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/IsOldChangeNewSO", Method = "POST")]
        public bool IsOldChangeNewSO(OldChangeNewQueryFilter filter)
        {
            return ObjectFactory<IOldChangeNewQueryDA>.Instance.IsOldChangeNewSO(filter);
        }
        #endregion

        #region Action
        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        [WebInvoke(UriTemplate = "/Invoice/Create", Method = "POST")]
        public OldChangeNewInfo Create(OldChangeNewInfo info)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.Create(info);
        }

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/UpdateOldChangeNewRebate", Method = "PUT")]
        public OldChangeNewInfo UpdateOldChangeNewRebate(OldChangeNewInfo info)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.UpdateOldChangeNewRebate(info);
        }

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/UpdateOldChangeNewStatus", Method = "PUT")]
        public OldChangeNewInfo UpdateOldChangeNewStatus(OldChangeNewInfo info)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.UpdateOldChangeNewStatus(info);
        }

        /// <summary>
        /// 批量更新以旧换新状态信息
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/BtachUpdateOldChangeNewStatus", Method = "PUT")]
        public string BtachUpdateOldChangeNewStatus(List<OldChangeNewInfo> infos)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.BatchUpdateOldChangeNewStatus(infos);
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/BtachMaintainReferenceID", Method = "PUT")]
        public string MaintainReferenceID(List<OldChangeNewInfo> infos)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.BatchMaintainReferenceID(infos);
        }

        /// <summary>
        /// 添加财务备注
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/MaintainStatusWithNote",Method="PUT")]
        public OldChangeNewInfo MaintainStatusWithNote(OldChangeNewInfo info)
        {
            return ObjectFactory<OldChangeNewAppService>.Instance.MaintainStatusWithNote(info);
        }
        #endregion
    }
}
