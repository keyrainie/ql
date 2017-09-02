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
    [VersionExport(typeof(ProductReviewMailLogProcessor))]
    public class ProductReviewMailLogProcessor
    {
        private IProductReviewMailLogDA productReviewMailLogDA = ObjectFactory<IProductReviewMailLogDA>.Instance;


        
        #region 有关评论所使用到的邮件LOG

        /// <summary>
        /// 获取关于产品评论的邮件log列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductReviewMailLog QueryProductReviewMailLog(int sysNo)
        {
            return productReviewMailLogDA.QueryProductCommentMailLog(sysNo, "R");//产品评论
        }

        /// <summary>
        /// 回复邮件操作，并发送邮件
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductConsultMailLog(ProductReview item)
        {
            if (productReviewMailLogDA.CheckProductCommentMailLog(item.ProductReviewMailLog))
                productReviewMailLogDA.UpdateProductCommentMailLog(item.ProductReviewMailLog);
            else
                productReviewMailLogDA.CreateProductCommentMailLog(item.ProductReviewMailLog);

            #region 发送邮件
            KeyValueVariables replaceVariables = new KeyValueVariables();

            ECCentral.BizEntity.IM.ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
            replaceVariables.AddKeyValue(@"ProductID", product.ProductID);
            replaceVariables.AddKeyValue(@"ProductName", product.ProductName);

            replaceVariables.AddKeyValue(@"Content", item.ProductReviewMailLog.Content);
            replaceVariables.AddKeyValue(@"EmailText", item.ProductReviewMailLog.TopicMailContent.Content);
            replaceVariables.AddKeyValue(@"All", DateTime.Now.ToString());
            replaceVariables.AddKeyValue(@"InDate-Y", DateTime.Now.Year.ToString());
            replaceVariables.AddKeyValue(@"InDate-M", DateTime.Now.Month.ToString());
            replaceVariables.AddKeyValue(@"InDate-D", DateTime.Now.Day.ToString());
            replaceVariables.AddKeyValue(@"Year", DateTime.Now.Year.ToString());

            ECCentral.BizEntity.Customer.CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(item.CustomerSysNo.Value);
            replaceVariables.AddKeyValue(@"CustomerName", customer.BasicInfo.CustomerID);
            if (string.IsNullOrEmpty(customer.BasicInfo.Email))
                throw new BizException("邮件地址为空！");
            else
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(customer.BasicInfo.Email, "MKT_ProductConsultMailContent", replaceVariables, false);
            #endregion
        }

        /// <summary>
        /// 获取关于咨询的邮件log列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductReviewMailLog QueryProductConsultMailLog(int sysNo)
        {
            return productReviewMailLogDA.QueryProductCommentMailLog(sysNo, "C");//产品咨询
        }
        #endregion

    }
}
