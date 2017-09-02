using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class ProductShiftDetailQueryEntity:ICompany,ILanguage
    {
        public DateTime? OutTimeEnd { get; set; }

        public DateTime? OutTimeBegin { get; set; }
        
        public int? StockSysNoA { get; set; }
        
        public int? StockSysNoB { get; set; }
        
        public string GoldenTaxNo { get; set; }
        
        public int? ShiftSysNo { get; set; }
        
        public int? ShiftType { get; set; }

        public List<int> StItemSysNos { get; set; }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        public string LanguageCode
        {
            get;
            set;
        }

        #endregion
    }
}
