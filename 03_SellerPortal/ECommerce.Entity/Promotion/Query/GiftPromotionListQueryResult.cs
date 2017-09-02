using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.WebFramework;

namespace ECommerce.Entity.Promotion
{
    public class GiftPromotionListQueryResult
    {
        public int SysNo { get; set; }
        public string PromotionName { get; set; }
        public SaleGiftType? Type { get; set; }
        public SaleGiftStatus? Status { get; set; }
        public string TypeName
        {
            get
            {
                return Type.HasValue ? Type.Value.GetEnumDescription() : string.Empty;
            }
        }
        public string StatusName
        {
            get
            {
                return Status.HasValue ? Status.Value.GetEnumDescription() : string.Empty;
            }
        }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime InDate { get; set; }

        public string BeginDateString
        {
            get
            {
                return BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public string EndDateString
        {
            get
            {
                return EndDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public string InDateString
        {
            get
            {
                return InDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public string InUser { get; set; }
    }
}
