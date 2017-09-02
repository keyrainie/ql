using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IVendorSystemInfoDA
    {
        /// <summary>
        /// 系统日志查询
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="dataCount">查询总数</param>
        /// <returns>数据集合</returns>
        DataTable LogQuery(VendorSystemQueryFilter filter, out int dataCount);

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        void WriteLog(VendorPortalLog log);
    }
}
