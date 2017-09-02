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
        private ProductDiscussAppService productDiscussAppService = ObjectFactory<ProductDiscussAppService>.Instance;


        #region 产品讨论

        /// <summary>
        /// 查询产品讨论
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductDiscuss", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductDiscuss(ProductDiscussQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductDiscuss(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 产品讨论审核通过后显示在website页面中。
        /// </summary>
        /// <param name="itemID"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchApproveProductDiscuss", Method = "PUT")]
        public virtual void BatchApproveProductDiscuss(List<int> items)
        {
            productDiscussAppService.BatchApproveProductDiscuss(items);
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="itemID"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchRefuseProductDiscuss", Method = "PUT")]
        public virtual void BatchRefuseProductDiscuss(List<int> items)
        {
            productDiscussAppService.BatchRefuseProductDiscuss(items);
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="itemID"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchReadProductDiscuss", Method = "PUT")]
        public virtual void BatchReadProductDiscuss(List<int> items)
        {
            productDiscussAppService.BatchReadProductDiscuss(items);
        }

        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="filter"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/EditProductDiscuss", Method = "POST")]
        public void EditProductDiscuss(ProductDiscussDetail item)
        {
            productDiscussAppService.EditProductDiscuss(item);
        }

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadProductDiscuss", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public ProductDiscussDetail LoadProductDiscuss(int sysNo)
        {
            return productDiscussAppService.LoadProductDiscuss(sysNo);
        }

        #endregion

        #region 产品讨论—回复（ProductDiscussReply）
        /// <summary>
        /// 添加产品评论回复    1.	在网站登录账户且有权限才能发表。需要审核才能展示在网页中。2.	IPP系统中发表回复。
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/CreateProductDiscussReply", Method = "POST")]
        public virtual void AddProductDiscussReply(ProductDiscussReply item)
        {
            productDiscussAppService.AddProductDiscussReply(item);
        }

        /// <summary>
        /// 审核产品讨论回复，然后在website中展示。产品讨论回复批量审核
        /// </summary>
        /// <param name="itemID"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchApproveProductDiscussReply", Method = "PUT")]
        public virtual void BatchApproveProductDiscussReply(List<int> items)
        {
            productDiscussAppService.BatchApproveProductDiscussReply(items);
        }

        /// <summary>
        /// 作废讨论回复，不展示在website中。产品讨论回复批量屏蔽
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchVoidProductDiscussReply", Method = "PUT")]
        public virtual void BatchVoidProductDiscussReply(List<int> items)
        {
            productDiscussAppService.BatchVoidProductDiscussReply(items);
        }

        /// <summary>
        /// 产品讨论回复批量阅读
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchReadProductDiscussReply", Method = "PUT")]
        public virtual void BatchReadProductDiscussReply(List<int> items)
        {
            productDiscussAppService.BatchReadProductDiscussReply(items);
        }

        /// <summary>
        /// 查询产品讨论回复
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductDiscussReply", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductDiscussReply(ProductDiscussReplyQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductDiscussReply(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion

    }
}
