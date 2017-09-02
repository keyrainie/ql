using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;
using System.Data;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/CallingLog/QuerySOList", Method = "POST")]
        public QueryResult QuerySOList(CustomerCallingQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICallingLogQueryDA>.Instance.QuerySOList(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/CallingLog/QueryRMAList", Method = "POST")]
        public QueryResult QueryRMAList(CustomerCallingQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICallingLogQueryDA>.Instance.QueryRMAList(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/CallingLog/QueryComplainList", Method = "POST")]
        public QueryResult QueryComplainList(CustomerCallingQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICallingLogQueryDA>.Instance.QueryComplainList(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/CallingLog/QueryCalling", Method = "POST")]
        public QueryResult QueryCalling(CustomerCallingQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICallingLogQueryDA>.Instance.QueryCallingLog(query, out totalCount),
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/CallingLog/GetRMARegisterList", Method = "POST")]
        public QueryResult GetRMARegisterList(RMARegisterQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ICallingLogQueryDA>.Instance.GetRMARegisterList(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/CallingLog/GetUpdateUser", Method = "POST")]
        public QueryResult GetUpdateUser(string companyCode)
        {
            DataTable dt = ObjectFactory<ICallingLogQueryDA>.Instance.GetUpdateUser(companyCode);
            return new QueryResult()
            {
                Data =dt,
                TotalCount = dt.Rows.Count
            };
        }


        [WebInvoke(UriTemplate = "/CallsEvents/QueryCallsEventLog", Method = "POST")]
        public QueryResult QueryCallsEventLog(CustomerCallsEventLogFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICallingLogQueryDA>.Instance.QueryCallsEventLog(query, out totalCount);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i]["ReasonCodeSysNo"] != DBNull.Value)
                {
                    int reasonCodeSysNo = (int)dataTable.Rows[i]["ReasonCodeSysNo"];
                    dataTable.Rows[i]["ReasonCodePath"] = ObjectFactory<CallsEventsAppService>.Instance.GetReasonCodePath(reasonCodeSysNo, query.CompanyCode);
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 创建顾客来电记录
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/CallsEvents/Create", Method = "POST")]
        public void CreateCallsEvents(CallsEvents request)
        {
            ObjectFactory<CallsEventsAppService>.Instance.Create(request);
        }
        /// <summary>
        /// 创建顾客来电记录跟进日志
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/CallsEvents/CreateCallsEventsFollowUpLog", Method = "POST")]
        public void CreateCallsEventsFollowUpLog(CallsEventsFollowUpLog request)
        {
            ObjectFactory<CallsEventsAppService>.Instance.CreateCallsEventsFollowUpLog(request);
        }


        [WebInvoke(UriTemplate = "/CallsEvents/CallingToComplain", Method = "POST")]
        public void CallingToComplain(SOComplaintCotentInfo request)
        {
            ObjectFactory<CallsEventsAppService>.Instance.CallingToComplaint(request);
        }

        [WebInvoke(UriTemplate = "/CallsEvents/CallingToRMA", Method = "POST")]
        public void CallingToRMA(InternalMemoInfo request)
        {
            ObjectFactory<CallsEventsAppService>.Instance.CallingToRMA(request);
        }

        [WebInvoke(UriTemplate = "/CallsEvents/Load/{SysNo}", Method = "GET")]
        public CallsEvents LoadCallsEvents(string sysno)
        {
            return ObjectFactory<CallsEventsAppService>.Instance.Load(int.Parse(sysno));
        }


    }
}
