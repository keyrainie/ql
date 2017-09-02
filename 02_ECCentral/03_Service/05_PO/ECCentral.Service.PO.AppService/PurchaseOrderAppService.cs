using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;
using System.Data;
using ECCentral.BizEntity;
using System.Threading;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(PurchaseOrderAppService))]
    public class PurchaseOrderAppService
    {
        #region [Fields]
        private PurchaseOrderProcessor m_PurchaseOrderProcessor;
        private IIMBizInteract m_IMBizInteract;
        private IInventoryBizInteract m_InventoryBizInteract;
        private IInvoiceBizInteract m_InvoiceBizInteract;

        public IInvoiceBizInteract InvoiceBizInteract
        {
            get
            {
                if (null == m_InvoiceBizInteract)
                {
                    m_InvoiceBizInteract = ObjectFactory<IInvoiceBizInteract>.Instance;
                }
                return m_InvoiceBizInteract;
            }
        }

        public IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                if (null == m_InventoryBizInteract)
                {
                    m_InventoryBizInteract = ObjectFactory<IInventoryBizInteract>.Instance;
                }
                return m_InventoryBizInteract;
            }
        }

        public IIMBizInteract IMBizInteract
        {
            get
            {
                if (null == m_IMBizInteract)
                {
                    m_IMBizInteract = ObjectFactory<IIMBizInteract>.Instance;
                }
                return m_IMBizInteract;
            }
        }

        public PurchaseOrderProcessor PurchaseOrderProcessor
        {
            get
            {
                if (null == m_PurchaseOrderProcessor)
                {
                    m_PurchaseOrderProcessor = ObjectFactory<PurchaseOrderProcessor>.Instance;
                }
                return m_PurchaseOrderProcessor;
            }
        }
        #endregion

        public PurchaseOrderInfo LoadPurchaseOrderInfo(int poSysNo)
        {
            return PurchaseOrderProcessor.LoadPO(poSysNo);
        }

        public PurchaseOrderItemInfo LoadPurchaseOrderItemInfo(int itemSysNo)
        {
            return PurchaseOrderProcessor.LoadPOItemInfo(itemSysNo);
        }

        public PurchaseOrderInfo CreatePurchaseOrderInfo(PurchaseOrderInfo info)
        {
            return PurchaseOrderProcessor.CreatePO(info);
        }

        public PurchaseOrderInfo UpdatePurchaseOrderInfo(PurchaseOrderInfo poInfo)
        {
            switch (poInfo.PurchaseOrderBasicInfo.MemoInfo.Note)
            {
                case "EditDeletePOItem":
                    return PurchaseOrderProcessor.EditDeletePOItem(poInfo);
                case "EditAddOnePOItem":
                    return PurchaseOrderProcessor.EditAddOnePOItem(poInfo);
                case "EditUpdateOnePOItem":
                    return PurchaseOrderProcessor.EditUpdateOnePOItem(poInfo);
                case "EditAllEditOnePOItem":
                    return PurchaseOrderProcessor.EditAllEditOnePOItem(poInfo);
                case "EditAllAddOnePOItem":
                    return PurchaseOrderProcessor.EditAllAddOnePOItem(poInfo);
                default:
                    return PurchaseOrderProcessor.UpdatePO(poInfo);
            }
        }

        public PurchaseOrderInfo CheckPOInfo(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.CheckPO(poInfo);
        }

        public PurchaseOrderInfo SubmitAuditPurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.SubmitPO(poInfo);
        }

        public PurchaseOrderInfo VerifyPurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.WaitingInStockPO(poInfo, true);
        }

        public PurchaseOrderInfo CancelVerifyPurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.CancelVerifyPO(poInfo);
        }

        public PurchaseOrderInfo RefusePurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.RefusePO(poInfo);
        }

        public PurchaseOrderInfo AbandonPurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.AbandonPO(poInfo);
        }

        public PurchaseOrderInfo CancelAbandonPurchaseOrder(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.CancelAbandonPO(poInfo);
        }

        public PurchaseOrderInfo PMConfirmWithVendor(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.PMConfirmWithVendor(poInfo);
        }

        public PurchaseOrderInfo UpdateInfoStockMemo(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.UpdatePOInStockMemo(poInfo);
        }

        public PurchaseOrderInfo StopInStockPO(PurchaseOrderInfo poInfo)
        {
            return PurchaseOrderProcessor.StopInStockPO(poInfo);
        }

        public PurchaseOrderEIMSRuleInfo QueryEIMSRuleInfoByNumber(int ruleNumber)
        {
            return PurchaseOrderProcessor.GetEIMSRuleInfoBySysNo(ruleNumber);
        }
        public PurchaseOrderEIMSRuleInfo QueryEIMSRuteInfoByAssignedCode(string id)
        {
            return PurchaseOrderProcessor.GetEIMSRuleInfoByAssignedCode(id);
        }

        public virtual PurchaseOrderETATimeInfo SubmitETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            return PurchaseOrderProcessor.SubmitETAInfo(etaInfo);
        }

        public virtual PurchaseOrderETATimeInfo PassETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            return PurchaseOrderProcessor.PassETAInfo(etaInfo);
        }

        public virtual PurchaseOrderETATimeInfo CancelETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            return PurchaseOrderProcessor.CancelETAInfo(etaInfo);
        }

        public virtual PurchaseOrderInfo ConfirmVendorPortalPurchaseOrder(PurchaseOrderInfo info)
        {
            return PurchaseOrderProcessor.ConfirmVendorPortalPurchaseOrder(info);
        }

        public virtual PurchaseOrderInfo AuditVendorPortalPurchaseOrder(PurchaseOrderInfo info)
        {
            return PurchaseOrderProcessor.AuditVendorPortalPurchaseOrder(info);
        }

        public virtual PurchaseOrderInfo RenewCreatePO(PurchaseOrderInfo info)
        {
            return PurchaseOrderProcessor.RenewCreatePO(info);
        }

        public virtual void RetreatVendorPortalPurchaseOrder(int poSysNo, string retreatType)
        {
            PurchaseOrderProcessor.RetreatVendorPortalPurchaseOrder(poSysNo, retreatType);
        }

        public virtual PurchaseOrderItemInfo AddPurchaseOrderItemFromProductInfo(PurchaseOrderItemProductInfo productInfo)
        {
            return PurchaseOrderProcessor.AddPurchaseOrderItemFromProductInfo(productInfo);
        }

        public virtual List<PurchaseOrderItemProductInfo> GetPurchaseOrderGiftInfo(List<int> productSysNoList)
        {
            return PurchaseOrderProcessor.GetPurchaseOrderGiftInfo(productSysNoList);
        }

        public virtual List<PurchaseOrderItemProductInfo> GetPurchaseOrderAccessoriesInfo(List<int> productSysNoList)
        {
            return PurchaseOrderProcessor.GetPurchaseOrderAccessoriesInfo(productSysNoList);
        }

        public virtual void UpdateMailAddressAndHasSentMail(int poSysNo, string mailAddress, string companyCode)
        {
            PurchaseOrderProcessor.UpdateMailAddressAndHasSentMail(poSysNo, mailAddress, companyCode);
        }

        public virtual void UpdatePurchaeOrderBatchInfo(PurchaseOrderItemInfo info)
        {
            PurchaseOrderProcessor.UpdatePurchaeOrderBatchInfo(info);
        }

        public virtual List<ProductManagerInfo> GetAuthorizedPMList(PMQueryType queryType, string currentName, string companyCode)
        {
            return IMBizInteract.GetPMListByType(queryType, currentName, companyCode);
        }

        public virtual List<WarehouseInfo> GetPurchaseOrderWarehouseList(string companyCode)
        {
            return PurchaseOrderProcessor.GetPurchaseOrderWarehouseList(companyCode);
        }

        public virtual List<EIMSInfo> LoadPurchaseOrderEIMSInfo(int poSysNo)
        {
            return PurchaseOrderProcessor.LoadPOEIMSInfo(poSysNo);
        }

        public virtual void UpdatePurchaseOrderInstockAmt(int poSysNo)
        {
            PurchaseOrderProcessor.UpdatePurchaseOrderInstockAmt(poSysNo);
        }

        public virtual int SetPurchaseOrderStatusClose(int poSysNo, string closeUser)
        {
            return PurchaseOrderProcessor.SetPurchaseOrderClose(poSysNo, closeUser);
        }

        public virtual int AdjustPurchaseOrderQtyInventory(InventoryAdjustContractInfo info)
        {
            ExternalDomainBroker.AdjustProductInventory(info);
            return 1;
        }

        public virtual string GetMailContentForAutoClosePOJob(int poSysNo)
        {
            string returnString = string.Empty;
            PurchaseOrderInfo getPOInfo = PurchaseOrderProcessor.LoadPO(poSysNo);
            if (null != getPOInfo)
            {
                WarehouseInfo getStockInfo = InventoryBizInteract.GetWarehouseInfoBySysNo(getPOInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value);
                string getStockName = getStockInfo.WarehouseName;
                string getCompanyName = "网信（香港）有限公司";
                string getCompanyAddress = "香港九龙湾启祥道20号大昌行集团大厦8楼";
                string getComapnyTel = "（852）27683388";
                string getCompanyWebsite = "";
                string getStockAddress = getStockInfo.ReceiveAddress;
                string getStockContact = getStockInfo.ReceiveContact;
                string getStockTel = getStockInfo.ReceiveContactPhoneNumber;
                switch (getPOInfo.PurchaseOrderBasicInfo.StockInfo.SysNo)
                {
                    case 50:
                    case 51:
                        break;
                    case 52:
                        //香港:
                        getCompanyAddress = "香港九龙湾启祥道20号大昌行集团大厦8楼";
                        getComapnyTel = "（852）27683388";
                        break;
                    case 53:
                        ////广州:
                        //getCompanyAddress = "广东省广州市天河区中山大道西779号";
                        //getComapnyTel = "020-85559980,020-85559981";
                        break;
                    //case 54:
                    //    //成都:
                    //    getCompanyAddress = "成都双流县大件路西南航空港新地物流园区（西南民大新校区对面）";
                    //    getComapnyTel = "15982082844";
                    //    break;
                    //case 55:
                    //    //武汉:
                    //    getCompanyAddress = "武汉市东西湖区革新大道（四明路与五环路之间）长江物流园C库10号门";
                    //    getComapnyTel = "13339983123";
                    //    break;
                    //case 59:
                    //    //上海市闵行:
                    //    getCompanyAddress = "上海市闵行区虹梅南路3988号2号库";
                    //    getComapnyTel = "13122693665";
                    //    break;
                    default:
                        break;
                }

                KeyValueVariables keyValues = new KeyValueVariables();
                keyValues.Add("StockName", getPOInfo.PurchaseOrderBasicInfo.StockInfo.StockName);
                keyValues.Add("ETATime", getPOInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToLongTimeString());
                keyValues.Add("CompanyName", getCompanyName);
                keyValues.Add("CompanyAddress", getCompanyAddress);
                keyValues.Add("CompanyTel", getComapnyTel);
                keyValues.Add("CompanyWebSite", getCompanyWebsite);
                keyValues.Add("StockAddress", getStockAddress);
                keyValues.Add("StockContact", getStockContact);
                keyValues.Add("StockTel", getStockTel);
                keyValues.Add("displayNo", "");


                string getOldSNNumber = PurchaseOrderProcessor.GetWareHouseReceiptSerialNumber(poSysNo);
                string numberString = getOldSNNumber.Trim();
                if (getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    string number = "0";

                    if (getOldSNNumber.Trim().Equals("") == false)
                    {
                        number = getOldSNNumber.Trim();
                    }
                    numberString = number;
                }
                else
                {
                    numberString = "无";
                }
                keyValues.Add("numberString", numberString);
                keyValues.Add("shipTypeString", getPOInfo.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName);

                decimal totalInPagedec = 0;
                string totalInPage = string.Empty;
                foreach (PurchaseOrderItemInfo item in getPOInfo.POItems)
                {
                    totalInPagedec += (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.OrderPrice.HasValue ? item.OrderPrice.Value : 0);
                }
                totalInPage = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPagedec.ToString("#########0.00");
                keyValues.Add("totalInPage", totalInPage);

                string totalAmt = string.Empty;
                string totalReturnPoint = string.Empty;
                if (getPOInfo.PurchaseOrderBasicInfo.PM_ReturnPointSysNo > 0)
                {
                    totalAmt = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()) - Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.UsingReturnPoint)).ToString("#########0.00");
                    totalReturnPoint = "产品总价：" + getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()).ToString("#########0.00") + " &nbsp;&nbsp; " + "使用返点：" + (Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.UsingReturnPoint)).ToString("#########0.00");
                }
                else
                {
                    totalAmt = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()).ToString("#########0.00");
                    totalReturnPoint = "";
                }
                keyValues.Add("totalAmt", totalAmt);
                keyValues.Add("totalReturnPoint", totalReturnPoint);
                keyValues.Add("PMName", getPOInfo.PurchaseOrderBasicInfo.ProductManager.UserInfo.UserName);
                keyValues.Add("CurrencyName", ObjectFactory<ICommonBizInteract>.Instance.GetCurrencyInfoBySysNo(getPOInfo.PurchaseOrderBasicInfo.CurrencyCode.Value).CurrencyName);

                keyValues.Add("entity.POID", getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
                keyValues.Add("DateTime.Now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                keyValues.Add("vendor.VendorName", getPOInfo.VendorInfo.VendorBasicInfo.VendorNameLocal);
                keyValues.Add("vendor.SysNo", getPOInfo.VendorInfo.SysNo.Value.ToString());
                keyValues.Add("vendor.Address", getPOInfo.VendorInfo.VendorBasicInfo.Address);
                keyValues.Add("vendor.Contact", getPOInfo.VendorInfo.VendorBasicInfo.Contact);
                keyValues.Add("vendor.Fax", getPOInfo.VendorInfo.VendorBasicInfo.Fax);
                keyValues.Add("entity.InTime", getPOInfo.PurchaseOrderBasicInfo.InTime.HasValue ? getPOInfo.PurchaseOrderBasicInfo.InTime.Value.ToString() : "");
                keyValues.Add("entity.PayTypeName", getPOInfo.PurchaseOrderBasicInfo.PayType.PayTypeName);
                keyValues.Add("entity.Memo", getPOInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
                keyValues.Add("entity.InStockMemo", getPOInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);
                keyValues.Add("SendTimeString", DateTime.Now.ToString());

                KeyTableVariables keyTables = new KeyTableVariables();
                DataTable productAccessoryList = new DataTable();

                DataTable productItemList = new DataTable();
                productItemList.Columns.Add("item.ProductID");
                productItemList.Columns.Add("item.IsVirtualStockProduct");
                productItemList.Columns.Add("item.ProductMode");
                productItemList.Columns.Add("item.BriefName");
                productItemList.Columns.Add("item.CurrencySymbol");
                productItemList.Columns.Add("item.OrderPrice");
                productItemList.Columns.Add("item.PurchaseQty");
                productItemList.Columns.Add("item.Quantity");
                productItemList.Columns.Add("item.PurchaseQtyOrderPrice");
                productItemList.Columns.Add("item.QuantityOrderPrice");

                getPOInfo.POItems.ForEach(x =>
                {
                    DataRow dr = productItemList.NewRow();
                    dr["item.ProductID"] = x.ProductID;
                    dr["item.IsVirtualStockProduct"] = x.IsVirtualStockProduct;
                    dr["item.ProductMode"] = x.ProductMode;
                    dr["item.BriefName"] = x.BriefName;
                    dr["item.CurrencySymbol"] = x.CurrencySymbol;
                    dr["item.OrderPrice"] = x.OrderPrice.Value.ToString("#########0.00");
                    dr["item.PurchaseQty"] = x.PurchaseQty.Value.ToString();
                    dr["item.Quantity"] = x.Quantity.Value.ToString();
                    dr["item.PurchaseQtyOrderPrice"] = (x.PurchaseQty.Value * x.OrderPrice.Value).ToString("#########0.00");
                    dr["item.QuantityOrderPrice"] = (x.Quantity.Value * x.OrderPrice.Value).ToString("#########0.00");
                    productItemList.Rows.Add(dr);
                });

                DataTable dt = PurchaseOrderProcessor.GetPurchaseOrderAccessories(poSysNo);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow sr in dt.Rows)
                    {
                        DataRow dr = productAccessoryList.NewRow();
                        dr["ProductID"] = dr["ProductID"].ToString();
                        dr["AccessoriesID"] = dr["AccessoriesID"].ToString();
                        dr["AccessoriesIDAndName"] = dr["AccessoriesIDAndName"].ToString();
                        dr["Qty"] = dr["Qty"].ToString();
                        productAccessoryList.Rows.Add(dr);
                    }
                }
                keyTables.Add("tblProductItemsList", productItemList);
                keyTables.Add("tblProductAccessoryList", productItemList);

                MailMessage mailMsg = new MailMessage();
                EmailTemplateHelper.BuildEmailBodyByTemplate(mailMsg, "PO_AutoCloseMail", keyValues, keyTables, Thread.CurrentThread.CurrentCulture.Name);
                returnString = mailMsg.Body;
                return returnString;
            }
            else
            {
                throw new BizException("未找到相关PO单数据!");
            }
        }

        public virtual List<PurchaseOrderInfo> GetNeedToClosePurchaseOrderList()
        {
            return PurchaseOrderProcessor.GetNeedToClosePurchaseOrderList();
        }

        public virtual int SendPurchaseOrderCloseSSBMessage(int poSysNo, int userSysNo, string companyCode)
        {
            PurchaseOrderProcessor.SendCloseMessage(poSysNo, userSysNo, companyCode);
            return 1;
        }

        public virtual List<PurchaseOrderInfo> GetPurchaseOrderForETA(string companyCode)
        {
            return PurchaseOrderProcessor.GetPurchaseOrderListForETA(companyCode);
        }

        public virtual PayItemInfo GetPayItemInfoByPOSysNo(int poSysNo)
        {
            return InvoiceBizInteract.GetFinancePayItemByPOSysNo(poSysNo);
        }

        public virtual void InsertPayItemInfo(PayItemInfo info)
        {
            InvoiceBizInteract.InsertFinancePayItemInfo(info);
        }

        public int AbandonPOForJob(int poSysNo)
        {
            return PurchaseOrderProcessor.AbandonPOForJob(poSysNo);
        }
        public int AbandonETAForJob(int poSysNo)
        {
            return PurchaseOrderProcessor.AbandonETAForJob(poSysNo);
        }

        public int UpdateExtendPOInfoForJob(int poSysNo, int productSysNo)
        {
            return PurchaseOrderProcessor.UpdateExtendPOInfoForJob(poSysNo, productSysNo);
        }

        public virtual int SendMailWhenAuditPurchaseOrder(string mailContent, int poSysNo, string mailAddress)
        {
            return PurchaseOrderProcessor.SendMailWhenAuditPurchaseOrder(mailContent,poSysNo, mailAddress);
        }

        public bool CheckOperateRightForCurrentUser(ProductPMLine info)
        {
            return ExternalDomainBroker.CheckOperateRightForCurrentUser(info.ProductSysNo.Value, info.PMSysNo.Value);
        }

        public List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo)
        {
            return ExternalDomainBroker.GetProductLineSysNoByProductList(productSysNo);
        }

        public List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
        {
            return ExternalDomainBroker.GetProductLineInfoByPM(pmSysNo);
        }
    }
}
