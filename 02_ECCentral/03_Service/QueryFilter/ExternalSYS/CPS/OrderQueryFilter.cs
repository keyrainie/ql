using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class OrderQueryFilter
    {
       public PagingInfo PageInfo { get; set; }
       /// <summary>
       /// 获取或设置单据类型
       /// </summary>
       public CPSOrderType? OrderType { get; set; }

       /// <summary>
       /// 获取或设置单据编号
       /// </summary>
       public int? OrderSysNo { get; set; }

       public string OrderSysNoList { get; set; }

       /// <summary>
       /// 获取或设置主渠道ID
       /// </summary>
       public string MasterChannelID { get; set; }

       /// <summary>
       /// 获取或设置子渠道ID
       /// </summary>
       public string SubChannelID { get; set; }

       /// <summary>
       /// 获取或设置下单日期(Begin)
       /// </summary>
       public DateTime? CreateDateBegin { get; set; }

       /// <summary>
       /// 获取或设置下单日期(End)
       /// </summary>
       public DateTime? CreateDateEnd { get; set; }

       /// <summary>
       /// 获取或设置交易完成日期 (Begin)
       /// </summary>
       public DateTime? FinishDateBegin { get; set; }

       /// <summary>
       /// 获取或设置交易完成日期 (End)
       /// </summary>
       public DateTime? FinishDateEnd { get; set; }

       /// <summary>
       /// 获取或设置结算日期 (Begin)
       /// </summary>
       public DateTime? SettlementDateBegin { get; set; }

       /// <summary>
       /// 获取或设置结算日期 (End)
       /// </summary>
       public DateTime? SettlementDateEnd { get; set; }

       /// <summary>
       /// 获取或设置结算状态
       /// </summary>
       public FinanceStatus? SettledStatus { get; set; }
    }
}
