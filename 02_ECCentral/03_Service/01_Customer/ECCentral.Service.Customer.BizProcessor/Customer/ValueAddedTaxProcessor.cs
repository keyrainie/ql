using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(ValueAddedTaxProcessor))]
    public class ValueAddedTaxProcessor
    {

        private IValueAddedTaxDA valueAddedTaxDA = ObjectFactory<IValueAddedTaxDA>.Instance;

        #region 增值税信息

        /// <summary>
        /// 创建增值税信息
        /// </summary>
        /// <param name="vat"></param>
        /// <returns></returns>
        public virtual ValueAddedTaxInfo CreateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            return valueAddedTaxDA.CreateValueAddedTaxInfo(vat);
        }

        /// <summary>
        /// 更新增值税信息
        /// </summary>
        /// <param name="vat"></param>
        /// <returns></returns>
        public virtual void UpdateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            valueAddedTaxDA.UpdateValueAddedTaxInfo(vat);
        }

        /// <summary>
        /// 查询增值税列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual List<ValueAddedTaxInfo> QueryValueAddedTaxInfo(int customerSysNo)
        {
            return valueAddedTaxDA.QueryValueAddedTaxInfo(customerSysNo);
        }

        /// <summary>
        /// 查询默认增值税信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual ValueAddedTaxInfo GetDefaultValueAddedTaxInfo(int customerSysNo)
        {
            return QueryValueAddedTaxInfo(customerSysNo).Where(p => p.IsDefault.Value).FirstOrDefault();
        }

        #endregion 增值税信息
    }
}
