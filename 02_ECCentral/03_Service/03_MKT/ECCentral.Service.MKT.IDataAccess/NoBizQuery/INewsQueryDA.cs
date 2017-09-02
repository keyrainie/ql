using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface INewsQueryDA
    {
        /// <summary>
        /// 新闻公告查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet QueryNews(NewsInfoQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询公告及促销评论
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryNewsAdvReply(NewsAdvReplyQueryFilter filter, out int totalCount);
    }
}
