using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
  public interface ICommissionToCashDA
    {
      /// <summary>
      /// 根据query获取兑现申请信息
      /// </summary>
      /// <returns></returns>
      DataTable GetCommissionToCashByQuery(CommissionToCashQueryFilter query,out int TotalCount);

      /// <summary>
      /// 审核
      /// </summary>
      /// <param name="info"></param>
      void AuditCommisonToCash(CommissionToCashInfo info);

      /// <summary>
      /// 更新实际支付金额
      /// </summary>
      /// <param name="info"></param>
      void UpdateCommissionToCashPayAmt(CommissionToCashInfo info);

      /// <summary>
      /// 确认支付
      /// </summary>
      /// <param name="info"></param>
      void ConfirmCommisonToCash(CommissionToCashInfo info);

      /// <summary>
      /// 插入
      /// </summary>
      /// <param name="commissionToCashInfo"></param>
      void Insert(CommissionToCashInfo commissionToCashInfo);

    }
}
