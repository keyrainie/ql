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

namespace ECCentral.Portal.UI.ExternalSYS.Models.VendorUserInfo
{
    public class VendorProductVM : ModelBase
    {
        public int SysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductMode { get; set; }

        public int C3SysNo { get; set; }

        public int? ManufacturerSysNo { get; set; }

        public int Status { get; set; }

        public string VendorName { get; set; }

        public string StatusStr 
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return "Show";
                    case 0:
                        return "Valid";
                    default:
                        return "--";
                }
            }
        }

        private bool m_IsCheck;

        public bool IsCheck
        {
            get { return m_IsCheck; }
            set { SetValue("IsCheck", ref m_IsCheck, value); }
        }
    }
}
