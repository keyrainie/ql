using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class ProductStepPrice
    {
        public int ProductSysNo { set; get; }
        public int BaseCount { set; get; }
        public int TopCount { set; get; }
        public decimal StepPrice { set; get; }
    }
}
