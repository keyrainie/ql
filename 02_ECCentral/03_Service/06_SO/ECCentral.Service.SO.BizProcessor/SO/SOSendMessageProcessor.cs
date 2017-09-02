using System;
using System.Linq;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Transactions;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;

namespace ECCentral.Service.SO.BizProcessor
{

    [VersionExport(typeof(SOSendMessageProcessor))]
    public class SOSendMessageProcessor
    {
        #region 发送内布邮件

        public void ElectronicSOPriceNotExists(SOInfo soInfo)
        {
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("SOSysNo", soInfo.SysNo);
            keyValueVariables.Add("StockSysNo", soInfo.Items[0].StockSysNo);
            ExternalDomainBroker.SendInternalEmail("Invoice_Generate_Error", keyValueVariables);
        }
        #endregion

        #region 发送外部邮件
        private ECCentral.BizEntity.Customer.CustomerBasicInfo GetCustomerBaseInfo(int customerSysNo)
        {
            return ExternalDomainBroker.GetCustomerInfo(customerSysNo).BasicInfo;
        }

        private void SendEmailToCustomerForSOInfo(SOInfo soInfo, string emailTemplateID)
        {
            try
            {
                ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = GetCustomerBaseInfo(soInfo.BaseInfo.CustomerSysNo.Value);
                string customerEmail = customerInfo.Email == null ? null : customerInfo.Email.Trim();

                if (customerEmail == null)
                {
                    return;
                }
                KeyValueVariables keyValueVariables = new KeyValueVariables();
                KeyTableVariables keyTableVariables = new KeyTableVariables();

                #region 填充基本属性
                keyValueVariables.Add("SOSysNo", soInfo.BaseInfo.SysNo.Value.ToString());
                keyValueVariables.Add("SOID", soInfo.BaseInfo.SOID);
                keyValueVariables.Add("CustomerName", customerInfo.CustomerName);
                keyValueVariables.Add("CustomerID", customerInfo.CustomerID);
                keyValueVariables.Add("OrderTime", soInfo.BaseInfo.CreateTime.Value.ToString(SOConst.DateTimeFormat));
                keyValueVariables.Add("InvoiceHeader", soInfo.InvoiceInfo.Header);
                keyValueVariables.Add("ReceiveName", soInfo.ReceiverInfo.Name);
                ECCentral.BizEntity.Common.AreaInfo areaInfo = ExternalDomainBroker.GetAreaInfoByDistrictSysNo(soInfo.ReceiverInfo.AreaSysNo.Value);
                keyValueVariables.Add("ProvinceName", areaInfo.ProvinceName);
                keyValueVariables.Add("CityName", areaInfo.CityName);
                keyValueVariables.Add("DistrictName", areaInfo.DistrictName);
                keyValueVariables.Add("ReceiveAddress", soInfo.ReceiverInfo.Address);
                keyValueVariables.Add("ReceiveZip", soInfo.ReceiverInfo.Zip);
                keyValueVariables.Add("ReceivePhone", String.IsNullOrEmpty(soInfo.ReceiverInfo.Phone) ? soInfo.ReceiverInfo.MobilePhone : soInfo.ReceiverInfo.Phone);
                ECCentral.BizEntity.Common.PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(soInfo.BaseInfo.PayTypeSysNo.Value);
                keyValueVariables.Add("PayType", payType.PayTypeName);
                ECCentral.BizEntity.Common.ShippingType shippingType = ExternalDomainBroker.GetShippingTypeBySysNo(soInfo.ShippingInfo.ShipTypeSysNo.Value);
                keyValueVariables.Add("ShipType", shippingType.ShippingTypeName);
                keyValueVariables.Add("ShipPeriod", shippingType.Period);
                keyValueVariables.Add("CashPay", soInfo.BaseInfo.CashPay.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("ShipPrice", soInfo.BaseInfo.ShipPrice.Value.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("PremiumAmount", soInfo.BaseInfo.PremiumAmount.Value.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("ReceivableAmount", soInfo.BaseInfo.ReceivableAmount.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("GainPoint", soInfo.BaseInfo.GainPoint.Value);
                keyValueVariables.Add("Weight", soInfo.ShippingInfo.Weight);

                string changeAmount = (soInfo.BaseInfo.OriginalReceivableAmount - soInfo.BaseInfo.ReceivableAmount).ToString(SOConst.DecimalFormat);
                keyValueVariables.Add("ChangeAmount", changeAmount);
                keyValueVariables.Add("ChangeAmountDisplay", changeAmount != (0M).ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("GiftCardPay", soInfo.BaseInfo.GiftCardPay.Value.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("GiftCardDisplay", soInfo.BaseInfo.GiftCardPay != 0);
                keyValueVariables.Add("PointPay", soInfo.BaseInfo.PointPayAmount);
                keyValueVariables.Add("PointPayDisplay", soInfo.BaseInfo.PointPay != 0);
                keyValueVariables.Add("PrePay", soInfo.BaseInfo.PrepayAmount.Value.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("PrePayDisplay", soInfo.BaseInfo.PrepayAmount != 0);
                keyValueVariables.Add("PayPrice", soInfo.BaseInfo.PayPrice.Value.ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("PayPriceDisplay", soInfo.BaseInfo.PayPrice != 0);
                keyValueVariables.Add("PromotionAmount", Math.Abs(soInfo.BaseInfo.PromotionAmount.Value).ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("PromotionDisplay", soInfo.BaseInfo.PromotionAmount != 0);
                keyValueVariables.Add("NowYear", DateTime.Now.Year);
                keyValueVariables.Add("TariffAmt", soInfo.BaseInfo.TariffAmount.HasValue?soInfo.BaseInfo.TariffAmount.Value.ToString(SOConst.DecimalFormat) : 0M.ToString(SOConst.DecimalFormat));
                #endregion

                //int weight = 0;

                #region 替换邮件模板内连接追踪代码
                ReplaceCM_MMC(emailTemplateID, keyValueVariables);
                #endregion

                #region 填充商品
                string imgSrc = string.Empty;
                string pagePath = string.Empty;
                soInfo.Items.ForEach(item =>
                {
                    //weight += item.Weight.Value * item.Quantity.Value;
                    string tbKey = String.Format("Items_{0}", item.ProductType.ToString());
                    pagePath = "http://www.kjt.com/product/detail/" + item.ProductSysNo ;
                    imgSrc = "http://image.kjt.com/neg/P60/" + item.ProductID + ".jpg";
                    DataTable tableList = null;
                    if (!keyTableVariables.ContainsKey(tbKey))
                    {
                        tableList = new DataTable();
                        tableList.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ProductID"),
                        new DataColumn("ProductName"),
                        new DataColumn("Price"),
                        new DataColumn("Quantity"),
                        new DataColumn("Amount"),
                        new DataColumn("PagePath"),
                        new DataColumn("ImgSrc")
                    });
                        keyTableVariables.Add(tbKey, tableList);
                        
                    }
                    else
                    {
                        tableList = keyTableVariables[tbKey];
                    }
                    //连接增加追踪代码
                    ReplaceProductCM_MMC(emailTemplateID, ref pagePath, item.ProductID);
                    tableList.Rows.Add(new string[] 
                { 
                    item.ProductType == SOProductType.Coupon ? null : item.ProductID, 
                    item.ProductName, 
                    item.OriginalPrice.Value.ToString(SOConst.DecimalFormat), 
                    item.Quantity.Value.ToString(), 
                    (item.Quantity.Value * item.OriginalPrice.Value).ToString(SOConst.DecimalFormat),
                    pagePath,
                    imgSrc
                });

                });
                #endregion

                #region 组合销售
                List<SOPromotionInfo> comboPromotionList = soInfo.SOPromotions.FindAll(p => { return p.PromotionType == SOPromotionType.Combo; });
                keyValueVariables.Add("PromotionInfoDisplay", soInfo.BaseInfo.PromotionAmount != 0);
                if (comboPromotionList != null)
                {
                    DataTable comboTable = new DataTable();
                    comboTable.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("ComboName"),
                    new DataColumn("ComboDiscount"),
                    new DataColumn("ComboTime"),
                    new DataColumn("ComboTotalDiscount")
                });
                    keyTableVariables.Add("ComboList", comboTable);
                    List<ECCentral.BizEntity.MKT.ComboInfo> comboInfoList = ExternalDomainBroker.GetComboList(comboPromotionList.Select<SOPromotionInfo, int>(p => p.PromotionSysNo.Value).ToList<int>());
                    if (comboInfoList != null)
                    {
                        comboPromotionList.ForEach(promotion =>
                        {
                            ECCentral.BizEntity.MKT.ComboInfo comboInfo = comboInfoList.FirstOrDefault(cb => cb.SysNo == promotion.PromotionSysNo);
                            comboTable.Rows.Add(new string[] 
                        { 
                            comboInfo == null ? null : comboInfo.Name.Content, 
                            (promotion.DiscountAmount.Value / promotion.Time.Value).ToString(SOConst.DecimalFormat), 
                            promotion.Time.Value.ToString(), 
                            promotion.DiscountAmount.Value.ToString(SOConst.DecimalFormat) 
                        });
                        });
                    }
                }
                #endregion

                #region 填充推荐商品信息
                string result = string.Empty;
                //测试订单号：388280  
                List<CommendatoryProductsInfo> list = ObjectFactory<ISODA>.Instance.GetCommendatoryProducts(int.Parse(soInfo.BaseInfo.SOID));
                if (list != null && list.Count > 0)
                {
                    result = "<br /><table width=\"650px\" style=\"border-collapse: collapse; border: 1px solid #ddd;\" cellspacing=\"0\" cellpadding=\"0\">\n" +
                                "<tr style=\"background:#fff;\">\n" +
                                  "<td style=\"width:30px; padding:10px 0 0 10px;\"><img src=\"http://c1.neweggimages.com.cn/NeweggPic2/Marketing/201108/chuhuo/images/icon_4.jpg\" /></td>\n" +
                                  "<td style=\"text-align:left; padding:10px 0 0 5px;\"><span style=\"font-family:\"微软雅黑\";font-size:16px; display:inline-block; padding:0; padding-left:5px;\"><strong>我们猜您可能还对下面的商品感兴趣</strong></span></td>\n" +
                                "</tr>\n" +
                            "</table>\n" +
                            "<div style=\"padding-top:10px;border-collapse: collapse; border: 1px solid #ddd;width:648px\">" +
                                "<table cellspacing=\"0\" cellpadding=\"0\" style=\"padding-bottom:10px;border-collapse: collapse; border:0;\">\n" +
                                    "<tr style=\" background:#fff;\">\n" +
                                        "[RevommendProductList1]\n" +
                                    "</tr>\n" +
                                    "<tr style=\"background:#fff;\">\n" +
                                        "[RevommendProductList2]\n" +
                                    "</tr>\n" +
                                "</table>\n" +
                            "</div>";
                    string item1 = string.Empty,
                    item2 = string.Empty;

                    IEnumerator<CommendatoryProductsInfo> rator = list.Take(3).GetEnumerator();
                    while (rator.MoveNext())
                    {
                        CommendatoryProductsInfo entity = rator.Current;
                        item1 += ReplaceCommendatoryProduct(entity, emailTemplateID);
                    }
                    rator = list.Skip(3).Take(3).GetEnumerator();
                    while (rator.MoveNext())
                    {
                        CommendatoryProductsInfo entity = rator.Current;
                        item2 += ReplaceCommendatoryProduct(entity, emailTemplateID);
                    }

                    result = result.Replace("[RevommendProductList1]", item1)
                                    .Replace("[RevommendProductList2]", item2);

                    keyValueVariables.Add("CommendatoryProducts", result);
                }
                #endregion

                #region 填充备注信息
                string memo = string.Empty;
                if (!string.IsNullOrEmpty(soInfo.BaseInfo.MemoForCustomer))
                {
                    memo = @"<table border='0' cellpadding='5' cellspacing='0' style='width: 650px; border-collapse: collapse;font-size: 9pt;'>
                                <tr>
                                    <td style='border: 1px solid #ddd; color: #FF4E00; font-size: 10.5pt; font-weight: bold; background: #F2F2F2;'>
                                        备注信息
                                    </td>
                                </tr>
                                <tr>
                                    <td id='Memo' style='border: 1px solid #ddd;'>
                                        " + soInfo.BaseInfo.MemoForCustomer + @"
                                    </td>
                                </tr>
                            </table>";
                    keyValueVariables.Add("Memo", memo);
                }
                #endregion

                ExternalDomainBroker.SendExternalEmail(customerEmail, emailTemplateID, keyValueVariables, keyTableVariables, customerInfo.FavoriteLanguageCode);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        private string ReplaceCommendatoryProduct(CommendatoryProductsInfo entity, string emailTemplateID)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            string item = "<td align=\"left\" valign=\"top\" style=\"padding-left:25px;\">" +
                            "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:180px; overflow:hidden; background:#fff;\">" +
                                "<tr>" +
                                    "<td width=\"86\"><a [ProductItemLink]><img style=\"border:0;width:80px;height:80px;\" src=\"[ProductImageTag]\" /></a></td>" +
                                    "<td width=\"94\" valign=\"middle\";><span style=\"padding-left:10px; color:#ff5100;\">￥<b>[NeweggPrice]</b></span></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan=\"2\">" +
                                    "<p style=\"line-height:18px; height:36px; width:180px; overflow:hidden; margin-bottom:8px;\"><a [ProductItemLink] style=\"color:#484848; text-decor6ation:none;\">[ProductTitle]</a></p>" +
                                    "</td>" +
                                "</tr>" +
                            "</table>" +
                          "</td>";

            string productLinkUrl = string.Format("http://www.kjt.com/product/detail/{0}", entity.ProductSysNo);
            ReplaceProductCM_MMC(emailTemplateID, ref productLinkUrl, entity.ProductID);
            productLinkUrl = string.Format("href=\"{0}\" target=\"_blank\"", productLinkUrl);
            string imageUrl = string.Format("http://image.kjt.com/neg/P120/{0}?v={1}", entity.DefaultImage, entity.ImageVersion);

            string title = entity.ProductName;

            string price = entity.Price.ToString("##,###0.00");

            item = item.Replace("[ProductItemLink]", productLinkUrl)
                        .Replace("[ProductImageTag]", imageUrl)
                        .Replace("[ProductTitle]", title)
                        .Replace("[NeweggPrice]", price);

            return item;
        }

        private static void ReplaceProductCM_MMC(string emailTemplateID, ref string productLinkUrl, string productID)
        {
            string tmp = productLinkUrl;
            if (string.IsNullOrEmpty(tmp))
            {
                return;
            }

            string now = DateTime.Now.ToString("yyyyMMdd");
            string param = string.Format("cm_mmc=emc-_-tran{0}-_-Recproduct-_-{1}", now, productID);
            if (tmp.IndexOf('?') > 0)
            {
                param = "&" + param;
            }
            else
            {
                param = "?" + param;
            }

            switch (emailTemplateID)
            {
                case "SO_Created":
                case "SO_Audit_Passed":
                case "SO_OutStock":
                case "SO_Splited":
                    break;
                default:
                    param = string.Empty;
                    break;
            }
            tmp = tmp + param;

            productLinkUrl = tmp;
        }

        /// <summary>
        /// 替换邮件模板内连接追踪代码
        /// </summary>
        /// <param name="emailType"></param>
        private static void ReplaceCM_MMC(string emaileTemplateID, KeyValueVariables keyValueVariables)
        {
            string now = DateTime.Now.ToString("yyyyMMdd");
            string homePage = string.Format("?cm_mmc=emc-_-tran{0}-_-homepage", now);
            string countDown = string.Format("?cm_mmc=emc-_-tran{0}-_-countdown", now);
            string mobile = string.Format("?cm_mmc=emc-_-tran{0}-_-mobile", now);

            switch (emaileTemplateID)
            {
                case "SO_Created":
                case "SO_Audit_Passed":
                case "SO_OutStock":
                case "SO_Splited":
                    break;
                default:
                    homePage = countDown = mobile = string.Empty;
                    break;
            }

            keyValueVariables.Add("CM_MMC_HomePage", homePage);
            keyValueVariables.Add("CM_MMC_CountDown", countDown);
            keyValueVariables.Add("CM_MMC_Mobile", mobile);
        }

        public void SOCreatedSendEmailToCustomer(SOInfo soInfo)
        {
            SendEmailToCustomerForSOInfo(soInfo, "SO_Created");
        }

        public void SOAuditedSendEmailToCustomer(SOInfo soInfo)
        {
            SendEmailToCustomerForSOInfo(soInfo, "SO_Audit_Passed");
        }

        public void SOOutStockSendEmailToCustomer(SOInfo soInfo)
        {
            SendEmailToCustomerForSOInfo(soInfo, "SO_OutStock");
        }

        public void SendCommentNotifyEmailToCustomer(SOInfo soInfo)
        {
            SendEmailToCustomerForSOInfo(soInfo, "SO_CommentNotify");
        }

        public void AbandonSOEmailToCustomer(SOInfo soInfo)
        {
            SendEmailToCustomerForSOInfo(soInfo, "SO_Abandon");
        }

        public void PriceChangedSendMail(SOInfo soInfo)
        {
            try
            {
                List<SOInternalMemoInfo> memoList = ObjectFactory<SOInternalMemoProcessor>.Instance.GetBySOSysNo(soInfo.SysNo.Value);
                List<int> productSysNoList = (from item in soInfo.Items
                                              select item.ProductSysNo.Value).ToList();
                List<ECCentral.BizEntity.IM.ProductInfo> productList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);
                if (productList != null)
                {
                    DataTable dtMemo = new DataTable();
                    dtMemo.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("LogTime"),
                        new DataColumn("Content"),
                        new DataColumn("Note")
                    });
                    productList.ForEach(p =>
                    {
                        SOItemInfo item = soInfo.Items.Find(i => i.ProductSysNo.Value == p.SysNo);
                        if (item != null)
                        {
                            KeyValueVariables keyValueVariables = new KeyValueVariables();
                            KeyTableVariables keyTableVariables = new KeyTableVariables();
                            string pmEmailAddress = p.ProductBasicInfo.ProductManager.UserInfo.EmailAddress;
                            StringBuilder mailBody = new StringBuilder();
                            keyValueVariables.Add("SOSysNo", soInfo.SysNo.Value);
                            keyValueVariables.Add("ProductID", item.ProductID);
                            keyValueVariables.Add("ProductName", item.ProductName);
                            keyValueVariables.Add("Quantity", item.Quantity);
                            keyValueVariables.Add("Weight", item.Weight);
                            keyValueVariables.Add("Price", item.Price);
                            keyValueVariables.Add("GainAveragePoint", item.GainAveragePoint);
                            keyValueVariables.Add("PromotionAmount", item.PromotionAmount);
                            keyValueVariables.Add("Warranty", item.Warranty);
                            if (memoList != null && memoList.Count > 0 && dtMemo.Rows.Count == 0)
                            {
                                foreach (SOInternalMemoInfo internalMemo in memoList)
                                {
                                    DataRow dr = dtMemo.NewRow();
                                    dr["LogTime"] = internalMemo.LogTime.HasValue ? internalMemo.LogTime.Value.ToString(SOConst.DateTimeFormat) : String.Empty;
                                    dr["Content"] = internalMemo.Content;
                                    dr["Note"] = internalMemo.Note;
                                    dtMemo.Rows.Add(dr);
                                }
                            }
                            keyValueVariables.Add("MemoListDisplay", dtMemo.Rows.Count > 0);
                            keyTableVariables.Add("MemoList", dtMemo);
                            ExternalDomainBroker.SendInternalEmail("SO_Product_PriceChanged", keyValueVariables, keyTableVariables);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        public void ActivateElectronicSendMailToCustomer(SOInfo soInfo)
        {
            try
            {
                ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = GetCustomerBaseInfo(soInfo.BaseInfo.CustomerSysNo.Value);
                string customerEmail = customerInfo.Email == null ? null : customerInfo.Email.Trim();

                if (customerEmail == null)
                {
                    return;
                }
                KeyValueVariables keyValueVariables = new KeyValueVariables();
                keyValueVariables.Add("CustomerID", customerInfo.CustomerID);
                keyValueVariables.Add("Quantity", soInfo.Items[0].Quantity);
                keyValueVariables.Add("TotalAmount", (soInfo.Items[0].Quantity.Value * soInfo.Items[0].OriginalPrice.Value).ToString(SOConst.DecimalFormat));
                keyValueVariables.Add("ExpireYear", DateTime.Now.AddYears(2).Year);
                keyValueVariables.Add("ExpireMonth", DateTime.Now.Month);
                keyValueVariables.Add("ExpireDay", DateTime.Now.Day);
                keyValueVariables.Add("NowYear", DateTime.Now.Year);
                ExternalDomainBroker.SendExternalEmail(customerEmail, "SO_ActivateElectronicGiftCard", keyValueVariables, customerInfo.FavoriteLanguageCode);

            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// 拆单邮件
        /// </summary>
        /// <param name="masterSOInfo"></param>
        /// <param name="subSOList"></param>
        public void SendSplitSOEmail(SOInfo masterSOInfo, List<SOInfo> subSOList)
        {
            try
            {
                ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = GetCustomerBaseInfo(masterSOInfo.BaseInfo.CustomerSysNo.Value);
                string customerEmail = customerInfo.Email == null ? null : customerInfo.Email.Trim();

                if (customerEmail == null)
                {
                    return;
                }
                #region 填充基本信息
                KeyValueVariables keyValueVariables = new KeyValueVariables();
                KeyTableVariables keyTableVariables = new KeyTableVariables();
                keyValueVariables.Add("CustomerID", customerInfo.CustomerID);
                keyValueVariables.Add("CustomerName", customerInfo.CustomerName);
                keyValueVariables.Add("ReceiveName", masterSOInfo.InvoiceInfo.Header);
                keyValueVariables.Add("ReceiveContact", masterSOInfo.ReceiverInfo.Name);
                ECCentral.BizEntity.Common.AreaInfo areaInfo = ExternalDomainBroker.GetAreaInfoByDistrictSysNo(masterSOInfo.ReceiverInfo.AreaSysNo.Value);
                keyValueVariables.Add("ProvinceName", areaInfo.ProvinceName);
                keyValueVariables.Add("CityName", areaInfo.CityName);
                keyValueVariables.Add("DistrictName", areaInfo.DistrictName);
                keyValueVariables.Add("ReceiveAddress", masterSOInfo.ReceiverInfo.Address);
                keyValueVariables.Add("ReceiveZip", masterSOInfo.ReceiverInfo.Zip);
                keyValueVariables.Add("ReceivePhone", String.IsNullOrEmpty(masterSOInfo.ReceiverInfo.Phone) ? masterSOInfo.ReceiverInfo.MobilePhone : masterSOInfo.ReceiverInfo.Phone);

                keyValueVariables.Add("SOSysNo", masterSOInfo.SysNo.Value);
                keyValueVariables.Add("SubSOSysNoList", String.Join(",", subSOList.Select<SOInfo, int>(subSOInfo => subSOInfo.SysNo.Value).ToArray()));
                keyValueVariables.Add("Memo", masterSOInfo.BaseInfo.Memo);
                keyValueVariables.Add("NowYear", DateTime.Now.Year);
                #endregion 填充基本信息

                #region 替换邮件模板内连接追踪代码
                ReplaceCM_MMC("SO_Splited", keyValueVariables);
                #endregion

                #region 填充拆单信息
                string pagePath = string.Empty;
                string imgSrc = string.Empty; 

                int i = 1;
                ECCentral.BizEntity.Common.PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(masterSOInfo.BaseInfo.PayTypeSysNo.Value);
                ECCentral.BizEntity.Common.ShippingType shippingType = ExternalDomainBroker.GetShippingTypeBySysNo(masterSOInfo.ShippingInfo.ShipTypeSysNo.Value);

                DataTable subSO = new DataTable();
                subSO.Columns.AddRange(new DataColumn[] 
            { 
                new DataColumn("GainPoint"),
                new DataColumn("SubSOIndex"),
                new DataColumn("PayType"),
                new DataColumn("SOSysNo"),
                new DataColumn("OrderTime"),
                new DataColumn("ShipType"),
                new DataColumn("ShipPeriod"),
                new DataColumn("CashPay"),
                new DataColumn("ShipPrice"),
                new DataColumn("PremiumAmount"),
                new DataColumn("ReceivableAmount"),
                new DataColumn("ChangeAmount"), 
                new DataColumn("ChangeAmountDisplay"), 
                new DataColumn("GiftCardPay"), 
                new DataColumn("GiftCardDisplay"), 
                new DataColumn("PointPay"), 
                new DataColumn("PointPayDisplay"), 
                new DataColumn("PrePay"), 
                new DataColumn("PrePayDisplay"), 
                new DataColumn("PayPrice"), 
                new DataColumn("PayPriceDisplay"), 
                new DataColumn("PromotionAmount"), 
                new DataColumn("PromotionDisplay"), 
                new DataColumn("Weight"), 
                new DataColumn("SOItem",typeof(DataTable))
                
            });
                subSOList.ForEach(subSOInfo =>
                {
                    DataTable subSOItemList = new DataTable();
                    subSOItemList.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ProductID"),
                        new DataColumn("ProductName"),
                        new DataColumn("Price"),
                        new DataColumn("Quantity"),
                        new DataColumn("Amount"),
                        new DataColumn("PagePath"),
                        new DataColumn("ImgSrc")
                    });
                    subSOInfo.Items.ForEach(subSOItem =>
                    {
                        pagePath = "http://www.kjt.com/product/detail/" + subSOItem.ProductSysNo ;
                        imgSrc = "http://image.kjt.com/neg/P60/" + subSOItem.ProductID + ".jpg";
                        ReplaceProductCM_MMC("SO_Splited", ref pagePath, subSOItem.ProductID);
                        subSOItemList.Rows.Add(new string[]
                            {
                                subSOItem.ProductID,
                                subSOItem.ProductName,
                                subSOItem.OriginalPrice.Value.ToString(SOConst.DecimalFormat),
                                subSOItem.Quantity.Value.ToString(), 
                                (subSOItem.Quantity.Value * subSOItem.OriginalPrice.Value).ToString(SOConst.DecimalFormat),
                                pagePath,
                                imgSrc
                            });
                    });

                    string changeAmount = (subSOInfo.BaseInfo.OriginalReceivableAmount - subSOInfo.BaseInfo.ReceivableAmount).ToString(SOConst.DecimalFormat);
                    subSO.Rows.Add(new object[]
                        {
                            subSOInfo.BaseInfo.GainPoint,
                            i++,
                            payType.PayTypeName,
                            subSOInfo.BaseInfo.SOID,
                            subSOInfo.BaseInfo.CreateTime.Value.ToString(SOConst.DateTimeFormat),
                            shippingType.ShippingTypeName,
                            shippingType.Period,
                            subSOInfo.BaseInfo.CashPay.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.ShipPrice.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.PremiumAmount.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.ReceivableAmount.ToString(SOConst.DecimalFormat),
                            changeAmount,
                            changeAmount != (0M).ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.GiftCardPay.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.GiftCardPay != 0,
                            subSOInfo.BaseInfo.PointPay,
                            subSOInfo.BaseInfo.PointPay != 0,
                            subSOInfo.BaseInfo.PrepayAmount.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.PrepayAmount != 0,
                            subSOInfo.BaseInfo.PayPrice.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.PayPrice != 0,
                            subSOInfo.BaseInfo.PromotionAmount.Value.ToString(SOConst.DecimalFormat),
                            subSOInfo.BaseInfo.PromotionAmount != 0,
                            subSOInfo.ShippingInfo.Weight,
                            subSOItemList
                        });

                    KeyTableVariables subSOItemTableVariables = new KeyTableVariables();
                });
                #endregion 

                keyTableVariables.Add("SubSOList", subSO);

                ExternalDomainBroker.SendExternalEmail(customerEmail, "SO_Splited", keyValueVariables, keyTableVariables, customerInfo.FavoriteLanguageCode);

            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        public void RecommendCustomerAddExperienceSendMail(ECCentral.BizEntity.Customer.CustomerBasicInfo recommendCustomerInfo, string newCustomerID, decimal experience)
        {
            try
            {
                string customerEmail = recommendCustomerInfo.Email == null ? null : recommendCustomerInfo.Email.Trim();
                if (customerEmail == null)
                {
                    return;
                }
                KeyValueVariables keyValueVariables = new KeyValueVariables();
                keyValueVariables.Add("RmdCustomerID", recommendCustomerInfo.CustomerID);
                keyValueVariables.Add("NewCustomerID", newCustomerID);
                keyValueVariables.Add("Experience", experience);
                ExternalDomainBroker.SendExternalEmail(customerEmail, "SO_ShipOut_UserAddExperience", keyValueVariables, recommendCustomerInfo.FavoriteLanguageCode);

            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        #endregion

        #region 发送短信
        public void SendSMS(SOInfo soInfo, BizEntity.Customer.SMSType type)
        {
            try
            {
                ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = GetCustomerBaseInfo(soInfo.BaseInfo.CustomerSysNo.Value);
                string customerLanguageCode = customerInfo.FavoriteLanguageCode;

                string smsContent = ObjectFactory<ECCentral.Service.IBizInteract.ICustomerBizInteract>.Instance.GetSMSContent(soInfo.WebChannel == null ? null : soInfo.WebChannel.ChannelID, customerLanguageCode, soInfo.ShippingInfo.ShipTypeSysNo.Value, type);

                if (string.IsNullOrEmpty(smsContent))
                {
                    return;
                }
                string mobilePhone = string.Empty;
                if (!string.IsNullOrEmpty(soInfo.ReceiverInfo.MobilePhone))
                {
                    mobilePhone = soInfo.ReceiverInfo.MobilePhone;
                }
                else
                {
                    mobilePhone = soInfo.ReceiverInfo.Phone;
                }

                if (smsContent.IndexOf("SO#") != -1)
                {
                    smsContent = smsContent.Replace("SO#", soInfo.BaseInfo.SOID);
                }
                else
                {
                    return;
                }

                // 上海仓库自提且分期付款,特殊短信内容
                ExternalDomainBroker.SendSMS(mobilePhone, smsContent, BizEntity.Common.SMSPriority.Normal);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }


        /// <summary>
        /// 发送增票SMS短信
        /// </summary>
        /// <param name="soInfo">SO 信息</param>        
        public void SendVATSMS(SOInfo soInfo)
        {
            try
            {
                if (soInfo.InvoiceInfo.IsVAT.Value)
                {
                    int stockSysNo = soInfo.Items[0].StockSysNo.Value;

                    string mobilePhone = string.Empty;
                    if (!string.IsNullOrEmpty(soInfo.ReceiverInfo.MobilePhone))
                    {
                        mobilePhone = soInfo.ReceiverInfo.MobilePhone;
                    }
                    else
                    {
                        mobilePhone = soInfo.ReceiverInfo.Phone;
                    }
                    ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = GetCustomerBaseInfo(soInfo.BaseInfo.CustomerSysNo.Value);
                    string customerLanguageCode = customerInfo.FavoriteLanguageCode;
                    string smsContent = ObjectFactory<ECCentral.Service.IBizInteract.ICustomerBizInteract>.Instance.GetSMSContent(soInfo.WebChannel == null ? null : soInfo.WebChannel.ChannelID, customerLanguageCode, soInfo.ShippingInfo.ShipTypeSysNo.Value, BizEntity.Customer.SMSType.OrderOutBound);

                    if (!AppSettingHelper.NEG_NotVAT_StockSysNoList.Exists(no => no == stockSysNo))
                    {
                        //发送短信
                        smsContent = ResourceHelper.Get("NEG_SMS_SO_VATSend");
                        ExternalDomainBroker.SendSMS(mobilePhone, smsContent, BizEntity.Common.SMSPriority.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }
        #endregion
    }
}