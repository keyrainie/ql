using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Refund
{
    public class RefundPointInfo : SOIncomeRefundInfo, ICloneable
    {
        public decimal OrderAmt { get; set; }

        public decimal PayAmt { get; set; }

        public int PayTypeSysNo { get; set; }

        public string ReferenceID { get; set; }

        public string ExternalKey { get; set; }

        public string ResponseContent { get; set; }

        public int? CurrencySysNo { get; set; }

        public RefundPointStatus RefundStatus { get; set; }

        public DateTime EditDate { get; set; }

        public string InUser { get; set; }

        public string EditUser { get; set; }

        public string MemberID { get; set; }

        public decimal RefundAmount { get; set; }

        public DateTime TTTime { get; set; }

        public int RefundLogSysNo { get; set; }

        public bool FirstRedeem { get; set; }

        public int RefundInfoType { get; set; }

        public string RefundInfo { get; set; }

        public bool IsRedeemFailure { get; set; }

        public object Clone()
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, this);
                buffer.Position = 0;
                object temp = formatter.Deserialize(buffer);
                return temp;
            }
        }
    }
}
