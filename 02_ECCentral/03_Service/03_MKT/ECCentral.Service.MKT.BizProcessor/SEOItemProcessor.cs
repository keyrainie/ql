using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using System.Data;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SEOItemProcessor))]
    public class SEOItemProcessor
    {
        private ISEOItemDA seoDA = ObjectFactory<ISEOItemDA>.Instance;

        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SEOItem LoadSEOInfo(int sysNo)
        {
            SEOItem item= seoDA.LoadSEOInfo(sysNo);
            item.ProductList = seoDA.GetProductRangeBySeoSysNo(sysNo);
            item.CategoryList = seoDA.GetCategoryRangeBySeoSysNo(sysNo);
            return item;
        }

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddSEOInfo(SEOItem item)
        {
            if (item == null)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_CreatedObjectISNull"));
            else if (item.PageType.Value == 0)
                item.PageID = 0;
            if (!string.IsNullOrEmpty(item.PageTitle) && item.PageTitle.Length > 500)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxTitleLength"));
            else if (!string.IsNullOrEmpty(item.PageKeywords.Content) && item.PageKeywords.Content.Length > 1000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageKeywordsLength"));
            else if (!string.IsNullOrEmpty(item.PageAdditionContent) && item.PageAdditionContent.Length > 2000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageAdditionContentLength"));
            else if (!string.IsNullOrEmpty(item.PageDescription.Content) && item.PageDescription.Content.Length > 2000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageDescriptionLength"));
            if (item.PageType != 25)
            {
                if (item.Status == ADStatus.Active && seoDA.CheckSEOItem(item))
                    throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_ExsitTheObject"));
            }

            using (TransactionScope scope = new TransactionScope())
            {
                List<SeoCategory> CategordList = new List<SeoCategory>();
                List<SeoProductItem> productList = new List<SeoProductItem>();
                int pageid = item.PageID.Value;
                if (item.IsExtendValid == true && item.Status == ADStatus.Active && item.PageType.Value == 3 && item.PageID.Value != -1)
                    Category3ExtendValid(item);
                item.PageID = pageid;
                item = seoDA.AddSEOInfo(item);
                CreateCategoryMetadataLog(item);
                seoDA.DeletetProductRangeBySysNo(item.SysNo);
                foreach (var c in item.CategoryList)
                {
                    if (seoDA.IsExistsCategory(c.SysNo))
                    {
                        seoDA.CreateProductRange(item.SysNo, item.User.UserName, CategorySysNo: c.SysNo);
                    }
                    else
                    {
                        CategordList.Add(c);
                    }
                }
                foreach (var p in item.ProductList)
                {
                    if (seoDA.IsExistsProductByProductId(p.ProductId))
                    {
                        seoDA.CreateProductRange(item.SysNo, item.User.UserName, productId: p.ProductId);
                    }
                    else
                    {
                        productList.Add(p);
                    }
                }
                if (productList.Count > 0)
                {
                    //throw new BizException(string.Format("商品{0}不存在!", (from p in productList select p.ProductId).ToList().Join("\r")));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_NotExsitTheProduct"), (from p in productList select p.ProductId).ToList().Join("\r")));
                }
                if (CategordList.Count > 0)
                {
                    //throw new BizException(string.Format("类别{0}不存在!", (from p in CategordList select p.CategoryName).ToList().Join("\r")));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_NotExsitTheCategory"), (from p in CategordList select p.CategoryName).ToList().Join("\r")));
                }
                scope.Complete();
            }
            
        }

        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateSEOInfo(SEOItem item)
        {
            if (item == null)
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_CreatedObjectISNull"));
            
            if (!string.IsNullOrEmpty(item.PageTitle) && item.PageTitle.Length > 500)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxTitleLength"));
            else if (!string.IsNullOrEmpty(item.PageKeywords.Content) && item.PageKeywords.Content.Length > 1000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageKeywordsLength"));
            else if (!string.IsNullOrEmpty(item.PageAdditionContent) && item.PageAdditionContent.Length > 2000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageAdditionContentLength"));
            else if (!string.IsNullOrEmpty(item.PageDescription.Content) && item.PageDescription.Content.Length > 2000)
                throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_MaxPageDescriptionLength"));


            if (item.PageType != 25) //详细商品页面不需要检测
            {
                if (item.Status == ADStatus.Active && seoDA.CheckSEOItem(item))
                    throw new BizException(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_ExsitTheObject"));
            }

            using (TransactionScope scope = new TransactionScope())
            {
                List<SeoCategory> CategordList = new List<SeoCategory>();
                List<SeoProductItem> productList = new List<SeoProductItem>();
                int pageid = item.PageID.Value;
                if (item.IsExtendValid == true && item.Status == ADStatus.Active && item.PageType.Value == 3 && item.PageID.Value != -1)
                    Category3ExtendValid(item);
                item.PageID = pageid;
                seoDA.UpdateSEOInfo(item);
                CreateCategoryMetadataLog(item);
                seoDA.DeletetProductRangeBySysNo(item.SysNo);
                foreach (var c in item.CategoryList)
                {
                    if (seoDA.IsExistsCategory(c.SysNo))
                    {
                        seoDA.CreateProductRange(item.SysNo, item.User.UserName, CategorySysNo: c.SysNo);
                    }
                    else
                    {
                        CategordList.Add(c);
                    }
                }
                foreach (var p in item.ProductList)
                {
                    if (seoDA.IsExistsProductByProductId(p.ProductId))
                    {
                        seoDA.CreateProductRange(item.SysNo, item.User.UserName, productId: p.ProductId);
                    }
                    else
                    {
                        productList.Add(p);
                    }
                }
                if (productList.Count > 0)
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_NotExsitTheProduct"), (from p in productList select p.ProductId).ToList().Join("\r")));
                }
                if (CategordList.Count > 0)
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.SEOInfo", "SEOInfo_NotExsitTheCategory"), (from p in CategordList select p.CategoryName).ToList().Join("\r")));
                }
                scope.Complete();
            }
            
        }

        /// <summary>
        /// 创建SEO的日志
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateCategoryMetadataLog(SEOItem item)
        {
            CategoryMetadataLog log = new CategoryMetadataLog();
            log.CompanyCode = item.CompanyCode;
            log.Operation = "Create";
            log.Description = item.PageDescription.Content;
            log.CategoryMetadataInfoSysNo = item.SysNo;
            seoDA.CreateCategoryMetadataLog(log);
        }

        /// <summary>
        /// 扩展生效
        /// </summary>
        /// <param name="item"></param>
        public virtual void Category3ExtendValid(SEOItem item)
        {
            var relatedECCategory3List = ObjectFactory<IECCategoryDA>.Instance.GetRelatedECCategory3SysNo(item.PageID.Value);
            foreach (var rc3 in relatedECCategory3List)
            {
                item.PageID = rc3.C3SysNo;
                if (!seoDA.CheckSEOItem(item))
                {
                    seoDA.AddSEOInfo(item);
                }
            }
        }


        public virtual string GetVendorName(SEOItem item)
        {
            return seoDA.GetVendorName(item);
        }

        public virtual string GetBrandNameSpe(SEOItem item)
        {
            return seoDA.GetBrandNameSpe(item);
        }

        public virtual string GetSaleAdvertisement(SEOItem item)
        {
            return seoDA.GetSaleAdvertisement(item);
        }

        public virtual string GetBrandName(SEOItem item)
        {
            return seoDA.GetBrandName(item);
        }

        public virtual string GetCNameFromCategory1(SEOItem item)
        {
            return seoDA.GetCNameFromCategory1(item);
        }

        public virtual string GetCNameFromCategory2(SEOItem item)
        {
            return seoDA.GetCNameFromCategory2(item);
        }

        public virtual string GetCNameFromCategory3(SEOItem item)
        {
            return seoDA.GetCNameFromCategory3(item);
        }
    }
}
