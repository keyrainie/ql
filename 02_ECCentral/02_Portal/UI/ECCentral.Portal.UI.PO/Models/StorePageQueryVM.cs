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

namespace ECCentral.Portal.UI.PO.Models
{
    public class StorePageQueryVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private int? merchantSysNo;
        public int? MerchantSysNo
        {
            get { return merchantSysNo; }
            set { base.SetValue("MerchantSysNo", ref merchantSysNo, value); }
        }

        /// <summary>
        /// 页面类型
        /// </summary>
        private string pageType;
        public string PageType
        {
            get { return pageType; }
            set { base.SetValue("PageType", ref pageType, value); }
        }
    }
}
