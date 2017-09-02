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
using ECCentral.WPMessage.BizEntity;

namespace ECCentral.Portal.UI.WPMessage.Models
{
    public class WPMessageVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        private string categoryName;
        public string CategoryName
        {
            get { return categoryName; }
            set { SetValue("CategoryName", ref categoryName, value); }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { SetValue("Url", ref url, value); }
        }

        private string bizSysNo;
        public string BizSysNo
        {
            get { return bizSysNo; }
            set { SetValue("BizSysNo", ref bizSysNo, value); }
        }

        private string parameters;
        public string Parameters
        {
            get { return parameters; }
            set { SetValue("Parameters", ref parameters, value); }
        }

        private string memo;
        public string Memo
        {
            get { return memo; }
            set { SetValue("Memo", ref memo, value); }
        }

        private string processUserName;
        public string ProcessUserName
        {
            get { return processUserName; }
            set { SetValue("ProcessUserName", ref processUserName, value); }
        }

        private WPMessageStatus status;
        public WPMessageStatus Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }

        private DateTime createTime;
        public DateTime CreateTime
        {
            get { return createTime; }
            set { SetValue("CreateTime", ref createTime, value); }
        }
    }
}
