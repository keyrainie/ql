using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IGroupBuyingQueryDA
    {
        DataSet Query(GroupBuyingQueryFilter filter,out int totalCount);

        DataTable Query(int productSysNo);

        DataTable QueryFeedback(GroupBuyingFeedbackQueryFilter filter,out int totalCount);

        DataTable QueryBusinessCooperation(BusinessCooperationQueryFilter filter, out int totalCount);

        DataTable QuerySettlement(GroupBuyingSettlementQueryFilter filter, out int totalCount);

        DataTable QueryGroupBuyingTicket(GroupBuyingTicketQueryFilter filter, out int totalCount);

        DataTable LoadGroupBuyingSettlementItemBySettleSysNo(int settlementSysNo);

        DataTable LoadTicketByGroupBuyingSysNo(int groupBuyingSysNo);
    }
}
