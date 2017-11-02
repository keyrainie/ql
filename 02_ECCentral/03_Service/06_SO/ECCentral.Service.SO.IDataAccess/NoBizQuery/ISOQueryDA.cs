using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.SO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.SO.IDataAccess.NoBizQuery
{
    public interface ISOQueryDA
    {
        DataTable Query(SORequestQueryFilter filter,out int dataCount);

        DataTable InvoiceChangeLogQuery(SOInvoiceChangeLogQueryFilter filter, out int dataCount);

        DataTable PendingListQuery(SOPendingQueryFilter filter, out int dataCount);

        DataTable PendingListQuery(SOPendingQueryFilter filter, out int dataCount,bool isNeedChange);

        DataTable InternalMemoQuery(SOInternalMemoQueryFilter filter, out int dataCount,bool isNeedChange = true);

        DataTable ComplainQuery(ComplainQueryFilter filter, out int totalCount);

        DataTable ComplainQuery(ComplainQueryFilter filter, out int totalCount, bool isNeedChangeCodeName);

        DataTable OutStockQuery(SOOutStockQueryFilter filter, out int totalCount);

        DataTable OutStockQuery(SOOutStockQueryFilter filter, out int totalCount, bool isNeedChange);

        DataTable PackageCoverSearchQuery(SOPackageCoverSearchFilter filter, out int totalCount);
        
        DataTable PackageCoverSearchQuery(SOPackageCoverSearchFilter filter, out int totalCount, bool isNeedChange);

        DataTable LogQuery(SOLogQueryFilter filter, out int totalCount);

        DataTable ThirdPartSOSearchQuery(SOThirdPartSOSearchFilter filter, out int totalCount);

        DataTable ThirdPartSOSearchQuery(SOThirdPartSOSearchFilter filter, out int totalCount, bool isNeedChange);

        DataTable WHSOOutStockQuery(WHSOOutStockQueryFilter filter, out int totalCount);

        DataTable GetDeliveryHistoryList(SODeliveryHistoryQueryFilter filter, out int totalCount);

        DataTable GetDeliveryAssignTask(SODeliveryAssignTaskQueryFilter filter, out int totalCount);

        DataTable OPCOfflineMasterQuery(OPCQueryFilter filter, out int totalCount);

        DataTable SpecialSOQuery(SpecialSOSearchQueryFilter filter, out int totalCount);

        DataTable WHUpdateQuery(SOWHUpdateQueryFilter filter, out int totalCount);

        DataTable OutStock4FinanceQuery(SOOutStock4FinanceQueryFilter filter, out int totalCount);

        DataTable GetDeliveryScoreList(SODeliveryScoreQueryFilter filter, out int totalCount);

        #region 中蛋定制查询
        
        DataTable OZZOOriginNoteQuery(DefaultQueryFilter filter, out int totalCount);

        DataTable SOInterceptQuery(SOInterceptQueryFilter filter, out int dataCount);

        #endregion

        DataTable QueryBackOrderItem(int soSysNo);

        DataTable QueryCheckItemAccountQty(int soSysNo);

        int QueryCalUnPayOrderQty(int productSysNo, int stockSysNo);

        #region 社团
        DataTable QuerySocietyOrder(SORequestQueryFilter filter, out int dataCount);
        #endregion
    }
}
