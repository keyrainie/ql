
namespace ECCentral.QueryFilter.Common
{
    /// <summary>
    /// 默认无条件，只有分页查询条件
    /// </summary>
    public class DefaultQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}
