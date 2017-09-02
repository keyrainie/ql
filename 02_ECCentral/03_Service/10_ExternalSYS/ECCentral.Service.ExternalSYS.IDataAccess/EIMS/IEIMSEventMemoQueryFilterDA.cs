using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IEIMSEventMemoQueryFilterDA
    {
        /// <summary>
        /// EIMS单据查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable EIMSEventMemoQuery(EIMSEventMemoQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询发票信息
        /// </summary>
        /// <param name="fitler"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable InvoiceInfoListQuery(EIMSInvoiceEntryQueryFilter fitler, out int totalCount);

        /// <summary>
        /// 根据单据号查询发票信息
        /// </summary>
        /// <param name="invoiceNo">单据号</param>
        /// <returns></returns>
        EIMSInvoiceInfo QueryInvoiceList(string invoiceNumber);

        /// <summary>
        /// 根据发票号查询发票信息
        /// </summary>
        /// <param name="invoiceInputSysNos"></param>
        /// <returns></returns>
        List<EIMSInvoiceInfoEntity> QueryEIMSInvoiceInputByInvoiceInputSysNo(List<string> invoiceInputSysNos);

        /// <summary>
        /// 根据发票查询单据信息
        /// </summary>
        /// <param name="invoiceInputSysNos"></param>
        /// <returns></returns>
        List<EIMSInvoiceInputExtendInfo> QueryEIMSInvoiceInputExtendByInvoiceInputSysNo(params int[] invoiceInputSysNos);

        /// <summary>
        /// 添加发票信息
        /// </summary>
        /// <param name="entities"></param>
        void CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities);

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="entities"></param>
        void UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities);
    }
}
