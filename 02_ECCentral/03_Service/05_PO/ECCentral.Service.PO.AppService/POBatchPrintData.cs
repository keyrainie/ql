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
    /// 采购单  -打印类
    /// </summary>
    [VersionExport(typeof(POBatchPrintData))]
    public class POBatchPrintData : IPrintDataBuild
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
            //string getPOSysNo = requestPostData["POSysNo"];
            string getPrintAccessory = requestPostData["PrintAccessory"];
            string getPageTitle = requestPostData["PrintTitle"];
            PurchaseOrderInfo poInfo = new PurchaseOrderInfo();
            string soSysNos = requestPostData["POSysNoList"];
            if (soSysNos != null && soSysNos.Trim() != String.Empty)
            {
                string[] noList = System.Web.HttpUtility.UrlDecode(soSysNos).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                List<int> soSysNoList = new List<int>();
                noList.ForEach(no =>
                {
                    int tno = 0;
                    if (int.TryParse(no, out tno))
                    {
                        soSysNoList.Add(tno);
                    }
                });

                if (soSysNoList.Count > 0)
                {
                    System.Data.DataTable dtPO = new System.Data.DataTable();
                    dtPO.Columns.AddRange(new System.Data.DataColumn[] 
                    { 
                        new DataColumn("PageTitle"), 
                        new DataColumn("PrintTitle"), 
                        new DataColumn("StockName"), 
                        new DataColumn("ETATime"), 
                        new DataColumn("CompanyName"), 
                        new DataColumn("CompanyAddress"), 
                        new DataColumn("CompanyTel"), 
                        new DataColumn("CompanyWebSite"), 
                        new DataColumn("PrintSerialNumber"), 
                        new DataColumn("PurchaseOrderID"), 
                        new DataColumn("PrintTime"), 
                        new DataColumn("VendorNameAndSysNo"), 
                        new DataColumn("VendorAddress"),
                        new DataColumn("StockAddress"), 
                        new DataColumn("StockContact"), 
                        new DataColumn("StockTel"), 
                        new DataColumn("InStockDate"), 
                        new DataColumn("CreateUserName"), 
                        new DataColumn("PayTypeName"), 
                        new DataColumn("VendorContact"), 
                        new DataColumn("VendorPhoneAndFax"), 
                        new DataColumn("ShipTypeName"), 
                        new DataColumn("CurrencyName"), 
                        new DataColumn("Memo"), 
                        new DataColumn("InStockMemo"), 
                        new DataColumn("TotalReturnPoint"), 
                        new DataColumn("TotalAmt"), 
                        new DataColumn("NeedShowInStockQty"), 
                        new DataColumn("tblProductList",typeof(DataTable)), 
                        new DataColumn("tblAccessoriesList",typeof(DataTable)), 
                        new DataColumn("tblEIMSList",typeof(DataTable)),                        
                        new DataColumn("PurchaseQtyTotal"),
                        new DataColumn("NeedShowInStockQtySummary"),
                        new DataColumn("QuantityTotal"),
                        new DataColumn("AmountTotal"),
                        new DataColumn("AllCost"),
                        new DataColumn("AllTax"),
                        new DataColumn("TaxAndCost"),
                        new DataColumn("NeedShowEIMSList",typeof(bool)),
                        new DataColumn("NeedShowAccessoriesList",typeof(bool)),
                        new DataColumn("EimsAmtTotal"),
                        new DataColumn("UsedThisPlaceAmountTotal"),
                    });

                    tableVariables.Add("POList", dtPO);

                    foreach (int getPOSysNo in soSysNoList) {
                        DataRow dr = dtPO.NewRow();
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

                        dr["PageTitle"]=HttpUtility.UrlDecode(getPageTitle);
                        dr["PrintTitle"]= getPrintTitle;

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
                        dr["StockName"]=getStockName;
                        dr["ETATime"] = ETATime;
                        dr["CompanyName"] = CompanyName;
                        dr["CompanyAddress"] = CompanyAddress;
                        dr["CompanyTel"] = ComapnyTel;
                        dr["CompanyWebSite"] = CompanyWebsite;

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

                        dr["PrintSerialNumber"]=getSerialNumber;
                        dr["PurchaseOrderID"] =poInfo.PurchaseOrderBasicInfo.PurchaseOrderID;
                        dr["PrintTime"]= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        dr["VendorNameAndSysNo"] =string.Format("{0}({1})", poInfo.VendorInfo.VendorBasicInfo.VendorNameLocal, poInfo.VendorInfo.SysNo.Value);
                        dr["VendorAddress"] =poInfo.VendorInfo.VendorBasicInfo.Address;
                        dr["StockAddress"] =StockAddress;
                        dr["StockContact"]=StockContact;
                        dr["StockTel"] =StockTel;

                        string getInStockDate = string.Empty;
                        if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                        {
                            getInStockDate = poInfo.PurchaseOrderBasicInfo.InTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                         dr["InStockDate"] =getInStockDate;
                         dr["CreateUserName"] =CommonBizInteract.GetUserFullName(poInfo.PurchaseOrderBasicInfo.CreateUserSysNo.Value.ToString(), true);
                         dr["PayTypeName"] =poInfo.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName;
                         dr["VendorContact"] =poInfo.VendorInfo.VendorBasicInfo.Contact;
                         dr["VendorPhoneAndFax"] =poInfo.VendorInfo.VendorBasicInfo.Phone + " FAX : " + poInfo.VendorInfo.VendorBasicInfo.Fax;
                         dr["ShipTypeName"] =poInfo.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName;
                         dr["CurrencyName"] =poInfo.PurchaseOrderBasicInfo.CurrencyName;
                         dr["Memo"] =poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo;
                         dr["InStockMemo"]=poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo;

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

                        dr["TotalReturnPoint"]= getTotalReturnPoint;
                        dr["TotalAmt"]= getTotalAmt;
                        #endregion

                        #region [构建商品列表信息]

                        bool needShowInStockQty = ((poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed) == true ? true : false);
                        dr["NeedShowInStockQty"]=needShowInStockQty;

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
                            DataRow drp = tblProductInfo.NewRow();
                            drp["ProductID"] = x.ProductID + (x.IsVirtualStockProduct == true ? "[虚库商品]" : string.Empty);
                            drp["ProductMode"] = x.ProductMode;
                            drp["BMCode"] = x.BMCode;
                            drp["NeedShowInStockQtyHtml"] = needShowInStockQty;
                            drp["BriefName"] = x.BriefName;
                            drp["OrderPrice"] = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + x.OrderPrice.Value.ToString("f2");
                            drp["PurchaseQty"] = x.PurchaseQty;
                            drp["InStockQty"] = x.Quantity;
                            drp["ProductPriceSummary"] = (needShowInStockQty ? (x.Quantity.Value * x.OrderPrice.Value).ToString("f2") : (x.PurchaseQty.Value * x.OrderPrice.Value).ToString("f2"));
                            drp["TaxRateType"] = ((int)poInfo.PurchaseOrderBasicInfo.TaxRateType.Value).ToString();//税率
                            tblProductInfo.Rows.Add(drp);

                            purchaseQtyTotal += x.PurchaseQty.Value;
                            quantityTotal += x.Quantity.Value;
                            amountTotal += (needShowInStockQty ? x.Quantity.Value * x.OrderPrice.Value : x.PurchaseQty.Value * x.OrderPrice.Value);
                        });

                         dr["tblProductList"]=tblProductInfo;
                        //总和:
                         dr["PurchaseQtyTotal"]= purchaseQtyTotal;
                         dr["NeedShowInStockQtySummary"]= needShowInStockQty;
                         dr["QuantityTotal"]= needShowInStockQty ? quantityTotal.ToString() : string.Empty;
                         dr["AmountTotal"]= amountTotal.ToString("f2");

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

                        dr["AllCost"]= allCostStr;
                        dr["AllTax"]= allTaxStr;
                        dr["TaxAndCost"]=taxAndCostStr;
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
                            dr["NeedShowAccessoriesList"]= needShowAccessoriesList;
                            if (needShowAccessoriesList)
                            {
                                foreach (DataRow dra in dtPOAccessories.Rows)
                                {
                                    DataRow drRow = tblAccessoriesList.NewRow();
                                    drRow["ProductID"] = dra["ProductID"].ToString();
                                    drRow["AccessoryIDAndName"] = dra["AccessoriesID"].ToString() + "]" + dra["AccessoriesIDAndName"].ToString();
                                    drRow["Qty"] = dra["Qty"].ToString();
                                    tblAccessoriesList.Rows.Add(drRow);
                                }
                            }
                            dr["tblAccessoriesList"]= tblAccessoriesList;
                        }
                        else
                        {
                            dr["NeedShowAccessoriesList"]= false;
                        }
                        #endregion

                        #region [构建返点信息]

                        decimal eimsAmtTotal = 0.00m;
                        decimal usedThisPlaceAmountTotal = 0.00m;

                        bool needShowEIMSList = poInfo.EIMSInfo == null || poInfo.EIMSInfo.EIMSInfoList.Count <= 0 ? false : true;
                        dr["NeedShowEIMSList"]= needShowEIMSList;
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
                                DataRow dre = tblEIMSList.NewRow();
                                dre["EIMSNo"] = item.EIMSSysNo;
                                dre["EIMSName"] = item.EIMSName;
                                dre["EIMSTotalAmt"] = item.EIMSTotalAmt.HasValue ? item.EIMSTotalAmt.Value.ToString("f2") : "0.00";
                                dre["RelateNotReceivedAmount"] = (item.RelateAmount - item.ReceivedAmount).HasValue ? (item.RelateAmount - item.ReceivedAmount).Value.ToString("f2") : "0.00";
                                dre["ReceivedAmount"] = item.ReceivedAmount.HasValue ? item.ReceivedAmount.Value.ToString("f2") : "0.00";
                                dre["EIMSAmt"] = item.EIMSAmt.HasValue ? item.EIMSAmt.Value.ToString("f2") : "0.00";
                                dre["UseThisPlaceAmount"] = (item.EIMSAmt - item.LeftAmt).HasValue ? (item.EIMSAmt - item.LeftAmt).Value.ToString("f2") : "0.00";
                                dre["LeaveUseThisPlaceAmount"] = (item.EIMSTotalAmt - item.RelateAmount).HasValue ? (item.EIMSTotalAmt - item.RelateAmount).Value.ToString("f2") : "0.00";

                                tblEIMSList.Rows.Add(dre);

                                eimsAmtTotal += item.EIMSAmt.HasValue ? item.EIMSAmt.Value : 0;
                                usedThisPlaceAmountTotal += Convert.ToDecimal(dre["UseThisPlaceAmount"].ToString());
                            }
                        }
                        dr["tblEIMSList"] =tblEIMSList;
                        //总和:
                        dr["EimsAmtTotal"]= eimsAmtTotal.ToString("f2");
                        dr["UsedThisPlaceAmountTotal"]= usedThisPlaceAmountTotal.ToString("f2");
                        dtPO.Rows.Add(dr);
                        #endregion
                    }
                }
            }
        }
    }
}
