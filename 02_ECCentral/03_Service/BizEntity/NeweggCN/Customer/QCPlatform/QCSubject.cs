using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    public class QCSubject : IIdentity, IWebChannel
    {
        public int? SysNo { get; set; }

        public Common.WebChannel WebChannel { get; set; }

        public string CompanyCode { get; set; }

        public string Name { get; set; }

        public int? OrderNum { get; set; }

        public QCSubjectStatus? Status { get; set; }

        public int? ParentSysNo { get; set; }

        public List<QCSubject> ChildrenList { get; set; }
    }
}
