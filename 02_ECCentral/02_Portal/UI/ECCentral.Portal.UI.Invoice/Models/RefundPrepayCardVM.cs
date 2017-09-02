using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
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

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class RefundPrepayCardVM : ModelBase
    {
        /// <summary>
        /// 神州运通退款申请单号
        /// </summary>
        public int? SOIncomeBankInfoSysNo { get; set; }

        public int? UserSysNo { get; set; }

        public string CompanyCode
        {
            get;
            set;
        }

        public int? SysNo
        {
            get;
            set;
        }
    }
}
