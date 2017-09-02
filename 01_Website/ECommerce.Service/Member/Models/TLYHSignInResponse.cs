using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Member.Models
{
    [Serializable]
    public class TLYHSignInResponse
    {
        public string customerId { get; set; }
        public string defalutAccountNo { get; set; }
        public string sessionId { get; set; }
        public string merchantNo { get; set; }
        public string orderNo { get; set; }
        public decimal orderAmt { get; set; }
        public string orderDate { get; set; }
        public DateTime orderTime { get; set; }
        public string currencyType { get; set; }
        public string goodsName { get; set; }
        public int goodsNum { get; set; }
        public string customerType { get; set; }
        public string certDN { get; set; }
        public string refNo { get; set; }
        public string tokenType { get; set; }
        public string tokenNo { get; set; }
        public string accountAlias { get; set; }
        public string accountFlag { get; set; }
        public string accountLastUse { get; set; }
        public string accountNo { get; set; }
        public string AccountType { get; set; }
        public string accountOpenNode { get; set; }
        public string accountName { get; set; }
        public string accountRight { get; set; }
        public string scoreType { get; set; }
        public int scoreMount { get; set; }
        public int scoreMountExpire { get; set; }
        public string status { get; set; }
        public string ifAutoJump { get; set; }
        public string jumpWaitTime { get; set; }
        public string merURL { get; set; }
        public string ifInfoMer { get; set; }
        public string infoMerURL { get; set; }
        public string mobileNo { get; set; }
    }
}
