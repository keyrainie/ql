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
    [VersionExport(typeof(IRmaPolicyQueryDA))]
    public class RmaPolicyQueryDA : IRmaPolicyQueryDA
    {
        #region IRmaPolicyQueryDA Members

        public DataTable QueryRmaPolicy(RmaPolicyQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryRmaPolicy");
            cmd.SetParameterValue("@RmaStatus", query.Status);
            cmd.SetParameterValue("@RmaType", query.Type);
            cmd.SetParameterValue("@CreateUserName", query.CreateUserName);
            cmd.SetParameterValue("@SysNo", query.SysNo);
            cmd.SetParameterValue("@pageIndex", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@pageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@sortField", query.PagingInfo.SortBy);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(RmaPolicyStatus) },
                    {"Type",typeof(RmaPolicyType)},
                    {"IsOnlineRequst",typeof(IsOnlineRequst)},

                 };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount = (int)cmd.GetParameterValue("@totalCount");
            return dt;
        }

        #endregion

        #region IRmaPolicyQueryDA Members


        public RmaPolicyInfo QueryRmaPolicyBySysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryRmaPolicyBySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            RmaPolicyInfo info = cmd.ExecuteEntity<RmaPolicyInfo>();
            return info;
        }

        public RmaPolicyInfo GetStandardRmaPolicy()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetStandardRmaPolicy");
            RmaPolicyInfo info = cmd.ExecuteEntity<RmaPolicyInfo>();
            return info;
        }

        #endregion

        #region IRmaPolicyQueryDA Members


        public List<RmaPolicyInfo> GetAllRmaPolicy()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllRmaPolicy");
            return cmd.ExecuteEntityList<RmaPolicyInfo>();
        }

        #endregion
    }
}
