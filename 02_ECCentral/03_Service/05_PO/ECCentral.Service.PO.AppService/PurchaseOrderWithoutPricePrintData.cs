using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;
using System.Data;
using System.Threading;
using ECCentral.BizEntity.Common;
using System.Web;

namespace ECCentral.Service.PO.AppService
{
    /// <summary>
    /// 不含金额的采购单  -打印类
    /// </summary>
    [VersionExport(typeof(PurchaseOrderWithoutPricePrintData))]
    public class PurchaseOrderWithoutPricePrintData : IPrintDataBuild
    {
        #region [Fields]
        private PurchaseOrderProcessor m_PurchaseOrderProcessor;
        private ICommonBizInteract m_CommonBizInteract;
        private IInventoryBizInteract m_InventoryBizInteract;

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

        public ICommonBizInteract CommonBizInteract
        {
            get
            {
                if (null == m_CommonBizInteract)
                {
                    m_CommonBizInteract = ObjectFactory<ICommonBizInteract>.Instance;
                }
                return m_CommonBizInteract;
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
        #endregion


        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string getPOSysNo = requestPostData["POSysNo"];
            string getPrintAccessory = requestPostData["PrintAccessory"];
            string getPageTitle = requestPostData["PrintTitle"];
            PurchaseOrderInfo poInfo = new PurchaseOrderInfo();


            if (!string.IsNullOrEmpty(getPOSysNo))
            {
                int poSysNo = Convert.ToInt32(getPOSysNo);
                poInfo = PurchaseOrderProcessor.LoadPO(poSysNo);
                poInfo.EIMSInfo.EIMSInfoList = PurchaseOrderProcessor.LoadPOEIMSInfoForPrint(poSysNo);

                #region [构建供应商和采购单的基本信息]
                string getPrintTitle = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.Consign)
                {
                    getPrintTitle = string.Format("采购单({0})", EnumHelper.GetDisplayText(poInfo.PurchaseOrderBasicInfo.ConsignFlag));
                }
                else
                {
                    if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType.HasValue && poInfo.PurchaseOrderBasicInfo.PurchaseOrderType != PurchaseOrderType.Normal)
                    {
                        getPrintTitle = string.Format("采购单({0})", EnumHelper.GetDisplayText(poInfo.PurchaseOrderBasicInfo.PurchaseOrderType));
                    }
                    else
                    {
                        getPrintTitle = "采购单";
                    }
                }

                variables.Add("PageTitle", HttpUtility.UrlDecode(getPageTitle));
                variables.Add("PrintTitle", getPrintTitle);

                WarehouseInfo getStockInfo = InventoryBizInteract.GetWarehouseInfoBySysNo(poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value);
                string getStockName = getStockInfo.WarehouseName;
                string CompanyName = "";
                string CompanyAddress = "";
                string ComapnyTel = "";
                string CompanyWebsite = "";
                string StockAddress = getStockInfo.ReceiveAddress;
                string StockContact = getStockInfo.ReceiveContact;
                string StockTel = getStockInfo.ReceiveContactPhoneNumber;
                string ETATime = poInfo.PurchaseOrderBasicInfo.ETATimeInfo == null || !poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? string.Empty : poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                switch (poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo)
                {
                    //case 50:
                    //    if (poInfo.PurchaseOrderBasicInfo.ITStockInfo != null && poInfo.PurchaseOrderBasicInfo.ITStockInfo.SysNo.HasValue)
                    //    {
                    //        string getITStockInfoName = InventoryBizInteract.GetWarehouseInfoBySysNo(poInfo.PurchaseOrderBasicInfo.ITStockInfo.SysNo.Value).WarehouseName;
                    //        getStockName = string.Format("经中转到{0}", getITStockInfoName);
                    //    }

                    //    break;
                    case 51:
                        //上海
                        break;
                    case 52:
                        //香港:
                        CompanyAddress = "香港九龙湾启祥道20号大昌行集团大厦8楼";
                        ComapnyTel = "（852）27683388";
                        break;
                    case 53:
                        //日本:
                        break;
                    //case 54:
                    //    //成都:
                    //    CompanyAddress = "成都双流县大件路西南航空港新地物流园区（西南民大新校区对面）";
                    //    ComapnyTel = "15982082844";
                    //    break;
                    //case 55:
                    //    //武汉:
                    //    CompanyAddress = "武汉市东西湖区革新大道（四明路与五环路之间）长江物流园C库10号门";
                    //    ComapnyTel = "13339983123";
                    //    break;
                    //case 59:
                    //    //上海市闵行:
                    //    CompanyAddress = "上海市闵行区虹梅南路3988号2号库";
                    //    ComapnyTel = "13122693665";
                    //    break;
                    default:
                        break;
                }
                variables.Add("StockName", getStockName);
                variables.Add("ETATime", ETATime);
                variables.Add("CompanyName", CompanyName);
                variables.Add("CompanyAddress", CompanyAddress);
                variables.Add("CompanyTel", ComapnyTel);
                variables.Add("CompanyWebSite", CompanyWebsite);

                string getSerialNumber = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    getSerialNumber = CommonBizInteract.GetSystemConfigurationValue("PONumber", poInfo.CompanyCode);
                    if (!string.IsNullOrEmpty(getSerialNumber))
                    {
                        //TODO:此方法尚未实现:
                        CommonBizInteract.UpdateSystemConfigurationValue("PONumber", (Convert.ToInt32(getSerialNumber) + 1).ToString(), poInfo.CompanyCode);
                    }
                    string getOldSerialNumber = PurchaseOrderProcessor.GetWareHouseReceiptSerialNumber(poInfo.SysNo.Value);
                    //更新PO单当前的流水号:
                    PurchaseOrderProcessor.UpdateWareHouseReceiptSerialNumber(poInfo.SysNo.Value, getSerialNumber);
                    if (!string.IsNullOrEmpty(getOldSerialNumber))
                    {
                        getSerialNumber = string.Format("{0}({1})", getSerialNumber, getOldSerialNumber);
                    }
                }
                else
                {
                    getSerialNumber = "无";
                }

