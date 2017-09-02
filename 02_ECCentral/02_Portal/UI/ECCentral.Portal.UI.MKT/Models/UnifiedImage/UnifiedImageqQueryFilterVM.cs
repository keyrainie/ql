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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class UnifiedImageQueryFilterVM :ModelBase
    {

        public UnifiedImageQueryFilterVM()
        {
            PagingInfo = new PagingInfo();
            PagingInfo.PageIndex = 0;
            PagingInfo.PageSize = 1;
            PagingInfo.SortBy = "";
        }
        public  PagingInfo PagingInfo { get; set; }

        public string ImageName { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTiemTo { get; set; } 
    }
}
