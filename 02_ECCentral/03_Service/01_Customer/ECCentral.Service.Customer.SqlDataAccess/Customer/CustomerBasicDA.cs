using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICustomerBasicDA))]
    public class CustomerBasicDA : ICustomerBasicDA
    {
        #region ICustomerBasicDA Members

        [Caching(new string[] { "customerSysNo" }, ExpireTime = "00:05:00")]
        public virtual CustomerBasicInfo GetCustomerBasicInfoBySysNo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCustomerBasicInfo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo.ToString());
            return cmd.ExecuteEntity<CustomerBasicInfo>();
        }

        [Caching(new string[] { "customerID" }, ExpireTime = "00:05:00")]
        public CustomerBasicInfo GetCustomerBasicInfoByID(string customerID)
        {
            DataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCustomerBasicInfoByID");
            cmd.SetParameterValue("@CustomerID", customerID);
            return cmd.ExecuteEntity<CustomerBasicInfo>();
        }

        public virtual List<CustomerBasicInfo> GetCustomerBasicInfoBySysNoList(string sysNos)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCustomerBasicInfoList");
            cmd.CommandText = cmd.CommandText.Replace("@CustomerSysno", sysNos);
            return cmd.ExecuteEntityList<CustomerBasicInfo>();
        }

        public virtual void CreateBasicInfo(BizEntity.Customer.CustomerBasicInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCustomerBasicInfo");
            cmd.SetParameterValue<CustomerBasicInfo>(entity);
            cmd.ExecuteNonQuery();
            entity.CustomerSysNo = Convert.ToInt32(cmd.GetParameterValue("@CustomerSysNo"));
        }

        [FlushCache(typeof(CustomerBasicDA), "GetCustomerBasicInfoBySysNo", "entity.CustomerSysNo")]
        public virtual void UpdateBasicInfo(BizEntity.Customer.CustomerBasicInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerBasicInfo");
            cmd.SetParameterValue<CustomerBasicInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdatePassword(PasswordInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePassword");
            cmd.SetParameterValue<PasswordInfo>(entity);
            cmd.ExecuteNonQuery();
        }
        public virtual List<CustomerBasicInfo> GetCustomerByCustomerIdList(string ids)
        {
            List<CustomerBasicInfo> customerList = new List<CustomerBasicInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerByCustomerIdList");
            cmd.SetParameterValue("@CustomerIDList", ids);
            return cmd.ExecuteEntityList<CustomerBasicInfo>();
        }

        public virtual List<BizEntity.Customer.CustomerBasicInfo> GetCustomerByEmailList(string emails)
        {
            List<CustomerBasicInfo> customerList = new List<CustomerBasicInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerByEmailList");
            cmd.SetParameterValue("@EmailList", emails);
            return cmd.ExecuteEntityList<CustomerBasicInfo>();
        }

        public virtual List<CustomerBasicInfo> GetSystemAccount(string webChannelID)
        {
            List<CustomerBasicInfo> customerList = new List<CustomerBasicInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSystemAccount");
            //cmd.SetParameterValue("@WebChannelID", webChannelID);
            return cmd.ExecuteEntityList<CustomerBasicInfo>();
        }

        /// <summary>
        /// 检查手机号码唯一性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CustomerBasicInfo CheckSameCellPhone(CustomerBasicInfo entity, string companyCode)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CheckSameCellPhone");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd, "C.[SysNo]"))
            {
                if (entity.CustomerSysNo != null)
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "C.[SysNo]",
                        DbType.Int32,
                        "@SysNo",
                        QueryConditionOperatorType.NotEqual,
                        entity.CustomerSysNo);
                }
                if (!string.IsNullOrEmpty(entity.CellPhone))
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "C.CellPhone",
                        DbType.String,
                        "@CellPhone",
                        QueryConditionOperatorType.Equal,
                        entity.CellPhone);
                }
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    companyCode);
                cmd.CommandText = sb.BuildQuerySql();
            }

            return cmd.ExecuteEntity<CustomerBasicInfo>();
        }

        #endregion

        #region ICustomerBasicDA Members

        public bool IsExists(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CustomerIsExists");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            if (cmd.ExecuteScalar() == null)
                return false;
            else
                return (int)cmd.ExecuteScalar() > 0;
        }

        #endregion




        public List<CustomerBasicInfo> GetCustomerByIdentityCard(string identityCard)
        {
            List<CustomerBasicInfo> customerList = new List<CustomerBasicInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerByIdentityCard");
            cmd.SetParameterValue("@IdentityCard", identityCard);
            return cmd.ExecuteEntityList<CustomerBasicInfo>();
        }
    }
}
