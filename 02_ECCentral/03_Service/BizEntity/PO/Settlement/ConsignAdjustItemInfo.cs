using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    public class ConsignAdjustItemInfo
    {

        public int ConsignAdjustSysNo { get; set; }

        public int DeductSysNo
        {
            get;
            set;
        }

        public decimal DeductAmt
        {
            get;
            set;
        }

        public string DeductName
        {
            get;
            set;
        }

        public AccountType AccountType
        {
            get;
            set;
        }

        public DeductMethod DeductMethod
        {
            get;
            set;
        }
    }
}
