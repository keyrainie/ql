using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IValueAddedTaxDA
    {
        /// <summary>
        /// 创建增值税信息
        /// </summary>
        /// <param name="vat">增值税BizEntity</param>
        /// <returns>增值税BizEntit</returns>
        ValueAddedTaxInfo CreateValueAddedTaxInfo(ValueAddedTaxInfo vat);

        /// <summary>
        /// 更新增值税信息
        /// </summary>
        /// <param name="vat">增值税BizEntity</param>
        /// <returns>增值税BizEntity</returns>
        void UpdateValueAddedTaxInfo(ValueAddedTaxInfo vat);

        /// <summary>
        /// 查询增值税信息列表
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <returns>增值税BizEntity列表</returns>
        List<ValueAddedTaxInfo> QueryValueAddedTaxInfo(int customerSysNo);
 
    }
}