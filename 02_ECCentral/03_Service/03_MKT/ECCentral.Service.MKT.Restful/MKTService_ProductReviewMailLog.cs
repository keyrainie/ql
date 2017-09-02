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
        private ProductReviewMailLogAppService productReviewMailLogAppService = ObjectFactory<ProductReviewMailLogAppService>.Instance;

        #region 有关评论所使用到的邮件LOG


        /// <summary>
        /// 获取关于产品评论的邮件log列表
        /// </summary>
        /// <param name="refSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductReviewMailLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ProductReviewMailLog QueryProductReviewMailLog(int refSysNo)
        {
            return productReviewMailLogAppService.QueryProductReviewMailLog(refSysNo);
        }

        #endregion


        #region 有关评论所使用到的邮件LOG

        /// <summary>
        /// 回复邮件操作
        /// </summary>
        /// <param name="log"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateProductConsultMailLog", Method = "PUT")]
        public virtual void UpdateProductConsultMailLog(ProductReview log)
        {
            productReviewMailLogAppService.UpdateProductConsultMailLog(log);
        }

        /// <summary>
        /// 获取关于咨询的邮件log列表
        /// </summary>
        /// <param name="refSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductCommentMailLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ProductReviewMailLog QueryProductConsultMailLog(int refSysNo)
        {
            return productReviewMailLogAppService.QueryProductConsultMailLog(refSysNo);
        }

        #endregion
    }
}
