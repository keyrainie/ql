using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductDiscussDA
    {
        #region 产品讨论
        /// <summary>
        /// 只有在网站登录账户且有权限才能发表
        /// </summary>
        /// <param name="item"></param>
        void CreateProductDiscuss(ProductDiscussDetail item);

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<ProductDiscussDetail> GetProductDiscuss(int productID);

        /// <summary>
        /// 设置产品讨论的状态，
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="status"></param>
        void BatchSetProductDiscussStatus(List<int> items, string status);

        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="item"></param>
        void EditProductDiscuss(ProductDiscussDetail item);

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="sysNo"></param>
        ProductDiscussDetail LoadProductDiscuss(int sysNo);
        #endregion

        #region 产品讨论—回复（ProductDiscussReply）


        /// <summary>
        /// 根据讨论论编号，加载相应的讨论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        List<ProductDiscussReply> GetProductDiscussReply(int sysNo);

        /// <summary>
        /// 讨论回复加1，并设置相关状态
        /// </summary>
        /// <param name="discussSysNo"></param>
        void UpdateProductDiscussReplyCount(int discussSysNo);

        /// <summary>
        /// 添加产品评论回复    1.	在网站登录账户且有权限才能发表。需要审核才能展示在网页中。2.	IPP系统中发表回复。
        /// </summary>
        /// <param name="item"></param>
        void AddProductDiscussReply(ProductDiscussReply item);

        /// <summary>
        /// 产品讨论回复批量审核
        /// </summary>
        /// <param name="items"></param>
        /// <param name="status"></param>
        void BatchSetProductDiscussReplyStatus(List<int> items, string status);

        #endregion

    }
}
