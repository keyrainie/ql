using System.ServiceModel.Web;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Common;
using System;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 查询投诉
        /// </summary>
        /// <param name="filter">过滤查询条件</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryComplain", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryComplain(ComplainQueryFilter filter)
        {
            QueryResult result = new QueryResult();
            int dataCount = 0;
            result.Data = ObjectFactory<ISOQueryDA>.Instance.ComplainQuery(filter, out dataCount);
            var changer = ObjectFactory<SOInternalMemoAppService>.Instance;
            foreach (DataRow row in result.Data.Rows)
            {
                object reasonCodeSysNo = row["ReasonCodeSysNo"];
                if (reasonCodeSysNo is DBNull) continue;
                string reasonCodePath = changer.GetReasonCodePath(Convert.ToInt32(reasonCodeSysNo), filter.CompanyCode);
                row["ReasonCodePath"] = reasonCodePath;
            }
            result.TotalCount = dataCount;
            return result;
        }

        /// <summary>
        /// 创建投诉信息
        /// </summary>
        /// <param name="info">投诉实体</param>
        /// <returns>创建成功后的实体</returns>
        [WebInvoke(UriTemplate = "/SO/CreateComplain", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SOComplaintInfo CreateComplain(SOComplaintCotentInfo info)
        {
            SOComplaintInfo soComplaintInfo = null;
            if (info != null)
            {
                soComplaintInfo = ObjectFactory<SOComplainAppService>.Instance.AddSOComplaintInfo(info);
            }
            return soComplaintInfo;
        }

        /// <summary>
        /// 更新投诉信息
        /// </summary>
        /// <param name="info">投诉实体</param>
        /// <returns>更新成功后的实体</returns>
        [WebInvoke(UriTemplate = "/SO/UpdateComplain", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public SOComplaintInfo UpdateComplain(SOComplaintInfo info)
        {
            if (info != null && info.ComplaintCotentInfo != null && info.ProcessInfo != null)
            {
                info = ObjectFactory<SOComplainAppService>.Instance.UpdateSOComplaintInfo(info);
            }
            return info;
        }

        /// <summary>
        /// 获取投诉信息
        /// </summary>
        /// <param name="sysNo">投诉单号</param>
        /// <returns>投诉信息</returns>
        [WebInvoke(UriTemplate = "/SO/Complain/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public SOComplaintInfo Get(string sysNo)
        {
            int tSysNo = int.TryParse(sysNo, out tSysNo) ? tSysNo : 0;
            return ObjectFactory<SOComplainAppService>.Instance.GetInfo(tSysNo);
        }

        /// <summary>
        /// 批量指派投诉信息给相应处理人
        /// </summary>
        /// <param name="infoList">请求实体</param>
        [WebInvoke(UriTemplate = "/SO/BathAssignSOComplaintInfo", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void BathAssignSOComplaintInfo(List<SOComplaintProcessInfo> infoList)
        {
            ObjectFactory<SOComplainAppService>.Instance.BatchAssignSOComplaintInfo(infoList);               
        }

        /// <summary>
        /// 批量取消指派投诉信息给相应处理人
        /// </summary>
        /// <param name="infoList">请求实体</param>
        [WebInvoke(UriTemplate = "/SO/BathCancelAssignSOComplaintInfo", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void BathCancelAssignSOComplaintInfo(List<SOComplaintProcessInfo> infoList)
        {
            ObjectFactory<SOComplainAppService>.Instance.BathCancelAssignSOComplaintInfo(infoList);          
        }

        /// <summary>
        /// 获取商品所在Domain的简单信息
        /// </summary>
        /// <param name="productID">商品编号</param>
        /// <returns>所在Domain的简单信息</returns>
        [WebInvoke(UriTemplate = "/SO/GetProductDomain/{productID}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public ProductDomain GetProductDomain(string productID)
        {
            return ObjectFactory<SOComplainAppService>.Instance.GetProductDomain(productID);
        }

        /// <summary>
        /// 发送回复邮件通知
        /// </summary>
        /// <param name="complainSysNo">投诉编号</param>
        [WebInvoke(UriTemplate = "/SO/SendReplyEmail/{complainSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public void SendReplyEmail(string complainSysNo)
        {
            int tSysNo = int.TryParse(complainSysNo, out tSysNo) ? tSysNo : 0;
            ObjectFactory<SOComplainAppService>.Instance.SendMail(tSysNo);
        }
    }
}
