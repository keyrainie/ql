using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;

namespace IPP.OrderMgmt.JobV31.Dac.FPCheck
{
    public class SOFPCheckDA
    {
 
        public static List<SOEntity4FPEntity> GetSOList4FPCheck(int topCount,string CompanyCode)
        {
            List<SOEntity4FPEntity> result = new List<SOEntity4FPEntity>();

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOList4FPCheck");
            command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());

            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);

            result = command.ExecuteEntityList<SOEntity4FPEntity>();

            result = (result == null) ? new List<SOEntity4FPEntity>() : result;

            return result;
        }

        public static List<SOEntity4CheckEntity> GetSingleSO4FPCheck(int SOSysNo)
        {
            List<SOEntity4CheckEntity> result;

            DataCommand command = DataCommandManager.GetDataCommand("GetSingleSO4FPCheck");

            command.SetParameterValue("@SOSysNo", SOSysNo);

            result = command.ExecuteEntityList<SOEntity4CheckEntity>();

            result = (result == null) ? new List<SOEntity4CheckEntity>() : result;

            return result;
        }

        public static List<int> GetChuanHuoSOSysNoListByProduct(int productSysNo, string customerIPAddress, DateTime createTime, string CompanyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetChuanHuoSOSysNoListByProduct");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CustomerIPAddress", customerIPAddress);
            command.SetParameterValue("@CreateTime", createTime);
            command.SetParameterValue("@CompanyCode", CompanyCode);


            queryResult = command.ExecuteEntityList<SingleValueEntity>();

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.Int32Value));
            }

            queryResult = null;
            GC.Collect();

            if (result == null)
                result = new List<int>();
            return result;
        }

        public static List<int> GetChuanHuoSOSysNoListByC3(int c3No, string customerIPAddress, DateTime createTime, string CompanyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetChuanHuoSOSysNoListByC3");

            command.SetParameterValue("@C3No", c3No);
            command.SetParameterValue("@CustomerIPAddress", customerIPAddress);
            command.SetParameterValue("@CreateTime", createTime);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            queryResult = command.ExecuteEntityList<SingleValueEntity>();

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.Int32Value));
            }

            queryResult = null;
            GC.Collect();

            if (result == null)
                result = new List<int>();
            return result;
        }

        public static List<int> GetDuplicatSOSysNoList(int SOSysNo, int productSysNo, int customerSysNo, DateTime createTime, string CompanyCode)
        {
            try
            {
                List<int> result = new List<int>();

                List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

                DataCommand command = DataCommandManager.GetDataCommand("GetDuplicatSOSysNoList");

                command.SetParameterValue("@ProductSysNo", productSysNo);
                command.SetParameterValue("@CustomerSysNo", customerSysNo);
                command.SetParameterValue("@CreateTime", createTime);
                command.SetParameterValue("@CompanyCode", CompanyCode);

                queryResult = command.ExecuteEntityList<SingleValueEntity>();

                if (queryResult != null
                    && queryResult.Count > 0)
                {
                    queryResult.ForEach(x => result.Add(x.Int32Value));
                }

                return result;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static List<int> GetAutoRMACustomerSysNos(string CompanyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetAutoRMACustomerSysNos");

            command.SetParameterValue("@CompanyCode", CompanyCode);

            queryResult = command.ExecuteEntityList<SingleValueEntity>();

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.Int32Value));
            }

            queryResult = null;
            GC.Collect();

            if (result == null)
                result = new List<int>();
            return result;
        }

        public static void UpdateMarkException(string DuplicateSOSysNo, int ProductSysNo, string soSysNos)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateMarkException");
            command.CommandText = command.CommandText.Replace("#SOSysNos#", soSysNos);

            command.AddInputParameter("@DuplicateSOSysNo", System.Data.DbType.String, DuplicateSOSysNo);
            command.AddInputParameter("@ProductSysNo", System.Data.DbType.Int32, ProductSysNo);


            command.ExecuteNonQuery();
        }

        public static void UpdateMarkFPStatus(int SOSysNo, int IsFPSO, string FPReason, bool isMarkRed)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateMarkFPStatus");

            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.SetParameterValue("@IsFPSO", IsFPSO);
            command.SetParameterValue("@FPReason", FPReason);
            if (isMarkRed)
            {
                command.SetParameterValue("@FPExtend", "RED");
            }
            else
            {
                command.SetParameterValue("@FPExtend", DBNull.Value);
            }

            command.ExecuteNonQuery();
        }

        public static bool IsExistSuspectInfo(SOEntity4FPEntity entity,string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsExistSuspectInfo");

            int searchType = 0;
            if (string.IsNullOrEmpty(entity.BrowseInfo) && string.IsNullOrEmpty(entity.IPAddress))
            {
                searchType = 1;
            }
            command.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("@EmailAddress", entity.EmailAddress);
            command.SetParameterValue("@BrowseInfo", entity.BrowseInfo);
            command.SetParameterValue("@IPAddress", entity.IPAddress);
            command.SetParameterValue("@MobilePhone", entity.MobilePhone);
            command.SetParameterValue("@ShippingAddress", entity.ShippingAddress);
            command.SetParameterValue("@ShippingContact", entity.ShippingContact);
            command.SetParameterValue("@Telephone", entity.Telephone);
            command.SetParameterValue("@SearchType", searchType);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return true;
            }
            return false;
        }

        public static string GetFromLinkSource(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFromLinkSource");
            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return o.ToString();
            }
            return null;
        }

        public static List<int> GetChaoHuoSOSysNoList(string receiveCellPhone
            , string receivePhone
            , int hours
            , DateTime orderDatetime
            , int? PointPromotionFlag
            , int? ShipPriceFlag
            , int? IsVATFlag
            , string CompanyCode)
        {
            List<int> result = new List<int>();
            List<SingleValueEntity> queryResult = null;


            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetChaoHuoSOSysNoList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    command.CommandText
                   , command
                   , null
                   , "SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.[CompanyCode]", System.Data.DbType.StringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, CompanyCode);

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.[Status] IN (0,1,4)");
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceivePhone", System.Data.DbType.AnsiString, "@ReceivePhone1", QueryConditionOperatorType.Equal, receivePhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceivePhone", System.Data.DbType.AnsiString, "@ReceiveCellPhone1", QueryConditionOperatorType.Equal, receiveCellPhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceiveCellPhone", System.Data.DbType.AnsiString, "@ReceivePhone2", QueryConditionOperatorType.Equal, receivePhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceiveCellPhone", System.Data.DbType.AnsiString, "@ReceiveCellPhone2", QueryConditionOperatorType.Equal, receiveCellPhone);
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                if (PointPromotionFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SM.PointPay > 0 OR (PC.PromotionValue IS NOT NULL AND PC.PromotionValue > 0))");
                }

                if (ShipPriceFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.ShipPrice = 0");
                }

                if (IsVATFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.IsVAT = 1");
                }

                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("DATEDIFF(hh, SM.OrderDate, '{0}' ) <= {1}", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), hours));
                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("DATEDIFF(hh, SM.OrderDate, '{0}' ) >= {1}", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), (-1 * hours)));


                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SM.OrderDate<= DATEADD(hh, {1}, '{0}' )", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), hours));
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SM.OrderDate>= DATEADD(hh, {1}, '{0}' )", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), (-1 * hours)));

                
                command.CommandText = sqlBuilder.BuildQuerySql();

                queryResult = command.ExecuteEntityList<SingleValueEntity>();
            }

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.Int32Value));
            }

            return result;

        }

        public static List<DubiousCustomerEntity> GetDubiousCustomersByCat(int Catalog)
        {
            List<DubiousCustomerEntity> result;

            DataCommand command = DataCommandManager.GetDataCommand("GetDubiousCustomersByCat");

            command.SetParameterValue("@Catalog", Catalog);

            result = command.ExecuteEntityList<DubiousCustomerEntity>();

            result = (result == null) ? new List<DubiousCustomerEntity>() : result;

            return result;
        }

        public static bool IsSpiteCustomer(int CustomerSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsSpiteCustomer");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            object o = command.ExecuteScalar();
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public static bool IsNewRejectionCustomerB(int CustomerSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsNewRejectionCustomerB");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        public static bool IsRejectionCustomer(string Addr, string CellPhone, string Phone, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsRejectionCustomer2");

            command.SetParameterValue("@Address", Addr);
            command.SetParameterValue("@CellPhone", CellPhone);
            command.SetParameterValue("@Phone", Phone);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            object o = command.ExecuteScalar();
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public static bool IsRejectionCustomer(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsRejectionCustomer");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public static bool IsNewOccupyStockCustomerA(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsNewOccupyStockCustomerA");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        public static bool IsNewOccupyStockCustomerB(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsNewOccupyStockCustomerB");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        public static bool IsOccupyStockCustomer(string Addr, string CellPhone, string Phone, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsOccupyStockCustomer2");

            command.SetParameterValue("@Address", Addr);
            command.SetParameterValue("@CellPhone", CellPhone);
            command.SetParameterValue("@Phone", Phone);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            object o = command.ExecuteScalar();
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public static bool IsOccupyStockCustomer(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsOccupyStockCustomer");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public static List<FPCheckMasterEntity> GetFPCheckMasterList(string CompanyCode)
        {
            List<FPCheckMasterEntity> result = new List<FPCheckMasterEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetFPCheckMasterList");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);
            result = command.ExecuteEntityList<FPCheckMasterEntity>();
            return result;
        }

        public static List<FPCheckItemEntity> GetFPCheckItemList(string CompanyCode)
        {
            List<FPCheckItemEntity> result = new List<FPCheckItemEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetFPCheckItemList");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);
            result = command.ExecuteEntityList<FPCheckItemEntity>();
            return result;
        }
    }
}
