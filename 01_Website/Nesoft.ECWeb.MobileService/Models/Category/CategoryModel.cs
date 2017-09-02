using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Category
{
    public class CategoryModel
    {
        public int CatID { get; set; }

        public string CatName { get; set; }

        public string ICon { get; set; }

        public List<CategoryModel> SubCategories { get; set; }
    }
}