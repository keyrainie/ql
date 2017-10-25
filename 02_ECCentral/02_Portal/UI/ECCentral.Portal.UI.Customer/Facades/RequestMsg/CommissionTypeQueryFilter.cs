using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
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

namespace ECCentral.Portal.UI.Customer.Facades.RequestMsg
{
    public class CommissionTypeQueryFilter
    {
        public int? SysNo { get; set; }
        public string CommissionTypeID { get; set; }
        public string CommissionTypeName { get; set; }
        public HYNStatus? IsOnlineShow { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
