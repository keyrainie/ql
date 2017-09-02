using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface ICpsUserDA
    {
        /// <summary>
        /// 根据SysNo获取实体
        /// </summary>
        /// <param name="cpsUserSysNo"></param>
        /// <returns></returns>
        CpsUserInfo GetCpsUser(int cpsUserSysNo);

        /// <summary>
        /// 根据query得到CPSUser信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetCpsUser(CpsUserQueryFilter query, out int totalCount);

       /// <summary>
       /// 更新状态
       /// </summary>
       /// <param name="info"></param>
        void UpdateUserStatus(CpsUserInfo info);

        /// <summary>
        /// 根据UserSysNo得到所有source
        /// </summary>
        /// <param name="UserSysNo"></param>
        /// <returns></returns>
        DataTable GetUserSource(CpsUserSourceQueryFilter query, out int totalCount);

        /// <summary>
        /// 更新Source
        /// </summary>
        /// <param name="Info"></param>
        void UpdateUserSource(CpsUserInfo info);

        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="info"></param>
        void UpdateBasicUser(CpsUserInfo info);

        /// <summary>
        /// 更新收款账户信息
        /// </summary>
        /// <param name="info"></param>
        void UpdateCpsReceivablesAccount(CpsUserInfo info);

        /// <summary>
        /// 创建source
        /// </summary>
        /// <param name="info"></param>
        void CreateUserSource(CpsUserInfo info);

        /// <summary>
        /// 检查是否已存在Source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool IsExistsUserSource(CpsUserInfo info);

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        void AuditUser(CpsUserInfo info);

        /// <summary>
        /// 审核不通过时写日志
        /// </summary>
        /// <param name="info"></param>
        void InsertChangeLog(CpsUserInfo info);

        /// <summary>
        /// 获取网站类型
        /// </summary>
        /// <returns></returns>
        DataTable GetWebSiteType();

        /// <summary>
        ///获取银行枚举
        /// </summary>
        /// <returns></returns>
        DataTable GetBankType();

        /// <summary>
        ///获取审核记录
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        DataTable GetAuditHistory(int SysNo);

        /// <summary>
        /// 增加用户余额
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="balanceAmt"></param>
        void AddUserBalanceAmt(int userSysNo, decimal balanceAmt);
    }
}
