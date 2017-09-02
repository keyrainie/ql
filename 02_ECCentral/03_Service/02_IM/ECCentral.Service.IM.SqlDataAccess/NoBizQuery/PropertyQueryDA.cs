using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPropertyQueryDA))]
    public class PropertyQueryDA : IPropertyQueryDA
    {
        /// <summary>
        /// 查询属性
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryPropertyList(PropertyQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryPropertyList");

            if (string.IsNullOrEmpty(queryCriteria.PropertyName))
            {
                dc.SetParameterValue("@PropertyDescription", "");
            }
            else
            {
                dc.SetParameterValue("@PropertyDescription", "%" + queryCriteria.PropertyName.Trim() + "%");
            }

            dc.SetParameterValue("@IsActive", queryCriteria.Status == null ? -999 : (int)queryCriteria.Status);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);

            var source = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");

            return source;
        }

        /// <summary>
        /// 根据PropertySysNo获取属性值列表
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public DataTable QueryPropertyValueListByPropertySysNo(PropertyValueQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryPropertyValueListByPropertySysNo");

            dc.SetParameterValue("@PropertySysNo", queryCriteria.PropertySysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);

            var source = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");

            return source;
        }

    }
}
