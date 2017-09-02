using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using Newegg.Oversea.Framework.Log;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess
{
    public class StatisticBiz
    {
        public void BatchInsertEventLog(List<EventLog> eventLogs)
        {
            if (eventLogs == null)
            {
                return;
            }

            var statisticDA = new StatisticDA();

            foreach (var log in eventLogs)
            {
                InsertEventLog(log);
            }
        }

        public void InsertEventLog(EventLog eventLog)
        {
            if (eventLog == null)
            {
                return;
            }

            var statisticDA = new StatisticDA();
            try
            {
                statisticDA.InsertEventLog(eventLog);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        public List<LoginStatisticItem> QueryLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            List<LoginStatisticItem> result = new List<LoginStatisticItem>();
            var da = new StatisticDA();
            List<LoginStatisticItem> items = da.QueryLoginStatistic(criteria);

            //由于数据库的数据没有连续的数据进行呈现，所以在没有数据的点，应该初始化成0
            DateTime startDate = criteria.DateFrom.HasValue ? criteria.DateFrom.Value : DateTime.MinValue;
            DateTime endDate = GetEndDate(criteria.DateTo);
            int days = (endDate.Date - startDate.Date).Days;
            if (startDate > endDate)
            {
                days = -1;
            }
            for (int i = 0; i <= days; i++)
            {
                LoginStatisticItem findResult = items.SingleOrDefault(item => item.InDate.Date.Equals(endDate.AddDays(-i).Date));
                if (findResult != null)
                {
                    result.Add(findResult);
                }
                else
                {
                    result.Add(new LoginStatisticItem
                    {
                        Count = 0,
                        InDate = endDate.AddDays(-i)
                    });
                }
            }
            return result;
        }

        public List<LoginStatisticItem> QueryUniqueLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            List<LoginStatisticItem> result = new List<LoginStatisticItem>();
            var da = new StatisticDA();
            //由于数据库的数据没有连续的数据进行呈现，所以在没有数据的点，应该初始化成0
            List<LoginStatisticItem> items = da.QueryUniqueLoginStatistic(criteria);
            DateTime startDate = criteria.DateFrom.HasValue ? criteria.DateFrom.Value : DateTime.Now;
            DateTime endDate = GetEndDate(criteria.DateTo);
            int days = (endDate.Date - startDate.Date).Days;
            if (startDate > endDate)
            {
                days = -1;
            }
            for (int i = 0; i <= days; i++)
            {
                LoginStatisticItem findResult = items.SingleOrDefault(item => item.InDate.Date.Equals(endDate.AddDays(-i).Date));
                if (findResult != null)
                {
                    result.Add(findResult);
                }
                else
                {
                    result.Add(new LoginStatisticItem
                    {
                        Count = 0,
                        InDate = endDate.AddDays(-i)
                    });
                }
            }
            return result;
        }

        private DateTime GetEndDate(DateTime? datetime)
        {
            if (datetime.HasValue && datetime.Value.Date >= DateTime.Now.Date)
            {
                return DateTime.Now.Date.AddDays(-1);
            }
            else if (datetime.HasValue)
            {
                return datetime.Value.Date;
            }
            return DateTime.Now.Date;
        }
    }
}
