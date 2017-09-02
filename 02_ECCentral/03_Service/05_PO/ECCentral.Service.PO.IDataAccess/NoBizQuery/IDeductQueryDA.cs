using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IDeductQueryDA
    {
        /// <summary>
        /// 加载扣款项维护信息
        /// </summary>
        DataTable LoadDeductInfo(DeductQueryFilter queryFilter,out int TotalCount);
    }
}
