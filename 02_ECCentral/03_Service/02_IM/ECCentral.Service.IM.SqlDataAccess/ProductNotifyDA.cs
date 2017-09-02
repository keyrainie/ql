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
    [VersionExport(typeof(IProductNotifyDA))]
    public class ProductNotifyDA : IProductNotifyDA
    {
        public DataTable GetProductNotifyByQuery(ProductNotifyQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductNotifyList");
            dc.SetParameterValue("@C1SysNo1", query.Category1SysNo);
            dc.SetParameterValue("@C2SysNo1", query.Category2SysNo);
            dc.SetParameterValue("@C3SysNo1", query.Category3SysNo);
            dc.SetParameterValue("@Status", query.Status);
            dc.SetParameterValue("@ProductSysNo", query.ProductSysNo);
            dc.SetParameterValue("@StartTime", query.StartTime);
            dc.SetParameterValue("@EndTime", query.EndTime);
            dc.SetParameterValue("@Email", query.Email);
            dc.SetParameterValue("@CustomerID", query.CustomserID);
            dc.SetParameterValue("@PMSysNo", query.PMSysNo);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            
            dc.SetParameterValue("@SortField", query.PageInfo.SortBy);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable(3, typeof(NotifyStatus));
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;
        }
    }
}