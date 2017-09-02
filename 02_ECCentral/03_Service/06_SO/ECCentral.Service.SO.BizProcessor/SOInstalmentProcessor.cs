using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOInstalmentProcessor))]
    public class SOInstalmentProcessor
    {
        ISOInstalmentDA m_da = ObjectFactory<ISOInstalmentDA>.Instance;

        public virtual List<int> GetAllInstalmentPayTypeSysNos()
        {
            return m_da.GetAllInstalmentPayTypeSysNos();
        }

        public virtual List<int> GetOnlinePayTypeSysNos()
        {
            return m_da.GetOnlinePayTypeSysNos();
        }

        public virtual SOInstallmentInfo SaveSOInstallmentWhenCreateSO(SOInstallmentInfo entity)
        {
            return m_da.SaveSOInstallmentWhenCreateSO(entity);
        }

        public virtual SOInstallmentInfo UpdateSOInstallmentWithoutCreditCardInfo(SOInstallmentInfo entity)
        {
            return m_da.UpdateSOInstallmentWithoutCreditCardInfo(entity);
        }

        public virtual SOInstallmentInfo SaveSOInstallment(SOInstallmentInfo entity)
        {
            SOInfo soInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(entity.SOSysNo.Value);
            SOHolder hold = new SOHolder();
            hold.CurrentSO = soInfo;
            hold.CheckSOIsWebHold();

            if (!string.IsNullOrEmpty(entity.ContractNumber) && entity.ContractNumber.Contains("*"))
            {
                SOInstallmentInfo queryResult = ObjectFactory<ISODA>.Instance.GetInstalmentBySOSysNo(entity.SOSysNo.Value, "8601");
                if (queryResult != null)
                    entity.ContractNumber = queryResult.ContractNumber;
            }
            if (!string.IsNullOrEmpty(entity.CreditCardNumber))
            {
                //对保存到数据库的信用卡号加密
                entity.CreditCardNumberEnc = CryptoManager.GetCrypto(CryptoAlgorithm.Cus_TripleDES).Encrypt(entity.CreditCardNumber);
                entity.CreditCardNumber = entity.CreditCardNumber.Substring(0, 1) + "*** **** **** " + entity.CreditCardNumber.Substring(12, entity.CreditCardNumber.Length - 12);
            }

            return m_da.SaveSOInstallment(entity);
        }
    }
}
