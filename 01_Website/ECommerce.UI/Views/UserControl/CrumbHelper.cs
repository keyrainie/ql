using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ECommerce.WebFramework; 
using ECommerce.Entity;
using System.Web.Caching;

namespace ECommerce.UI
{
    public class CrumbHelper
    {
        public static MvcHtmlString BuildCrumb(List<CrumbItem> itemList)
        { 
            StringBuilder sb=new StringBuilder();
            if(itemList!=null && itemList.Count >0)
            {
                sb.Append("<div class=\"crumb\">");
                sb.Append("<a href=\"" + PageHelper.BuildUrl("Web_Index") + "\">" + LanguageHelper.GetText("首页") + "</a>");
                int i = 1;
                foreach (CrumbItem item in itemList)
                {
                    if (i < itemList.Count)
                    {
                        sb.Append("<b>></b>");
                        if (string.IsNullOrWhiteSpace(item.ItemUrl))
                        {
                            sb.Append(string.Format("{0}", item.ItemName));
                        }
                        else
                        {
                            sb.Append(string.Format("<a href=\"{0}\">{1}</a>", item.ItemUrl, item.ItemName));
                        }
                    }
                    else
                    {
                        sb.Append("<b>></b>");
                        sb.Append(string.Format("<strong>{0}</strong>", item.ItemName));
                    }

                    i++;
                }
                sb.Append("</div>");
            }

            return new MvcHtmlString(sb.ToString());
        }

       


    }



    public class CrumbItem
    {
        public CrumbItem(string itemName, string itemUrl)
        {
            ItemName = itemName;
            ItemUrl = itemUrl;
        }

        public string ItemName { get; set; }

        public string ItemUrl { get; set; }
    }
}