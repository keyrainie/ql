using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Invoice;
using ECommerce.Entity.Common;
using ECommerce.Entity.Invoice;

namespace ECommerce.Service.Invoice
{
    public static class SettleService
    {
        public static QueryResult SettleQuery(SettleQueryFilter filter)
        {
            int count = 0;
            return new QueryResult(SettleDA.SettleQuery(filter, out count), filter, count);
        }

        public static int GetNotSettleedSysNo(int merchantsysno)
        {
            SettleQueryFilter filter = new SettleQueryFilter();
            filter.MerchantSysNo = merchantsysno;
            filter.PageIndex = 0;
            filter.PageSize = 1;

            int count = 0;
            DataTable dt = SettleDA.SettleQuery(filter, out count);
            if (dt.Rows.Count > 0)
            {
                return (int)dt.Rows[0]["SysNo"];
            }
            else
            {
                return 0;
            }
        }


        public static CommissionMasterInfo GetCommissionMasterInfoBySysNo(int sysno, int merchantSysNO)
        {
            return SettleDA.GetCommissionMasterInfoBySysNo(sysno, merchantSysNO);
        }

        public static List<CommissionItemInfo> GetCommissionItem(int sysNo, int merchantSysNO)
        {
            return SettleDA.GetCommissionItem(sysNo, merchantSysNO);

        }


        public static List<CommissionItemLogDetailInfo> QueryCommissionLogDetail(int merchantsysno, string type, List<int> commissionSysNoList)
        {
            return SettleDA.QueryCommissionLogDetail(merchantsysno, type, commissionSysNoList);
        }
    }

}
