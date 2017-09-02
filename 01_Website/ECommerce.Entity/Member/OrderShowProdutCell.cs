using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class OrderShowProdutCell
    {

        public int SysNo
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }

        public string PromotionTitle
        {
            get;
            set;
        }

        public AuditingStatus ShowAuditingStatus
        {
            get;
            set;
        }


        public DateTime InDate
        {
            get;
            set;
        }


        public string ImageUrl
        {
            get;
            set;
        }
    }
}
