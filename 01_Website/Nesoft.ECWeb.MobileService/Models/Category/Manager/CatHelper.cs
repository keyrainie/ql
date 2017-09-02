using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.Utility;
using Nesoft.ECWeb.MobileService.Models.Category.Config;

namespace Nesoft.ECWeb.MobileService.Models.Category
{
    public class CatHelper
    {
        private static CategoryIconsConfig GetCachedConfig()
        {
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/CategoryIcons.config");
            return CacheManager.ReadXmlFileWithLocalCache<CategoryIconsConfig>(configPath);
        }

        /// <summary>
        /// 获取一级分类图标
        /// </summary>
        /// <param name="catID"></param>
        /// <returns></returns>
        public static string GetIcon(int catID)
        {
            var config = GetCachedConfig();
            if (config.CatList != null)
            {
                var found = config.CatList.FirstOrDefault(item => item.ID == catID);
                if (found != null)
                {
                    return found.Icon;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取二级分类图标
        /// </summary>
        /// <param name="catID"></param>
        /// <param name="subCatID"></param>
        /// <returns></returns>
        public static string GetSubIcon(int catID, int subCatID)
        {
            var config = GetCachedConfig();
            if (config.CatList != null)
            {
                var found = config.CatList.FirstOrDefault(item => item.ID == catID);
                if (found != null && found.SubCatList != null)
                {
                    var subCat = found.SubCatList.FirstOrDefault(item => item.ID == subCatID);
                    if (subCat != null)
                    {
                        return subCat.Icon;
                    }
                }
            }
            return "";
        }
    }
}