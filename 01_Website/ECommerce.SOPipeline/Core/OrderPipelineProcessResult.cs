using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public class OrderPipelineProcessResult
    {
        internal static OrderPipelineProcessResult Succeed(OrderInfo  orderInfo)
        {
            return new OrderPipelineProcessResult() { HasSucceed = true, ReturnData=orderInfo };
        }

        internal static OrderPipelineProcessResult Failed(List<string> errorMsgs,OrderInfo orderInfo)
        {
            OrderPipelineProcessResult rst = new OrderPipelineProcessResult();
            rst.HasSucceed = false;
            rst.m_ErrorMessages = errorMsgs;
            rst.ReturnData = orderInfo;
            return rst;
        }

        private List<string> m_ErrorMessages;

        public bool HasSucceed { get; set; }

        public List<string> ErrorMessages
        {
            get
            {
                //Lazy load
                if (m_ErrorMessages == null)
                {
                    m_ErrorMessages = new List<string>();
                }
                return m_ErrorMessages;
            }
            set
            {   
                m_ErrorMessages = value;
            }
        }

        public OrderInfo ReturnData { get;  set; }
    }
}
