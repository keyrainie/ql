using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class SOIncomeSplitedMessage : ECCentral.Service.Utility.EventMessage
    {
        public SOIncomeSplitedMessage()
        {
            SubSOIncomeSysNoList = new List<int>();
        }

        public override string Subject
        {
            get { return "ECC_SOIncome_Split"; }
        }

        public int MasterSOIncomeSysNo { get; set; }

        public List<int> SubSOIncomeSysNoList { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
