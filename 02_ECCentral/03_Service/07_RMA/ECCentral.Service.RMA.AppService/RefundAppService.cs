using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.EventConsumer;
using System.Xml.Linq;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(RefundAppService))]
    public class RefundAppService
    {

        #region Readonly Varable

        private readonly string FROMSERVICE = "http://soa.newegg.com/SOA/USA/CrossApplication/V30/E5DBS01/VP_ProcessControlPanelMessageService";
        //private readonly List<string> TOSERVICELIST = new List<string>() { "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService" };
        private readonly string TOSERVICELIST = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService";
        private readonly string ArticleCategory = "VendorPortal";
        private readonly string ArticleType1 = "ControlPanelManagement";
        private readonly string ArticleType2 = "Download";
        #endregion

        public virtual RefundInfo Create(RefundInfo entity)
        {
            return ObjectFactory<RefundProcessor>.Instance.Create(entity);
        }

        public virtual void Update(RefundInfo entity)
        {
            ObjectFactory<RefundProcessor>.Instance.Update(entity);
        }

        public virtual List<int> GetWaitingSOForRefund()
        {
            return ObjectFactory<RefundProcessor>.Instance.GetWaitingSOForRefund();
        }

        public virtual List<CodeNamePair> GetRefundReasons()
        {
            return ObjectFactory<RefundProcessor>.Instance.GetRefundReasons();
        }

        public virtual RefundInfo LoadBySysNo(int sysNo, out string customerName,
            out CustomerContactInfo contactInfo,
            out PromotionCode_Customer_Log promotionCodeLog)
        {
            RefundInfo refund = ObjectFactory<RefundProcessor>.Instance.LoadWithItemsBySysNo(sysNo);
            CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(refund.CustomerSysNo.Value);
            customerName = customer.BasicInfo.CustomerName;
            refund.IncomeBankInfo = ExternalDomainBroker.GetSOIncomeRefundInfo(sysNo, RefundOrderType.RO);
            contactInfo = null;
            if (refund.RefundPayType == RefundPayType.BankRefund && refund.IncomeBankInfo != null
                && string.IsNullOrEmpty(refund.IncomeBankInfo.CardNumber))
            {
                if (refund.RefundItems != null && refund.RefundItems.Count > 0)
                {
                    RMARequestInfo request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(refund.RefundItems[0].RegisterSysNo.Value);
                    contactInfo = ObjectFactory<CustomerContactProcessor>.Instance.LoadByRequestSysNo(request.SysNo.Value);
                }
            }
            promotionCodeLog = ExternalDomainBroker.GetPromotionCodeLog(refund.SOSysNo.Value);
            if (promotionCodeLog != null)
            {
                promotionCodeLog.UsedOrderSysNo = ExternalDomainBroker.GetSOSysNoByCouponSysNo(promotionCodeLog.CouponCodeSysNo.Value);
            }
            return refund;
        }

        public virtual RefundInfo Calculate(RefundInfo info)
        {
            return ObjectFactory<RefundProcessor>.Instance.Calculate(info);
        }

        private string BuildSendMessageXml(SSBMessageHeader ssbHeader, IPPRequestRegisterStatusChangeSSBEntity ssbBody)
        {
            string bodyStr = ssbBody.ToXmlString();
            bodyStr = RemoveNameSpace(bodyStr);
            string headerStr = ssbHeader.ToXmlString();
            headerStr = RemoveNameSpace(headerStr);
            //this.BuildMessageHeaderXml();
            //this.BuildMessageBodyXml();
            return string.Format("<RequestRoot>{0}</RequestRoot>", (headerStr + string.Format("<Body>{0}</Body>", bodyStr)));
        }

        private string RemoveNameSpace(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            foreach (XElement element in document.Root.DescendantsAndSelf())
            {
                if (element.Name.Namespace != XNamespace.None)
                {
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);
                }
                if (element.Attributes().Where<XAttribute>(delegate(XAttribute a)
                {
                    if (!a.IsNamespaceDeclaration)
                    {
                        return (a.Name.Namespace != XNamespace.None);
                    }
                    return true;
                }).Any<XAttribute>())
                {
                    element.ReplaceAttributes(element.Attributes().Select<XAttribute, XAttribute>(delegate(XAttribute a)
                    {
                        if (a.IsNamespaceDeclaration)
                        {
                            return null;
                        }
                        if (!(a.Name.Namespace != XNamespace.None))
                        {
                            return a;
                        }
                        return new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value);
                    }));
                }
            }
            return document.ToString();
        }

        public virtual RefundInfo SubmitAudit(RefundInfo entity)
        {
            return ObjectFactory<RefundProcessor>.Instance.SubmitAudit(entity);
        }

        public virtual RefundInfo CancelSubmitAudit(RefundInfo entity)
        {
            return ObjectFactory<RefundProcessor>.Instance.CancelSubmitAudit(entity);
        }

        public virtual RefundInfo Abandon(int sysNo)
        {
            return ObjectFactory<RefundProcessor>.Instance.Abandon(sysNo);
        }

        public virtual RefundInfo Refund(int sysNo)
        {
            RefundInfo entity = ObjectFactory<RefundProcessor>.Instance.Refund(sysNo);

            ///获取退款单对应的单件信息
            List<RegisterForRefund> list = null;
            if (entity != null && entity.SysNo.HasValue)
            {
                list = ObjectFactory<RefundProcessor>.Instance.GetRegistersForRefund(entity.SysNo.Value);
                if (list != null && list.Count > 0)
                {
                    #region SSB
                    if (list != null)
                    {
                        // 获取申请单信息
                        int? registerSysNo = list[0].RegisterSysNo;
                        RMARequestInfo requestEntity = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(registerSysNo.Value);
                        if (requestEntity != null)
                        {

                            if (requestEntity.ShippingType != null && requestEntity.StockType != null && requestEntity.InvoiceType != null)
                            {
                                //退款完成后2,3,5,6,向VP发送已完成的SSB；
                                if (
                                    (requestEntity.InvoiceType == InvoiceType.SELF && requestEntity.StockType == StockType.MET && requestEntity.ShippingType == DeliveryType.SELF)
                                    ||
                                     (requestEntity.InvoiceType == InvoiceType.SELF && requestEntity.StockType == StockType.MET && requestEntity.ShippingType == DeliveryType.MET)
                                    ||
                                     (requestEntity.InvoiceType == InvoiceType.MET && requestEntity.StockType == StockType.MET && requestEntity.ShippingType == DeliveryType.SELF)
                                    ||
                                     (requestEntity.InvoiceType == InvoiceType.MET && requestEntity.StockType == StockType.MET && requestEntity.ShippingType == DeliveryType.MET)
                                    )
                                {
                                    IPPRequestRegisterStatusChangeSSBEntity ssb = new IPPRequestRegisterStatusChangeSSBEntity();
                                    ssb.RequestSysNo = requestEntity.SysNo.Value;
                                    ssb.Status = "CMP";
                                    ssb.RegisterInfoList = new List<RegisterInfo>();
                                    foreach (var item in list)
                                    {
                                        ssb.RegisterInfoList.Add(new RegisterInfo() { RegisterSysNo = item.RegisterSysNo.Value });
                                    }

                                    SSBMessageHeader ssbHeader = new SSBMessageHeader()
                                    {
                                        Language = "CH",
                                        Sender = "VendorPortal",
                                        CompanyCode = "8601",
                                        Action = "Update",
                                        Version = "1.0",
                                        Type = "IPPRequestRegisterStatusChange",
                                        OriginalGUID = Guid.NewGuid().ToString()
                                    };

                                    SSBSender.SendV3(
                                        FROMSERVICE,
                                        TOSERVICELIST,
                                        ArticleCategory,
                                        ArticleType1,
                                        ArticleType2,
                                        this.BuildSendMessageXml(ssbHeader, ssb),
                                        "NCService");
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            return entity;
        }

        public virtual RefundInfo CancelRefund(int sysNo)
        {
            return ObjectFactory<RefundProcessor>.Instance.CancelRefund(sysNo);
        }

        public virtual RefundInfo UpdateFinanceNote(RefundInfo entity)
        {
            return ObjectFactory<RefundProcessor>.Instance.UpdateFinanceNote(entity);
        }

        public virtual void GetShipFee(RefundInfo entity,
            out decimal totalAmt,
            out decimal premiumAmt,
            out decimal shippingCharge,
            out decimal payPrice,
            out decimal historyRefund)
        {
            ObjectFactory<RefundProcessor>.Instance.GetShipFee(entity,
                out totalAmt,
                out premiumAmt,
                out shippingCharge,
                out payPrice,
                out historyRefund);
        }

        public virtual List<RefundInfo> CreateRefundForRMAAuto(AutoRefundInfo entity)
        {
            return ObjectFactory<AutoRefundProcessor>.Instance.CreateRefund(entity);
        }
    }
}
