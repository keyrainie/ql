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
     [VersionExport(typeof(IProductShowDA))]
    public class ProductShowDA : IProductShowDA
    {
         public DataTable GetProductShowByQuery(ProductShowQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductShowList");
            cmd.SetParameterValue("@FirstOnlineTimeFrom", query.FirstOnlineTimeFrom);
            cmd.SetParameterValue("@FirstOnlineTimeTo", query.FirstOnlineTimeTo);
            cmd.SetParameterValue("@EditDateFrom", query.EditDateFrom);
            cmd.SetParameterValue("@EditDateTo", query.EditDateTo);
            cmd.SetParameterValue("@Status", query.Status);
            cmd.SetParameterValue("@C1SysNo",query.Category1SysNo);
            cmd.SetParameterValue("@C2SysNo", query.Category2SysNo);
            cmd.SetParameterValue("@C3SysNo", query.Category3SysNo);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(3,typeof(ProductStatus));
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }
    }
}
