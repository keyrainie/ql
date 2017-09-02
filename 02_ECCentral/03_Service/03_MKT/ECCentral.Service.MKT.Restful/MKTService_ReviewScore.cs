using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private ReviewScoreAppService reviewScoreAppService = ObjectFactory<ReviewScoreAppService>.Instance;

        #region 评分项定义

        /// <summary>
        /// 更新评分项定义
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateReviewScoreItem", Method = "PUT")]
        public virtual void UpdateReviewScoreItem(ReviewScoreItem item)
        {
            reviewScoreAppService.UpdateReviewScoreItem(item);
        }

        /// <summary>
        /// 添加评分项定义
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/CreateReviewScoreItem", Method = "POST")]
        public virtual void CreateReviewScoreItem(ReviewScoreItem item)
        {
            reviewScoreAppService.CreateReviewScoreItem(item);
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadReviewScoreItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ReviewScoreItem LoadReviewScoreItem(int sysNo)
        {
            return reviewScoreAppService.LoadReviewScoreItem(sysNo);
        }

        /// <summary>
        /// 评分项定义查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductReviewScore", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductReviewScore(ReviewScoreItemQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductReviewScore(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/SetReviewScoreValid", Method = "PUT")]
        public virtual void SetReviewScoreValid(List<int> item)
        {
            reviewScoreAppService.SetReviewScoreValid(item);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/SetReviewScoreInvalid", Method = "PUT")]
        public virtual void SetReviewScoreInvalid(List<int> item)
        {
            reviewScoreAppService.SetReviewScoreInvalid(item);
        }
        #endregion

        #region  评论模式设置（RemarkMode）
        /// <summary>
        /// 评论模式查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryRemarkModeList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryRemarkModeList(RemarkModeQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryRemarkModeList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载新闻公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadRemarkMode", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual RemarkMode LoadRemarkMode(string companyCode)
        {
            return reviewScoreAppService.LoadRemarkMode(companyCode);
        }


        /// <summary>
        /// 更新评论模式
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateRemarkMode", Method = "PUT")]
        public virtual void UpdateRemarkMode(RemarkMode item)
        {
            reviewScoreAppService.UpdateRemarkMode(item);
        }

        /// <summary>
        /// 批量更新公告及促销评论，讨论，评论，咨询模式
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateOtherRemarkMode", Method = "PUT")]
        public virtual void UpdateOtherRemarkMode(List<RemarkMode> items)
        {
            reviewScoreAppService.UpdateOtherRemarkMode(items);
        }

        #endregion

    }
}
