using ECCentral.QueryFilter.Common;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    /// <summary>
    /// 公用Data帮助类
    /// </summary>
    public static class HelpDA
    {
        /// <summary>
        /// InfoToEntity
        /// </summary>
        /// <param name="pagingInfo"></param>
        /// <returns></returns>
        public static PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = SortFieldMapping(pagingInfo.SortBy),
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }
        private static string SortFieldMapping(string sortBy)
        {
            return sortBy;
        }
    }
}
