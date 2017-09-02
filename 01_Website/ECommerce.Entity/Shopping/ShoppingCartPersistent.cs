using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shopping
{
    public class ShoppingCartPersistent
    {
        //public string Key { get; set; }
        public int KeyAscii { get; set; }
        public int? CustomerSysNo { get; set; }
        public string ShoppingCart { get; set; }
        public string ShoppingCartMini { get; set; }
        public DateTime InDate { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
