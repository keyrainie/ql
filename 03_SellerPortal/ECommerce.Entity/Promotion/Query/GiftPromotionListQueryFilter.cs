using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class GiftPromotionListQueryFilter : QueryFilter
    {

        public string ActivitySysNo { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public string ActivityStatus { get; set; }

        public string ActivityTypeEnumText
        {
            get
            {
                if (!string.IsNullOrEmpty(ActivityType))
                {
                    SaleGiftType giftTypeEnum = (SaleGiftType)Enum.Parse(typeof(SaleGiftType), ActivityType);
                    return giftTypeEnum.ToString().Substring(0, 1).ToUpper();
                }
                return null;
            }
        }

        public string ActivityStatusEnumText
        {
            get
            {
                if (!string.IsNullOrEmpty(ActivityStatus))
                {
                    switch (ActivityStatus)
                    {
                        case "0":
                            return "O";
                        case "1":
                            return "P";
                        case "2":
                            return "R";
                        case "3":
                            return "A";
                        case "4":
                            return "S";
                        case "5":
                            return "F";
                        case "6":
                            return "D";
                        default:
                            break;
                    }
                }
                return null;
            }
        }
        public string ActivityStatusText { get; set; }
        public string MainProductSysNo { get; set; }
        public string GiftProductSysNo { get; set; }
        public string ActivityDateFrom { get; set; }
        public string ActivityDateTo { get; set; }
        public string InUser { get; set; }
        public int VendorSysNo { get; set; }
        public string CompanyCode { get; set; }
    }
}
