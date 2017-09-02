using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOKFCProcessor))]
    public class SOKFCProcessor
    {
        ISOKFCDA m_da = ObjectFactory<ISOKFCDA>.Instance;

        public KnownFraudCustomer GetKnownFraudCustomerInfoByCustomerSysNo(int customerSysNo) 
        {
            KnownFraudCustomer kfcInfo = m_da.GetKFCByCustomerSysNo(customerSysNo);
            if (kfcInfo== null)
            {
                kfcInfo = new KnownFraudCustomer();
            }
            return kfcInfo;
        }

        public void InsertKnownFraudCustomer(KnownFraudCustomer entity)
        {
            if (m_da.GetKFCByCustomerSysNo(entity.CustomerSysNo.Value) != null)
            {
                return;
            }
            int maxBlockMark = 0;
            if (!int.TryParse(ExternalDomainBroker.GetSystemConfigurationValue("GRADENUM", entity.CompanyCode),out maxBlockMark))
            {
                maxBlockMark = 5;
            }

            List<KnownFraudCustomer> list = m_da.GetKFCByIPAndTel(entity.IPAddress, entity.MobilePhone, entity.Telephone, entity.CompanyCode);

            //存在同一个IP地址且同一个手机号码相同记录重复5条
            //则其相关的客户信息升级到Block
            if (list != null && list.Count + 1 >= maxBlockMark)
            {
                list.ForEach(item =>
                {
                    item.KFCType = KFCType.QiZha;
                    item.LastEditDate = item.CreateDate;
                    item.LastEditUserName = item.CreateUserName;

                    m_da.UpdateKnowFrandCustomerStatus(item);
                });
                entity.KFCType = KFCType.QiZha;

                m_da.InsertKnowFrandCustomer(entity);
            }
            else
            {
                m_da.InsertKnowFrandCustomer(entity);
            }
        }

        public void UpdateKnownFraudCustomerStatus(KnownFraudCustomer entity)
        {
              m_da.UpdateKnowFrandCustomerStatus(entity);
        }
    }
}
