using ECCentral.BizEntity.MKT;
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

namespace ECCentral.QueryFilter.MKT
{
    public class ECCManageCategoryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string CategoryName { get; set; }
        public ECCCategoryManagerStatus? Status { get; set; }
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }
        public string CompanyCode { get; set; }
        public ECCCategoryManagerType? Type { get; set; }
    }
}
