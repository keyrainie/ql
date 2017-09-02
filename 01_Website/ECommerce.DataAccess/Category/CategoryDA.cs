using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.Product;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Category
{
    public class CategoryDA
    {

        public static List<RecommendProduct> QueryWeekRankingForC1(int c1SysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetWeekRankingForC1");
            cmd.SetParameterValue("@C1SysNo", c1SysNo);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

      


        public static List<CategoryInfo> QueryCategories()
        {
            var cmd = DataCommandManager.GetDataCommand("QueryCategories");
            return cmd.ExecuteEntityList<CategoryInfo>();
        }


        public static List<RecommendProduct> QueryWeekRankingForC2(int c2SysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetWeekRankingForC2");
            cmd.SetParameterValue("@C2SysNo", c2SysNo);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }


        public static List<RecommendProduct> QueryWeekRankingForC3(int c3SysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetWeekRankingForC3");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }
        
    }
}
