using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(VendorRefundProcessor))]
    public class VendorRefundProcessor
    {
        public virtual void DeductOnVendorQty(int registerSysNo, int stockSysNo, int productSysNo, int deductQuantity)
        {
#warning 发送SSB消息 sp:[dbo].[UP_InventoryDeduction]
            //string msg = BulidSSB(entity);
            //int rowCount = this.m_vendorRefundDAL.DeductOnVendorQty(msg);
        }

        public virtual void BatchCloseRegisterForVendorRefund(List<int> registerSysNoList)
        {            
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                foreach (int registerSysNO in registerSysNoList)
                {
                    RMARegisterInfo reg = new RMARegisterInfo()
                    {
                        SysNo = registerSysNO,                                               
                    };
                    ObjectFactory<RegisterProcessor>.Instance.CloseForVendorRefund(reg);
                }
                scope.Complete();
            }
        }
    }
}
