using System.Data;

using ECCentral.QueryFilter.IM;


namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IAccessoryQueryDA
    {
        /// <summary>
        /// 查询附件
        /// </summary>
        /// <returns></returns>
        DataTable QueryAccessory(AccessoryQueryFilter queryCriteria, out int totalCount);
    }
}
