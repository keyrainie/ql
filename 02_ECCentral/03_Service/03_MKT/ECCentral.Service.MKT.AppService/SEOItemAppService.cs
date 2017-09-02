using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(SEOItemAppService))]
    public class SEOItemAppService
    {
        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SEOItem LoadSEOInfo(int sysNo)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.LoadSEOInfo(sysNo);
        }

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="item"></param>
        public void AddSEOInfo(SEOItem item)
        {
            ObjectFactory<SEOItemProcessor>.Instance.AddSEOInfo(item);
        }

        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="item"></param>
        public void UpdateSEOInfo(SEOItem item)
        {
            ObjectFactory<SEOItemProcessor>.Instance.UpdateSEOInfo(item);
        }



        public virtual string GetVendorName(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetVendorName(item);
        }

        public virtual string GetBrandNameSpe(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetBrandNameSpe(item);
        }

        public virtual string GetSaleAdvertisement(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetSaleAdvertisement(item);
        }

        public virtual string GetBrandName(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetBrandName(item);
        }

        public virtual string GetCNameFromCategory1(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetCNameFromCategory1(item);
        }

        public virtual string GetCNameFromCategory2(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetCNameFromCategory2(item);
        }

        public virtual string GetCNameFromCategory3(SEOItem item)
        {
            return ObjectFactory<SEOItemProcessor>.Instance.GetCNameFromCategory3(item);
        }
    }
}
