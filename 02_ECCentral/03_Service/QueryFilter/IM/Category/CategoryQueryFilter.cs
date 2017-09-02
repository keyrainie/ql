using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class CategoryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string CategoryName { get; set; }
        public CategoryStatus? Status { get; set; }
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }
        public string CompanyCode { get; set; }
        public CategoryType? Type { get; set; }
    }
}
