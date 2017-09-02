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
    [VersionExport(typeof(ProductDiscussProcessor))]
    public class ProductDiscussProcessor
    {
        private IProductDiscussDA productDiscussDA = ObjectFactory<IProductDiscussDA>.Instance;

        #region 产品讨论
        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual ProductDiscussDetail LoadProductDiscuss(int sysNo)
        {
            ProductDiscussDetail item = productDiscussDA.LoadProductDiscuss(sysNo);
            item.ProductDiscussReplyList = productDiscussDA.GetProductDiscussReply(sysNo);
            return item;
        }

        /// <summary>
        /// 只有在网站登录账户且有权限才能发表
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateProductDiscuss(ProductDiscussDetail item)
        {
            productDiscussDA.CreateProductDiscuss(item);
        }

        /// <summary>
        /// 产品讨论审核通过后显示在website页面中。
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchApproveProductDiscuss(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussStatus(items, "A");
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchRefuseProductDiscuss(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussStatus(items, "D");
        }

        /// <summary>
        /// 设置产品评论阅读状态
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchReadProductDiscuss(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussStatus(items, "E");
        }

        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditProductDiscuss(ProductDiscussDetail item)
        {
            productDiscussDA.EditProductDiscuss(item);
        }

        #endregion

        #region 产品讨论—回复（ProductDiscussReply）
        /// <summary>
        /// 添加产品评论回复    1.	在网站登录账户且有权限才能发表。需要审核才能展示在网页中。2.	IPP系统中发表回复。
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddProductDiscussReply(ProductDiscussReply item)
        {
            productDiscussDA.UpdateProductDiscussReplyCount(item.DiscussSysNo.Value);
            productDiscussDA.AddProductDiscussReply(item);
        }

        /// <summary>
        /// 产品讨论回复批量审核
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchApproveProductDiscussReply(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussReplyStatus(items, "A");
        }

        /// <summary>
        /// 产品讨论回复批量屏蔽
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchVoidProductDiscussReply(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussReplyStatus(items, "D");
        }

        /// <summary>
        /// 产品讨论回复批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchReadProductDiscussReply(List<int> items)
        {
            productDiscussDA.BatchSetProductDiscussReplyStatus(items, "E");
        }

        #endregion

    }
}
