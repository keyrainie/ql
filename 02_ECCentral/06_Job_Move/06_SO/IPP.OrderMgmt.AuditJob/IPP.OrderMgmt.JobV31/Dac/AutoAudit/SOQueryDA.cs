using System;
using System.Collections.Generic;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.OrderMgmt.JobV31.Dac.AutoAudit
{
    public class SOQueryDA
    {

        public static List<SOQueryEntity> GetSOList4Audit(int topCount,string companyCode)
        {
            List<SOQueryEntity> result = new List<SOQueryEntity>();

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOList4Audit");
            command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, companyCode);

            result = command.ExecuteEntityList<SOQueryEntity>();

            return result;
        }

        public static List<int> GetOOSSOSysNos()
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetOOSSOSysNos");

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

        public static List<int> GetSOSysNosWhichHasC3Product(string CompanyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetSOSysNosWhichHasC3Product");
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

        public static List<int> GetSOSysNosWhichHasProductID(string compancode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();
            DataCommand command = DataCommandManager.GetDataCommand("GetSOSysNosWhichHasProductID");
            command.SetParameterValue("@CompanyCode", compancode);
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


        public static List<int> GetSOSysNosHasWhichSOIncomeInfo(string companyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetSOSysNosHasWhichSOIncomeInfo");
            command.SetParameterValue("@CompanyCode", companyCode);
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
        public static List<SONetPayEntity> GetSONetPayInfoList(string CompanyCode)
        {
            List<SONetPayEntity> result = new List<SONetPayEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetSONetPayInfoList");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<SONetPayEntity>();

            return result;
        }

        public static int GetSOCount4OneDay(int customerSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOCount4OneDay");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            int result = command.ExecuteScalar<int>();

            return Convert.ToInt32(result);
        }

        public static string GetCertificaterNameBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCertificaterNameBySOSysNo");

            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<string>();
        }

        public static string GetVatCompanyNameBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVatCompanyNameBySOSysNo");

            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<string>();
        }

        public static List<CSTBOrderCheckMasterEntity> GetCSTBOrderCheckMasterList(string companyCode)
        {
            List<CSTBOrderCheckMasterEntity> result = new List<CSTBOrderCheckMasterEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCSTBOrderCheckMasterList");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, companyCode);
            result = command.ExecuteEntityList<CSTBOrderCheckMasterEntity>();
            return result;
        }

        public static List<CSTBOrderCheckItemEntity> GetCSTBOrderCheckItemList(string companyCode)
        {
            List<CSTBOrderCheckItemEntity> result = new List<CSTBOrderCheckItemEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCSTBOrderCheckItemList");
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, companyCode);
            result = command.ExecuteEntityList<CSTBOrderCheckItemEntity>();
            return result;
        }

        public static int GetFraudType(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFraudType");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            object result = command.ExecuteScalar();
            return result == null ? 0 : (int)result;
        }

        public static List<DateTime> GetNoCSWorkDay(string companycode)
        {
            List<DateTime> result = new List<DateTime>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetNoCSWorkDay");

            command.SetParameterValue("@CompanyCode", companycode);
            queryResult = command.ExecuteEntityList<SingleValueEntity>();

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.DateTimeValue));
            }

            queryResult = null;
            GC.Collect();

            if (result == null)
                result = new List<DateTime>();
            return result;        
        }

        public static bool IsGroupBuySettled(int? gbSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsGroupBuySettled");
            command.SetParameterValue("@SysNo", gbSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            object o = command.ExecuteScalar();

            if (o == null)
                return false;
            else
                return true;
        }


        public static List<HolidayEntity> GetHolidays(string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetHolidays");
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteEntityList<HolidayEntity>();
        }


    }
}
