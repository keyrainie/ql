using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess
{

    public interface IProductPageKeywordsDA
    {
        #region 产品页面关键字（ProductPageKeywords）

        /// <summary>
        /// 从Queue中得到所有要更新的产品
        /// </summary>
        List<ProductKeywordsQueue> GetProductIDsFromQueue(string companyCode);
        
        /// <summary>
        /// //根据categoryId从数据中得到catgegoryKeywods 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        List<CategoryKeywords> GetCategroyKeywordsForBatchUpdate(string companyCode, string categoryIds);
        
        /// <summary>
        /// 更新keywords0
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="ProductSysNo"></param>
        /// <param name="Keywords0"></param>
        /// <returns></returns>
        int UpdateKeyWords0ByProductSysNo(string companyCode, int? ProductSysNo, string Keywords0);

        List<ProductKeywordsQueue> GetAllC3Categories(string companyCode);
        
        List<ProductKeywordsQueue> GetProductByC3SysNo(string companyCode, int? categoryId);

        ProductKeywordsQueue GetSingleProduct(string companyCode, int? productSysNo);
        /// <summary>
        /// 加载产品页面关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //ProductPageKeywords LoadProductPageKeywords(int sysNo);

        /// <summary>
        /// 获取页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<UserInfo> GetProductKeywordsEditUserList(string companyCode);

        /// <summary>
        /// 更新产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        void UpdateProductPageKeywords(ProductPageKeywords item);

        /// <summary>
        /// 删除产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        //void BatchDeleteProductPageKeywords(List<int> item);

        /// <summary>
        /// 批量添加产品页面关键字Keywords0
        /// </summary>
        /// <param name="item"></param>
        //void AddProductPageKeywords(ProductPageKeywords item);


        DataTable ReadExcelFileToDataTable(string fileName);

        /// <summary>
        /// 批量导入产品页面关键字
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="companyCode"></param>
        void InsertProductKeywordsListBatch(string productID, string companyCode);

        /// <summary>
        /// 得到某类下所有的属性
        /// </summary>
        /// <param name="GetPropertyByCategory3SysNo"></param>
        /// <returns></returns>
        DataTable GetPropertyByCategory3SysNo(int Category3SysNo);

       /// <summary>
       /// 根据属性得到所有属性值
       /// </summary>
       /// <param name="PropertySysNo"></param>
       /// <returns></returns>
        DataTable GetPropertyValueByPropertySysNo(int PropertySysNo);

        #endregion
    }
}
