using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{

    [VersionExport(typeof(ProductPageKeywordsAppService))]
    public class ProductPageKeywordsAppService
    {
        #region 产品页面关键字（ProductPageKeywords）
        /// <summary>
        /// 加载产品页面关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //public virtual ProductPageKeywords LoadProductPageKeywords(int sysNo)
        //{
        //    return ObjectFactory<ProductPageKeywordsProcessor>.Instance.LoadProductPageKeywords(sysNo);
        //}
        
        /// <summary>
        /// JOB更新产品页面关键字
        /// </summary>
        public virtual void BatchUpdateKeywordsFromQueue(string companyCode)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.BatchUpdateKeywordsFromQueue(companyCode);
        }

        public virtual void BatchUpdateKeywordsForKeywords0(string companyCode)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.BatchUpdateKeywordsForKeywords0(companyCode);
        }

        public virtual void BatchUpdateKeywordsFromProduct(ECCentral.BizEntity.IM.ProductInfo product)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.BatchUpdateKeywordsFromProduct(product);
        }
        /// <summary>
        /// 获取页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.Common.UserInfo> GetProductKeywordsEditUserList(string companyCode)
        {
            return ObjectFactory<ProductPageKeywordsProcessor>.Instance.GetProductKeywordsEditUserList(companyCode);
        }
        /// <summary>
        /// 更新产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductPageKeywords(ProductPageKeywords item)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.UpdateProductPageKeywords(item);
        }

        /// <summary>
        /// 删除或添加产品页面关键字
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="batchAdd"></param>
        /// <param name="key0"></param>
        /// <param name="key1"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        public virtual void BatchUpdateProductPageKeywords(List<string> productList, bool batchAdd, string key0, string key1, string languageCode, string companyCode)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.BatchUpdateProductPageKeywords(productList, batchAdd, key0, key1, languageCode, companyCode);
        }

        /// <summary>
        /// 批量添加产品页面关键字
        /// </summary>
        /// <param name="item"></param>
        //public virtual void AddProductPageKeywords(ProductPageKeywords item)
        //{
        //    ObjectFactory<ProductPageKeywordsProcessor>.Instance.AddProductPageKeywords(item);
        //}
        /// <summary>
        /// 上传批量添加产品页面关键字
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        public virtual void BatchImportProductKeywords(string uploadFileInfo)
        {
            ObjectFactory<ProductPageKeywordsProcessor>.Instance.BatchImportProductKeywords(uploadFileInfo);
        }
        #endregion
    }
}
