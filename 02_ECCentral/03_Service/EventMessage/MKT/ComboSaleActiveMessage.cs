using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    [Serializable]
    public class ComboSaleActiveMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_ComboSale_Actived";
            }
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int ComboSaleSysNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ComboSaleName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
