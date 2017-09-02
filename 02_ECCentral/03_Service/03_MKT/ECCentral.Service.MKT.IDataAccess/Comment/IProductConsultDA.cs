using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductConsultDA
    {
        #region 咨询管理（ProductConsult）

        /// <summary>
        /// 添加或更新回复,并更新咨询的回复次数
        /// </summary>
        /// <param name="item"></param>
        void UpdateProductConsultDetailReply(ProductConsultReply item);

        /// <summary>
        /// 加载购物咨询
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ProductConsult LoadProductConsult(int sysNo);

        /// <summary>
        /// 批量购物咨询的审核状态
        /// </summary>
        /// <param name="items"></param>
        void BatchSetProductConsultStatus(List<int> items, string status);
        #endregion

        #region  产品咨询回复（ProductConsultReply）

        /// <summary>
        /// 咨询回复之批准回复
        /// </summary>
        /// <param name="item"></param>
        int ApproveProductConsultRelease(ProductConsultReply item);

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="item"></param>
        int SendSSBForApproveProductConsultRelease(ProductConsultReply item);

        /// <summary>
        /// 咨询回复之批准拒绝
        /// </summary>
        /// <param name="item"></param>
        int RejectProductConsultRelease(ProductConsultReply item);

        /// <summary>
        /// 获取产品咨询回复列表,针对某一个咨询的回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //List<ProductConsultReply> GetProductConsultReplyListBySysNo(int sysNo);

        /// <summary>
        /// 检查是否存在厂商回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool CheckProductConsultFactoryReply(int sysNo, string status);

        /// <summary>
        /// 检查是否已经存在该咨询的回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        bool CheckProductConsultReply(int consultSysNo);

        /// <summary>
        /// 更新咨询回复
        /// </summary>
        /// <param name="item"></param>
        void UpdateProductConsultReply(ProductConsultReply item);

        /// <summary>
        /// 新建咨询回复
        /// </summary>
        /// <param name="item"></param>
        void CreateProductConsultReply(ProductConsultReply item);

        /// <summary>
        /// 更新咨询管理人回复数量
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="bMore"></param>
        void UpdateProductConsultReplyCount(int sysNo, bool bMore);

        /// <summary>
        /// 批量设置咨询回复的状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="status"></param>
        void BatchSetProductConsultReplyStatus(List<int> items, string status);

        /// <summary>
        /// 获取关于咨询的所有回复，除去厂商回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        List<ProductConsultReply> GetProductConsultReplyList(int consultSysNo);

        /// <summary>
        /// 获取厂商关于咨询的回复列表
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        List<ProductConsultReply> GetProductConsultFactoryReplyList(int consultSysNo);

        /// <summary>
        /// 根据咨询编号，加载相应的回复。
        /// </summary>
        /// <returns></returns>
        List<ProductConsultReply> LoadProductConsultReply(int itemID);

        /// <summary>
        /// 拒绝发布厂商回复
        /// </summary>
        /// <param name="itemID"></param>
        void AuditRefuseProductConsultReply(int itemID);

        /// <summary>
        /// 审核购物咨询回复，并在website中展示。
        /// </summary>
        /// <param name="itemID"></param>
        void AuditApproveProductConsultReply(int itemID);

        /// <summary>
        /// 作废评论回复，不展示在website中。
        /// </summary>
        /// <param name="itemID"></param>
        void VoidProductConsultReply(int itemID);


        #endregion
    }
}
