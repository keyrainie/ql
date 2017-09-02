using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.EventConsumer.VendorPortal;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Invoice
{
    [TestClass]
    public class InvoiceTest
    {
        [TestMethod]
        public void Test_GetInvoiceMasterInfo()
        {
            var result = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOInvoiceMaster(8938550);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("maaili129521", result[0].CustomerID);
            Assert.AreEqual(789017, result[0].CustomerSysNo);
            Assert.AreEqual(399, result[0].InvoiceAmt);
            Assert.AreEqual(20, result[0].PayTypeSysNo);
            Assert.AreEqual(0, result[0].RMANumber);
            Assert.AreEqual(0, result[0].OriginalInvoiceNumber);
            Assert.AreEqual(51, result[0].StockSysNo);
            //Assert.AreEqual("2010-01-02 07:39:20.153", result[0].OrderDate.ToString());
            //Assert.AreEqual("2010-01-02 00:00:00.000", result[0].DeliveryDate.ToString());
            Assert.AreEqual(0, result[0].SalesManSysNo);
            Assert.AreEqual(false, result[0].IsWholeSale);
            Assert.AreEqual(false, result[0].IsPremium);
            Assert.AreEqual(13, result[0].ShipTypeSysNo);
            Assert.AreEqual(0M, result[0].ExtraAmt);
            Assert.AreEqual(399M, result[0].SOAmt);
            Assert.AreEqual(0M, result[0].DiscountAmt);
            Assert.AreEqual(50, result[0].GainPoint);
            Assert.AreEqual(399M, result[0].CashPaid);
            Assert.AreEqual(0M, result[0].PointPaid);

            Assert.AreEqual(0M, result[0].PrepayAmt);
            Assert.AreEqual(0M, result[0].PromotionAmt);
            Assert.AreEqual(0M, result[0].GiftCardPayAmt);
            Assert.AreEqual(3361, result[0].ReceiveAreaSysNo);

            Assert.AreEqual("曹征", result[0].ReceiveContact);
            Assert.AreEqual("曹征", result[0].ReceiveName);
            Assert.AreEqual("13564520000", result[0].ReceivePhone);
            Assert.AreEqual("13564520000", result[0].ReceiveCellPhone);
            Assert.AreEqual("红", result[0].ReceiveAddress);
            Assert.AreEqual("200100", result[0].ReceiveZip);
            Assert.AreEqual("00299094", result[0].InvoiceNo);
            Assert.AreEqual("8601", result[0].CompanyCode.Trim());
        }

        [TestMethod]
        public void Test_GetListBySOSysNoList()
        {
            List<int> soSysNoList = new List<int>() { 30487818 };
            var result = ObjectFactory<SOIncomeProcessor>.Instance.GetListBySOSysNoList(soSysNoList);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(30487818, result[0].OrderSysNo);
            Assert.AreEqual(1, (int)result[0].OrderType);
            Assert.AreEqual(5588063, result[0].SysNo);
            Assert.AreEqual(284, result[0].OrderAmt);
            Assert.AreEqual(1, (int)result[0].IncomeStyle);
            Assert.AreEqual(1, (int)result[0].Status);
            Assert.AreEqual(19999, result[0].PointPay);
            Assert.AreEqual("8601", result[0].CompanyCode.Trim());
        }

        [TestMethod]
        public void Test_GetPayItemListByVendorSysNo()
        {
            var hash = new System.Collections.Hashtable();
            hash.Add(237763, 217697);
            hash.Add(237764, 217698);
            hash.Add(237765, 217699);
            hash.Add(237766, 217700);
            hash.Add(246383, 226032);
            hash.Add(246384, 226033);
            hash.Add(246385, 226034);
            hash.Add(246386, 226035);
            hash.Add(252239, 231745);
            hash.Add(252240, 231746);
            hash.Add(252241, 231747);
            hash.Add(252242, 231748);
            hash.Add(257472, 236743);
            hash.Add(257473, 236744);
            hash.Add(262173, 241312);
            hash.Add(262174, 241313);
            hash.Add(262175, 241314);
            hash.Add(262176, 241315);
            hash.Add(270928, 249868);
            hash.Add(270929, 249869);
            hash.Add(270930, 249870);
            hash.Add(270931, 249871);

            //var result = ObjectFactory<PayItemProcessor>.Instance.GetListByVendorSysNo(1875, ECCentral.BizEntity.Invoice.PayItemStatus.Origin);
            //Assert.AreEqual(22, result.Count);
            //foreach (var item in result)
            //{
            //    Assert.AreEqual(hash[item.SysNo.Value], item.PaySysNo.Value);
            //    Assert.AreEqual(0, (int)item.Status);
            //    Assert.AreEqual(1, (int)item.OrderType);
            //}
        }

        [TestMethod]
        public void Test_CreateInvoce()
        {
            InvoiceInfo invoiceEntity = new InvoiceInfo();
            invoiceEntity.CompanyCode = "8601";

            invoiceEntity.MasterInfo = new InvoiceMasterInfo();
            invoiceEntity.MasterInfo.CashPaid = 100;
            invoiceEntity.MasterInfo.CompanyCode = "8601";
            invoiceEntity.MasterInfo.CustomerID = "CustomerID";
            invoiceEntity.MasterInfo.CustomerSysNo = 0;
            invoiceEntity.MasterInfo.DeliveryDate = DateTime.Now;
            invoiceEntity.MasterInfo.DiscountAmt = 100;
            invoiceEntity.MasterInfo.ExtraAmt = 200;
            invoiceEntity.MasterInfo.FinanceNote = "FinanceNote";
            invoiceEntity.MasterInfo.GainPoint = 0;
            invoiceEntity.MasterInfo.GiftCardPayAmt = 100;
            invoiceEntity.MasterInfo.InvoiceAmt = 100;
            invoiceEntity.MasterInfo.InvoiceDate = DateTime.Now;
            invoiceEntity.MasterInfo.InvoiceMemo = "Memo";
            invoiceEntity.MasterInfo.InvoiceNo = "InvoiceNo";
            invoiceEntity.MasterInfo.InvoiceNote = "InvoiceNote";
            invoiceEntity.MasterInfo.InvoiceType = InvoiceType.MET;
            invoiceEntity.MasterInfo.IsPremium = true;
            invoiceEntity.MasterInfo.IsUseChequesPay = true;
            invoiceEntity.MasterInfo.IsWholeSale = true;
            invoiceEntity.MasterInfo.MerchantSysNo = 0;
            invoiceEntity.MasterInfo.OrderDate = DateTime.Now;
            invoiceEntity.MasterInfo.OriginalInvoiceNumber = 0;
            invoiceEntity.MasterInfo.PayTypeName = "PayTypeName";
            invoiceEntity.MasterInfo.PayTypeSysNo = 0;
            invoiceEntity.MasterInfo.PointPaid = 100;
            invoiceEntity.MasterInfo.PremiumAmt = 100;
            invoiceEntity.MasterInfo.PrepayAmt = 100;
            invoiceEntity.MasterInfo.PromotionAmt = 100;
            invoiceEntity.MasterInfo.PromotionCodeSysNo = 0;
            invoiceEntity.MasterInfo.ReceiveAddress = "ReceiveAddress";
            invoiceEntity.MasterInfo.ReceiveAreaSysNo = 0;
            invoiceEntity.MasterInfo.ReceiveCellPhone = "ReceiveCellPhone";
            invoiceEntity.MasterInfo.ReceiveContact = "ReceiveContact";
            invoiceEntity.MasterInfo.ReceiveName = "ReceiveName";
            invoiceEntity.MasterInfo.ReceivePhone = "ReceivePhone";
            invoiceEntity.MasterInfo.ReceiveZip = "000000";
            invoiceEntity.MasterInfo.ReferenceSONumber = 0;
            invoiceEntity.MasterInfo.RMANumber = 0;
            invoiceEntity.MasterInfo.SalesManSysNo = 0;
            invoiceEntity.MasterInfo.ShippingCharge = 100;
            invoiceEntity.MasterInfo.ShippingType = DeliveryType.MET;
            invoiceEntity.MasterInfo.ShipTypeSysNo = 0;
            invoiceEntity.MasterInfo.SOAmt = 100;
            invoiceEntity.MasterInfo.SONumber = 0;
            invoiceEntity.MasterInfo.StockSysNo = 0;
            invoiceEntity.MasterInfo.StockType = StockType.MET;
            invoiceEntity.MasterInfo.SpecialComment = "SpecialComment";
            invoiceEntity.MasterInfo.PromotionCustomerSysNo = 0;

            InvoiceTransactionInfo tran1 = new InvoiceTransactionInfo();
            tran1.BriefName = "Nnit_Test_BriefName_1";
            tran1.CashPaid = 100;
            tran1.CompanyCode = "8601";
            tran1.DiscountAmt = 100;
            tran1.ExtendPrice = 100;
            tran1.ExtraAmt = 100;
            tran1.GainPoint = 0;
            tran1.GiftCardPayAmt = 100;
            tran1.GiftSysNo = 0;
            tran1.ItemCode = "ItemCode_1";
            tran1.ItemDescription = "ItemDescription_1";
            tran1.ItemType = ECCentral.BizEntity.SO.SOProductType.Accessory;
            tran1.MasterProductSysNo = "MasterProductSysNo_1";
            tran1.MasterSysNo = 0;
            tran1.OriginalPrice = 100;
            tran1.PayType = ECCentral.BizEntity.IM.ProductPayType.MoneyOnly;
            tran1.PointPaid = 100;
            tran1.PremiumAmt = 100;
            tran1.PrepayAmt = 100;
            tran1.PriceType = ECCentral.BizEntity.SO.SOProductPriceType.Member;
            tran1.PrintDescription = "PrintDescription_1";
            tran1.ProductSysNo = 0;
            tran1.PromotionDiscount = 100;
            tran1.Quantity = 1;
            tran1.ReferenceSONumber = 0;
            tran1.ShippingCharge = 100;
            tran1.UnitCost = 100;
            tran1.UnitCostWithoutTax = 100;
            tran1.UnitPrice = 100;
            tran1.Warranty = "Warranty_1";
            tran1.Weight = 100;

            InvoiceTransactionInfo tran2 = new InvoiceTransactionInfo();
            tran2.BriefName = "Nnit_Test_BriefName_2";
            tran2.CashPaid = 100;
            tran2.CompanyCode = "8601";
            tran2.DiscountAmt = 100;
            tran2.ExtendPrice = 100;
            tran2.ExtraAmt = 100;
            tran2.GainPoint = 0;
            tran2.GiftCardPayAmt = 100;
            tran2.GiftSysNo = 0;
            tran2.ItemCode = "ItemCode_2";
            tran2.ItemDescription = "ItemDescription_2";
            tran2.ItemType = ECCentral.BizEntity.SO.SOProductType.Accessory;
            tran2.MasterProductSysNo = "MasterProductSysNo_2";
            tran2.MasterSysNo = 0;
            tran2.OriginalPrice = 100;
            tran2.PayType = ECCentral.BizEntity.IM.ProductPayType.MoneyOnly;
            tran2.PointPaid = 100;
            tran2.PremiumAmt = 100;
            tran2.PrepayAmt = 100;
            tran2.PriceType = ECCentral.BizEntity.SO.SOProductPriceType.Member;
            tran2.PrintDescription = "PrintDescription_2";
            tran2.ProductSysNo = 0;
            tran2.PromotionDiscount = 100;
            tran2.Quantity = 2;
            tran2.ReferenceSONumber = 0;
            tran2.ShippingCharge = 100;
            tran2.UnitCost = 100;
            tran2.UnitCostWithoutTax = 100;
            tran2.UnitPrice = 100;
            tran2.Warranty = "Warranty_2";
            tran2.Weight = 100;

            invoiceEntity.InvoiceTransactionInfoList = new List<InvoiceTransactionInfo>();
            invoiceEntity.InvoiceTransactionInfoList.Add(tran1);
            invoiceEntity.InvoiceTransactionInfoList.Add(tran2);

            InvoiceInfo invoice = ObjectFactory<InvoiceProcessor>.Instance.Create(invoiceEntity);
            Assert.AreEqual("8601", invoice.CompanyCode.Trim());
        }

        [TestMethod]
        public void Test_InvoiceDetailReportQuery()
        {
            int totalCount;
            InvoiceDetailReportQueryFilter filter = new InvoiceDetailReportQueryFilter();
            filter.PagingInfo = new QueryFilter.Common.PagingInfo();
            filter.PagingInfo.PageIndex = 0;
            filter.PagingInfo.PageSize = 50;
            filter.PagingInfo.SortBy = null;

            filter.OrderType = "SO";
            filter.StockSysNo = 51;
            filter.OrderID = "10149481";

            DataTable dt = ObjectFactory<IInvoiceReportQueryDA>.Instance.InvoiceDetailReportQuery(filter, out totalCount);
        }

        [TestMethod]
        public void Test_SendInvoiceChangeStatusSSB()
        {
            VendorPortalInvoiceChangeStatusMessage msg = new VendorPortalInvoiceChangeStatusMessage()
            {
                Status = VendorPortalType.R.ToString(),
                EditUser = "test user",
                SysNo = 1,
                Note = "just for test",
                MsgType = "just for test"
            };
            new VendorPortalInvoiceChangeStatusMessageProcessor().HandleEvent(msg);
        }

        [TestMethod]
        public void Test_CreatePayable()
        {
            PayableInfo entity = new PayableInfo();
            entity.AlreadyPayAmt = 0;
            entity.AuditDatetime = DateTime.Now;
            entity.AuditUserSysNo = -1;
            entity.BatchNumber = 0;
            entity.CompanyCode = "8601";
            entity.CurrencySysNo = 0;
            entity.EIMSAmt = 0;
            entity.EIMSNo = 0;
            entity.EstimatedTimeOfPay = DateTime.Now;
            entity.InStockAmt = 0;
            entity.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
            entity.InvoiceStatus = PayableInvoiceStatus.Absent;
            entity.InvoiceUpdateTime = DateTime.Now;
            entity.Note = "Just for NUnit Test";
            entity.OrderAmt = 0;
            entity.OrderStatus = 0;
            entity.OrderType = PayableOrderType.FinanceSettleOrder;
            entity.OrderSysNo = 11;
            entity.PayItemList = null;
            entity.PayStatus = PayableStatus.UnPay;
            entity.AuditStatus = PayableAuditStatus.NotAudit;
            entity.PMSysNo = 0;
            entity.RawOrderAmt = 0;
            entity.UpdateInvoiceUserSysNo = 0;

            ObjectFactory<PayableProcessor>.Instance.Create(entity);
        }
    }
}