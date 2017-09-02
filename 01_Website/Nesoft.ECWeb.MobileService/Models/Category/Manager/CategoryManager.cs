using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Entity.Category;
using Nesoft.ECWeb.MobileService.Models.Version;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.App;
using Nesoft.ECWeb.MobileService.AppCode;

namespace Nesoft.ECWeb.MobileService.Models.Category
{
    public class CategoryManager
    {
        public List<CategoryModel> GetCategoryList()
        {
            var allInfo = CategoryFacade.QueryCategoryInfosForHomePage();
            List<CategoryModel> result = new List<CategoryModel>();
            ClientType clientType = HeaderHelper.GetClientType();
            string subFoler = "h";
            if (clientType == ClientType.Android)
            {
                if (HeaderHelper.IsAndroidXHigh())
                {
                    subFoler = "xh";
                }
            }
            else
            {
                subFoler = "ios";
            }
            string baseUrl = "";
            MobileAppConfig config = AppSettings.GetCachedConfig();
            if (config != null && config.MobileAppServiceHost != null)
            {
                config.MobileAppServiceHost = config.MobileAppServiceHost.Trim();
                if (!config.MobileAppServiceHost.EndsWith("/"))
                {
                    config.MobileAppServiceHost += "/";
                }
                baseUrl = config.MobileAppServiceHost;
            }
            foreach (var c1 in allInfo)
            {
                var m1 = Transform(c1);
                //获取一级分类图标
                m1.ICon = BuildCatIcon(baseUrl, "c1", subFoler, c1.CategoryID.ToString(),"png");//CatHelper.GetIcon(c1.CategoryID);
                result.Add(m1);
                foreach (var c2 in c1.SubCategories)
                {
                    var m2 = Transform(c2);
                    //获取二级分类图标
                    m2.ICon = BuildCatIcon(baseUrl, "c2", subFoler, c2.CategoryID.ToString(), "jpg");//CatHelper.GetSubIcon(c1.CategoryID, c2.CategoryID);
                    m1.SubCategories.Add(m2);
                    foreach (var c3 in c2.SubCategories)
                    {
                        var m3 = Transform(c3);
                        m2.SubCategories.Add(m3);
                    }
                }
            }

            return result;
        }

        private CategoryModel Transform(CategoryInfo catInfo)
        {
            var m1 = new CategoryModel();
            m1.CatID = catInfo.CategoryID;
            m1.CatName = catInfo.CategoryName;
            m1.SubCategories = new List<CategoryModel>();

            return m1;
        }

        private string BuildCatIcon(string baseUrl,string level,string subFolder,string sysNo,string suffix)
        {
            return string.Format("{0}Resources/images/{1}/{2}/CATE_ICON_{3}.{4}", baseUrl, level, subFolder, sysNo, suffix);
        }
    }
}