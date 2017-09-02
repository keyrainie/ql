using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.ReconReport;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IReconReportDA
    {
        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransactionNumbers"></param>
        /// <returns></returns>
        int UpdateSAPStatus(List<int> TransactionNumbers);

        /// <summary>
        /// 创建SAP报表Excel
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<SAPInterfaceExchangeInfo> CreateExcel(DateTime? from, DateTime? to);

        /// <summary>
        /// 生成AR or AP科目报表数据
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<SAPInterfaceExchangeInfo> CreateOtherExcel(DateTime? from, DateTime? to);
    }
}
