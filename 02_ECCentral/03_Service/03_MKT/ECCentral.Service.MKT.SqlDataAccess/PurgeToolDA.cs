using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
     [VersionExport(typeof(IPurgeToolDA))]
    public class PurgeToolDA : IPurgeToolDA
    {
         /// <summary>
        /// 根据query得到PurgeTool
         /// </summary>
         /// <param name="query"></param>
         /// <returns></returns>
        public DataTable GetPurgeToolByQuery(PurgeToolQueryFilter query,out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPurgeToolByQuery");
            cmd.SetParameterValue("@Status", query.ClearType);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);
            DataTable dt = new DataTable();
            dt= cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }

        /// <summary>
        /// 创建CreatePurgeTool
        /// </summary>
        /// <param name="info"></param>
        public void CreatePurgeTool(PurgeToolInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreatePurgeTool");
            cmd.SetParameterValue("@PurgeUrl", info.Url);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@PurgeDate", info.ClearDate);
            cmd.SetParameterValue("@InUser", info.User.UserName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();
        }
    }
}
