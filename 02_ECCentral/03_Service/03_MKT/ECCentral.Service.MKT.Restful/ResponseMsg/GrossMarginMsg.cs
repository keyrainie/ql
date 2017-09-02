using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.Restful.ResponseMsg
{
    public class GrossMarginMsg
    {
        public decimal GrossMargin { get; set; }
        public decimal GrossMarginWithOutPointAndGift { get; set; }
        public decimal GrossMarginRate { get; set; }
        public decimal GrossMarginRateWithOutPointAndGift { get; set; }

        public int? GiftSysNo { get; set; }
        public int? CouponSysNo { get; set; }

        #region 毛利率相关
        /// <summary>
        /// 毛利
        /// </summary>
        public decimal CountDownMargin { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal CountDownMarginRate { get; set; }

        #endregion
    }
}
