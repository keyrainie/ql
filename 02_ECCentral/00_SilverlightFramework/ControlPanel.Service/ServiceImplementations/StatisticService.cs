using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.ServiceModel.Web;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [RestService]
    public class StatisticService
    {
        [WebInvoke(UriTemplate = "BatchTraceEventLog", Method = "POST")]
        public void BatchTraceEventLog(List<EventLog> eventLogs)
        {
            var biz = new StatisticBiz();
            biz.BatchInsertEventLog(eventLogs);
        }

        [WebInvoke(UriTemplate = "TraceEventLog", Method = "POST")]
        public void TraceEventLog(EventLog eventLog)
        {
            var da = new StatisticDA();
            da.InsertEventLog(eventLog);
        }

        [WebInvoke(UriTemplate = "QueryEventLog", Method = "POST")]
        public List<EventLogView> QueryEventLog(QueryEventLogCriteria criteria)
        {
            var da = new StatisticDA();
            List<EventLogView> list = da.QueryEventLog(criteria);
            if (list != null)
            {
                foreach (EventLogView item in list)
                {
                    UserInfo u  = CPContext.GetUserInfoFromAD(item.UserID, CPContext.GetADDomain());
                    if (u != null && !string.IsNullOrWhiteSpace(u.FullName))
                    {
                        item.UserID = string.Format("{0} - {1} ({2})", u.UserID, u.FullName, u.Department);
                    }
                }
            }
            return list;
        }

        [WebInvoke(UriTemplate = "QueryLoginStatistic", Method = "POST")]
        public List<LoginStatisticItem> QueryLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            var da = new StatisticBiz();
            return da.QueryLoginStatistic(criteria);
        }

        [WebInvoke(UriTemplate = "QueryUniqueLoginStatistic", Method = "POST")]
        public List<LoginStatisticItem> QueryUniqueLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            var da = new StatisticBiz();
            return da.QueryUniqueLoginStatistic(criteria);
        }

        [WebInvoke(UriTemplate = "QueryPVStatistic", Method = "POST")]
        public List<PVStatisticItem> QueryPVStatistic(QueryPVStatisticCriteria criteria)
        {
            var da = new StatisticDA();
            return da.QueryPVStatistic(criteria);
        }

        [WebInvoke(UriTemplate = "QueryUserPVStatistic", Method = "POST")]
        public List<UserPVStatisticItem> QueryUserPVStatistic(QueryUserPVStatisticCriteria criteria)
        {
            var da = new StatisticDA();
            List<UserPVStatisticItem> list =  da.QueryUserPVStatistic(criteria);
            if (list != null)
            {
                foreach (UserPVStatisticItem item in list)
                {
                    UserInfo u = CPContext.GetUserInfoFromAD(item.UserId, CPContext.GetADDomain());
                    if (u != null && !string.IsNullOrWhiteSpace(u.FullName))
                    {
                        item.UserId = string.Format("{0} - {1} ({2})", u.UserID, u.FullName, u.Department);
                    }
                }
            }
            return list;
        }

        [WebInvoke(UriTemplate = "QueryActionStatistic", Method = "POST")]
        public List<ActionStatisticItem> QueryActionStatistic(QueryActionStatisticCriteria criteria)
        {
            var da = new StatisticDA();
            return da.QueryActionStatistic(criteria);
        }
    }
}
