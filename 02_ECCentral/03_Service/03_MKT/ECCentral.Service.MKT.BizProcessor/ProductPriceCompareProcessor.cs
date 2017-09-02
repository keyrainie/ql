using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductPriceCompareProcessor))]
    public class ProductPriceCompareProcessor
    {
        private IProductPriceCompareDA _ProductPriceCompareDA = ObjectFactory<IProductPriceCompareDA>.Instance;

        public List<CodeNamePair> GetInvalidReasons()
        {
            return CodeNamePairManager.GetList("MKT", "ProductPriceCompare_InvalidReason");
        }

        public ProductPriceCompareEntity Load(int sysNo)
        {
            var productPriceCompare = _ProductPriceCompareDA.Load(sysNo);

            var productInfo = ExternalDomainBroker.GetProductInfo(productPriceCompare.ProductSysNo);
            if (productInfo != null)
            {
                productPriceCompare.ProductID = productInfo.ProductID;
                productPriceCompare.ProductName = productInfo.ProductBasicInfo.ProductBriefName;
            }
            var customerInfo = ExternalDomainBroker.GetCustomerInfo(productPriceCompare.CustomerSysNo);
            if (customerInfo != null)
            {
                productPriceCompare.CustomerID = customerInfo.BasicInfo.CustomerID;
                productPriceCompare.CustomerName = customerInfo.BasicInfo.CustomerName;
                productPriceCompare.CustomerEmail = customerInfo.BasicInfo.Email;
            }

            return productPriceCompare;
        }

        //价格举报有效
        public void UpdateProductPriceCompareValid(int sysNo)
        {
            var productPriceCompare = this.Load(sysNo);
            if (productPriceCompare.Status != ProductPriceCompareStatus.WaitAudit)
            {
                //throw new BizException("只有待审核的记录可以审核通过。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductPriceCompare", "ProductPriceCompare_CannotAuditPass"));
            }
            using (TransactionScope ts = new TransactionScope())
            {
                productPriceCompare.Status = ProductPriceCompareStatus.AuditPass;
                productPriceCompare.DisplayLinkStatus = DisplayLinkStatus.Display;
                _ProductPriceCompareDA.UpdateProductPriceCompareValid(productPriceCompare);

                //发邮件通知客户
                SendEmailValid(productPriceCompare.CustomerID, productPriceCompare.CustomerEmail, productPriceCompare.ProductName);

                ts.Complete();
            }
        }

        //价格举报无效
        public void UpdateProductPriceCompareInvalid(int sysNo, string commaSeperatedReasonCodes)
        {
            var productPriceCompare = this.Load(sysNo);
            if (productPriceCompare.Status != ProductPriceCompareStatus.WaitAudit)
            {
                //throw new BizException("只有待审核的记录可以审核拒绝。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductPriceCompare", "ProductPriceCompare_CannotAuditDecline"));
            }
            //根据逗号分隔的id得到相应的描述，并发送邮件
            var ids = commaSeperatedReasonCodes.Split(",".ToCharArray());
            var reasonCodeNames = this.GetInvalidReasons();
            string reasons = "";
            foreach (var id in ids)
            {
                var foundReason = reasonCodeNames.FirstOrDefault(item => item.Code == id);
                if (foundReason != null)
                {
                    reasons += "<br/>" + foundReason.Name;
                }
            }
            using (TransactionScope ts = new TransactionScope())
            {
                productPriceCompare.Status = ProductPriceCompareStatus.AuditDecline;
                productPriceCompare.InvalidReason = 0;

                _ProductPriceCompareDA.UpdateProductPriceCompareInvalid(productPriceCompare);

                //发邮件通知客户
                SendEmailInvalid(productPriceCompare.CustomerID, productPriceCompare.CustomerEmail, productPriceCompare.ProductName, commaSeperatedReasonCodes);

                ts.Complete();
            }
        }

        //价格举报恢复
        public void UpdateProductPriceCompareResetLinkShow(int sysNo)
        {
            var productPriceCompare = this.Load(sysNo);
            productPriceCompare.DisplayLinkStatus = DisplayLinkStatus.Hide;
            _ProductPriceCompareDA.UpdateProductPriceCompareResetLinkShow(productPriceCompare);
        }

        private void SendEmailValid(string customerID, string customerEmail, string productName)
        {
            if (string.IsNullOrEmpty(customerEmail))
            {
                return;
            }

            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("ProductName", productName);
            vars.Add("CustomerID", customerID);
            vars.Add("Year", DateTime.Now.Year);
            vars.Add("Month", DateTime.Now.Month);
            vars.Add("Day", DateTime.Now.Day);
            EmailHelper.SendEmailByTemplate(customerEmail, "ProductPriceCompare_AuditPass", vars, false);

        }

        private void SendEmailInvalid(string customerID, string customerEmail, string productName, string invalidReason)
        {
            if (string.IsNullOrEmpty(customerEmail))
            {
                return;
            }

            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("ProductName", productName);
            vars.Add("CustomerID", customerID);
            vars.Add("InvalidReason", invalidReason);
            vars.Add("Year", DateTime.Now.Year);
            vars.Add("Month", DateTime.Now.Month);
            vars.Add("Day", DateTime.Now.Day);
            EmailHelper.SendEmailByTemplate(customerEmail, "ProductPriceCompare_AuditDecline", vars, false);
        }


    }
}
