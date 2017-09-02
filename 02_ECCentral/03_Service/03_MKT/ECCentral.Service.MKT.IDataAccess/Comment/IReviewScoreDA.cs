using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IReviewScoreDA
    {
        #region 评分项定义

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        void SetReviewScoreValid(List<int> item);

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        void SetReviewScoreInvalid(List<int> item);

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="sysNo"></param>
        ReviewScoreItem LoadReviewScoreItem(int sysNo);
        ReviewScoreItem LoadReviewScoreItem(string name, string companyCode);
        /// <summary>
        /// 创建评分项定义
        /// </summary>
        /// <param name="item"></param>
        void CreateReviewScoreItem(ReviewScoreItem item);

        /// <summary>
        /// 编辑评分项定义
        /// </summary>
        /// <param name="item"></param>
        void UpdateReviewScoreItem(ReviewScoreItem item);

        #endregion

        #region  评论模式设置（RemarkMode）

        /// <summary>
        /// 加载新闻公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        RemarkMode LoadRemarkMode(string companyCode);

        /// <summary>
        /// 更新评论模式
        /// </summary>
        /// <param name="item"></param>
        void UpdateRemarkMode(RemarkMode item);

        /// <summary>
        /// 更新公告及促销评论，讨论，评论，咨询模式
        /// </summary>
        /// <param name="item"></param>
        void UpdateOtherRemarkMode(RemarkMode item);
        #endregion

    }
}
