using IPP.OrderMgmt.SendSOPayToEPortJob.Dac.Common;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendSOPayToEPortJob.BusinessEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SendSOPayToEPortJob.Dac.Common
{
    public class NingBoAPI
    {
        public static JobContext SentSo(JobContext context, List<int> soIdList)
        {
            //电商企业代码
            string CustomsCode = System.Configuration.ConfigurationManager.AppSettings["CustomsCode"];
            //电商企业名称
            string OrgName = System.Configuration.ConfigurationManager.AppSettings["OrgName"];
            //店铺代码
            string OrderShop = System.Configuration.ConfigurationManager.AppSettings["OrderShop"];
            //电商代码
            string OrderFrom = System.Configuration.ConfigurationManager.AppSettings["OrderFrom"];
            //APIUrl
            string APIQA = System.Configuration.ConfigurationManager.AppSettings["APIQA"];
            string APIPRD = System.Configuration.ConfigurationManager.AppSettings["APIPRD"];
            //快递配置
            string SFKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["SFKuaiDiQA"];
            string YZSDKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["YZSDKuaiDiQA"];
            string ZTKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["ZTKuaiDiQA"];
            string YZXBKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["YZXBKuaiDiQA"];
            string STKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["STKuaiDiQA"];
            string JDKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["JDKuaiDiQA"];
            string YTKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["YTKuaiDiQA"];
            string BSKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["BSKuaiDiQA"];
            string YDKuaiDiQA = System.Configuration.ConfigurationManager.AppSettings["YDKuaiDiQA"];
            //账号
            string userid = System.Configuration.ConfigurationManager.AppSettings["userid"];
            //密钥
            string PassWord = System.Configuration.ConfigurationManager.AppSettings["PassWord"];
            //API类型
            string msgtype = System.Configuration.ConfigurationManager.AppSettings["msgtype"];
            //关区代码
            string customs = System.Configuration.ConfigurationManager.AppSettings["customs"];
            context.Message += string.Format("{0}宁波跨境开始推送订单，发现{1}个订单\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "), soIdList.Count);
            if (soIdList.Count > 0)
            {
                foreach (int SOID in soIdList)
                {
                    OrderInfo orderInfo = CommonDA.GetCenterSODetailInfo(SOID);
                    List<SOItemInfo> itemListPromotion = orderInfo.SOItemList.FindAll(t => t.ProductType == SOItemType.Promotion);
                    //优惠金额合计
                    decimal DisAmount = -orderInfo.Amount.DiscountAmt;
                    if (itemListPromotion.Count > 0)
                    {
                        DisAmount = DisAmount - itemListPromotion[0].OriginalPrice;
                    }
                    StringBuilder XMLBuilder = new StringBuilder();
                    XMLBuilder.Append("<?xml version='1.0' encoding='UTF-8' ?>");
                    XMLBuilder.Append("<Message><Header>");
                    XMLBuilder.Append("<CustomsCode>" + CustomsCode + "</CustomsCode>");
                    XMLBuilder.Append("<OrgName>" + OrgName + "</OrgName>");
                    XMLBuilder.Append("<CreateTime>" + orderInfo.OrderDate.ToString("yyyy-MM-dd HH:mm:ss") + "</CreateTime></Header>");
                    XMLBuilder.Append("<Body>");
                    #region 订单信息
                    XMLBuilder.Append("<Order>");
                    XMLBuilder.Append("<Operation>" + 0 + "</Operation>");  //操作标识（0=新增，1=更新）
                    XMLBuilder.Append("<MftNo></MftNo>");                   //申报单号，更新场合时为必填
                    XMLBuilder.Append("<OrderShop>" + OrderShop + "</OrderShop>");
                    XMLBuilder.Append("<OTOCode></OTOCode>");
                    XMLBuilder.Append("<OrderFrom>" + OrderFrom + "</OrderFrom>");
                    XMLBuilder.Append("<PackageFlag></PackageFlag>");      //是否组合装标识（0=不是，1=是）
                    XMLBuilder.Append("<OrderNo>" + orderInfo.SoSysNo + "</OrderNo>");
                    if (orderInfo.Amount.ShipPrice > 0)
                    {
                        XMLBuilder.Append("<PostFee>" + orderInfo.Amount.ShipPrice.ToString("f3") + "</PostFee>");
                    }
                    else
                    {
                        XMLBuilder.Append("<PostFee>0</PostFee>");
                    }
                    XMLBuilder.Append("<Amount>" + orderInfo.RealPayAmt.ToString("f3") + "</Amount>");
                    XMLBuilder.Append("<BuyerAccount>" + orderInfo.CustomerID + "</BuyerAccount>");
                    XMLBuilder.Append("<Phone>" + orderInfo.CellPhone + "</Phone>");
                    XMLBuilder.Append("<Email>" + orderInfo.Email + "</Email>");
                    if (orderInfo.TariffAmt > 50)
                    {
                        XMLBuilder.Append("<TaxAmount>" + orderInfo.TariffAmt.ToString("f3") + "</TaxAmount>");
                    }
                    else
                    {
                        XMLBuilder.Append("<TaxAmount>0</TaxAmount>");
                    }
                    #region 优惠金额
                    if (DisAmount > 0)
                    {
                        XMLBuilder.Append("<DisAmount>" + DisAmount.ToString("f3") + "</DisAmount>");
                        XMLBuilder.Append("<Promotions>");
                        //捆绑
                        if (orderInfo.SOPromotions.Count > 0)
                        {
                            foreach (var item in orderInfo.SOPromotions)
                            {
                                XMLBuilder.Append("<Promotion>");
                                XMLBuilder.Append("<ProAmount>" + (-item.DiscountAmount).Value.ToString("f3") + "</ProAmount>");
                                XMLBuilder.Append("<ProRemark>" + item.PromotionName + "</ProRemark>");
                                XMLBuilder.Append("</Promotion>");
                            }
                        }
                        //优惠卷
                        if (itemListPromotion.Count > 0)
                        {
                            XMLBuilder.Append("<Promotion>");
                            XMLBuilder.Append("<ProAmount>" + (-itemListPromotion[0].OriginalPrice).ToString("f3") + "</ProAmount>");
                            XMLBuilder.Append("<ProRemark>" + itemListPromotion[0].BriefName + "</ProRemark>");
                            XMLBuilder.Append("</Promotion>");
                        }
                        XMLBuilder.Append("</Promotions>");
                    }
                    else
                    {
                        XMLBuilder.Append("<DisAmount>0</DisAmount>");
                    }
                    #endregion
                    #region 商品明细
                    XMLBuilder.Append("<Goods>");
                    if (orderInfo.SOItemList.Count > 0)
                    {
                        List<SOItemInfo> soitemL = orderInfo.SOItemList.FindAll(t => t.ProductType != SOItemType.Promotion);
                        foreach (var item in soitemL)
                        {
                            decimal total = 0;
                            if (orderInfo.TariffAmt <= 50)
                            {
                                total = item.Quantity * item.OriginalPrice;
                            }
                            else
                            {
                                total = item.Quantity * (item.OriginalPrice + item.TariffPrice);
                            }
                            XMLBuilder.Append("<Detail>");
                            XMLBuilder.Append("<ProductId>" + item.Product_SKUNO + "</ProductId>");
                            XMLBuilder.Append("<GoodsName>" + item.BriefName + "</GoodsName>");
                            XMLBuilder.Append("<Qty>" + item.Quantity + "</Qty>");
                            XMLBuilder.Append("<Unit>" + item.ApplyUnit + "</Unit>");
                            XMLBuilder.Append("<Price>" + item.OriginalPrice.ToString("f3") + "</Price>");
                            XMLBuilder.Append("<Amount>" + total.ToString("f3") + "</Amount>");
                            XMLBuilder.Append("</Detail>");
                        }
                    }
                    XMLBuilder.Append("</Goods>");
                    #endregion
                    XMLBuilder.Append("</Order>");
                    #endregion
                    #region 支付信息
                    XMLBuilder.Append("<Pay>");
                    XMLBuilder.Append("<Paytime>" + orderInfo.PayTime.ToString("yyyy-MM-dd HH:mm:ss") + "</Paytime>");
                    XMLBuilder.Append("<PaymentNo>" + orderInfo.PayTransNumber + "</PaymentNo>");
                    XMLBuilder.Append("<OrderSeqNo>" + orderInfo.PayTransNumber + "</OrderSeqNo>");
                    if (orderInfo.Payment.PayTypeID == 112)
                    {
                        XMLBuilder.Append("<Source>02</Source>");
                    }
                    else if (orderInfo.Payment.PayTypeID == 113)
                    {
                        XMLBuilder.Append("<Source>13</Source>");
                    }
                    else
                    {
                        XMLBuilder.Append("<Source>01</Source>");
                    }
                    XMLBuilder.Append("<Idnum>" + orderInfo.IDCardNumber + "</Idnum>");
                    XMLBuilder.Append("<Name>" + orderInfo.Name + "</Name>");
                    XMLBuilder.Append("</Pay>");
                    #endregion
                    #region 收货信息
                    XMLBuilder.Append("<Logistics>");
                    XMLBuilder.Append("<LogisticsNo></LogisticsNo>");
                    if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(SFKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>顺丰速运</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(YTKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>圆通速递</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(STKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>申通快递</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(ZTKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>中通速递</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(BSKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>百世物流</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(YDKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>韵达速递</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(YZSDKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>邮政速递</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(YZXBKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>邮政小包</LogisticsName>");
                    }
                    else if (orderInfo.ShipType.ShipTypeSysNo == int.Parse(JDKuaiDiQA))
                    {
                        XMLBuilder.Append("<LogisticsName>京东快递</LogisticsName>");
                    }
                    else
                    {
                        XMLBuilder.Append("<LogisticsName>" + orderInfo.ShipType.ShipTypeName + "</LogisticsName>");
                    }

                    XMLBuilder.Append("<Consignee>" + orderInfo.ReceiveContact + "</Consignee>");
                    XMLBuilder.Append("<Province>" + orderInfo.ProvinceName + "</Province>");
                    XMLBuilder.Append("<City>" + orderInfo.CityName + "</City>");
                    if (string.IsNullOrEmpty(orderInfo.DistrictName))
                    {
                        XMLBuilder.Append("<District>" + orderInfo.CityName + "</District>");
                    }
                    else
                    {
                        XMLBuilder.Append("<District>" + orderInfo.DistrictName + "</District>");
                    }
                    XMLBuilder.Append("<ConsigneeAddr>" + orderInfo.ReceiveAddress + "</ConsigneeAddr>");
                    XMLBuilder.Append("<ConsigneeTel>" + orderInfo.ReceiveCellPhone + "</ConsigneeTel>");
                    XMLBuilder.Append("<MailNo>" + orderInfo.ReceiveZip + "</MailNo>");
                    XMLBuilder.Append("<GoodsName></GoodsName>");
                    XMLBuilder.Append("</Logistics>");
                    #endregion
                    XMLBuilder.Append("</Body>");
                    XMLBuilder.Append("</Message>");

                    #region API
                    Uri address = new Uri(APIQA);

                    // Create the web request
                    HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                    // Add authentication to request
                    //request.Credentials = new NetworkCredential(userid, PassWord);
                    // Set type to POST
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Create the data we want to send
                    string Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string timestamp = System.Web.HttpUtility.UrlEncode(Date, Encoding.UTF8);

                    string sign = ECCentral.Service.Utility.Hash_MD5.GetMD5(userid + PassWord + Date);
                    string xmlstr = System.Web.HttpUtility.UrlEncode(XMLBuilder.ToString());
                    StringBuilder data = new StringBuilder();
                    data.Append("userid=" + userid);
                    data.Append("&timestamp=" + timestamp);
                    data.Append("&sign=" + sign);
                    data.Append("&xmlstr=" + xmlstr);
                    data.Append("&msgtype=" + msgtype);
                    data.Append("&customs=" + customs);
                    // Create a byte array of the data we want to send
                    byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
                    // Set the content length in the request headers
                    request.ContentLength = byteData.Length;
                    // Write data
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                    // Get response
                    string strBuff = "";
                    char[] cbuffer = new char[256];
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        try
                        {
                            // Get the response stream
                            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                            // Console application output
                            //Console.WriteLine(reader.ReadToEnd());
                            int byteRead = reader.Read(cbuffer, 0, 256);
                            while (byteRead != 0)
                            {
                                string strResp = new string(cbuffer, 0, byteRead);
                                strBuff = strBuff + strResp;
                                byteRead = reader.Read(cbuffer, 0, 256);
                            }
                            Console.WriteLine(strBuff);
                            XDocument doc = XDocument.Parse(strBuff);
                            XElement root = doc.Root;
                            XElement Header = root.Element("Header");
                            //T：操作成功；F：操作失败
                            string Result = GetElementValue(Header, "Result");
                            //结果描述（操作失败时必需）
                            string ResultMsg = GetElementValue(Header, "ResultMsg");
                            //申报单号（审核成功返回）
                            string MftNo;
                            if (Result == "T")
                            {
                                MftNo = GetElementValue(Header, "MftNo");
                                if (!string.IsNullOrEmpty(MftNo))
                                {
                                    CommonDA.UpdateOrderStatus(orderInfo.SoSysNo);
                                }
                            }
                            context.Message += strBuff;
                        }
                        catch (Exception ex)
                        {
                            context.Message += ex.Message;
                        }
                    }
                    #endregion
                }
            }

            context.Message += string.Format("\r\n{0}宁波跨境推送结束\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            return context;
        }
        /// <summary>
        /// 获取子节点值
        /// </summary>
        /// <param name="parentElement">根节点</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        private static string GetElementValue(XElement parentElement, string key)
        {
            if (parentElement != null && !string.IsNullOrEmpty(key))
            {
                XElement element = parentElement.Element(key.Trim());
                if (element != null)
                    return element.Value;
            }

            return string.Empty;
        }
    }
}
