using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using AutoClose.Model;
using AutoClose.DAL;
using IPPOversea.POmgmt.Configuration;

namespace AutoClose.Biz
{
    public class CreateEmailContent
    {
        private static XmlDataDocument doc = null;
        string imgSrc = string.Empty;
        string displayNo = string.Empty;
        string CompanyName = string.Empty;
        string CompanyAddress = string.Empty;
        string CompanyTel = string.Empty;
        string CompanyWebSite = string.Empty;

        string StockContact = string.Empty;
        string StockTel = string.Empty;
        string VendorMemo = string.Empty;
        string StockAddress = string.Empty;

        string numberString = "无";
        string PMName = string.Empty;
        string shipTypeString = string.Empty; //送货方式
        string CurrencyName = string.Empty;
        string totalInPage = string.Empty;    //本页小计
        string totalReturnPoint = string.Empty;
        string totalAmt = string.Empty;
        string StockName = "";
        string ETATime = "";
        Vendor vendor = null;
        PO entity = null;
        List<POItem> items = null;
        public CreateEmailContent(int id, string emails)
        {
            entity = EmailDA.QueryPOEntity(id);
            entity = EmailDA.GetReturnPoint(entity);

            if (entity != null)
            {
              //  entity.TotalAmt = entity.TotalAmt.ToString().Replace(entity.CurrencySymbol, "");
                items = AutoCloseDA.QueryPOItemsForPrint(entity);
            }
            SetBaseInfo(entity);
            displayNo = "";
            vendor = AutoClose.DAL.EmailDA.GetVendorBySysNo(entity.VendorSysNo);
            SetPoNumber(entity); //设置流水号
            SetSendType(); //设置送货方式
            SettotalInPage();//设置本页小计
            SettotalReturnPointAndtotalAmt();//设置totalReturnPoint和totalAmt
            SetPmName();//设置我方采购员
            SetCurrency();//设置货币种类
        }
        //发送邮件
        public void SendMail(string coutent, string email)
        {
            AutoCloseDA.SendEmail(0, email, "PO关闭邮件", coutent);
        }
        private void SetCurrency()
        {
            CurrencyInfo currency = EmailDA.QueryCurrencyInfoBySysNo(entity.CurrencySysNo);
            if (currency != null)
            {
                CurrencyName = currency.CurrencyName;
            }
        }

