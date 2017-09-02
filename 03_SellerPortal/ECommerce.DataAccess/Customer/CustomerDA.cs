using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Customer;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Customer
{
    public class CustomerDA
    {
        public static void AdjustOrderedAmount(int customerSysNo, decimal orderedAmt)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateConfirmedTotalAmt");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@ConfirmedTotalAmt", orderedAmt);
            cmd.ExecuteNonQuery();
        }

        public static CustomerBasicInfo GetCustomerInfo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerInfo");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            return cmd.ExecuteEntity<CustomerBasicInfo>();
        }

        public static object AdjustPoint(AdjustPointRequest adujstInfo, int userSysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AdjustPoint");
            cmd.SetParameterValue("@CustomerSysno", adujstInfo.CustomerSysNo);
            cmd.SetParameterValue("@Point", adujstInfo.Point);
            cmd.SetParameterValue("@PointType", adujstInfo.PointType);
            cmd.SetParameterValue("@Source", adujstInfo.Source);
            cmd.SetParameterValue("@Memo", adujstInfo.Memo);
            cmd.SetParameterValue("@InUser", userSysNo);
            cmd.SetParameterValue("@OperationType", adujstInfo.OperationType);
            cmd.SetParameterValue("@SoSysNo", adujstInfo.SOSysNo);
            cmd.SetParameterValue("@ExpireDate", adujstInfo.PointExpiringDate);
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            object obj = cmd.GetParameterValue("@returnCode");
            return obj;
        }

        public static void UpdatePrepay(int customerSysNo, decimal prepayAmt)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePrepay");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@ValidPrepayAmt", prepayAmt);
            cmd.ExecuteNonQuery();
        }

        public static void CreatePrepayLog(CustomerPrepayLog log)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertPrepayLog");
            cmd.SetParameterValue<CustomerPrepayLog>(log);
            cmd.ExecuteNonQuery();
        }


        public static void UpdateExperience(int customerSysNo, decimal totalSO)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTotalSOMoney");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@TotalSOMoney", totalSO);
            cmd.ExecuteNonQuery();
        }

        public static void InsertExperienceLog(CustomerExperienceLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertExperienceLog");
            cmd.SetParameterValue<CustomerExperienceLog>(entity);
            cmd.ExecuteNonQuery();
        }

        public static void SetRank(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerRank");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.ExecuteNonQuery();
        }

        public static List<ShippingAddressInfo> QueryShippingAddress(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerReceiverInfo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntityList<ShippingAddressInfo>();
        }
    }
}
