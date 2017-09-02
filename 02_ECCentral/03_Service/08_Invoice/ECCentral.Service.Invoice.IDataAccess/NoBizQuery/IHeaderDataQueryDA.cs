using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IHeaderDataQueryDA
    {
        /// <summary>
        /// 查询上传SAP数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        DataTable QuerySAPDOCHeader(HeaderDataQueryFilter filter, out int TotalCount);

        /// <summary>
        /// 查询上传SAP数据明细
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        DataTable QuerySAPDOCHeaderDetail(HeaderDataQueryFilter filter, out int TotalCount);

        /// <summary>
        /// 查询公司代码(SAP)
        /// </summary>
        /// <returns></returns>
        DataTable QueryCompany();
    }
}
