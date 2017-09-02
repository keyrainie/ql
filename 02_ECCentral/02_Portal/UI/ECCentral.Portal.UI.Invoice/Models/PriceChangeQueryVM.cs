using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
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
    public class PriceChangeQueryVM : ModelBase
    {
        public PriceChangeQueryVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<RequestPriceStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public PagingInfo PagingInfo { get; set; }

        private string sysNo;
        public string SysNo { get { return sysNo; } set { SetValue("SysNo", ref sysNo, value); } }

        private string productsysNo;
        public string ProductsysNo { get { return productsysNo; } set { SetValue("ProductsysNo", ref productsysNo, value); } }

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { SetValue("ProductName", ref productName, value); }
        }

        private string memo;
        [Validate(ValidateType.MaxLength,500)]
        public string Memo { get { return memo; } set { SetValue("Memo", ref memo, value); } }

        private DateTime? beginDate;
        public DateTime? BeginDate { get { return beginDate; } set { SetValue("BeginDate", ref beginDate, value); } }

        private DateTime? endDate;
        public DateTime? EndDate { get { return endDate; } set { SetValue("EndDate", ref endDate, value); } }

        private RequestPriceStatus? status;
        public RequestPriceStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status", ref status, value);
            }
        }

        private string c1SysNo;
        public string C1SysNo { get { return c1SysNo; } set { SetValue("C1SysNo", ref c1SysNo, value); } }

        private string c2SysNo;
        public string C2SysNo { get { return c2SysNo; } set { SetValue("C2SysNo", ref c2SysNo, value); } }

        private string c3SysNo;
        public string C3SysNo { get { return c3SysNo; } set { SetValue("C3SysNo", ref c3SysNo, value); } }

        public List<KeyValuePair<RequestPriceStatus?, string>> StatusList { get; set; }
    }
}
