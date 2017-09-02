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
using ECCentral.Service.EventMessage.MKT;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductConsultProcessor))]
    public class ProductConsultProcessor
    {
        private IProductConsultDA productConsultDA = ObjectFactory<IProductConsultDA>.Instance;

        private IProductReviewMailLogDA productReviewMailLogDA = ObjectFactory<IProductReviewMailLogDA>.Instance;
        #region 咨询管理（ProductConsult）

        /// <summary>
        /// 添加或更新回复,并更新咨询的回复次数
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductConsultDetailReply(ProductConsultReply item)
        {
            productConsultDA.CreateProductConsultReply(item);
            productConsultDA.UpdateProductConsultReplyCount(item.ConsultSysNo.Value, true);
            //comentDA.UpdateProductConsultDetailReply(item);
        }

        /// <summary>
        /// 加载购物咨询
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductConsult LoadProductConsult(int sysNo)
        {
            ProductConsult item = productConsultDA.LoadProductConsult(sysNo);
            item.ProductConsultReplyList = productConsultDA.GetProductConsultReplyList(sysNo);
            item.VendorReplyList = productConsultDA.GetProductConsultFactoryReplyList(sysNo);//厂商回复
            item.ProductReviewMailLog = productReviewMailLogDA.QueryProductCommentMailLog(sysNo, "C");
            return item;
        }

        /// <summary>
        /// 批量审核购物咨询
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultValid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(()=>{
                productConsultDA.BatchSetProductConsultStatus(items, "A");

                foreach (int sysno in items)
                {
                    EventPublisher.Publish<ConsultAuditMessage>(new ConsultAuditMessage
                    {
                        SysNo = sysno,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                }
            });
        }

        /// <summary>
        /// 批量作废购物咨询
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultInvalid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
            {
                productConsultDA.BatchSetProductConsultStatus(items, "D");

                foreach (int sysno in items)
                {
                    EventPublisher.Publish<ConsultVoidMessage>(new ConsultVoidMessage
                    {
                        SysNo = sysno,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                }
            });
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultRead(List<int> items)
        {
            productConsultDA.BatchSetProductConsultStatus(items, "E");
        }
        #endregion

        #region  产品咨询回复（ProductConsultReply）

        /// <summary>
        /// 创建购物咨询回复
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateProductConsultReply(ProductConsultReply item)
        {
            //根据ConsultSysNo检测是否存在该回复，如果存在就更新，不然就新建
            //回复可能只需要新建,需要检测一下代码
            //此处有疑问
            if (productConsultDA.CheckProductConsultReply(item.SysNo.Value))
                productConsultDA.UpdateProductConsultReply(item);
            else
            {
                productConsultDA.CreateProductConsultReply(item);
                productConsultDA.UpdateProductConsultReplyCount(item.SysNo.Value, true);
            }
        }

        /// <summary>
        /// 更新咨询管理回复
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual void UpdateProductConsultReply(ProductConsultReply item)
        {
            productConsultDA.UpdateProductConsultReply(item);
        }

        /// <summary>
        /// 咨询回复之批准回复
        /// </summary>
        /// <param name="item"></param>
        public virtual void ApproveProductConsultRelease(ProductConsultReply item)
        {
            //if (!comentDA.CheckProductConsultFactoryReply(item.SysNo.Value,"A"))//批准回复时，状态需要为A
            //    throw new BizException("不存在该厂商回复！");
            //comentDA.UpdateProductConsultReplyCount(item.SysNo.Value, true);

          TransactionScopeFactory.TransactionAction(() =>
          {
              if (productConsultDA.ApproveProductConsultRelease(item) == 1)
                  throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_VendorWithdrawData"));
              else
              {
                  item.Status = "A";
                  if (productConsultDA.SendSSBForApproveProductConsultRelease(item) == 0)
                      //throw new BizException("执行存储过程出错！");//记操作日志
                      throw new BizException(ResouceManager.GetMessageString("MKT.Comment","Comment_ExcuProError！"));//记操作日志
              }


              EventPublisher.Publish<ConsultReplyAuditMessage>(new ConsultReplyAuditMessage
              {
                  SysNo = item.SysNo.Value,
                  CurrentUserSysNo = ServiceContext.Current.UserSysNo
              });
          });
        }

        /// <summary>
        /// 咨询回复之批准拒绝
        /// </summary>
        /// <param name="item"></param>
        public virtual void RejectProductConsultRelease(ProductConsultReply item)
        {
            TransactionScopeFactory.TransactionAction(() =>
          {
              if (productConsultDA.RejectProductConsultRelease(item) == 1)
                  throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_VendorWithdrawData"));
              else
              {
                  item.Status = "D";
                  if (string.IsNullOrEmpty(item.ReplyContent))
                      item.ReplyContent = ResouceManager.GetMessageString("MKT.Comment", "Comment_VendorReject");
                  if (productConsultDA.SendSSBForApproveProductConsultRelease(item) == 0)
                      throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_ExcuProError！"));//记操作日志
              }
              // 发送待办消息
              EventPublisher.Publish<ConsultReplyAuditRefuseMessage>(new ConsultReplyAuditRefuseMessage
              {
                  SysNo = item.SysNo.Value,
                  CurrentUserSysNo = ServiceContext.Current.UserSysNo
              });
          });
        }

        /// <summary>
        /// 获取关于咨询的所有回复，除去厂商回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public virtual List<ProductConsultReply> GetProductConsultReplyList(int consultSysNo)
        {
            return productConsultDA.GetProductConsultReplyList(consultSysNo);
        }

        /// <summary>
        /// 获取厂商关于咨询的回复列表
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public virtual List<ProductConsultReply> GetProductConsultFactoryReplyList(int consultSysNo)
        {
            return productConsultDA.GetProductConsultFactoryReplyList(consultSysNo);
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyValid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
              {
                  productConsultDA.BatchSetProductConsultReplyStatus(items, "A");

                  foreach (int sysno in items)
                  {
                      EventPublisher.Publish<ConsultReplyAuditMessage>(new ConsultReplyAuditMessage
                      {
                          SysNo = sysno,
                          CurrentUserSysNo = ServiceContext.Current.UserSysNo
                      });
                  }
              });
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyInvalid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
          {
              productConsultDA.BatchSetProductConsultReplyStatus(items, "D");

              foreach (int sysno in items)
              {
                  EventPublisher.Publish<ConsultReplyVoidMessage>(new ConsultReplyVoidMessage
                  {
                      SysNo = sysno,
                      CurrentUserSysNo = ServiceContext.Current.UserSysNo
                  });
              }
          });
        }

        /// <summary>
        /// 批量阅读
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyRead(List<int> items)
        {
            productConsultDA.BatchSetProductConsultReplyStatus(items, "E");
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductConsultReplyTop(List<int> items)
        {
            productConsultDA.BatchSetProductConsultReplyStatus(items, "Y");
        }

        /// <summary>
        /// 批量取消置顶
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchCancelProductConsultReplyTop(List<int> items)
        {
            productConsultDA.BatchSetProductConsultReplyStatus(items, "N");
        }

        #endregion
    }
}
