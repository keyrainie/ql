using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Common
{
    /// <summary>
    /// 带有分页信息的查询结果Entity,所有带分页的查询Service返回类型都必须为此类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class QueryResult<T>
    {
        /// <summary>
        /// 查询结果List
        /// </summary>
        [DataMember]
        public List<T> ResultList { get; set; }
        /// <summary>
        /// 查询结果分页信息
        /// </summary>
        [DataMember]
        public PageInfo PageInfo { get; set; }

        public QueryResult()
        { }

        public QueryResult(List<T> result, QueryFilter filter, int totalCount)
        {
            ResultList = result;
            PageInfo = new PageInfo() { PageIndex = filter.PageIndex, PageSize = filter.PageSize, SortBy = filter.SortFields, TotalCount = totalCount };
        }
    }

    [Serializable]
    [DataContract]
    public class QueryResult
    {
        public QueryResult(DataTable result, QueryFilter filter, int totalCount)
        {
            ResultList = result;
            PageInfo = new PageInfo() { PageIndex = filter.PageIndex, PageSize = filter.PageSize, SortBy = filter.SortFields, TotalCount = totalCount };
        }

        [DataMember]
        public PageInfo PageInfo { get; set; }

        [DataMember]
        public DataTable ResultList { get; set; }
    }
}
