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
        private ProductConsultAppService productConsultAppService = ObjectFactory<ProductConsultAppService>.Instance;

        #region 咨询管理（ProductConsult）

        /// <summary>
        /// 添加或更新回复,并更新咨询的回复次数
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateProductConsultDetailReply", Method = "PUT")]
        public virtual void UpdateProductConsultDetailReply(ProductConsultReply item)
        {
            productConsultAppService.UpdateProductConsultDetailReply(item);
        }

        /// <summary>
        /// 获取产品咨询查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductConsult", Method = "POST")]//, ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductConsult(ProductConsultQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductConsult(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载购物咨询
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/LoadProductConsult", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ProductConsult LoadProductConsult(int sysNo)
        {
            return productConsultAppService.LoadProductConsult(sysNo);
        }

        /// <summary>
        /// 批量审核购物咨询
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultValid", Method = "PUT")]
        public virtual void BatchSetProductConsultValid(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultValid(items);
        }

        /// <summary>
        /// 批量作废购物咨询
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultInvalid", Method = "PUT")]
        public virtual void BatchSetProductConsultInvalid(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultInvalid(items);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultRead", Method = "PUT")]
        public virtual void BatchSetProductConsultRead(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultRead(items);
        }
        #endregion

        #region  产品咨询回复（ProductConsultReply）
        /// <summary>
        /// 批准发布操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/ApproveProductConsultRelease", Method = "PUT")]
        public void ApproveProductConsultRelease(ProductConsultReply item)
        {
            productConsultAppService.ApproveProductConsultRelease(item);
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/RejectProductConsultRelease", Method = "PUT")]
        public void RejectProductConsultRelease(ProductConsultReply item)
        {
            productConsultAppService.RejectProductConsultRelease(item);
        }


        /// <summary>
        /// 更新咨询管理回复
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/UpdateProductConsultReply", Method = "PUT")]
        public virtual void UpdateProductConsultReply(ProductConsultReply item)
        {
            productConsultAppService.UpdateProductConsultReply(item);
        }

        /// <summary>
        /// 获取产品咨询回复查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommentInfo/QueryProductConsultReply", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductConsultReply(ProductConsultReplyQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommentQueryDA>.Instance.QueryProductConsultReply(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }


        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultReplyValid", Method = "PUT")]
        public void BatchSetProductConsultReplyValid(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultReplyValid(items);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultReplyInvalid", Method = "PUT")]
        public void BatchSetProductConsultReplyInvalid(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultReplyInvalid(items);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultReplyRead", Method = "PUT")]
        public void BatchSetProductConsultReplyRead(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultReplyRead(items);
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchSetProductConsultReplyTop", Method = "PUT")]
        public void BatchSetProductConsultReplyTop(List<int> items)
        {
            productConsultAppService.BatchSetProductConsultReplyTop(items);
        }

        /// <summary>
        /// 批量取消置顶
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/CommentInfo/BatchCancelProductConsultReplyTop", Method = "PUT")]
        public void BatchCancelProductConsultReplyTop(List<int> items)
        {
            productConsultAppService.BatchCancelProductConsultReplyTop(items);
        }
        

        #endregion
 

    }
}
