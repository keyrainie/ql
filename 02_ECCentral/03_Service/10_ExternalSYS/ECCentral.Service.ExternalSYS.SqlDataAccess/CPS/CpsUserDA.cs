using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(ICpsUserDA))]
    public class CpsUserDA : ICpsUserDA
    {
        public CpsUserInfo GetCpsUser(int cpsUserSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCpsUserBySysNo");
            dc.SetParameterValue("@SysNo", cpsUserSysNo);
            return dc.ExecuteEntity<CpsUserInfo>();
        }

        public void AddUserBalanceAmt(int userSysNo, decimal balanceAmt)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AddUserBalanceAmt");
            dc.SetParameterValue("@UserSysNo", userSysNo);
            dc.SetParameterValue("@BalanceAmt", balanceAmt);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据query得到CPSUser信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetCpsUser(CpsUserQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCpsUser");
            cmd.SetParameterValue("@Status", query.AuditStatus);
            cmd.SetParameterValue("@UserType", query.UserType);
            cmd.SetParameterValue("@WebSiteType", query.WebSiteType);
            cmd.SetParameterValue("@IsAvailable", query.IsActive);
            cmd.SetParameterValue("@CustomerID", query.CustomerID);
            cmd.SetParameterValue("@ReceivablesName", query.ReceivablesName);
            cmd.SetParameterValue("@Email", query.Email);
            cmd.SetParameterValue("@Phone", query.Phone);
            cmd.SetParameterValue("@ImMessenger", query.ImMessenger);
            cmd.SetParameterValue("@RegisterDateFrom", query.RegisterDateFrom);
            cmd.SetParameterValue("@RegisterDateTo", query.RegisterDateTo);
            cmd.SetParameterValue("@PageIndex", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@SortFiled", query.PageInfo.SortBy);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(AuditStatus) },
                    { "UserType", typeof(UserType) },
                     { "IsAvailable", typeof(IsActive) },
                     { "BankLock", typeof(IsLock) },
                      { "BankAccountType", typeof(UserType)}
                 };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;

        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateUserStatus(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateUserStatus");
            cmd.SetParameterValue("@IsAvailable", info.UserBasicInfo.IsActive);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.ExecuteNonQuery();


        }
        /// <summary>
        /// 根据UserSysNo得到所有source
        /// </summary>
        /// <param name="UserSysNo"></param>
        /// <returns></returns>
        public DataTable GetUserSource(CpsUserSourceQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetUserSource");
            cmd.SetParameterValue("@UserSysNo", query.UserSysNo);
            cmd.SetParameterValue("@PageIndex", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }
        /// <summary>
        /// 更新Source
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateUserSource(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateUserSource");
            cmd.SetParameterValue("@Name", info.Source.ChanlName);
            cmd.SetParameterValue("@SysNo", info.Source.SysNo);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBasicUser(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBasicUser");
            cmd.SetParameterValue("@UserType", info.UserBasicInfo.UserType);
            cmd.SetParameterValue("@SiteTypeCode", info.UserBasicInfo.WebSiteCode);
            cmd.SetParameterValue("@WebSiteUrl", info.UserBasicInfo.WebSiteAddress);
            cmd.SetParameterValue("@WebSiteName", info.UserBasicInfo.WebSiteName);
            cmd.SetParameterValue("@Email", info.UserBasicInfo.Email);
            cmd.SetParameterValue("@ContactName", info.UserBasicInfo.Contact);
            cmd.SetParameterValue("@ContactPhone", info.UserBasicInfo.ContactPhone);
            cmd.SetParameterValue("@Address", info.UserBasicInfo.ContactAddress);
            cmd.SetParameterValue("@ZipCode", info.UserBasicInfo.Zipcode);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 更新收款账户信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCpsReceivablesAccount(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCpsReceivablesAccount");
            cmd.SetParameterValue("@BankName", info.ReceivablesAccount.BrankName);
            cmd.SetParameterValue("@BranchBank", info.ReceivablesAccount.BranchBank);
            cmd.SetParameterValue("@BankCardNumber", info.ReceivablesAccount.BrankCardNumber);
            cmd.SetParameterValue("@ReceivableName", info.ReceivablesAccount.ReceiveablesName);
            cmd.SetParameterValue("@BankLock", info.ReceivablesAccount.IsLock);
            cmd.SetParameterValue("@BankAccountType", info.ReceivablesAccount.ReceivablesAccountType);
            cmd.SetParameterValue("@BankCode", info.ReceivablesAccount.BrankCode);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 创建source
        /// </summary>
        /// <param name="info"></param>
        public void CreateUserSource(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateUserSource");
            cmd.SetParameterValue("@Name", info.Source.ChanlName);
            cmd.SetParameterValue("@Source", info.Source.Source);
            cmd.SetParameterValue("@UserSysNo", info.SysNo);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// 检查是否已存在Source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsExistsUserSource(CpsUserInfo info)
        {
            DataCommand cmd;
            if (info.Source.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsUserSourceByUpdate");
                cmd.SetParameterValue("@SysNo", info.Source.SysNo);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsUserSource");
            }
            cmd.SetParameterValue("@Name", info.Source.ChanlName);
            cmd.SetParameterValue("@UserSysNo", info.SysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag") > 0;
        }

        /// <summary>
        /// 获取网站类型
        /// </summary>
        /// <returns></returns>
        public DataTable GetWebSiteType()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetWebSiteType");
            return cmd.ExecuteDataTable();
        }


        public DataTable GetBankType()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBankType");
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        ///获取审核记录
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public DataTable GetAuditHistory(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAuditHistory");
            cmd.SetParameterValue("@SysNo", SysNo);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditUser(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AuditUser");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 审核拒绝时创建LOG
        /// </summary>
        /// <param name="info"></param>
        public void InsertChangeLog(CpsUserInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertChangeLog");
            cmd.SetParameterValue("@UniqueKey", info.SysNo.ToString());
            cmd.SetParameterValue("@Note", info.AuditNoClearanceInfo);
            cmd.SetParameterValue("@UserName", info.User.UserName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();


        }
    }
}
