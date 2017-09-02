using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IFinanceDA
    {
        #region 应付款汇总表查询
        DataTable FinanceQuery(FinanceQueryFilter filter, out int totalCount, out double totalPayAmt);

        DataTable FinanceExport(FinanceQueryFilter filter, out int totalCount, out double totalPayAmt);

        //获取PM组信息
        DataTable GetPMGroupList();
        #endregion

        //添加备注
        void AddMemo(int? sysNo,string memo,string companyCode);

        
        List<PayableInfo> PayableQuery(PayableCriteriaInfo info);

        List<PayItemInfo> QueryPayItems(PayableItemCriteriaInfo query);

        DataTable GetMemoBySysNo(int? sysNo);
    }
}
