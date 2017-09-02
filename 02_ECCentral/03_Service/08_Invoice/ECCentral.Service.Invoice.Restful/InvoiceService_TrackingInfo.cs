using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        #region TrackingInfo

        /// <summary>
        /// 批量创建跟踪单
        /// </summary>
        [WebInvoke(UriTemplate = "/TrackingInfo/BatchCreate", Method = "POST")]
        public string BatchCreateTrackingInfo(List<TrackingInfo> trackingInfoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchCreateTrackingInfo(trackingInfoList);
        }

        /// <summary>
        /// 更新跟踪单
        /// </summary>
        [WebInvoke(UriTemplate = "/TrackingInfo/Update", Method = "PUT")]
        public void UpdateTrackingInfo(TrackingInfo entity)
        {
            ObjectFactory<TrackingInfoAppService>.Instance.UpdateTrackingInfo(entity);
        }

        /// <summary>
        /// 批量报损跟踪单
        /// </summary>
        [WebInvoke(UriTemplate = "/TrackingInfo/BatchSubmit", Method = "PUT")]
        public string BatchSubmitTrackingInfo(List<int> sysNoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchSubmitTrackingInfo(sysNoList);
        }

        /// <summary>
        /// 批量关闭跟踪单
        /// </summary>
        [WebInvoke(UriTemplate = "/TrackingInfo/BatchClose", Method = "PUT")]
        public string BatchCloseTrackingInfo(List<int> sysNoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchCloseTrackingInfo(sysNoList);
        }

        /// <summary>
        /// 批量设置跟踪单责任人姓名
        /// </summary>
        /// <param name="trackingInfoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/TrackingInfo/BatchUpdateResponsibleUser", Method = "PUT")]
        public string BatchUpdateTrackingInfoResponsibleUser(List<TrackingInfo> trackingInfoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchUpdateTrackingInfoResponsibleUserName(trackingInfoList);
        }

        /// <summary>
        /// 批量设置跟踪单损失类型
        /// </summary>
        /// <param name="trackingInfoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/TrackingInfo/BatchUpdateTrackingInfoLossType", Method = "PUT")]
        public string BatchUpdateTrackingInfoLossType(List<TrackingInfo> trackingInfoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchUpdateTrackingInfoLossType(trackingInfoList);
        }

        #endregion TrackingInfo

        #region ResponsibleUser

        [WebInvoke(UriTemplate = "/ResponsibleUser/Create", Method = "POST")]
        public void CreateOrUpdateResponsibleUser(CreateResponsibleUserReq request)
        {
            ObjectFactory<TrackingInfoAppService>.Instance.CreateOrUpdateResponsibleUser(request.ResponsibleUser, request.OverrideWhenCreate.Value);
        }

        [WebInvoke(UriTemplate = "/ResponsibleUser/Exists", Method = "POST")]
        public bool ExistedResponsibleUser(ResponsibleUserInfo request)
        {
            return (ObjectFactory<TrackingInfoAppService>.Instance.GetExistedResponsibleUser(request) != null);
        }

        [WebInvoke(UriTemplate = "/ResponsibleUser/BatchAbandon", Method = "PUT")]
        public string BatchAbandonResponsibleUser(List<int> sysNoList)
        {
            return ObjectFactory<TrackingInfoAppService>.Instance.BatchAbandonResponsibleUser(sysNoList);
        }

        #endregion ResponsibleUser

        #region NoBizQuery

        /// <summary>
        /// 查询跟踪单
        /// </summary>
        /// <param name="request">跟踪单查询条件</param>
        /// <returns>跟踪单查询结果</returns>
        [WebInvoke(UriTemplate = "/TrackingInfo/Query", Method = "POST")]
        public virtual QueryResultList QueryTrackingInfo(TrackingInfoQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ITrackingInfoQueryDA>.Instance.QueryTrackingInfo(request, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["DataResult"], TotalCount = totalCount},
                new QueryResult(){ Data = dataSet.Tables["StatisticResult"]}
            };
        }

        /// <summary>
        /// 导出跟踪单
        /// </summary>
        /// <param name="request">跟踪单查询条件</param>
        /// <returns>跟踪单查询结果</returns>
        [WebInvoke(UriTemplate = "/TrackingInfo/Export", Method = "POST")]
        public virtual QueryResult ExportTrackingInfo(TrackingInfoQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ITrackingInfoQueryDA>.Instance.QueryTrackingInfo(request, out totalCount);
            var dataTable = dataSet.Tables["DataResult"];
            dataTable.Columns.Add("EditInfo");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                if (!row.IsNull("EditUser") && !string.IsNullOrWhiteSpace(row["EditUser"].ToString()))
                {
                    row["EditInfo"] = string.Format("{0}\r\n{1}", row["EditUser"].ToString().Split('\\')[0], ((DateTime)row["EditDate"]).ToString(InvoiceConst.StringFormat.LongDateFormat));
                }
                else
                {
                    row["EditInfo"] = DBNull.Value;
                }
            }
            return new QueryResult()
            {
                Data = dataTable
                ,TotalCount=dataTable.Rows.Count
            };
        }

        /// <summary>
        /// 查询跟踪单责任人
        /// </summary>
        /// <param name="request">跟踪单责任人查询条件</param>
        /// <returns>跟踪单责任人查询结果</returns>
        [WebInvoke(UriTemplate = "/ResponsibleUser/Query", Method = "POST")]
        public virtual QueryResult QueryResponsibleUser(ResponsibleUserQueryFilter request)
        {
            int totalCount;
            var table = ObjectFactory<ITrackingInfoQueryDA>.Instance.QueryResponsibleUser(request, out totalCount);
            return new QueryResult()
            {
                Data = table,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 手动创建跟踪单时用来查询单据信息
        /// </summary>
        /// <param name="request">单据查询条件</param>
        /// <returns>单据查询结果</returns>
        [WebInvoke(UriTemplate = "/TrackingInfo/QueryOrder", Method = "POST")]
        public virtual QueryResult QueryOrder(OrderQueryFilter request)
        {
            int totalCount;
            var table = ObjectFactory<ITrackingInfoQueryDA>.Instance.QueryOrder(request, out totalCount);
            return new QueryResult()
            {
                Data = table,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取所有的跟踪单责任人
        /// </summary>
        /// <returns>所有的跟踪单责任人列表</returns>
        [WebInvoke(UriTemplate = "/ResponsibleUser/GetAll/{companyCode}", Method = "GET")]
        public List<CodeNamePair> GetAllResponsibleUser(string companyCode)
        {
            return ObjectFactory<ITrackingInfoQueryDA>.Instance.GetResponsibleUsers(companyCode);
        }

        #endregion NoBizQuery
    }
}