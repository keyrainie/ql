using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductReviewMailLogDA
    {

        #region 有关评论所使用到的邮件LOG
        /// <summary>
        /// 检查邮件是否存在
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        bool CheckProductCommentMailLog(ProductReviewMailLog log);

        /// <summary>
        /// 新建回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        void CreateProductCommentMailLog(ProductReviewMailLog log);

        /// <summary>
        /// 回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        void UpdateProductCommentMailLog(ProductReviewMailLog log);

        /// <summary>
        /// 获取关于咨询的邮件log列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ProductReviewMailLog QueryProductCommentMailLog(int sysNo, string type);

        #endregion
    }
}
