using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store
{
    public class RecommendBanner
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string Priority { get; set; }

        public int Status { get; set; }
    }
}
