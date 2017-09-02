using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ICommentQueryDA
    {
        #region 留言管理

        /// <summary>
        /// 留言管理查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryLeaveWord(LeaveWordQueryFilter filter, out int totalCount);
        #endregion

        /// <summary>
        /// 评分项定义查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductReviewScore(ReviewScoreItemQueryFilter filter, out int totalCount);

        /// <summary>
        /// 评论模式查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryRemarkModeList(RemarkModeQueryFilter filter, out int totalCount);

        /// <summary>
        /// 产品评论查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductReview(ProductReviewQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 产品评论查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductReviewReply(ProductReviewReplyQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 产品咨询查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductConsult(ProductConsultQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 产品咨询回复查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductConsultReply(ProductConsultReplyQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 查询产品讨论
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductDiscuss(ProductDiscussQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 查询产品讨论回复
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductDiscussReply(ProductDiscussReplyQueryFilter queryCriteria, out int totalCount);
    }
}
