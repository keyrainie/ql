using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Topic
{
    [Serializable]
    public class Subscription
    {
        public int? TransactionNumber { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public string Status { get; set; }
        public string InUser { get; set; }
        public DateTime? InDate { get; set; }
        public string EditUser { get; set; }
        public DateTime? EditDate { get; set; }

        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public string StoreCompanyCode { get; set; }
        public string Type { get; set; }

        public int SubscriptionCategorySysNo { get; set; }
    }
}
