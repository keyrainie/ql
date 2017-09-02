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
    [VersionExport(typeof(ProductDiscussAppService))]
    public class ProductDiscussAppService
    {
        #region 产品讨论
        /// <summary>
        /// 只有在网站登录账户且有权限才能发表
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateProductDiscuss(ProductDiscussDetail item)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.CreateProductDiscuss(item);
        }



        /// <summary>
        /// 产品讨论审核通过后显示在website页面中。
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchApproveProductDiscuss(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchApproveProductDiscuss(items);
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchRefuseProductDiscuss(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchRefuseProductDiscuss(items);
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void BatchReadProductDiscuss(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchReadProductDiscuss(items);
        }

        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditProductDiscuss(ProductDiscussDetail item)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.EditProductDiscuss(item);
        }

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual ProductDiscussDetail LoadProductDiscuss(int sysNo)
        {
            return ObjectFactory<ProductDiscussProcessor>.Instance.LoadProductDiscuss(sysNo);
        }
        #endregion

        #region 产品讨论—回复（ProductDiscussReply）
        /// <summary>
        /// 添加产品评论回复    1.	在网站登录账户且有权限才能发表。需要审核才能展示在网页中。2.	IPP系统中发表回复。
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddProductDiscussReply(ProductDiscussReply item)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.AddProductDiscussReply(item);
        }

        /// <summary>
        /// 审核产品讨论回复，然后在website中展示。产品讨论回复批量审核
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchApproveProductDiscussReply(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchApproveProductDiscussReply(items);
        }

        /// <summary>
        /// 作废讨论回复，不展示在website中。产品讨论回复批量屏蔽
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchVoidProductDiscussReply(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchVoidProductDiscussReply(items);
        }

        /// <summary>
        /// 产品讨论回复批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchReadProductDiscussReply(List<int> items)
        {
            ObjectFactory<ProductDiscussProcessor>.Instance.BatchReadProductDiscussReply(items);
        }
        #endregion

    }
}
