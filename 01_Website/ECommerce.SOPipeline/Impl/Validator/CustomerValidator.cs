using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    public class CustomerValidator : IValidate
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
            bool checkCustomerPass = true;

            if (order.Customer == null)
            {
                checkCustomerPass = false;
            }
            else
            {
                bool customerExist = PipelineDA.IsExistCustomer(order.Customer.SysNo);

                if (!customerExist)
                {
                    checkCustomerPass = false;
                }
            }

            if (!checkCustomerPass)
            {
                errorMsg = "您尚未登录或当前用户不存在，请您重新登录后再试";
            }
            else
            {
                errorMsg = null;
            }

            return checkCustomerPass;
        }
    }
}
