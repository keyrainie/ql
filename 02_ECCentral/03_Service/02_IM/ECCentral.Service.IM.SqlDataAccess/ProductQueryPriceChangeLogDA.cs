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
      [VersionExport(typeof(IProductQueryPriceChangeLogDA))]
    public class ProductQueryPriceChangeLogDA : IProductQueryPriceChangeLogDA
    {
          public DataTable GetProductQueryPriceChangeLog(ProductPriceChangeLogQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCategoryQueryPriceChangeLogList");
            dc.SetParameterValue("@ProductSysno",query.ProductSysno);
            dc.SetParameterValue("@ProductID", query.ProductID);
            dc.SetParameterValue("@CreateDateFrom", query.CreateDateFrom);
            dc.SetParameterValue("@CreateDateTo", query.CreateDateTo);
            dc.SetParameterValue("@PriceLogType", query.PriceLogType);
            dc.SetParameterValue("@SortField", query.PageInfo.SortBy);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;

        }
    }
}