        private void SetPmName()
        {
            PMName = EmailDA.GetPMNameBySysNo(entity.CreateUserSysNo);
        }
        //设置totalReturnPoint和totalAmt
        private void SettotalReturnPointAndtotalAmt()
        {
            if (entity.PM_ReturnPointSysNo > 0)
            {
                totalAmt = entity.CurrencySymbol + Convert.ToDecimal(Convert.ToDecimal(entity.TotalAmt.ToString()) - Convert.ToDecimal(entity.UsingReturnPoint)).ToString("#########0.00");
                totalReturnPoint = "产品总价：" + entity.CurrencySymbol + Convert.ToDecimal(entity.TotalAmt.ToString()).ToString("#########0.00") + " &nbsp;&nbsp; " + "使用返点：" + (Convert.ToDecimal(entity.UsingReturnPoint)).ToString("#########0.00");
            }
            else
            {
                totalAmt = entity.CurrencySymbol + Convert.ToDecimal(entity.TotalAmt.ToString()).ToString("#########0.00");
                totalReturnPoint = "";
            }
        }
        //设置本页小计
        private void SettotalInPage()
        {
            decimal totalInPagedec = 0;
            foreach (POItem item in items)
            {
                totalInPagedec += item.Quantity * item.OrderPrice.Value;
            }
            totalInPage = entity.CurrencySymbol + totalInPagedec.ToString("#########0.00");
        }
        //设置流水号
        private void SetPoNumber(PO entity)
        {
            string oldnumber = AutoClose.DAL.EmailDA.GetWHReceiptSN(entity.SysNo);
            numberString = oldnumber.Trim();
            if (entity.Status == 4)
            {
                string number = "0";
                

                if (oldnumber.Trim().Equals("") == false)
                {
                    number =  oldnumber.Trim() ;
                }
                numberString=number;
            }
            else
            {
                numberString="无";
            }				
        }
        //设置送货方式
        private void SetSendType()
        {
            shipTypeString = EmailDA.GetShipTypeList(entity.ShipTypeSysNo); 
        }
        private void SetBaseInfo(PO entity)
        {
            StockName = entity.StockName;
            ETATime = entity.ETATime.ToLongTimeString();
            switch (entity.StockSysNo)
            {
                case 50:
                    switch (entity.ITStockSysNo)
                    {
                        case 52:
                            StockName = "经中转到北京仓";
                            break;
                        case 53:
                            StockName = "经中转到广州仓";
                            break;
                        case 54:
                            StockName = "经中转到成都仓";
                            break;
                        case 55:
                            StockName = "经中转到武汉仓";
                            break;
                        case 56:
                            StockName = "经中转到西安仓";
                            break;
                        case 57:
                            StockName = "经中转到济南仓";
                            break;
                        case 58:
                            StockName = "经中转到南京仓";
                            break;
                    }
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddress;
                    CompanyTel = NeweggConfig.Default.CompanyTel;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSite;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemo;
                    break;
                case 51:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddress;
                    CompanyTel = NeweggConfig.Default.CompanyTel;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSite;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemo;
                    break;
                case 52:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddressBJ;
                    CompanyTel = NeweggConfig.Default.CompanyTelBJ;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSiteBJ;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemoBJ;
                    break;
                case 53:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddressGZ;
                    CompanyTel = NeweggConfig.Default.CompanyTelGZ;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSiteGZ;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemoGZ;
                    break;
                case 54:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddressCD;
                    CompanyTel = NeweggConfig.Default.CompanyTelCD;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSiteCD;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemoGZ;
                    break;
                case 55:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddressWH;
                    CompanyTel = NeweggConfig.Default.CompanyTelWH;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSiteWH;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemoGZ;
                    break;
                case 59:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddressMH;
                    CompanyTel = NeweggConfig.Default.CompanyTelMH;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSiteMH;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemoGZ;
                    break;
                default:
                    CompanyName = NeweggConfig.Default.CompanyName;
                    CompanyAddress = NeweggConfig.Default.CompanyAddress;
                    CompanyTel = NeweggConfig.Default.CompanyTel;
                    CompanyWebSite = NeweggConfig.Default.CompanyWebSite;
                    StockAddress = entity.ReceiveAddress;
                    StockContact = entity.ReceiveContact;
                    StockTel = entity.ReceiveContactPhone;
                    VendorMemo = NeweggConfig.Default.VendorMemo;
                    break;
            }
        }
        public string EmailInfo()
        {
            if (doc == null)
            {
                doc = new XmlDataDocument();
                doc.Load(Application.StartupPath + "/MailTemplt/MailTemplate.xml");
            }
            string emailContentXml = doc.SelectSingleNode("//EmailContent").InnerText;
            emailContentXml = emailContentXml.Replace("#imgSrc#", imgSrc);
            emailContentXml = emailContentXml.Replace("#displayNo#", displayNo);
            emailContentXml = emailContentXml.Replace("#CompanyName#", CompanyName);
            emailContentXml = emailContentXml.Replace("#CompanyAddress#", CompanyAddress);
            emailContentXml = emailContentXml.Replace("#CompanyTel#", CompanyTel);
            emailContentXml = emailContentXml.Replace("#CompanyWebSite#", CompanyWebSite);

            emailContentXml = emailContentXml.Replace("#numberString#", numberString);
            emailContentXml = emailContentXml.Replace("#entity.POID#", entity.POID);
            emailContentXml = emailContentXml.Replace("#DateTime.Now#", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            emailContentXml = emailContentXml.Replace("#vendor.VendorName#", vendor.VendorName);

            emailContentXml = emailContentXml.Replace("#vendor.SysNo#", vendor.SysNo.ToString());
            emailContentXml = emailContentXml.Replace("#CompanyName#", CompanyName);
            emailContentXml = emailContentXml.Replace("#vendor.Address#", vendor.Address);
            emailContentXml = emailContentXml.Replace("#StockAddress#", StockAddress);
            emailContentXml = emailContentXml.Replace("#vendor.Contact#", vendor.Contact);
            emailContentXml = emailContentXml.Replace("#StockContact#", StockContact);
            emailContentXml = emailContentXml.Replace("#vendor.Phone#", vendor.Phone);
            emailContentXml = emailContentXml.Replace("#vendor.Fax#", vendor.Fax);

            emailContentXml = emailContentXml.Replace("#StockTel#", StockTel);
            emailContentXml = emailContentXml.Replace("#entity.InTime#", GetIntimeToString(entity.InTime));
            emailContentXml = emailContentXml.Replace("#PMName#", PMName);
            emailContentXml = emailContentXml.Replace("#entity.PayTypeName#", entity.PayTypeName);
            emailContentXml = emailContentXml.Replace("#vendor.Contact#", vendor.Contact);

            emailContentXml = emailContentXml.Replace("#shipTypeString#", shipTypeString);
            emailContentXml = emailContentXml.Replace("#CurrencyName#", CurrencyName);
            emailContentXml = emailContentXml.Replace("#entity.Memo#", entity.Memo);
            emailContentXml = emailContentXml.Replace("#entity.InStockMemo#", entity.InStockMemo);
            emailContentXml = emailContentXml.Replace("#totalInPage#", totalInPage);
            emailContentXml = emailContentXml.Replace("#totalReturnPoint#", totalReturnPoint);
            emailContentXml = emailContentXml.Replace("#totalAmt#", totalAmt);
            emailContentXml = emailContentXml.Replace("#listInfo()#", listInfo());

            emailContentXml = emailContentXml.Replace("#GetAtherInfo()#", GetAtherInfo());
            emailContentXml = emailContentXml.Replace("#VendorMemo#", VendorMemo);
            //GRC.GetStyle("images/neweggLogo.gif", true);
            emailContentXml = emailContentXml.Replace("#SendTimeString#", DateTime.Now.ToString());
            emailContentXml = emailContentXml.Replace("#ETATime#", ETATime);
            emailContentXml = emailContentXml.Replace("#StockName#", StockName);
            return emailContentXml;
        }

        private string GetIntimeToString(DateTime dateTime)
        {
            if (dateTime == null)
            {
                return "";
            }
            else if (dateTime > DateTime.Parse("1980-1-1"))
            {
                return dateTime.ToString();
            }
            else
            {
                return "";
            }
        }
        private string GetAtherInfo()
        {
            if (doc == null)
            {
                doc = new XmlDataDocument();
                doc.Load(Application.StartupPath + "/MailTemplt/MailTemplate.xml");
            }
            StringBuilder strBuider = new StringBuilder();
            System.Data.DataTable dt = AutoClose.DAL.AutoCloseDA.GetProductAccessoriesByPOSysno(entity.SysNo);
            if (dt.Rows.Count > 0)
            {
                string strs = doc.SelectSingleNode("//GetAtherInfoHead").InnerText;
                string atherBody = doc.SelectSingleNode("//GetAtherInfoBody").InnerText;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    string str = atherBody;
                    str = str.Replace("#ProductID#", dr["ProductID"].ToString());
                    str = str.Replace("#AccessoriesID#", dr["AccessoriesID"].ToString());
                    str = str.Replace("#AccessoriesIDAndName#", dr["AccessoriesIDAndName"].ToString());
                    str = str.Replace("#Qty#", dr["Qty"].ToString());
                    strBuider.Append(str);
                }
               strs =  strs.Replace("#GetAtherInfoBody#", strBuider.ToString());
                return strs;
            }
            return strBuider.ToString();
        }
        private string listInfo()
        {
            if (doc == null)
            {
                doc = new XmlDataDocument();
                doc.Load(Application.StartupPath + "/MailTemplt/MailTemplate.xml");
            }
            StringBuilder strbui = new StringBuilder();
            string strXml = doc.SelectSingleNode("//listInfo").InnerText;
            foreach (POItem item in items)
            {
                string strs = strXml;
                strs = strs.Replace("#item.ProductID#", item.ProductID);
                strs = strs.Replace("#item.IsVirtualStockProduct#", item.IsVirtualStockProduct);
                strs = strs.Replace("#item.ProductMode#", item.ProductMode);
                strs = strs.Replace("#item.BriefName#", item.BriefName);
                strs = strs.Replace("#entity.CurrencySymbol#", entity.CurrencySymbol);

                strs = strs.Replace("#item.OrderPrice#", item.OrderPrice.Value.ToString("#########0.00"));
                strs = strs.Replace("#item.PurchaseQty#", item.PurchaseQty.ToString());
                strs = strs.Replace("#item.Quantity#", item.Quantity.ToString());

                strs = strs.Replace("#item.PurchaseQtyOrderPrice#", (item.PurchaseQty * item.OrderPrice.Value).ToString("#########0.00"));
                strs = strs.Replace("#item.QuantityOrderPrice#", (item.Quantity * item.OrderPrice.Value).ToString("#########0.00"));
                strbui.Append(strs);
            }
            return strbui.ToString();
        }


    }
}
