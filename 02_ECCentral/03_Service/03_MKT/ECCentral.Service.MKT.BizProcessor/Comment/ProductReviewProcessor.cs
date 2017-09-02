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

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductReviewProcessor))]
    public class ProductReviewProcessor
    {
        private IProductReviewDA productReviewDA = ObjectFactory<IProductReviewDA>.Instance;

        private IProductReviewMailLogDA productReviewMailLogDA = ObjectFactory<IProductReviewMailLogDA>.Instance;
        #region 产品评论

        #region 有关评论所使用到的邮件LOG

        /// <summary>
        /// 回复邮件操作，并发送邮件
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductReviewMailLog(ProductReview item)
        {
            //保存邮件日志
            if (!string.IsNullOrEmpty(item.ProductReviewMailLog.TopicMailContent.Content) || !string.IsNullOrEmpty(item.ProductReviewMailLog.CSNote.Content))
            {
                if (productReviewMailLogDA.CheckProductCommentMailLog(item.ProductReviewMailLog))
                    productReviewMailLogDA.UpdateProductCommentMailLog(item.ProductReviewMailLog);
                else
                    productReviewMailLogDA.CreateProductCommentMailLog(item.ProductReviewMailLog);
            }
            productReviewDA.UpdateProductReview(item);

            #region 发送邮件
            KeyValueVariables replaceVariables = new KeyValueVariables();

            ECCentral.BizEntity.IM.ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
            replaceVariables.AddKeyValue(@"ProductID", product.ProductID);
            replaceVariables.AddKeyValue(@"ProductName", product.ProductName);

            replaceVariables.AddKeyValue(@"ProductLink", item.ProductID);
            replaceVariables.AddKeyValue(@"Title", item.Title);
            replaceVariables.AddKeyValue(@"ProductContent", string.Format(ResouceManager.GetMessageString("MKT.Comment", "Comment_ProductReviewMailMainContent"), item.Prons, item.Cons, item.Service));

            //replaceVariables.AddKeyValue(@"#InUser#", ServiceContext.Current.);
            //replaceVariables.AddKeyValue(@"#InDateAll#", DateTime.Now.ToString());
            replaceVariables.AddKeyValue(@"InDateAll-Y", DateTime.Now.Year.ToString());
            replaceVariables.AddKeyValue(@"InDateAll-M", DateTime.Now.Month.ToString());
            replaceVariables.AddKeyValue(@"InDateAll-D", DateTime.Now.Day.ToString());

            replaceVariables.AddKeyValue(@"EmailText", item.ProductReviewMailLog.TopicMailContent.Content);
            replaceVariables.AddKeyValue(@"All", DateTime.Now.ToString());
            replaceVariables.AddKeyValue(@"InDate-Y", DateTime.Now.Year.ToString());
            replaceVariables.AddKeyValue(@"InDate-M", DateTime.Now.Month.ToString());
            replaceVariables.AddKeyValue(@"InDate-D", DateTime.Now.Day.ToString());
            replaceVariables.AddKeyValue(@"Year", DateTime.Now.Year.ToString());

            ECCentral.BizEntity.Customer.CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(item.CustomerSysNo.Value);
            replaceVariables.AddKeyValue(@"CustomerName", customer.BasicInfo.CustomerID);
            if (string.IsNullOrEmpty(customer.BasicInfo.Email))
                //throw new BizException("邮件地址为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductReview", "ProductReview_EmailNotNull"));
            else
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(customer.BasicInfo.Email, "MKT_ProductReviewMailContent", replaceVariables, false);
            #endregion
        }

        #endregion

        /// <summary>
        /// 批量审核产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewValid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
          {
              productReviewDA.BatchSetProductReviewStatus(items, "A");

              foreach (int sysno in items)
              {
                  EventPublisher.Publish<ProductReviewAuditMessage>(new ProductReviewAuditMessage
                  {
                      SysNo = sysno,
                      CurrentUserSysNo = ServiceContext.Current.UserSysNo
                  });
              }
          });
        }

        /// <summary>
        /// 批量作废产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewInvalid(List<int> items)
        {
            productReviewDA.BatchSetProductReviewStatus(items, "D");
        }

        /// <summary>
        /// 批量阅读产品评论
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewRead(List<int> items)
        {
            productReviewDA.BatchSetProductReviewStatus(items, "E");
        }

        /// <summary>
        /// 保存备注
        /// </summary>
        /// <param name="item"></param>
        public virtual void SaveProductReviewRemark(ProductReview item)
        {
            if (item.IsIndexHotReview == YNStatus.Yes)//操作首页热评，更新或新建
                productReviewDA.UpdateHomepageForProductReview(item.SysNo.Value, "H");
            else
                productReviewDA.DeleteHomepageForProductReview(item.SysNo.Value, "H");

            if (item.IsServiceHotReview == YNStatus.Yes)//操作首页服务热评，更新或新建
                productReviewDA.UpdateHomepageForProductReview(item.SysNo.Value, "S");
            else
                productReviewDA.DeleteHomepageForProductReview(item.SysNo.Value, "S");


            //保存邮件日志
            if (!string.IsNullOrEmpty(item.ProductReviewMailLog.TopicMailContent.Content) || !string.IsNullOrEmpty(item.ProductReviewMailLog.CSNote.Content))
            {
                if (productReviewMailLogDA.CheckProductCommentMailLog(item.ProductReviewMailLog))
                    productReviewMailLogDA.UpdateProductCommentMailLog(item.ProductReviewMailLog);
                else
                    productReviewMailLogDA.CreateProductCommentMailLog(item.ProductReviewMailLog);
            }

            productReviewDA.UpdateProductReview(item);
        }


        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="item"></param>
        public virtual void SubmitReplyToCSProcess(ECCentral.BizEntity.SO.SOComplaintCotentInfo item)
        {
            ECCentral.BizEntity.Customer.CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(item.CustomerSysNo.Value);
            item.CustomerEmail = customer.BasicInfo.Email;
            item.CustomerName = customer.BasicInfo.CustomerName;
            item.CustomerPhone = customer.BasicInfo.CellPhone;
            ExternalDomainBroker.AddComplain(item);
        }

        /// <summary>
        /// 根据评论编号，加载相应的评论回复。
        /// </summary>
        /// <param name="itemID"></param>
        public virtual ProductReview LoadProductReview(int itemID)
        {
            ProductReview item = productReviewDA.LoadProductReview(itemID);//评论主题
            item.ProductReviewReplyList = productReviewDA.GetProductReviewReplyList(itemID);//评论回复列表
            item.VendorReplyList = productReviewDA.GetProductReviewFactoryReply(itemID);//厂商回复
            item.ProductReviewMailLog = productReviewMailLogDA.QueryProductCommentMailLog(itemID, "R");
            return item;
        }

        public virtual void DeleteProductReviewImage(string image)
        {
            productReviewDA.DeleteProductReviewImage(image);
        }

        #endregion

        #region 产品评论—回复
        /// <summary>
        /// 批量审核通过产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyValid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
          {
              productReviewDA.BatchSetProductReviewReplyStatus(items, "A");

              foreach (int sysno in items)
              {
                  EventPublisher.Publish<ProductReviewReplyAuditMessage>(new ProductReviewReplyAuditMessage
                  {
                      SysNo = sysno,
                      CurrentUserSysNo = ServiceContext.Current.UserSysNo
                  });
              }
          });
        }

        /// <summary>
        /// 批量作废产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyInvalid(List<int> items)
        {
            TransactionScopeFactory.TransactionAction(() =>
          {
              productReviewDA.BatchSetProductReviewReplyStatus(items, "D");

              foreach (int sysno in items)
              {
                  EventPublisher.Publish<ProductReviewVoidMessage>(new ProductReviewVoidMessage
                  {
                      SysNo = sysno,
                      CurrentUserSysNo = ServiceContext.Current.UserSysNo
                  });
              }
          });
        }

        /// <summary>
        /// 批量阅读产品评论—回复
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetProductReviewReplyRead(List<int> items)
        {
            productReviewDA.BatchSetProductReviewReplyStatus(items, "E");
        }

        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        ///1.	网友回复，需通过审核才展示。
        ///2.	厂商回复（通过Seller Portal），需通过审核才展示。
        ///3.	IPP系统中回复，默认直接展示。
        /// </summary>
        public virtual void AddProductReviewReply(ProductReviewReply item)
        {
            productReviewDA.AddProductReviewReply(item);
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductReviewVendorReplyStatus(ProductReviewReply item)
        {
            TransactionScopeFactory.TransactionAction(() =>
            {
                int x = productReviewDA.UpdateProductReviewVendorReplyStatus(item);

                switch (item.Status)
                {
                    //发布
                    case "A":
                        EventPublisher.Publish<ProductReviewReplyAuditMessage>(new ProductReviewReplyAuditMessage
                        {
                            SysNo = item.SysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        break;
                    //拒绝
                    case "D":
                        EventPublisher.Publish<ProductReviewReplyVoidMessage>(new ProductReviewReplyVoidMessage
                        {
                            SysNo = item.SysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        break;
                }


                if (x == 1001)
                    throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_VendorWithdrawData"));
                else if (x == 1002)
                    throw new BizException(ResouceManager.GetMessageString("MKT.Comment", "Comment_SubmitFailed"));
            });
        }
        #endregion

    }
}
