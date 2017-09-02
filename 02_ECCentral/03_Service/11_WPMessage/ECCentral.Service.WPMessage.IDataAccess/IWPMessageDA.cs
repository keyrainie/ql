using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPM = ECCentral.WPMessage.BizEntity;
using ECCentral.WPMessage.BizEntity;
using System.Data;
using ECCentral.WPMessage.QueryFilter;

namespace ECCentral.Service.WPMessage.IDataAccess
{
    public interface IWPMessageDA
    {
        #region 待办事项相关
        WPM.WPMessage AddWPMessage(WPM.WPMessage msg);

        List<WPM.WPMessage> GetWPMessage(int categorySysNo, string bizSysNo);

        void UpdateWPMessageToProcessing(int msgSysNo, int userSysNo);

        void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo, string memo);

        void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo);

        DataTable QueryWPMessageByUserSysNo(WPMessageQueryFilter filter, out int dataCount);

        bool HasWPMessageByUserSysNo(int userSysNo);
        #endregion

        #region 待办事项类型相关
        List<WPM.WPMessageCategory> GetAllCategory();
        List<WPMessageCategory> GetCategoryByUserSysNo(int userSysNo);

        void UpdateCategoryRoleByCategorySysNo(int categorySysNo, List<int> roleSysNo);
        List<int> GetRoleSysNoByCategorySysNo(int categorySysNo);

        void UpdateCategoryRoleByRoleSysNo(int roleSysNo, List<int> categorySysNoList);
        List<int> GetCategorySysNoByRoleSysNo(int roleSysNo);
        #endregion
    }
}
