using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    public abstract class ETPCalculatorBase : IETPCalculator
    {
        #region IETPCalculator Members

        public abstract DateTime? Calculate();

        #endregion IETPCalculator Members

        /// <summary>
        /// 转换时间（调整周三、六、日的时间）---【可以重写】
        /// </summary>
        /// <param name="time">需要转换的时间</param>
        /// <returns>转换后的时间</returns>
        protected virtual DateTime? ConvertWorkDate(DateTime? time)
        {
            return ETPCalculatorHelper.ConvertWorkDate(time);
        }

        /// <summary>
        /// 获取到货时间（发票到和发票未到）
        /// </summary>
        /// <returns>天数信息</returns>
        protected virtual int GetItemInAfterDay(VendorPayPeriodType payPeriodType)
        {
            return ETPCalculatorHelper.GetItemInAfterDay(payPeriodType);
        }
    }
}