using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvoiceMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;
using System.ComponentModel;
using System.Configuration;

namespace InvoiceMgmt.JobV31.Dac
{
   public class VIPCustomerPresentedPointsDA
   {
       #region 获取符合条件赠送积分的VIP客户 ----- 获取VIP卡用户年购10000元的VIP客户 ------    
       /// <summary>
       /// 获取符合条件赠送积分的VIP客户
       /// </summary>
       /// <returns>VIP客户List</returns>
       public static List<VIPCustomerEntity> GetVipCustomerOfNeedPresentedPointsList()
       {
           List<VIPCustomerEntity> result = new List<VIPCustomerEntity>();
           DataCommand command = DataCommandManager.GetDataCommand("GetVIPPresentedPointsList");      
           result = command.ExecuteEntityList<VIPCustomerEntity>();
           return result;
       }
       #endregion

       #region 赠送积分完成后 更新VIP客户 赠送状态 防止 重复赠送
       /// <summary>
       /// 赠送完成后更新VIP客户状态为以赠送积分状态
       /// </summary>
       /// <param name="customerSysNo">客户系统编号</param>
       public static void UpdateVIPCustomerPresentedPointsStatus(int customerSysNo)
       {          
           DataCommand command = DataCommandManager.GetDataCommand("UpdateVIPCustomerPresentedPointsStatus");     
           command.SetParameterValue("@CustomerSysNo", customerSysNo);     
           command.ExecuteNonQuery();
        }
       #endregion
   }
}
