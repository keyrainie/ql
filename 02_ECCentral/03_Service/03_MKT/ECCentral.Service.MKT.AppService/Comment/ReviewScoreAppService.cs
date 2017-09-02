using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ReviewScoreAppService))]
    public class ReviewScoreAppService
    {
        #region 评分项定义
        ///<summary>
        ///创建评分项定义
        ///</summary>
        ///<param name="item"></param>
        public void CreateReviewScoreItem(ReviewScoreItem item)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.CreateReviewScoreItem(item);
        }

        /// <summary>
        /// 编辑评分项定义
        /// </summary>
        /// <param name="item"></param>
        public void UpdateReviewScoreItem(ReviewScoreItem item)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.UpdateReviewScoreItem(item);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreValid(List<int> item)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.SetReviewScoreValid(item);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreInvalid(List<int> item)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.SetReviewScoreInvalid(item);
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <returns></returns>
        public virtual ReviewScoreItem LoadReviewScoreItem(int sysNo)
        {
            return ObjectFactory<ReviewScoreProcessor>.Instance.LoadReviewScoreItem(sysNo);
        }
        #endregion

        #region  评论模式设置（RemarkMode）
        /// <summary>
        /// 加载评论模式
        /// </summary>
        public virtual RemarkMode LoadRemarkMode(string companyCode)
        {
            return ObjectFactory<ReviewScoreProcessor>.Instance.LoadRemarkMode(companyCode);
        }

        /// <summary>
        /// 更新谁模式
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateRemarkMode(RemarkMode item)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.UpdateRemarkMode(item);
        }

        /// <summary>
        /// 更新公告及促销评论，讨论，评论，咨询模式
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateOtherRemarkMode(List<RemarkMode> items)
        {
            ObjectFactory<ReviewScoreProcessor>.Instance.UpdateOtherRemarkMode(items);
        }
        #endregion

      
    }
}
