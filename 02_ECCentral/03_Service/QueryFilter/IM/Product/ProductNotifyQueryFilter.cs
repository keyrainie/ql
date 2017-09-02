using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class ProductNotifyQueryFilter
    {
       public PagingInfo PageInfo { get; set; }
        public int? Category1SysNo
        {
            get;
            set;
        }

    
        public int? Category2SysNo
        {
            get;
            set;
        }

       
        public int? Category3SysNo
        {
            get;
            set;
        }

     
        public string CustomserID
        {
            get;
            set;
        }

      
        public int? ProductSysNo
        {
            get;
            set;
        }


        public NotifyStatus? Status
        {
            get;
            set;
        }

      
        public string Email
        {
            get;
            set;
        }


        public DateTime? StartTime
        {
            get;
            set;
        }

     
        public DateTime? EndTime
        {
            get;
            set;
        }

      
        public int? PMSysNo
        {
            get;
            set;
        }
    }
}
