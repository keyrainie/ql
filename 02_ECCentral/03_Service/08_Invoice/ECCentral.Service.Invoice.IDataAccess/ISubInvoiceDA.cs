using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface ISubInvoiceDA
    {
        /// <summary>
        /// 创建SubInvoice
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        SubInvoiceInfo Create(SubInvoiceInfo entity);

        /// <summary>
        /// 删除SubInvoice
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void DeleteBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据条件取得子发票列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<SubInvoiceInfo> GetListByCriteria(SubInvoiceInfo query);

        /// <summary>
        /// 加载子发票信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SubInvoiceInfo LoadBySysNo(int sysNo);
    }
}