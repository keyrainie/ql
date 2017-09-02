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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models
{
    public class CategoryQueryModel:ModelBase
    {
        private string m_globalID;
        private string m_localID;
        private string m_categoryName;
        private string m_logType;
        private string m_status;

        public string GlobalID
        {
            get 
            {
                return m_globalID;
            }
            set
            {
                SetValue<string>("GlobalID", ref m_globalID,value);
            }
        }

        public string LocalID
        {
            get
            {
                return m_localID;
            }
            set
            {
                SetValue<string>("LocalID", ref m_localID, value);
            }
        }

        public string CategoryName
        {
            get
            {
                return m_categoryName;
            }
            set
            {
                SetValue<string>("CategoryName", ref m_categoryName, value);
            }
        }

        public string MyLogType
        {
            get
            {
                return m_logType;
            }
            set
            {
                SetValue<string>("MyLogType", ref m_logType, value);
            }
        }

        public string MyStatus
        {
            get
            {
                return m_status;
            }
            set
            {
                SetValue<string>("MyStatus", ref m_status, value);
            }
        }

        /// <summary>
        /// Convert entity to service contract.
        /// </summary>
        /// <returns></returns>
        public LogCategoryQueryCriteria ToContract()
        {
            LogCategoryQueryCriteria contract = new LogCategoryQueryCriteria()
            {
                GlobalID = this.m_globalID,
                LocalID = this.m_localID,
                CategoryName = this.m_categoryName
            };

            switch (this.m_logType)
            {
                case "A":
                    contract.LogType = LogType.Audit;
                    break;
                case "D":
                    contract.LogType = LogType.Debug;
                    break;
                case "E":
                    contract.LogType = LogType.Error;
                    break;
                case "I":
                    contract.LogType = LogType.Info;
                    break;
                case "T":
                    contract.LogType = LogType.Trace;
                    break;
                default:
                    contract.LogType = new Nullable<LogType>();
                    break;
            }
            switch (this.m_status)
            {
                case "A":
                    contract.Status = Status.Active;
                    break;
                case "I":
                    contract.Status = Status.Inactive;
                    break;
                default:
                    contract.Status = new Nullable<Status>();
                    break;
            }

            return contract;
        }
    }
}
