using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.BizEntity.Common;
using ECCentral.Service.RMA.Restful.RequestMsg;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        #region RMA跟进日志相关处理人员
        /// <summary>
        /// 获取RMA日志创建者
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RMATrackingCreateUsers/GetAll", Method = "GET")]
        public List<UserInfo> GetRMATrackingCreateUsers()
        {
            return ObjectFactory<IRMATrackingQueryDA>.Instance.GetRMATrackingCreateUsers();
        }
        /// <summary>
        /// 获取RMA日志处理者
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RMATrackingUpdateUsers/GetAll", Method = "GET")]
        public List<UserInfo> GetRMATrackingUpdateUsers()
        {
            return ObjectFactory<IRMATrackingQueryDA>.Instance.GetRMATrackingUpdateUsers();
        }
        /// <summary>
        /// 获取RMA日志派发人员
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RMATrackingHandleUsers/GetAll", Method = "GET")]
        public List<UserInfo> GetRMATrackingHandleUsers()
        {
            return ObjectFactory<IRMATrackingQueryDA>.Instance.GetRMATrackingHandleUsers();
        }

        #endregion
        [WebInvoke(UriTemplate = "/RMATracking/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryRMATracking(RMATrackingQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IRMATrackingQueryDA>.Instance.QueryRMATracking(request, out totalCount);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i]["ReasonCodeSysNo"] != DBNull.Value)
                {
                    int reasonCodeSysNo = (int)dataTable.Rows[i]["ReasonCodeSysNo"];

                    dataTable.Rows[i]["ReasonCodePath"] = ObjectFactory<RMATrackingAppService>.Instance.GetReasonCodePath(reasonCodeSysNo, request.CompanyCode);
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 派发RMA跟进日志
        /// </summary>
        [WebInvoke(UriTemplate = "/RMATracking/Dispatch", Method = "PUT")]
        public void Dispatch(RMATrackingBatchActionReq msg)
        {
            ObjectFactory<RMATrackingAppService>.Instance.DispatchRMATracking(msg.SysNoList, msg.HandlerSysNo.Value);
        }

        /// <summary>
        /// 取消派发RMA跟进日志
        /// </summary>
        [WebInvoke(UriTemplate = "/RMATracking/CancelDispatch", Method = "PUT")]
        public void CancelDispatch(RMATrackingBatchActionReq msg)
        {
            ObjectFactory<RMATrackingAppService>.Instance.CancelDispatchRMATracking(msg.SysNoList);
        }

        /// <summary>
        /// 关闭跟踪单
        /// </summary>
        [WebInvoke(UriTemplate = "/RMATracking/Close", Method = "PUT")]
        public void CloseRMATracking(InternalMemoInfo msg)
        {
            ObjectFactory<RMATrackingAppService>.Instance.CloseRMATracking(msg);
        }

        /// <summary>
        /// 创建跟踪信息
        /// </summary>
        [WebInvoke(UriTemplate = "/RMATracking/Create", Method = "PUT")]
        public InternalMemoInfo CreateRMATracking(InternalMemoInfo msg)
        {
            return ObjectFactory<RMATrackingAppService>.Instance.CreateRMATracking(msg);
        }

    }
}
