using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using System.Xml.Linq;
using System.IO;
using Newegg.Oversea.Framework.Utilities;

namespace SendNoticeMailForGiftCard
{
    internal class DA
    {
        public static List<EmailList> GetCustomerLastBuyTime(int days)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetEmailList");
            command.SetParameterValue("@Days", days);
            command.SetParameterValue("@CompanyCode", "8601");
            List<EmailList> result = command.ExecuteEntityList<EmailList>();
            return result;
        }

        public static int InsertEmail(string emailAddress,string mailSubject,string mailBody)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertEmail");
            command.SetParameterValue("@emailAddress", emailAddress);
            command.SetParameterValue("@mailSubject", mailSubject);
            command.SetParameterValue("@mailBody", mailBody);
            int count = 0;
            count = command.ExecuteNonQuery();
            return count;
        }
        public static List<EmailList> GetExpiredCodeList(int days)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExpiredCodeList");
            command.SetParameterValue("@Days", days);
            command.SetParameterValue("@CompanyCode", "8601");
            List<EmailList> result = command.ExecuteEntityList<EmailList>();
            return result;
        }
        public static string ExpiredVoid(
            List<string> paramGiftCardlist,
            string editUser,
            string path
            )
        {
            if (paramGiftCardlist == null || paramGiftCardlist.Count == 0)
            {
                return string.Empty;
            }

            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>();
            foreach (var item in paramGiftCardlist)
            {
                GiftCardsElement.Add(
                    new GiftCard()
                    {
                        Code = item
                    }
                    );
            }
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "ExpiredVoid",
                    Version = "V1",
                    From = "IPP.Customer.Job",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = editUser,
                    Memo = string.Empty,
                    GiftCard = GiftCardsElement
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);
            DataCommand dc = DataCommandManager.GetDataCommand("OperateGiftCard");
            dc.SetParameterValue("@Msg", paramXml.ToString());
            dc.ExecuteNonQuery();

            object o = dc.GetParameterValue("@StatusCode");
            if (o != null && o != DBNull.Value)
            {
                return o.ToString();
            }
            return string.Empty;
        }
    }
}