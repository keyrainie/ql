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
    [VersionExport(typeof(ProductReviewMailLogAppService))]
    public class ProductReviewMailLogAppService
    {
        #region 有关评论所使用到的邮件LOG

        /// <summary>
        /// 回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        public virtual void UpdateProductConsultMailLog(ProductReview log)
        {
            ObjectFactory<ProductReviewMailLogProcessor>.Instance.UpdateProductConsultMailLog(log);
        }

        /// <summary>
        /// 获取关于咨询的邮件log列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductReviewMailLog QueryProductConsultMailLog(int sysNo)
        {
            return ObjectFactory<ProductReviewMailLogProcessor>.Instance.QueryProductConsultMailLog(sysNo);
        }


        /// <summary>
        /// 获取关于产品评论的邮件log列表
        /// </summary>
        /// <param name="refSysNo"></param>
        /// <returns></returns>
        public virtual ProductReviewMailLog QueryProductReviewMailLog(int refSysNo)
        {
            return ObjectFactory<ProductReviewMailLogProcessor>.Instance.QueryProductReviewMailLog(refSysNo);
        }
        #endregion

    }
}
