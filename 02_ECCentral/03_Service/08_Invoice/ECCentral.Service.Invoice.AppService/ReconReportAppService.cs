using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(ReconReportAppService))]
    public class ReconReportAppService
    {
        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransactionNumbers"></param>
        /// <returns></returns>
        public virtual int UpdateSAPStatus(List<int> TransactionNumbers)
        {
            return ObjectFactory<ReconReportProcessor>.Instance.UpdateSAPStatus(TransactionNumbers);
        }

        /// <summary>
        /// 自动生成报表
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public virtual void CreateReconReportByJob(DateTime? from, DateTime? to)
        {
            ObjectFactory<ReconReportProcessor>.Instance.CreateReconReportByJob(from, to);
        }

        /// <summary>
        /// 手动生成报表
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public virtual void CreateReconReportByWeb(DateTime? from, DateTime? to)
        {
            ObjectFactory<ReconReportProcessor>.Instance.CreateReconReportByWeb(from, to);
        }
    }
}
