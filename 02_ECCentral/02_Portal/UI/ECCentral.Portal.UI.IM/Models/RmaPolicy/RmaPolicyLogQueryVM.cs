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

namespace ECCentral.Portal.UI.IM.Models
{
    public class RmaPolicyLogQueryVM : ModelBase
    {
        private string rmaPolicySysNO;
        public string RmaPolicySysNO
        {
            get { return rmaPolicySysNO; }
            set { SetValue("RmaPolicySysNO", ref rmaPolicySysNO, value); }

        }
        private string rmaPolicy;
        public string RmaPolicy
        {
            get { return rmaPolicy; }
            set { SetValue("RmaPolicy", ref rmaPolicy, value); }
        }
        private string eidtUserName;
        public string EidtUserName
        {
            get { return eidtUserName; }
            set { SetValue("EidtUserName", ref eidtUserName, value); }
        }
        private DateTime? updateDateTo;
        public DateTime? UpdateDateTo
        {
            get { return updateDateTo; }
            set { SetValue("UpdateDateTo", ref updateDateTo, value); }
        }
        private DateTime? updateDateFrom;
        public DateTime? UpdateDateFrom
        {
            get { return updateDateFrom; }
            set { SetValue("UpdateDateFrom", ref updateDateFrom, value); }
        }
    }
}
