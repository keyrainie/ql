using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.CommonDomain.QueryLogService;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models
{
    public class LogQueryModel : ModelBase
    {
        private string m_globalID;
        private string m_localID;
        private string m_categoryName;
        private string m_logType;
        private string m_createDateType = "L3";
        private DateTime? m_createDateFrom;
        private DateTime? m_createDateTo;
        private string m_logID;
        private string m_referenceKey;

        public string GlobalID
        {
            get { return m_globalID; }
            set
            {
                this.SetValue("GlobalID", ref m_globalID, value);
            }
        }

        public string LocalID
        {
            get { return m_localID; }
            set
            {
                this.SetValue("LocalID", ref m_localID, value);
            }
        }

        public string CategoryName
        {
            get { return m_categoryName; }
            set
            {
                this.SetValue("CategoryName", ref m_categoryName, value);
            }
        }

        public string LogType
        {
            get { return m_logType; }
            set
            {
                this.SetValue("LogType", ref m_logType, value);
            }
        }

        public string CreateDateType
        {
            get { return m_createDateType; }
            set
            {
                this.SetValue("CreateDateType", ref m_createDateType, value);
            }
        }

        public DateTime? CreateDateFrom
        {
            get { return m_createDateFrom; }
            set
            {
                this.SetValue("CreateDateFrom", ref m_createDateFrom, value);
            }
        }

        public DateTime? CreateDateTo
        {
            get { return m_createDateTo; }
            set
            {
                this.SetValue("CreateDateTo", ref m_createDateTo, value);
            }
        }

        public string LogID
        {
            get { return m_logID; }
            set
            {
                this.SetValue("LogID", ref m_logID, value);
            }
        }

        public string ReferenceKey
        {
            get { return m_referenceKey; }
            set
            {
                this.SetValue("ReferenceKey", ref m_referenceKey, value);
            }
        }
    }

    public static class ModelTransformer
    {
        public static string GetValue(this string str)
        {
            string result = str == null ? null : str.Trim();

            if (result == String.Empty)
            {
                result = null;
            }

            return result;
        }

        public static QueryLogService.LogType? GetLogType(this string strType)
        {
            QueryLogService.LogType? logType = null;

            int val;
            if (int.TryParse(strType, out val))
            {
                switch (val)
                {
                    case (int)QueryLogService.LogType.Audit:
                        logType = QueryLogService.LogType.Audit;
                        break;
                    case (int)QueryLogService.LogType.Debug:
                        logType = QueryLogService.LogType.Debug;
                        break;
                    case (int)QueryLogService.LogType.Error:
                        logType = QueryLogService.LogType.Error;
                        break;
                    case (int)QueryLogService.LogType.Info:
                        logType = QueryLogService.LogType.Info;
                        break;
                    case (int)QueryLogService.LogType.Trace:
                        logType = QueryLogService.LogType.Trace;
                        break;
                }
            }

            return logType;
        }

        public static LogQueryCriteria ToQueryCriteria(this LogQueryModel queryModel)
        {
            LogQueryCriteria queryCriteria = null;

            if (queryModel != null)
            {
                queryCriteria = new LogQueryCriteria()
                {
                    GlobalID = queryModel.GlobalID.GetValue(),
                    LocalID = queryModel.LocalID.GetValue(),
                    CategoryName = queryModel.CategoryName.GetValue(),
                    LogType = queryModel.LogType.GetLogType(),
                    From = queryModel.CreateDateFrom,
                    To = queryModel.CreateDateTo,
                    LogID = queryModel.LogID.GetValue(),
                    ReferenceKey = queryModel.ReferenceKey.GetValue(),
                };
            }

            return queryCriteria;
        }
    }
}
