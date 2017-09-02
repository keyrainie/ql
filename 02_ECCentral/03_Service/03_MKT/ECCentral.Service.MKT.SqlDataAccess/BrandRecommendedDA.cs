using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.MKT;
using System.Xml;
using System.Data;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IBrandRecommendedDA))]
    public class BrandRecommendedDA : IBrandRecommendedDA
    {
        public DataTable GetCategory1List()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllCategory1List");
            return command.ExecuteDataTable();
        }

        public DataTable GetCategory2List()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllCategory2List");
            return command.ExecuteDataTable();
        }


        public DataTable GetBrandRecommendedList(BrandRecommendedQueryFilter query, out int totalCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetBrandRecommendedList");
            command.SetParameterValue("@BrandType", query.BrandType);
            command.SetParameterValue("@LevelCode", query.LevelCode);
            command.SetParameterValue("@LevelCodeParent", query.LevelCodeParent);
            command.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            command.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            command.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = command.ExecuteDataTable();
            totalCount = (int)command.GetParameterValue("@TotalCount");
            return dt;

        }

        public void UpdateBrandRecommended(BrandRecommendedInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBrandRank");
            command.SetParameterValue("@SysNo", info.Sysno);
            command.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            command.SetParameterValue("@EditUser", info.User.UserName);
            command.SetParameterValue("@Level_Name", info.Level_Name);
            command.ExecuteNonQuery();
        }


        public void CreateBrandRecommended(BrandRecommendedInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateBrandRecommended");
            command.SetParameterValue("@Level_No", info.Level_No);
            command.SetParameterValue("@Level_Code", info.Level_Code);
            command.SetParameterValue("@Level_Name", info.Level_Name);
            command.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            command.SetParameterValue("@InUser", info.User.UserName);
            command.ExecuteNonQuery();
        }

        public bool CheckExistBrandRecommended(BrandRecommendedInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckExistBrandRecommended");
            command.SetParameterValue("@Level_No", info.Level_No);
            command.SetParameterValue("@Level_Code", info.Level_Code);
            object obj= command.ExecuteScalar();
            return obj != null;
        }
    }
}
