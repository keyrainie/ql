using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Enums;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class ConsultationInfoViewModel
    {
        public int SysNo { get; set; }
        public int ProductSysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int ReplyCount { get; set; }
        public DateTime InDate { get; set; }
        public string InDateString { get { return InDate.ToString("yyyy年MM月dd日 HH:mm:ss"); } }
        public DateTime EditDate { get; set; }
        public string EditDateString { get { return EditDate.ToString("yyyy年MM月dd日 HH:mm:ss"); } }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public ProductStatus productStatus { get; set; }
        public string Status { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CashRebate { get; set; }
        public string ImageVersion { get; set; }
        public string DefaultImage { get; set; }
        public string ProductTitle { get; set; }
        public string ProductPromotionTitle { get; set; }
    }
}