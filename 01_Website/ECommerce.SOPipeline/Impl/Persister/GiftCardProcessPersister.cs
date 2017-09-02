using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ECommerce.SOPipeline.Impl
{
    public class GiftCardProcessPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            if (order.GiftCardList == null || order.GiftCardList.Count <= 0)
            {
                return;
            }

            foreach (GiftCardInfo giftCardInfo in order.GiftCardList)
            {
                //更新礼品卡信息
                PipelineDA.UpdateGiftCardInfo(giftCardInfo);
            }

            foreach (var kvs in order.SubOrderList)
            {
                OrderInfo subPreOrderInfo = kvs.Value;

                XDocument xDoc = new XDocument(
                    new XElement("Message",
                           new XElement("Header"
                                   , new XElement("Action", "SOConsume")
                                   , new XElement("Version", "V1")
                                   , new XElement("NameSpace", "http://soa.ECommerce.com/CustomerProfile")
                                   , new XElement("From", "EC")
                                   , new XElement("CurrencySysNo", order["CurrencySysNo"])
                                   , new XElement("Language", "zh-CN")
                                   , new XElement("CompanyCode", "8601")
                                   , new XElement("StoreCompanyCode", "8601")
                           ),
                           new XElement("Body"
                                   , new XElement("Memo", "")
                                   , new XElement("EditUser", "WebSite")
                           )
                    ));

                int priority = 1;
                foreach (GiftCardInfo giftCardInfo in subPreOrderInfo.GiftCardList)
                {
                    giftCardInfo["SOSysNo"] = subPreOrderInfo.ID;
                    giftCardInfo["CustomerSysNo"] = subPreOrderInfo.Customer.SysNo;
                    giftCardInfo["CurrencySysNo"] = subPreOrderInfo["CurrencySysNo"];

                    //创建礼品卡使用日志
                    PipelineDA.CreateGiftCardRedeemLog(giftCardInfo);

                    XElement giftCardElement = new XElement("GiftCard");
                    giftCardElement.Add(new XElement("Code", giftCardInfo.Code.Trim()));
                    giftCardElement.Add(new XElement("Priority", priority++));
                    giftCardElement.Add(new XElement("UseAmount", giftCardInfo.UseAmount));
                    giftCardElement.Add(new XElement("CustomerSysno", subPreOrderInfo.Customer.SysNo));
                    giftCardElement.Add(new XElement("ReferenceSOSysNo", subPreOrderInfo.ID));

                    xDoc.Root.Element("Body").Add(giftCardElement);
                }

                //创建礼品卡操作记录
                string xDocMsg = xDoc.ToString();
                PipelineDA.CreateGiftCardOperateLog(xDocMsg);
            }
        }
    }
}
