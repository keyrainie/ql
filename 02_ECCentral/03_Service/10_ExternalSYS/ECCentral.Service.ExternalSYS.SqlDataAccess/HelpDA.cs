using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
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
