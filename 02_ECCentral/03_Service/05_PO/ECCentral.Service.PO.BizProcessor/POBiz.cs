using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.PO.Commission;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.BizProcessor
{
    public class POBiz
    {
        private ICollectionPaymentDA m_CollectionPaymentDA;
        public ICollectionPaymentDA CollectionPaymentDA
        {
            get
            {
                if (null == m_CollectionPaymentDA)
                {
                    m_CollectionPaymentDA = ObjectFactory<ICollectionPaymentDA>.Instance;
                }
                return m_CollectionPaymentDA;
            }
        }
        public void WriteLogPOBatchInstock(POBatchInfo info)
        {
            string strContent = string.Empty;
            strContent += "POSysNo:" + info.POSysNo;
            strContent += "BatchNumber:" + info.BatchNumber;
            strContent += "BatchInStockAmt:" + info.BatchInStockAmt;
            strContent += "OperationUserSysNo:" + info.OperationUserSysNo;
            strContent += "POStatus:" + info.POStatus;
            Logger.WriteLog(strContent, "PO");
        }

        public void UpdatePOInstockAmtAndStatus(int poSysNo, int poStatus)
        {
            CollectionPaymentDA.UpdatePOInstockAmtAndStatus(poSysNo, poStatus);
        }

        public void UpdateInvoiceInfo(int poSysNo)
        {
            POEntity po = CollectionPaymentDA.GetPOMaster(poSysNo);
            if (po == null)
            {
                throw new BizException(string.Format("不存在编号为{0}的PO单!", poSysNo));
            }
            if (4 != (int)po.Status.Value && 6 != (int)po.Status.Value)
            {
                throw new BizException("当前Po单不是部分入库或已入库状态，不能生成应付款!");
            }

        }
    }
}
