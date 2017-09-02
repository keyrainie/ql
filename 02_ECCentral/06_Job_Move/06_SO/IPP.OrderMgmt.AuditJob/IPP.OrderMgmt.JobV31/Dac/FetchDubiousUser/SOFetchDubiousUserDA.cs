using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.FetchDubiousUser;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;

namespace IPP.OrderMgmt.JobV31.Dac.FetchDubiousUser
{
    public class SOFetchDubiousUserDA
    {

        public static string UserDisplayName;
        public static string UserLoginName;
        public static string CompanyCode;
        public static string StoreCompanyCode;
        public static string StoreSourceDirectoryKey;

        public static List<UsersOfOrderEntity> GetDistinctUsersFromSOList()
        {
            List<UsersOfOrderEntity> result = new List<UsersOfOrderEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetDistinctCustomersFromSO");
   //         command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<UsersOfOrderEntity>();

            result = (result == null) ? new List<UsersOfOrderEntity>() : result;

            return result;
        }

        public static float GetUserRP(int CustomerSysNo)
        {
            float result;  

            DataCommand command = DataCommandManager.GetDataCommand("GetUserRP");
            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                result = Convert.ToSingle(o);
                return result;
            }
            return 0;
        }

        public static void AddRejectionUsers(int CustomerSysNo, int Catalog, string ColumnName)
        {
          //  DataCommand command = DataCommandManager.GetDataCommand("AddRejectionUsers");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AddRejectionUsers");
            command.CommandText = command.CommandText.Replace("#COLUMN_NAME#", ColumnName);

            command.AddInputParameter("@CustomerSysNo", System.Data.DbType.Int32, CustomerSysNo);
            command.AddInputParameter("@Catalog", System.Data.DbType.Int32, Catalog);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

        //    command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
         //   command.SetParameterValue("@Catalog", Catalog);
         //   command.SetParameterValue("@Column_Name", ColumnName);

            command.ExecuteNonQuery();

        }

        public static void AddSinglePhoneNumber(string PhoneNumber, int DuType)
        {
            //  DataCommand command = DataCommandManager.GetDataCommand("AddRejectionUsers");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AddSinglePhoneNumber");

            command.AddInputParameter("@PhoneNumber", System.Data.DbType.String, PhoneNumber);
            command.AddInputParameter("@DuType", System.Data.DbType.Int32, DuType);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

            //    command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
            //   command.SetParameterValue("@Catalog", Catalog);
            //   command.SetParameterValue("@Column_Name", ColumnName);

            command.ExecuteNonQuery();

        }

        public static List<SingleValueEntity> GetRejectionUserPhone(int CustomerSysNo)
        {
            List<SingleValueEntity> result = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetRejectionUserPhone");
            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = command.ExecuteEntityList<SingleValueEntity>();

            result = (result == null) ? new List<SingleValueEntity>() : result;

            return result;
        }

        public static List<SingleValueEntity> GetRejectionUserPhone2(string ReceiveAddress)
        {
            List<SingleValueEntity> result = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetRejectionUserPhone2");
            command.SetParameterValue("@ReceiveAddress", ReceiveAddress);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<SingleValueEntity>();

            result = (result == null) ? new List<SingleValueEntity>() : result;

            return result;
        }

        public static List<AddressOfOrderEntity> GetDistinctAddressFromSOList()
        {
            List<AddressOfOrderEntity> result = new List<AddressOfOrderEntity>();

   //         CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetDistinctCustomersFromSO");
            DataCommand command = DataCommandManager.GetDataCommand("GetDistinctAddressFromSO");
   //         command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<AddressOfOrderEntity>();

            result = (result == null) ? new List<AddressOfOrderEntity>() : result;

            return result;
        }

        public static void AddRejectionUsers2(string ReceiveAddress)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AddRejectionUsers2");

            command.AddInputParameter("@ReceiveAddress", System.Data.DbType.String, ReceiveAddress);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

            command.ExecuteNonQuery();

        }

        public static void RemoveRejectionUser(int CustomerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("RemoveRejectionUser");

            command.AddInputParameter("@RemoveID", System.Data.DbType.Int32, CustomerSysNo);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

            command.ExecuteNonQuery();

        }

        public static List<OccupyStockUserEntity> GetOccupyStockUserList(int customerSysNo)
        {
            List<OccupyStockUserEntity> result = new List<OccupyStockUserEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetOccupyStockUsers");

            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            result = command.ExecuteEntityList<OccupyStockUserEntity>();

            result = (result == null) ? new List<OccupyStockUserEntity>() : result;

            return result;
        }

        public static void RemoveOccupyStockUser(string CustomerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("RemoveOccupyStockUser");

//            command.AddInputParameter("@RemoveID", System.Data.DbType.String, CustomerSysNo);
            command.ReplaceParameterValue("@RemoveID", CustomerSysNo);
            command.ReplaceParameterValue("@CompanyCode", CompanyCode);

            command.ExecuteNonQuery();

        }

        public static void AddOccupyStockUsers(int Catalog, string Content)
        {
            //  DataCommand command = DataCommandManager.GetDataCommand("AddRejectionUsers");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AddOccupyStockUsers");

            command.AddInputParameter("@Content", System.Data.DbType.String, Content);
            command.AddInputParameter("@Catalog", System.Data.DbType.Int32, Catalog);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

            //    command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
            //   command.SetParameterValue("@Catalog", Catalog);
            //   command.SetParameterValue("@Column_Name", ColumnName);

            command.ExecuteNonQuery();

        }

        public static void AddSpiteUsers()
        {
            //  DataCommand command = DataCommandManager.GetDataCommand("AddRejectionUsers");

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AddSpiteUsers");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);
            command.ExecuteNonQuery();

        }

        public static List<ExpiredSpiteCustomerEntity> GetExpiredSpiteCustomers()
        {
            List<ExpiredSpiteCustomerEntity> result = new List<ExpiredSpiteCustomerEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetExpiredSpiteCustomers");
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = command.ExecuteEntityList<ExpiredSpiteCustomerEntity>();

            result = (result == null) ? new List<ExpiredSpiteCustomerEntity>() : result;

            return result;
        }

        public static void RemoveSpiteCustomers(string IDStr)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("RemoveSpiteCustomers");
            command.CommandText = command.CommandText.Replace("#RemoveIDStr#", IDStr);
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);
            command.ExecuteNonQuery();

        }

    }
}
