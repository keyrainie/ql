using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroAutoConfirm.Model
{
    public class ZeroConfirmSOIncomeJobResp
    {
        public List<ZeroConfirmSOIncomeJobResultItem> Result
        {
            get;
            set;
        }
    }

    public class ZeroConfirmSOIncomeJobResultItem
    {
        public string SysNo
        {
            get;
            set;
        }

        public string ErrorDescription
        {
            get;
            set;
        }
    }
}