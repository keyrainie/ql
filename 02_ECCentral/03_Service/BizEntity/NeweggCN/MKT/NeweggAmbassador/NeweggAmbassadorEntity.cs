using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Enum;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT
{

    public class NeweggAmbassadorEntity
    {
        public int? AmbassadorSysNo { get; set; }

        public string AmbassadorID { get; set; }

        public string CompanyCode { get; set; }

        public AmbassadorStatus? CustomerMark { get; set; }  //'大使状态'-- [1：未激活；2：已激活]

        
    }
}
