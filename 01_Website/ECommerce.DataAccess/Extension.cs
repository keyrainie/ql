using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility;
using ECommerce.Entity;

namespace ECommerce.DataAccess
{
    public static class Extension
    {
        public static PagedResult<T> CreateResult<T>(this QueryFilter filter)
        {
            PagedResult<T> result = new PagedResult<T>();
            result.PageNumber = filter.PageIndex;
            result.PageSize = filter.PageSize;
            return result;
        }

        public static PagingInfoEntity ConvertToPaging(this QueryFilterBase filter)
        {
            var paging = new PagingInfoEntity()
            {
                SortField = filter.PageInfo.SortBy,
                StartRowIndex = (filter.PageInfo.PageIndex - 1) * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };
            return paging;
        }

        public static PageInfo ConvertToPageInfo(this QueryFilterBase filter, int totalCount)
        {
            int pageIndex = filter.PageInfo.PageIndex;
            if ((pageIndex * filter.PageInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % filter.PageInfo.PageSize) == 0)
                {
                    pageIndex = totalCount / filter.PageInfo.PageSize;
                }
                else
                {
                    pageIndex = totalCount / filter.PageInfo.PageSize + 1;
                }
            }

            var pageInfo = new PageInfo();
            pageInfo.TotalCount = totalCount;
            pageInfo.PageIndex = pageIndex;
            pageInfo.PageSize = filter.PageInfo.PageSize;
            pageInfo.SortBy = filter.PageInfo.SortBy;
            return pageInfo;
        }
    }
}
