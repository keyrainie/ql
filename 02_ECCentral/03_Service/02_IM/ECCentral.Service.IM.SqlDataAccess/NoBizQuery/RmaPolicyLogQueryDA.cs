using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
     [VersionExport(typeof(IRmaPolicyLogQueryDA))]
    public class RmaPolicyLogQueryDA : IRmaPolicyLogQueryDA
    {
       
        #region IRmaPolicyLogQueryDA Members

        public DataTable GetRmaPolicyLog(RmaPolicyLogQueryFilter filter, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRmaPolicyLog");
            cmd.SetParameterValue("@RmaPolicySysNo", filter.RmaPolicySysNO);
            cmd.SetParameterValue("@RmaPolicy", filter.RmaPolicy);
            cmd.SetParameterValue("@UpdateDateTo", filter.UpdateDateTo);
            cmd.SetParameterValue("@UpdateDateFrom", filter.UpdateDateFrom);
            cmd.SetParameterValue("@EditUserName", filter.EidtUserName);
            cmd.SetParameterValue("@pageIndex", filter.PagingInfo.PageIndex);
            cmd.SetParameterValue("@pageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@sortField", filter.PagingInfo.SortBy);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(RmaPolicyStatus) },
                    {"Type",typeof(RmaPolicyType)},
                    {"IsOnlineRequst",typeof(IsOnlineRequst)},
                    {"OperationType",typeof(RmaLogActionType)},
                 };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount = (int)cmd.GetParameterValue("@totalCount");
            return dt;
        }

        #endregion
    }
}
