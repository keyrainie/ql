//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;


namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IManufacturerQueryDA))]
    public class ManufacturerQueryDA : IManufacturerQueryDA
    {

        /// <summary>
        /// 查询生产商
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryManufacturer(ManufacturerQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryManufacturer");

            if (string.IsNullOrEmpty(queryCriteria.ManufacturerNameLocal))
            {
                dc.SetParameterValue("@ManufacturerName", "");
            }
            else
            {
                dc.SetParameterValue("@ManufacturerName", "%" + queryCriteria.ManufacturerNameLocal.Trim() + "%");
            }

            dc.SetParameterValue("@Status", queryCriteria.Status == null ? -999 : (int)queryCriteria.Status);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);

            var source = dc.ExecuteDataTable(5, typeof(ManufacturerStatus)); ;
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return source;
        }

        /// <summary>
        /// 获取品牌旗舰店首页类别
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetManufacturerCategoryBySysNo(ManufacturerQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetBrandShipCategoriesByManufacturerSysNo");
            dc.SetParameterValue("@ManufacturerSysNo", queryCriteria.ManufacturerID);
            dc.SetParameterValue("@SortBy", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);
            var source = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return source;
        }
    }
}
