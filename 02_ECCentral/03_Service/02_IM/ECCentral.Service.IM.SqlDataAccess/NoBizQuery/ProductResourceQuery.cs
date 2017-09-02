//************************************************************************
// 用户名				泰隆优选
// 系统名				图片管理
// 子系统名		        图片管理查询实现
// 作成者				Tom
// 改版日				2012.6.01
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.IM.Resource;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
     [VersionExport(typeof(IResourceQueryDA))]
    public class ProductResourceQuery : IResourceQueryDA
    {
        /// <summary>
        /// 获取商品资源文件信息
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryResourceList(ResourceQueryFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            if (queryCriteria.CommonSKUNumberList == null || queryCriteria.CommonSKUNumberList.Count == 0)
            {
                return null;
            }
            var commonSKUNumbers ="'"+ queryCriteria.CommonSKUNumberList.Join("','")+"'";
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryResourceList");
            dataCommand.SetParameterValue("@CommonSKUNumberList", commonSKUNumbers);
            dataCommand.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            var dt = dataCommand.ExecuteDataTable();
            return dt;
        }
    }
}
