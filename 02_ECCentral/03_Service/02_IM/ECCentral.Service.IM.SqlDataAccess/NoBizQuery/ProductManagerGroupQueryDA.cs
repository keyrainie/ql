//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		        PM组管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.ProductManager;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductManagerGroupQueryDA))]
    public class ProductManagerGroupQueryDA : IProductManagerGroupQueryDA
    {

        /// <summary>
        /// 查询PM组
        /// </summary>
        /// <returns></returns>
        public DataTable QueryProductManagerGroupInfo(ProductManagerGroupQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryProductManagerGroupInfo");

            if (string.IsNullOrEmpty(queryCriteria.PMGroupName))
            {
                dc.SetParameterValue("@PMGroupName", "");
            }
            else
            {
                dc.SetParameterValue("@PMGroupName", "%" + queryCriteria.PMGroupName.Trim() + "%");
            }

            dc.SetParameterValue("@Status", queryCriteria.Status == null ? -999 : (int)queryCriteria.Status);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);

            var source = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");

            return source;
        }

        /// <summary>
        /// 获取组PM信息
        /// </summary>
        /// <param name="pmGroupSysNo"></param>
        /// <returns></returns>
        public List<ProductManagerInfo> QueryAllProductManagerInfoByPMGroupSysNo(int pmGroupSysNo)
        {
            List<ProductManagerInfo> entityList = new List<ProductManagerInfo>();

            DataCommand dc = DataCommandManager.GetDataCommand("QueryAllProductManagerInfoByPMGroupSysNo");
            dc.SetParameterValue("@PMGroupSysNo", pmGroupSysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            entityList = dc.ExecuteEntityList<ProductManagerInfo>();

            return entityList;
        }

        /// <summary>
        /// 查询不在其他PM组的PM集合
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> QueryAllProductManagerInfo(int productManagerGroupInfoSysNo)
        {
            throw new NotImplementedException();
        }


        public List<ProductManagerInfo> QueryAllProductManagerInfo()
        {
            List<ProductManagerInfo> entityList = new List<ProductManagerInfo>();

            DataCommand dc = DataCommandManager.GetDataCommand("QueryAllProductManagerInfo");
            dc.SetParameterValue("@CompanyCode", "8601");
            entityList = dc.ExecuteEntityList<ProductManagerInfo>();

            return entityList;
        }
    }
}
