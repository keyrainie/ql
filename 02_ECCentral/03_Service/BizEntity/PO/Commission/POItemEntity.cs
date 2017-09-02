using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO.Commission
{
    public class POItemEntity : ICompany
    {

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion

        public int? UnActivatyCount { get; set; }

        public string BatchInfo { get; set; }

        public int? AcquireReturnPointType { get; set; }
        public decimal? AcquireReturnPoint { get; set; }

        public int? SysNo { get; set; }

        public int? POSysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public string BriefName { get; set; }

        public int? Quantity { get; set; }

        public int? Weight { get; set; }

        public Decimal? OrderPrice { get; set; }

        public Decimal? ApportionAddOn { get; set; }        //摊销被取消

        public Decimal? UnitCost { get; set; }              //采购成本

        public Decimal? ReturnCost { get; set; }

        public int? PurchaseQty { get; set; }

        public int? CheckStatus { get; set; }

        public string CheckReasonMemo { get; set; }

        public Decimal? LastOrderPrice { get; set; }

        public int? ExecptStatus { get; set; }

        public string ProductID { get; set; }

        public decimal? UnitCostWithoutTax { get; set; }

        public decimal? JDPrice { get; set; }

        public int? AvailableQty { get; set; }      //有效库存

        public int? m1 { get; set; } //上月销售总量

        public int? CurrencySysNo { get; set; }

        public decimal? CurrentUnitCost { get; set; }

        public decimal? CurrentPrice { get; set; }

        public DateTime? LastAdjustPriceDate { get; set; }

        public DateTime? LastInTime { get; set; }


        public int ManufacturerSysNo { get; set; }

        public int C2SysNo { get; set; }

        public int C3SysNo { get; set; }

        public decimal? VirtualPrice { get; set; }

        //crl 18210 prince 2011-03-07
        public int? ReadyQuantity { get; set; }
    }
}
