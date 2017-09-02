using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.Common;

using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ReviewScoreProcessor))]
    public class ReviewScoreProcessor
    {
        private IReviewScoreDA reviewScoreDA = ObjectFactory<IReviewScoreDA>.Instance;

        #region 评分项定义
        /// <summary>
        /// 创建评分项定义
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateReviewScoreItem(ReviewScoreItem item)
        {
            ReviewScoreItem existsItem = reviewScoreDA.LoadReviewScoreItem(item.Name.Content,item.CompanyCode);
            if (existsItem!=null)
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_ScoreItemNameExists"));
            }
            reviewScoreDA.CreateReviewScoreItem(item);
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual ReviewScoreItem LoadReviewScoreItem(int sysNo)
        {
            return reviewScoreDA.LoadReviewScoreItem(sysNo);
        }

        /// <summary>
        /// 编辑评分项定义
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateReviewScoreItem(ReviewScoreItem item)
        {
            reviewScoreDA.UpdateReviewScoreItem(item);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreValid(List<int> item)
        {
            reviewScoreDA.SetReviewScoreValid(item);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreInvalid(List<int> item)
        {
            reviewScoreDA.SetReviewScoreInvalid(item);
        }
        #endregion

        #region  评论模式设置（RemarkMode）

        /// <summary>
        /// 加载新闻公告及促销评论模式
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual RemarkMode LoadRemarkMode(string companyCode)
        {
            return reviewScoreDA.LoadRemarkMode(companyCode);
        }

        /// <summary>
        /// 更新评论模式
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateRemarkMode(RemarkMode item)
        {
            reviewScoreDA.UpdateRemarkMode(item);
        }

        /// <summary>
        /// 批量更新公告及促销评论，讨论，评论，咨询模式
        /// </summary>
        /// <param name="items"></param>
        public virtual void UpdateOtherRemarkMode(List<RemarkMode> items)
        {
            foreach (RemarkMode item in items)
            {
                reviewScoreDA.UpdateOtherRemarkMode(item);
            }
        }
        #endregion

    }
}
