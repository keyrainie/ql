using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.IM;
using System.Xml;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategoryRelatedDA))]
    public  class CategoryRelateDA:ICategoryRelatedDA
    {

        public DataTable GetCategoryRelatedByQuery(CategoryRelatedQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCategoryRelatedList");
            dc.SetParameterValue("@C1SysNo1", query.C1SysNo1);
            dc.SetParameterValue("@C2SysNo1", query.C2SysNo1);
            dc.SetParameterValue("@C3SysNo1", query.C3SysNo1);
            dc.SetParameterValue("@C1SysNo2", query.C1SysNo2);
            dc.SetParameterValue("@C2SysNo2", query.C2SysNo2);
            dc.SetParameterValue("@C3SysNo2", query.C3SysNo2);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            dc.SetParameterValue("@SortField", query.PageInfo.SortBy);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;
            
        }

        public int CreateCategoryRelated(CategoryRelatedInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateCategoryRelated");
            dc.SetParameterValue("@C3SysNo1", info.C3SysNo1);
            dc.SetParameterValue("@C3SysNo2", info.C3SysNo2);
            dc.SetParameterValue("@Priority", info.Priority);
            dc.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@CompanyCode", info.CompanyCode);
            dc.SetParameterValue("@LanguageCode", info.LanguageCode);
            dc.ExecuteNonQuery();
            int flag = Convert.ToInt32(dc.GetParameterValue("@Flag"));

            return flag;
        }

        public void DeleteCategoryRelated(string sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteCategoryRelated");
            dc.SetParameterValue("@SysNo", sysno);
            dc.ExecuteNonQuery();
        }
    }
}