                variables.Add("PrintSerialNumber", getSerialNumber);
                variables.Add("PurchaseOrderID", poInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
                variables.Add("PrintTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                variables.Add("VendorNameAndSysNo", string.Format("{0}({1})", poInfo.VendorInfo.VendorBasicInfo.VendorNameLocal, poInfo.VendorInfo.SysNo.Value));
                variables.Add("VendorAddress", poInfo.VendorInfo.VendorBasicInfo.Address);
                variables.Add("StockAddress", StockAddress);
                variables.Add("StockContact", StockContact);
                variables.Add("StockTel", StockTel);

                string getInStockDate = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    getInStockDate = poInfo.PurchaseOrderBasicInfo.InTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                variables.Add("InStockDate", getInStockDate);
                variables.Add("CreateUserName", CommonBizInteract.GetUserFullName(poInfo.PurchaseOrderBasicInfo.CreateUserSysNo.Value.ToString(), true));
                variables.Add("PayTypeName", poInfo.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName);
                variables.Add("VendorContact", poInfo.VendorInfo.VendorBasicInfo.Contact);
                variables.Add("VendorPhoneAndFax", poInfo.VendorInfo.VendorBasicInfo.Phone + " FAX : " + poInfo.VendorInfo.VendorBasicInfo.Fax);
                variables.Add("ShipTypeName", poInfo.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName);
                variables.Add("CurrencyName", poInfo.PurchaseOrderBasicInfo.CurrencyName);
                variables.Add("Memo", poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
                variables.Add("InStockMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);

                string getTotalAmt = string.Empty;
                string getTotalReturnPoint = string.Empty;
                decimal? eimsAmt = 0.00m;
                decimal? totalInPage = 0;
                poInfo.POItems.ForEach(x =>
                {
                    totalInPage += x.PurchaseQty * x.OrderPrice;

                });
                poInfo.EIMSInfo.EIMSInfoList.ForEach(x =>
                {
                    eimsAmt += x.EIMSAmt.HasValue ? x.EIMSAmt.Value : 0m;
                });
                if (eimsAmt > 0.00m)
                {
                    getTotalAmt = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(totalInPage - eimsAmt).ToString("f2");
                    getTotalReturnPoint = "产品总价：" + poInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPage.Value.ToString("f2") + " &nbsp;&nbsp; " + "使用返点：" + eimsAmt.Value.ToString("f2");
                }
                else
                {
                    getTotalAmt = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPage.Value.ToString("f2");
                    getTotalReturnPoint = "";
                }

                variables.Add("TotalReturnPoint", getTotalReturnPoint);
                variables.Add("TotalAmt", getTotalAmt);
                #endregion

                #region [构建商品列表信息]

                bool needShowInStockQty = ((poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed) == true ? true : false);
                variables.Add("NeedShowInStockQty", needShowInStockQty);

                int purchaseQtyTotal = 0;
                int quantityTotal = 0;
                decimal amountTotal = 0.00m;

                DataTable tblProductInfo = new DataTable();
                tblProductInfo.Columns.Add("ProductID");
                tblProductInfo.Columns.Add("ProductMode");
                tblProductInfo.Columns.Add("BMCode");
                tblProductInfo.Columns.Add("BriefName");
                tblProductInfo.Columns.Add("OrderPrice");
                tblProductInfo.Columns.Add("PurchaseQty");
                tblProductInfo.Columns.Add("InStockQty");
                tblProductInfo.Columns.Add("ProductPriceSummary");
                tblProductInfo.Columns.Add("NeedShowInStockQtyHtml");

                tblProductInfo.Columns.Add("TaxRateType");

                poInfo.POItems.ForEach(x =>
                {
                    DataRow dr = tblProductInfo.NewRow();
                    dr["ProductID"] = x.ProductID + (x.IsVirtualStockProduct == true ? "[虚库商品]" : string.Empty);
                    dr["ProductMode"] = x.ProductMode;
                    dr["BMCode"] = x.BMCode;
                    dr["NeedShowInStockQtyHtml"] = needShowInStockQty;
                    dr["BriefName"] = x.BriefName;
                    dr["OrderPrice"] = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + x.OrderPrice.Value.ToString("f2");
                    dr["PurchaseQty"] = x.PurchaseQty;
                    dr["InStockQty"] = x.Quantity;
                    dr["ProductPriceSummary"] = (needShowInStockQty ? (x.Quantity.Value * x.OrderPrice.Value).ToString("f2") : (x.PurchaseQty.Value * x.OrderPrice.Value).ToString("f2"));
                    dr["TaxRateType"] = ((int)poInfo.PurchaseOrderBasicInfo.TaxRateType.Value).ToString();//税率
                    tblProductInfo.Rows.Add(dr);

                    purchaseQtyTotal += x.PurchaseQty.Value;
                    quantityTotal += x.Quantity.Value;
                    amountTotal += (needShowInStockQty ? x.Quantity.Value * x.OrderPrice.Value : x.PurchaseQty.Value * x.OrderPrice.Value);
                });

                tableVariables.Add("tblProductList", tblProductInfo);
                //总和:
                variables.Add("PurchaseQtyTotal", purchaseQtyTotal);
                variables.Add("NeedShowInStockQtySummary", needShowInStockQty);
                variables.Add("QuantityTotal", needShowInStockQty ? quantityTotal.ToString() : string.Empty);
                variables.Add("AmountTotal", amountTotal.ToString("f2"));

                #endregion

                #region 构建合计信息

                Func<PurchaseOrderTaxRate, decimal, decimal> Shuijin = (a, b) =>
                {
                    return ((decimal)(((int)a) / 100.00)) * b / ((decimal)(((int)a) / 100.00) + 1);
                };

                Func<PurchaseOrderTaxRate, decimal, decimal> Jiakuan = (a, b) =>
                {
                    return b / ((decimal)(((int)a) / 100.00) + 1);
                };

                Func<decimal?, decimal> IsNullDecimal = (a) =>
                {
                    if (a != null && a.HasValue) { return a.Value; } else { return 0m; }
                };

                var taxAndCost = IsNullDecimal(poInfo.PurchaseOrderBasicInfo.TotalActualPrice);
                var taxAndCostStr = taxAndCost.ToString("C");
                var allCost = Jiakuan(poInfo.PurchaseOrderBasicInfo.TaxRateType.Value, taxAndCost);
                var allCostStr = allCost.ToString("C");
                var allTax = Shuijin(poInfo.PurchaseOrderBasicInfo.TaxRateType.Value, taxAndCost);
                var allTaxStr = allTax.ToString("C");

                variables.Add("AllCost", allCostStr);
                variables.Add("AllTax", allTaxStr);
                variables.Add("TaxAndCost", taxAndCostStr);
                #endregion

                #region [构建配件信息]
                if (getPrintAccessory == "1")
                {
                    DataTable dtPOAccessories = PurchaseOrderProcessor.GetPurchaseOrderAccessories(poInfo.SysNo.Value);
                    DataTable tblAccessoriesList = new DataTable();
                    tblAccessoriesList.Columns.Add("ProductID");
                    tblAccessoriesList.Columns.Add("AccessoryIDAndName");
                    tblAccessoriesList.Columns.Add("Qty");

                    bool needShowAccessoriesList = dtPOAccessories == null || dtPOAccessories.Rows.Count <= 0 ? false : true;
                    variables.Add("NeedShowAccessoriesList", needShowAccessoriesList);
                    if (needShowAccessoriesList)
                    {
                        foreach (DataRow dr in dtPOAccessories.Rows)
                        {
                            DataRow drRow = tblAccessoriesList.NewRow();
                            drRow["ProductID"] = dr["ProductID"].ToString();
                            drRow["AccessoryIDAndName"] = dr["AccessoriesID"].ToString() + "]" + dr["AccessoriesIDAndName"].ToString();
                            drRow["Qty"] = dr["Qty"].ToString();
                            tblAccessoriesList.Rows.Add(drRow);
                        }
                    }
                    tableVariables.Add("tblAccessoriesList", tblAccessoriesList);
                }
                else
                {
                    variables.Add("NeedShowAccessoriesList", false);
                }
                #endregion

                #region [构建返点信息]

                decimal eimsAmtTotal = 0.00m;
                decimal usedThisPlaceAmountTotal = 0.00m;

                bool needShowEIMSList = poInfo.EIMSInfo == null || poInfo.EIMSInfo.EIMSInfoList.Count <= 0 ? false : true;
                variables.Add("NeedShowEIMSList", needShowEIMSList);
                DataTable tblEIMSList = new DataTable();
                tblEIMSList.Columns.Add("EIMSNo");
                tblEIMSList.Columns.Add("EIMSName");
                tblEIMSList.Columns.Add("EIMSTotalAmt");
                tblEIMSList.Columns.Add("RelateNotReceivedAmount");
                tblEIMSList.Columns.Add("ReceivedAmount");
                tblEIMSList.Columns.Add("EIMSAmt");
                tblEIMSList.Columns.Add("UseThisPlaceAmount");
                tblEIMSList.Columns.Add("LeaveUseThisPlaceAmount");

                if (needShowEIMSList)
                {
                    foreach (var item in poInfo.EIMSInfo.EIMSInfoList)
                    {
                        DataRow dr = tblEIMSList.NewRow();
                        dr["EIMSNo"] = item.EIMSSysNo;
                        dr["EIMSName"] = item.EIMSName;
                        dr["EIMSTotalAmt"] = item.EIMSTotalAmt.HasValue ? item.EIMSTotalAmt.Value.ToString("f2") : "0.00";
                        dr["RelateNotReceivedAmount"] = (item.RelateAmount - item.ReceivedAmount).HasValue ? (item.RelateAmount - item.ReceivedAmount).Value.ToString("f2") : "0.00";
                        dr["ReceivedAmount"] = item.ReceivedAmount.HasValue ? item.ReceivedAmount.Value.ToString("f2") : "0.00";
                        dr["EIMSAmt"] = item.EIMSAmt.HasValue ? item.EIMSAmt.Value.ToString("f2") : "0.00";
                        dr["UseThisPlaceAmount"] = (item.EIMSAmt - item.LeftAmt).HasValue ? (item.EIMSAmt - item.LeftAmt).Value.ToString("f2") : "0.00";
                        dr["LeaveUseThisPlaceAmount"] = (item.EIMSTotalAmt - item.RelateAmount).HasValue ? (item.EIMSTotalAmt - item.RelateAmount).Value.ToString("f2") : "0.00";

                        tblEIMSList.Rows.Add(dr);

                        eimsAmtTotal += item.EIMSAmt.HasValue ? item.EIMSAmt.Value : 0;
                        usedThisPlaceAmountTotal += Convert.ToDecimal(dr["UseThisPlaceAmount"].ToString());
                    }
                }
                tableVariables.Add("tblEIMSList", tblEIMSList);
                //总和:
                variables.Add("EimsAmtTotal", eimsAmtTotal.ToString("f2"));
                variables.Add("UsedThisPlaceAmountTotal", usedThisPlaceAmountTotal.ToString("f2"));

                #endregion
            }
        }
    }
}
