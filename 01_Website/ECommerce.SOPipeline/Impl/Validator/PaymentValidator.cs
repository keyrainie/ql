using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Payment;

namespace ECommerce.SOPipeline.Impl
{
    public class PaymentValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            bool pass = true;

            //先检查主单
            pass = InnerValidate(order, out errorMsg);

            if (!pass) return pass;

            //检查拆分后的子单
            if (order.SubOrderList != null && order.SubOrderList.Count > 0)
            {
                OrderInfo subOrder = null;
                foreach (var kvs in order.SubOrderList)
                {
                    subOrder = kvs.Value;
                    pass = InnerValidate(subOrder, out errorMsg);
                    if (!pass) return pass;
                }
            }
            errorMsg = null;
            return true;
        }

        private bool InnerValidate(OrderInfo order, out string errorMsg)
        {
            if (string.IsNullOrEmpty(order.PayTypeID))
            {
                errorMsg = "请选择一种支付方式";
                return false;
            }

            PayTypeInfo payTypeInfo = PipelineDA.GetPayTypeBySysNo(order.PayTypeID);

            if (payTypeInfo == null)
            {
                errorMsg = "不支持您选择的支付方式，请重新选择";
                return false;
            }

            errorMsg = null;
            return true;
        }
    }
}
