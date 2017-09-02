//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		        PM管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductManagerQueryDA))]
    public class ProductManagerQueryDA : IProductManagerQueryDA
    {
        /// <summary>
        /// 查询PM
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductManagerInfo(ProductManagerQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryProductManagerInfo");

            if (string.IsNullOrEmpty(queryCriteria.UserName))
            {
                dc.SetParameterValue("@PMUserName", "");
            }
            else
            {
                dc.SetParameterValue("@PMUserName", "%" + queryCriteria.UserName.Trim() + "%");
            }
            dc.SetParameterValue("@PMID", string.IsNullOrEmpty(queryCriteria.UserID) ? "" : queryCriteria.UserID);

            dc.SetParameterValue("@Status", queryCriteria.Status == null ? -999 : (int)queryCriteria.Status);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);
            
            var source = dc.ExecuteDataTable();
            totalCount = Convert.ToInt32(dc.GetParameterValue("@TotalCount"));

            return source;
        }
    }
}
