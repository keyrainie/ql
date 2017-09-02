using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class OrderShowMaster
    {
        public OrderShowMaster()
        {
            ProductList = new List<OrderShowProdutCell>();
        }


        public int SysNo
        {
            get;
            set;
        }
        public int CustomerSysNo
        {
            get;
            set;
        }
        public int SoSysno
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }

        public DateTime OrderDate
        {
            get;
            set;
        }

        public List<OrderShowProdutCell> ProductList
        {
            get;
            set;
        }
    }
}
