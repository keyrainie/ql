using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IPropertyQueryDA
    {
        /// <summary>
        /// 查询属性
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryPropertyList(PropertyQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 根据PropertySysNo获取属性值列表
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryPropertyValueListByPropertySysNo(PropertyValueQueryFilter queryCriteria, out int totalCount);

    }
}
