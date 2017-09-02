using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.BizProcessor;

namespace ECCentral.Service.IM.AppService
{
      [VersionExport(typeof(BrandRequestAppService))]
   public class BrandRequestAppService
    { 
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditBrandRequest(BrandRequestInfo info)
        {
            ObjectFactory<BrandRequestProcessor>.Instance.AuditBrandRequest(info);
        }
        /// <summary>
       /// 提交审核 
       /// </summary>
       /// <param name="info"></param>
        public void InsertBrandRequest(BrandRequestInfo info)
        {
            ObjectFactory<BrandRequestProcessor>.Instance.InsertBrandRequest(info);
     
        }
    }
}
