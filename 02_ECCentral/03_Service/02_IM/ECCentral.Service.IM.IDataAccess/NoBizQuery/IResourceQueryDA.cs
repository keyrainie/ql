using System.Data;
using ECCentral.QueryFilter.IM.Resource;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public  interface IResourceQueryDA
    {
        /// <summary>
        /// 查询属性
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryResourceList(ResourceQueryFilter queryCriteria, out int totalCount);
    }
}
