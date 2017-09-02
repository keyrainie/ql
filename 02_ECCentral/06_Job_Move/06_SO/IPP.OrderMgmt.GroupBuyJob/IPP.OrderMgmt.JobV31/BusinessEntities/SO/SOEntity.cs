using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class SOEntity
    {
        public SOMasterEntity SOMaster
        {
            get;
            set;
        }

       
        public List<SOItemEntity> SOItemList
        {
            get;
            set;
        }

       

        public SOCheckShippingEntity SOCheckShipping
        {
            get;
            set;
        }


    }
}
