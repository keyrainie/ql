using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class AddProductReviewRequestViewModel
    {
        public int ProductSysNo { get; set; }
        public string Title { get; set; }
        public string Prons { get; set; }
        public string Cons { get; set; }
        public string Service { get; set; }
        public decimal Score1 { get; set; }
        public decimal Score2 { get; set; }
        public decimal Score3 { get; set; }
        public decimal Score4 { get; set; }
        public int OrderSysNo { get; set; }
    }
}