using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    public class BindingGiftCardInfoInitializer : IInitialize
    {
        public void Initialize(ref OrderInfo order)
        {
            order.BindingGiftCardList = PipelineDA.GetCustomerBindingGiftCardInfoList(order.Customer.SysNo);
            if (order.BindingGiftCardList != null)
            {
                //礼品卡按到期时间从近到远排列，引导用户优先使用快到期的礼品卡
                order.BindingGiftCardList.Sort((giftCard1, giftCard2) =>
                {
                    var diffDate1 = giftCard1.ValidEndDate.Subtract(DateTime.Today);
                    var diffDate2 = giftCard2.ValidEndDate.Subtract(DateTime.Today);

                    return diffDate1.CompareTo(diffDate2);
                });

                foreach (var giftCardInfo in order.BindingGiftCardList)
                {
                    giftCardInfo["ActAvailableAmount"] = giftCardInfo.AvailableAmount;
                }
            }
        }
    }
}
