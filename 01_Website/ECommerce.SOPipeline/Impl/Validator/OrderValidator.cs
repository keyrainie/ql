using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    public class OrderValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            bool checkResult = PipelineDA.CheckCustomerOrderFrequency(order.Customer.SysNo, TimeSpan.FromMinutes(1));
            if (!checkResult)
            {
                errorMsg = "请不要在1分钟之内重复下单，请到帐户中心查看您的订单。";
                return checkResult;
            }
            errorMsg = "";
            return checkResult;
        }
    }
}
