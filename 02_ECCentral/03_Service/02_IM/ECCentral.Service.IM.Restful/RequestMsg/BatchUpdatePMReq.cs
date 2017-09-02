using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class BatchUpdatePMReq
    {
        public BatchUpdatePMReq()
        {
            this.DepartmentCategoryList = new List<ProductDepartmentCategory>();
        }

        public int? ProductDomainSysNo { get; set; }

        public int? PMSysNo { get; set; }

        public List<ProductDepartmentCategory> DepartmentCategoryList { get; set; }       
    }

    public class DeleteProductDomainReq
    {
        public int? ProductDomainSysNo { get; set; }

        public int? DepartmentCategorySysNo { get; set; }
    }
}
