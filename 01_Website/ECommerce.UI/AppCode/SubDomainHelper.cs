using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ECommerce.WebFramework;
using ECommerce.Facade.Store;
using ECommerce.Entity.Store;
using ECommerce.Entity;

namespace ECommerce.UI
{
    public class SubDomainHelper
    {
        /// <summary>
        /// 构造二级域名的Url（开发人员请根据项目需求，确定该处是否该使用本方法）。 
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <param name="pageSysNo"></param>
        /// <returns></returns>
        public static string BuildStoreUrl(int sellerSysNo, int? pageSysNo)
        {
            List<StoreDomainPage> list = StoreFacade.GetAllStoreDomainHomePageList();
            StoreDomainPage storePage = list.Find(f => f.SellerSysNo == sellerSysNo);

            string webDomain = ConstValue.WebDomain.TrimEnd("/".ToCharArray());
            string url = "#";
            if (storePage == null)
            {
                url = PageHelper.BuildUrl("StorePage", sellerSysNo, pageSysNo, false); 
            }
            else
            {
                Uri uri = HttpContext.Current.Request.Url;
                string portName = uri.Port == 80 ? "" : ":" + uri.Port.ToString();

                string domainOnlyHost = ConstValue.WebDomainOnlyHost.Trim();

                url = "http://" + storePage.SecondDomain + "." + domainOnlyHost + portName + "/store/" + storePage.SellerSysNo.ToString() + "/" + storePage.HomePageSysNo.ToString();
            }


            return url;
        }


    }
}