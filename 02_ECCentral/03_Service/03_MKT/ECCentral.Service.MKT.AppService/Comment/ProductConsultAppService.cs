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
    [VersionExport(typeof(ProductConsultAppService))]
    public class ProductConsultAppService
    {
        #region 咨询管理（ProductConsult）

        /// <summary>
        /// 添加或更新回复,并更新咨询的回复次数
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductConsultDetailReply(ProductConsultReply item)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.UpdateProductConsultDetailReply(item);
        }

        /// <summary>
        /// 加载购物咨询
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductConsult LoadProductConsult(int sysNo)
        {
            return ObjectFactory<ProductConsultProcessor>.Instance.LoadProductConsult(sysNo);
        }

        /// <summary>
        /// 批量审核购物咨询
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultValid(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultValid(items);
        }

        /// <summary>
        /// 批量作废购物咨询
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultInvalid(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultInvalid(items);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultRead(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultRead(items);
        }
        #endregion

        #region  产品咨询回复（ProductConsultReply）
        /// <summary>
        /// 批准发布操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public virtual void ApproveProductConsultRelease(ProductConsultReply item)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.ApproveProductConsultRelease(item);
        }

        /// <summary>
        /// 咨询回复之批准拒绝
        /// </summary>
        /// <param name="item"></param>
        public virtual void RejectProductConsultRelease(ProductConsultReply item)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.RejectProductConsultRelease(item);
        }



        /// <summary>
        /// 创建购物咨询
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateProductConsultReply(ProductConsultReply item)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.CreateProductConsultReply(item);
        }

        /// <summary>
        /// 更新咨询管理
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual void UpdateProductConsultReply(ProductConsultReply item)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.UpdateProductConsultReply(item);
        }

        /// <summary>
        /// 获取关于咨询的所有回复，除去厂商回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public virtual List<ProductConsultReply> GetProductConsultReplyList(int consultSysNo)
        {
            return ObjectFactory<ProductConsultProcessor>.Instance.GetProductConsultReplyList(consultSysNo);
        }

        /// <summary>
        /// 获取厂商关于咨询的回复列表
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public virtual List<ProductConsultReply> GetProductConsultFactoryReplyList(int consultSysNo)
        {
            return ObjectFactory<ProductConsultProcessor>.Instance.GetProductConsultFactoryReplyList(consultSysNo);
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyValid(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultReplyValid(items);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyInvalid(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultReplyInvalid(items);
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyRead(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultReplyRead(items);
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyTop(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchSetProductConsultReplyTop(items);
        }

        /// <summary>
        /// 批量取消置顶
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchCancelProductConsultReplyTop(List<int> items)
        {
            ObjectFactory<ProductConsultProcessor>.Instance.BatchCancelProductConsultReplyTop(items);
        }



        #endregion

    }
}
