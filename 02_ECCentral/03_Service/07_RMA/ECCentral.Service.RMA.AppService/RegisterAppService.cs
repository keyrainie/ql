using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using System.Data;
using ECCentral.Service.EventConsumer;
using System.Xml.Linq;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(RegisterAppService))]
    public class RegisterAppService
    {
        #region "SSB Constant"
        private readonly string FROMSERVICE = "http://soa.newegg.com/SOA/USA/CrossApplication/V30/E5DBS01/VP_ProcessControlPanelMessageService";
        //private readonly List<string> TOSERVICELIST = new List<string>() { "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService" };
        private readonly string TOSERVICELIST = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService";
       
        private readonly string     ArticleCategory = "VendorPortal";
        private readonly string     ArticleType1 = "ControlPanelManagement";
        private readonly string ArticleType2 = "Download";
       
        #endregion
        public virtual void Update(RMARegisterInfo register)
        {
            ObjectFactory<RegisterProcessor>.Instance.Update(register);
        }

        public virtual RMARegisterInfo UpdateBasicInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.UpdateBasicInfo(register);
        }

        public virtual RMARegisterInfo UpdateCheckInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.UpdateCheckInfo(register);
        }

        public virtual RMARegisterInfo UpdateResponseInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.UpdateResponseInfo(register);
        }

        public virtual RMARegisterInfo LoadForEditBySysNo(int sysNo,
            out string businessModel,
            out ProcessType processType,
            out InvoiceType? invoiceType,
            out CustomerInfo customerInfo,
            out RMARequestInfo requestInfo,
            out List<ProductInventoryInfo> productInventoryInfo,
            out int? refundSysNo,
            out ProductInventoryType inventoryType)
        {
            RMARegisterInfo register = ObjectFactory<RegisterProcessor>.Instance.LoadForEditBySysNo(sysNo);

            refundSysNo = default(int?);
            if (register.BasicInfo.RefundStatus != null)
            {
                refundSysNo = ObjectFactory<RefundProcessor>.Instance.GetRefundSysNoByRegisterSysNo(register.SysNo.Value);
            }
            var request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(register.SysNo.Value);

            requestInfo = request;

            businessModel = ObjectFactory<RequestProcessor>.Instance.GetBusinessModel(request);

            processType = ProcessType.UnKnown;
            invoiceType = request.InvoiceType;
            inventoryType = ProductInventoryType.Company;
            //设置商家处理或泰隆优选处理
            if (request.InvoiceType.HasValue && request.ShippingType.HasValue && request.StockType.HasValue)
            {
                if (request.InvoiceType == InvoiceType.SELF && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.StockType == BizEntity.Invoice.StockType.SELF)
                {
                    processType = ProcessType.NEG;
                }
                else if (
                     (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.SELF)
                     ||
                     (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.MET)
                    ||
                      (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.SELF)
                    ||
                     (request.StockType == BizEntity.Invoice.StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.MET)
                    )
                {
                    processType = ProcessType.MET;
                }
            }

            //获取商品信息
            var product = ExternalDomainBroker.GetProductInfo(register.BasicInfo.ProductSysNo.Value);
            if (product != null)
            {
                register.BasicInfo.ProductID = product.ProductID;
                register.BasicInfo.ProductName = product.ProductName;
                inventoryType = product.InventoryType;
            }

            if (register.RevertInfo.RevertProductSysNo != null)
            {
                if (register.RevertInfo.RevertProductSysNo == register.BasicInfo.ProductSysNo)//如果是同一Item，不用再次查询IM接口
                {
                    register.RevertInfo.RevertProductID = register.BasicInfo.ProductID;
                }
                else
                {
                    var revertProduct = ExternalDomainBroker.GetProductInfo(register.RevertInfo.RevertProductSysNo.Value);
                    if (revertProduct != null)
                    {
                        register.RevertInfo.RevertProductID = revertProduct.ProductID;
                    }
                }
            }
            customerInfo = ExternalDomainBroker.GetCustomerInfo(request.CustomerSysNo.Value);

            //获取库存信息以及二手品信息
            var rPorductList = GetSecondProductInfoList(register.BasicInfo.ProductID);
            List<ProductInventoryInfo> inventoryInfoList = new List<ProductInventoryInfo>();
            foreach (var obj in rPorductList)
            {
                inventoryInfoList.Add(ObjectFactory<RegisterProcessor>.Instance.GetWarehouseProducts(processType, int.Parse(obj.SysNo.ToString()), register.BasicInfo.ShippedWarehouse)[0]);
            }
            
            productInventoryInfo = ObjectFactory<RegisterProcessor>.Instance.GetWarehouseProducts(processType, register.BasicInfo.ProductSysNo.Value, register.BasicInfo.ShippedWarehouse);
            if (productInventoryInfo != null && productInventoryInfo.Count > 0)
            {
                productInventoryInfo.ForEach(p =>
                {
                    p.ProductID = register.BasicInfo.ProductID;
                });
                if (inventoryInfoList != null && inventoryInfoList.Count > 0)
                {
                    for (int i = 0; i < inventoryInfoList.Count; i++)
                    {
                        inventoryInfoList[i].ProductID = rPorductList[i].ProductID;
                        productInventoryInfo.Add(inventoryInfoList[i]);
                    }
                }
            }
            return register;
        }

        public virtual List<ProductInfo> GetSecondProductInfoList(string ProductID)
        {
            return ExternalDomainBroker.GetSimpleProductList(ProductID + "R");
        }

        public virtual RMARegisterInfo SetWaitingReturn(int sysNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.SetWaitingReturn(sysNo);
        }

        public virtual void CancelWaitingReturn(int sysNo)
        {
            ObjectFactory<RegisterProcessor>.Instance.CancelWaitingReturn(sysNo);
        }

        public virtual RMARegisterInfo SetWaitingOutbound(int sysNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.SetWaitingOutbound(sysNo);
        }

        public virtual void CancelWaitingOutbound(int sysNo)
        {
            ObjectFactory<RegisterProcessor>.Instance.CancelWaitingOutbound(sysNo);
        }


        public virtual RMARegisterInfo SetWaitingRefund(int sysNo)
        {
            //#warning Send SSB To SellerPortal
           // return ObjectFactory<RegisterProcessor>.Instance.SetWaitingRefund(sysNo);
            RMARegisterInfo request=  ObjectFactory<RegisterProcessor>.Instance.SetWaitingRefund(sysNo);
            if (request != null)
            {
                IPPRequestRegisterStatusChangeSSBEntity ssbBody = new IPPRequestRegisterStatusChangeSSBEntity();
                ssbBody.RequestSysNo = request.SysNo.Value;
                ssbBody.Status = "Refund";
                ssbBody.RegisterInfoList = new List<RegisterInfo>();
                ssbBody.RegisterInfoList.Add(new RegisterInfo() { RegisterSysNo = sysNo });

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
                    FROMSERVICE
                    , TOSERVICELIST
                    , ArticleCategory
                    , ArticleType1
                    , ArticleType2
                    , this.BuildSendMessageXml(ssbHeader, ssbBody)
                    , "NCService"

               );
            }
            return request;
           
        }

        public virtual void CancelWaitingRefund(int sysNo)
        {
            //#warning Send SSB To SellerPortal
            var opResult=ObjectFactory<RegisterProcessor>.Instance.CancelWaitingRefund(sysNo);
            RMARequestInfo request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(sysNo);
            if (opResult && request != null)
            {
                IPPRequestRegisterStatusChangeSSBEntity ssbBody = new IPPRequestRegisterStatusChangeSSBEntity();
                ssbBody.RequestSysNo = request.SysNo.Value;
                ssbBody.Status = "Callback";
                ssbBody.RegisterInfoList = new List<RegisterInfo>();
                ssbBody.RegisterInfoList.Add(new RegisterInfo() { RegisterSysNo = sysNo });

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
                    FROMSERVICE
                    , TOSERVICELIST
                    , ArticleCategory
                    , ArticleType1
                    , ArticleType2
                    , this.BuildSendMessageXml(ssbHeader, ssbBody)
                    , "NCService"

               );
            }
            //return request;
        }

        public virtual RMARegisterInfo SetWaitingRevert(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.SetWaitingRevert(register);
        }

        public virtual void CancelWaitingRevert(RMARegisterInfo register)
        {
            ObjectFactory<RegisterProcessor>.Instance.CancelWaitingRevert(register.SysNo.Value, false);
        }

        public virtual RMARegisterInfo ApproveRevertAudit(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.RevertAudit(register, true);
        }

        public virtual RMARegisterInfo RejectRevertAudit(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterProcessor>.Instance.RevertAudit(register, false);
        }

        public virtual RMARegisterInfo Close(int sysNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.Close(sysNo);
        }

        public virtual RMARegisterInfo CloseCase(int sysNo)
        {
            //#warning Send SSB To SellerPortal
           
            //return ObjectFactory<RegisterProcessor>.Instance.Close(sysNo);
            var result= ObjectFactory<RegisterProcessor>.Instance.Close(sysNo);
             RMARequestInfo request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(sysNo);
             if (request != null)
             {
                 IPPRequestRegisterStatusChangeSSBEntity ssbBody = new IPPRequestRegisterStatusChangeSSBEntity();
                 ssbBody.RequestSysNo = request.SysNo.Value;
                 ssbBody.Status = "CMP";
                 ssbBody.RegisterInfoList = new List<RegisterInfo>();
                 ssbBody.RegisterInfoList.Add(new RegisterInfo() { RegisterSysNo = sysNo });

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
                     FROMSERVICE
                     , TOSERVICELIST
                     , ArticleCategory
                     , ArticleType1
                     , ArticleType2
                     , this.BuildSendMessageXml(ssbHeader,ssbBody)
                     , "NCService"
                     
                );
             }
             return result;

        }

        private string BuildSendMessageXml(SSBMessageHeader ssbHeader,IPPRequestRegisterStatusChangeSSBEntity ssbBody)
        {
            string bodyStr=ssbBody.ToXmlString();
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



        public virtual RMARegisterInfo ReOpen(int sysNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.ReOpen(sysNo);
        }

        public virtual RMARegisterInfo SetAbandon(int sysNo)
        {
            
            return ObjectFactory<RegisterProcessor>.Instance.SetAbandon(sysNo);
        }


        public virtual void LoadRegisterMemo(int registerSysNo, ref string memo, ref string productID, ref string productName, ref string vendorName)
        {
            ObjectFactory<RegisterProcessor>.Instance.LoadRegisterMemo(registerSysNo, ref  memo, ref  productID, ref  productName, ref  vendorName);
        }

        public RMARegisterInfo SyncERP(int sysNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.SyncERP(sysNo);
        }
    }
}
