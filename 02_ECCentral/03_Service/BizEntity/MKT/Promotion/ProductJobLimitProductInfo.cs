using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class ProductJobLimitProductInfo : ICompany, ILanguage
    {
        #region Job
        public int ReferenceSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        private string _inUser = "IPPSystemAdmin";
        public string InUser
        {
            get
            {
                return _inUser;
            }
            set
            {
                _inUser = value;
            }
        }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }
        #endregion
    }
}
