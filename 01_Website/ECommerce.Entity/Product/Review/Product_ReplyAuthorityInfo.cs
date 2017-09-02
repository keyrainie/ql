using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class Product_ReplyAuthorityInfo
    {
 
       
        public CustomerRankType CustomerRank
        {
            get;
            set;
        }

       
        public int CustomerRights
        {
            get;
            set;
        }

       
        public int IsAllowComment
        {
            get;
            set;
        }

       
        public int CustomerDailyReplyCount
        {
            get;
            set;
        }

       
        public int ReviewSysNo
        {
            get;
            set;
        }

    }
}
