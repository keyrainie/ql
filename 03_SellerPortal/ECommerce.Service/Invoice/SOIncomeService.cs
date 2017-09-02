using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Invoice;
using ECommerce.Entity.Invoice;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Service.Invoice
{
    public class SOIncomeService
    {
        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="soSysNo">单据系统编号</param>
        /// <param name="type">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        public static SOIncomeInfo GetValidSOIncomeInfo(int soSysNo, SOIncomeOrderType type)
        {
            return SOIncomeDA.GetValidSOIncomeInfo(soSysNo, type);
        }

        public static SOIncomeInfo CreateSOIncome(SOIncomeInfo entity)
        {
            SOIncomeQueryFilter query = new SOIncomeQueryFilter();
            query.SysNo = null;
            query.OrderSysNo = entity.OrderSysNo.HasValue ? entity.OrderSysNo.ToString() : null;
            query.OrderType = entity.OrderType;
            query.InIncomeStatusList = null;
            List<SOIncomeInfo> result = SOIncomeDA.GetListByCriteria(query);
            if (result.Exists(s => s.Status != SOIncomeStatus.Abandon))
            {
                throw new BusinessException("收款单记录已经存在，插入操作失败。");
            }

            SOIncomeDA.CreateSOIncome(entity);
            return entity;
        }
    }
}
