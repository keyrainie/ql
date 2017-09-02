using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ISEOItemDA
    {
        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SEOItem LoadSEOInfo(int sysNo);

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="item"></param>
        SEOItem AddSEOInfo(SEOItem item);

        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="item"></param>
        void UpdateSEOInfo(SEOItem item);
        
        /// <summary>
        /// 添加SEO维护 log
        /// </summary>
        /// <param name="item"></param>
        void CreateCategoryMetadataLog(CategoryMetadataLog item);

        /// <summary>
        /// 判断是否存在该需要创建的对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckSEOItem(SEOItem item);

        List<SEOItem> GetCategoryMetadataByC3SysNo(SEOItem item);

        /// <summary>
        /// 根据Seo SysNo 的商品范围
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        List<SeoProductItem> GetProductRangeBySeoSysNo(int SysNo);
        /// <summary>
        /// 根据Seo SysNo 的类别返回
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        List<SeoCategory> GetCategoryRangeBySeoSysNo(int SysNo);

        /// <summary>
        /// 删除商品范围
        /// </summary>
        /// <param name="SysNo"></param>
        void DeletetProductRangeBySysNo(int? SysNo);

        /// <summary>
        /// 检测商品是否存在
        /// </summary>
        /// <param name="ProductSysNo"></param>
        /// <returns></returns>
        bool IsExistsProductByProductId(string productId);

        /// <summary>
        /// 检测类别是否存在
        /// </summary>
        /// <param name="CategorySysNo"></param>
        /// <returns></returns>
        bool IsExistsCategory(int? CategorySysNo);

        /// <summary>
        /// 创建商品范围
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="CategorySysNo"></param>
        void CreateProductRange(int? seoSysNo,string UserName,string productId=null, int? CategorySysNo=null);


        string GetVendorName(SEOItem item);

        string GetBrandNameSpe(SEOItem item);

        string GetSaleAdvertisement(SEOItem item);

        string GetBrandName(SEOItem item);

        string GetCNameFromCategory1(SEOItem item);

        string GetCNameFromCategory2(SEOItem item);

        string GetCNameFromCategory3(SEOItem item);
    }
}
