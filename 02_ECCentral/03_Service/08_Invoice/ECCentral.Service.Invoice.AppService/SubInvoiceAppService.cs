using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(SubInvoiceAppService))]
    public class SubInvoiceAppService
    {
        /// <summary>
        /// 通过订单系统编号取得子发票列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>子发票列表</returns>
        public virtual List<SubInvoiceInfo> LoadSubInvoiceList(int soSysNo)
        {
            return ObjectFactory<SubInvoiceProcessor>.Instance.GetListByCriteria(new SubInvoiceInfo()
            {
                SOSysNo = soSysNo
            });
        }
    }
}