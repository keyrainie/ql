using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Laos;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using System.Configuration;
using ECCentral.Service.IM.IDataAccess;

namespace ECCentral.Service.IM.BizProcessor.IMAOP
{
    [Serializable]
    public sealed class SendEmailAttribute : OnMethodBoundaryAspect
    {
        private SendEmailType EmailType { get; set; }
        public SendEmailAttribute(SendEmailType type)
        {
            this.EmailType = type;
        }
        public override void OnEntry(MethodExecutionEventArgs args)
        {
            base.OnEntry(args);
        }

        public override void OnSuccess(MethodExecutionEventArgs args)
        {
            int requestsysno = 0;
            string ptptdUser = ConfigurationManager.AppSettings["PTPTDUser"];
            string subject = string.Empty;
            ProductPriceRequestInfo entity = new ProductPriceRequestInfo();
            if (EmailType == SendEmailType.ApprovePriceRequest) 
            {
                object[] objs = args.GetReadOnlyArgumentArray();
                entity = objs[0] as ProductPriceRequestInfo;

                requestsysno = entity.SysNo.Value;

                if (entity.RequestStatus == ProductPriceRequestStatus.Deny)//拒绝时的邮件发送
                {
                    subject = ResouceManager.GetMessageString("IM.Product", "AuditRejected");
                }
                else
                {
                    subject = ResouceManager.GetMessageString("IM.Product", "AuditPassed");
                }

                
            }
            else if (EmailType == SendEmailType.CancelPriceRequest)//撤销时的邮件发送
            {
                object[] objs = args.GetReadOnlyArgumentArray();
                entity = objs[0] as ProductPriceRequestInfo;

                requestsysno = entity.SysNo.Value;

                subject = ResouceManager.GetMessageString("IM.Product", "CancelAudit");

            }
            else if(EmailType == SendEmailType.CreatePriceRequest)
            {
                object[] objs = args.GetReadOnlyArgumentArray();
                entity = objs[1] as ProductPriceRequestInfo;

                requestsysno = entity.SysNo.Value;

                subject = ResouceManager.GetMessageString("IM.Product", "RequestAudit");
            }



            ProductPriceRequestInfo requestentity = ObjectFactory<ProductPriceRequestProcessor>.Instance.GetProductPriceRequestInfoBySysNo(requestsysno);


            string toAddress = string.Format("{0};{1}", entity.PMUserEmailAddress, entity.CurrentUserEmailAddress);

            if (!string.IsNullOrEmpty(entity.BackupPMUserEmailAddress))
            {
                toAddress = string.Format("{0};{1}", toAddress, entity.BackupPMUserEmailAddress);
            }
            if (!string.IsNullOrEmpty(ptptdUser))
            {
                toAddress = string.Format("{0};{1}", toAddress, ptptdUser);
            }

            string ccmail = GetCCEmail(requestentity);
            string updatetime = string.Empty;
            if (requestentity.LastUpdateTime.HasValue)
            {
                updatetime = requestentity.LastUpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //参数设置
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.AddKeyValue("Subject",subject);
            keyValueVariables.Add("ProductSysNo", Product.SysNo);
            keyValueVariables.Add("ProductName", Product.ProductName);
            keyValueVariables.Add("UpdateTime", updatetime);
            keyValueVariables.Add("SysNo", requestentity.SysNo);
            keyValueVariables.Add("ProductID", Product.ProductID);
            keyValueVariables.Add("BasicPrice", requestentity.OldPrice.BasicPrice);
            keyValueVariables.Add("BasicPriceNew", requestentity.BasicPrice);
            keyValueVariables.Add("UnitCost", requestentity.UnitCost);
            keyValueVariables.Add("CurrentPrice", requestentity.OldPrice.CurrentPrice);
            keyValueVariables.Add("CurrentPriceNew", requestentity.CurrentPrice);
            keyValueVariables.Add("Discount", requestentity.DiscountAmount);
            keyValueVariables.Add("CashRebate", requestentity.OldPrice.CashRebate);
            keyValueVariables.Add("CashRebateNew", requestentity.CashRebate);
            keyValueVariables.Add("Point", requestentity.OldPrice.Point);
            keyValueVariables.Add("PointNew", requestentity.Point);
            keyValueVariables.Add("ProductPayType", requestentity.OldPrice.PayType.ToString());
            keyValueVariables.Add("MaxPerOrder", requestentity.MinCountPerOrder);
            keyValueVariables.Add("MinMargin", requestentity.OldPrice.Margin);
            keyValueVariables.Add("MinMarginCurrent", requestentity.Margin);
            keyValueVariables.Add("Apply", requestentity.CreateUser.UserName);
            keyValueVariables.Add("PMMemo", requestentity.PMMemo);
            keyValueVariables.Add("TLMemo", requestentity.TLMemo);


            EmailHelper.SendEmailByTemplate(toAddress, ccmail, "", "IM_Product_ChangePrice", keyValueVariables, null, true, true);
            base.OnSuccess(args);
        }

        public override void OnExit(MethodExecutionEventArgs args)
        {
            base.OnExit(args);
        }


        private ProductInfo Product;
        public string GetCCEmail(ProductPriceRequestInfo entity)
        {
            var productPriceRequestDA = ObjectFactory<IProductPriceRequestDA>.Instance;
            Product = ObjectFactory<ProductProcessor>.Instance.GetProductInfo(productPriceRequestDA.GetProductSysNoBySysNo(entity.SysNo.Value));

            string ccMail = string.Empty;
            if (Product.ProductBasicInfo.ProductType == ProductType.OpenBox)
            {
                //decimal UnitCost = AppConst.DecimalNull, CurrentPrice = AppConst.DecimalNull;
                //if (entity.UnitCost == AppConst.DecimalNull || entity.CurrentPrice == AppConst.DecimalNull)
                //{
                //    return string.Empty;
                //}
                
                decimal dcmUnitCost50Percent = decimal.Multiply(entity.UnitCost, 0.5M);// 平均成本的5折
                decimal dcmUnitCost70Percent = decimal.Multiply(entity.UnitCost, 0.7M);// 平均成本的7折
                decimal dcmUnitCost80Percent = decimal.Multiply(entity.UnitCost, 0.8M);//
                if (entity.CurrentPrice < dcmUnitCost50Percent)
                {
                    ccMail = ConfigurationManager.AppSettings["PMLeaderEmail"].ToString();
                    //ProductManager.GetInstance().GetPMEmailForSecondHandProductSetPrice((int)AppEnum.Privilege.PMLeader);
                }
                else if (entity.CurrentPrice >= dcmUnitCost50Percent && entity.CurrentPrice < dcmUnitCost80Percent)
                {
                    ccMail = ConfigurationManager.AppSettings["SecondHandPriceAdjust3Email"].ToString();
                    //ccMail = ProductManager.GetInstance().GetPMEmailForSecondHandProductSetPrice((int)AppEnum.Privilege.SecondHandPriceAdjust3);
                }
                else if (entity.CurrentPrice >= dcmUnitCost80Percent && entity.CurrentPrice < 2 * entity.UnitCost)
                {
                    ccMail = ConfigurationManager.AppSettings["SecondHandPriceAdjust1Email"].ToString();
                    // ccMail = ProductManager.GetInstance().GetPMEmailForSecondHandProductSetPrice((int)AppEnum.Privilege.SecondHandPriceAdjust1);
                }
                else if (entity.CurrentPrice >= 2 * entity.UnitCost)
                {
                    ccMail = ConfigurationManager.AppSettings["SecondHandPriceAdjust1Email"].ToString();
                }
                return ccMail;
            }


            if ((entity.CurrentPrice - (decimal)(entity.Point / 10m)) > 0)
            //if (entity.UnitCost != 0)
            {
                if (entity.Margin > 0)
                {
                    if (((entity.CurrentPrice - (decimal)(entity.Point / 10m) - entity.UnitCost) / (entity.CurrentPrice - (decimal)(entity.Point / 10m))) >= entity.Margin * 0.70M &&
                        ((entity.CurrentPrice - (decimal)(entity.Point / 10m) - entity.UnitCost) / (entity.CurrentPrice - (decimal)(entity.Point / 10m))) < entity.Margin)
                    {
                        ccMail = ConfigurationManager.AppSettings["AuditProductPrice1"];
                    }
                    if (((entity.CurrentPrice - (decimal)(entity.Point / 10m) - entity.UnitCost) / (entity.CurrentPrice - (decimal)(entity.Point / 10m))) < entity.Margin * 0.70M)
                    {
                        ccMail = ConfigurationManager.AppSettings["AuditProductPrice2"];
                        //AppConfig.AuditProductPrice2;
                    }
                    if (entity.CurrentPrice >= 2 * entity.UnitCost)
                    {
                        ccMail += ";" + ConfigurationManager.AppSettings["AuditProductPrice2"];
                    }
                }
                else
                {
                    if (((entity.CurrentPrice - (decimal)(entity.Point / 10m) - entity.UnitCost) / (entity.CurrentPrice - (decimal)(entity.Point / 10m))) < entity.Margin)
                        ccMail = ConfigurationManager.AppSettings["AuditProductPrice2"];
                    //AppConfig.AuditProductPrice2;
                }
            }
            return ccMail;
        }
    }
    public enum SendEmailType
    {
        CreatePriceRequest,
        ApprovePriceRequest,
        UnApprovePriceRequest,
        CancelPriceRequest
    }
}
