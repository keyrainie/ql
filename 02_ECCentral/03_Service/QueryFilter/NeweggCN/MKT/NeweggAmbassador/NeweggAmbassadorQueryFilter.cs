using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 
    /// </summary>
    public class NeweggAmbassadorQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }

        public string AmbassadorName
        {
            get;
            set;
        }


        public AmbassadorStatus? Status
        {
            get;
            set;
        }

        public int? BigAreaSysNo
        {
            get;
            set;
        }


        public string AmbassadorID
        {
            get;
            set;
        }

        public string AreaSysNo
        {
            get;
            set;
        }


    }
}
