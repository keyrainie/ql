using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.BizProcessor;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOInstalmentAppService))]
    public class SOInstalmentAppService
    {
        public virtual List<int> GetAllInstalmentPayTypeSysNos()
        {
            return ObjectFactory<SOInstalmentProcessor>.Instance.GetAllInstalmentPayTypeSysNos();
        }

        public virtual List<int> GetOnlinePayTypeSysNos()
        {
            return ObjectFactory<SOInstalmentProcessor>.Instance.GetOnlinePayTypeSysNos();
        }

        public virtual SOInstallmentInfo SaveSOInstallmentWhenCreateSO(SOInstallmentInfo entity)
        {
            return ObjectFactory<SOInstalmentProcessor>.Instance.SaveSOInstallmentWhenCreateSO(entity);
        }

        public virtual SOInstallmentInfo UpdateSOInstallmentWithoutCreditCardInfo(SOInstallmentInfo entity)
        {
            return ObjectFactory<SOInstalmentProcessor>.Instance.UpdateSOInstallmentWithoutCreditCardInfo(entity);
        }

        public virtual SOInstallmentInfo SaveSOInstallment(SOInstallmentInfo entity)
        {
            return ObjectFactory<SOInstalmentProcessor>.Instance.SaveSOInstallment(entity);
        }
    }
}
