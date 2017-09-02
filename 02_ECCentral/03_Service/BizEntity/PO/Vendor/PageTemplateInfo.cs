using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    public class PageTemplateInfo
    {
        public string Key { get; set; }
        public int? PageTemplateType { get; set; }
        public string DataValue { get; set; }
        public string PageTypeKey { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string TemplateViewPath { get; set; }
        public string MockupUrl { get; set; }
        public int? Status { get; set; }
        public string Memo { get; set; }
        public int? Priority { get; set; }
    }
}
