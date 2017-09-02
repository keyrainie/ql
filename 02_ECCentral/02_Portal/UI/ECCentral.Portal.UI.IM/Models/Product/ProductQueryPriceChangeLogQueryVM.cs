using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductQueryPriceChangeLogQueryVM:ModelBase
    {

        public ProductQueryPriceChangeLogQueryVM()
        {
            ListProductPriceType = new List<cbItem>()
            {
                  new cbItem(){ItemKey=null,ItemValue="--" + ResProductPriceChangeLog.EnumProductPriceTypeText1 + "--"},
                new cbItem(){ItemKey="CountDown",ItemValue=ResProductPriceChangeLog.EnumProductPriceTypeText2},
                new cbItem(){ItemKey="PMAdjust",ItemValue=ResProductPriceChangeLog.EnumProductPriceTypeText3},
                new cbItem(){ItemKey="ComparePrice",ItemValue=ResProductPriceChangeLog.EnumProductPriceTypeText4},
                new cbItem(){ItemKey="Seller",ItemValue=ResProductPriceChangeLog.EnumProductPriceTypeText5},
                new cbItem(){ItemKey="GroupBuying",ItemValue=ResProductPriceChangeLog.EnumProductPriceTypeText6},
            };
        }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        private string productSysno;
         [Validate(ValidateType.Interger)]
        public string ProductSysno {
            get { return productSysno; }
            set { SetValue("ProductSysno", ref productSysno, value); }
         }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string PriceLogType { get; set; }

        public List<cbItem> ListProductPriceType { get; set; }

       
    }
         public class cbItem
        {
            public string ItemKey { get; set; }
            public string ItemValue { get; set; }
        }
}
