using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOInstalmentDA))]
    public partial class SOInstalmentDA:ISOInstalmentDA
    {
        public List<int> GetAllInstalmentPayTypeSysNos()
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetAllInstalmentPayTypeSysNos");
            command.SetParameterValue("@CompanyCode", "8601");
            List<SimpleObject> queryResult = command.ExecuteEntityList<SimpleObject>();

            queryResult.ForEach(x =>
            {
                result.Add(x.SysNo.Value);
            });
            return result;
        }

        public List<int> GetOnlinePayTypeSysNos()
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetOnlinePayTypeSysNos");
            List<SimpleObject> queryResult = command.ExecuteEntityList<SimpleObject>();

            queryResult.ForEach(x =>
            {
                result.Add(x.SysNo.Value);
            });
            return result;
        }

        public SOInstallmentInfo SaveSOInstallmentWhenCreateSO(SOInstallmentInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SaveSOInstallmentWhenCreateSO");
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
            dc.SetParameterValue("@BankSysNo", entity.BankSysNo);
            dc.SetParameterValue("@PhaseCount", entity.PhaseCount);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@ContractNumber", entity.ContractNumber);
            dc.SetParameterValue("@RealName", entity.RealName);
            dc.SetParameterValue("@CreditCardNumber", entity.CreditCardNumber);
            dc.SetParameterValue("@CreditCardNumberEnc", entity.CreditCardNumberEnc);
            dc.SetParameterValue("@IDNumber", entity.IDNumber);
            dc.SetParameterValue("@ExpireDate", entity.ExpireDate);
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);

            dc.ExecuteNonQuery();
            return entity;
        }

        public SOInstallmentInfo UpdateSOInstallmentWithoutCreditCardInfo(SOInstallmentInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateSOInstallmentWithoutCreditCardInfo");
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
            dc.SetParameterValue("@BankSysNo", entity.BankSysNo);
            dc.SetParameterValue("@PhaseCount", entity.PhaseCount);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@ContractNumber", entity.ContractNumber);
            dc.SetParameterValue("@RealName", entity.RealName);
            dc.SetParameterValue("@CreditCardNumber", entity.CreditCardNumber);
            dc.SetParameterValue("@CreditCardNumberEnc", entity.CreditCardNumberEnc);
            dc.SetParameterValue("@IDNumber", entity.IDNumber);
            dc.SetParameterValue("@ExpireDate", entity.ExpireDate);
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);

            dc.ExecuteNonQuery();
            return entity;
        }


        public SOInstallmentInfo SaveSOInstallment(SOInstallmentInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SaveSOInstallmentInfo");

            dc.SetParameterValue("@SOSysNo", entity.SOSysNo);
            dc.SetParameterValue("@BankSysNo", entity.BankSysNo);
            dc.SetParameterValue("@PhaseCount", entity.PhaseCount);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@ContractNumber", entity.ContractNumber);
            dc.SetParameterValue("@RealName", entity.RealName);
            dc.SetParameterValue("@CreditCardNumber", entity.CreditCardNumber);
            dc.SetParameterValue("@CreditCardNumberEnc", entity.CreditCardNumberEnc);
            dc.SetParameterValue("@IDNumber", entity.IDNumber);
            dc.SetParameterValue("@ExpireDate", entity.ExpireDate);
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();
            return entity;
        }

        public List<int> GetPayTypeSysNosOnBankOfChina()
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetPayTypeSysNosOnBankOfChina");
            command.SetParameterValue("@CompanyCode", "8601");
            List<SimpleObject> queryResult = command.ExecuteEntityList<SimpleObject>();

            queryResult.ForEach(x =>
            {
                result.Add(x.SysNo.Value);
            });
            return result;
        }

        public List<int> GetCMBCPhonePayTypeSysNos()
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetCMBCPhonePayTypeSysNos");
            command.SetParameterValue("@CompanyCode", "8601");
            List<SimpleObject> queryResult = command.ExecuteEntityList<SimpleObject>();

            queryResult.ForEach(x =>
            {
                result.Add(x.SysNo.Value);
            });
            return result;
        }
    }
}
