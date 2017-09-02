using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
     public partial interface ISOInstalmentDA
     {
         /// <summary>
         /// 获取所有分期付款支付方式SysNo
         /// </summary>
         /// <returns></returns>
         List<int> GetAllInstalmentPayTypeSysNos();
         /// <summary>
         /// 获取招商银行在线分期支付方式
         /// </summary>
         /// <returns></returns>
         List<int> GetOnlinePayTypeSysNos();
         /// <summary>
         /// 检查是否是中国银行分期付款且是电话支付
         /// </summary>
         List<int> GetPayTypeSysNosOnBankOfChina();
         /// <summary>
         /// 检查是否是招商银行电话支付方式
         /// </summary>
         List<int> GetCMBCPhonePayTypeSysNos();

         SOInstallmentInfo SaveSOInstallmentWhenCreateSO(SOInstallmentInfo entity);
         SOInstallmentInfo UpdateSOInstallmentWithoutCreditCardInfo(SOInstallmentInfo entity);
         SOInstallmentInfo SaveSOInstallment(SOInstallmentInfo entity);
    }
}
