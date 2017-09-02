using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(PrepayProcessor))]
    public class PrepayProcessor
    {
        #region IPrepayProcess Members

        public virtual void Adjust(BizEntity.Customer.CustomerPrepayLog adjustInfo)
        {
            CustomerInfo original = ObjectFactory<CustomerProcessor>.Instance.GetCsutomerDeatilInfo(adjustInfo.CustomerSysNo.Value);
            using (TransactionScope tran = new TransactionScope())
            {
                //1.更新余额
                ObjectFactory<IPrepayDA>.Instance.UpdatePrepay(adjustInfo.CustomerSysNo.Value, adjustInfo.AdjustAmount.Value);
                //2.记录业务日志
                ObjectFactory<IPrepayDA>.Instance.CreatePrepayLog(adjustInfo);
                //3.记录操作日志
                ExternalDomainBroker.CreateOperationLog(string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "UpdateValidPrepayAmt"), original.ValidPrepayAmt, original.ValidPrepayAmt + adjustInfo.AdjustAmount), BizEntity.Common.BizLogType.Basic_Customer_Update, adjustInfo.SOSysNo.Value, original.CompanyCode);
                tran.Complete();
            }
        }

        public virtual decimal GetValidPrepay(int customerSysNo)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetCsutomerDeatilInfo(customerSysNo).ValidPrepayAmt.Value;
        }

        #endregion
    }
}
