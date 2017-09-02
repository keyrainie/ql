using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
using System.IO;
using System.Data.OleDb;
namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductPageKeywordsDA))]
    public class ProductPageKeywordsDA : IProductPageKeywordsDA
    {


        #region 产品页面关键字（ProductPageKeywords）

        #region  JOB
        
        /// <summary>
        /// 从Queue中得到所有要更新的产品
        /// </summary>
        /// <param name="companyCode"></param>
        public List<ProductKeywordsQueue> GetProductIDsFromQueue(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_GetProductIDsFromQueue");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<ProductKeywordsQueue>();
        }

        /// <summary>
        /// //根据categoryId从数据中得到catgegoryKeywods 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        public List<CategoryKeywords> GetCategroyKeywordsForBatchUpdate(string companyCode, string categoryIds)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_GetCategroyKeywords");
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.SetParameterValue("@C3SysNos", categoryIds);
            return dc.ExecuteEntityList<CategoryKeywords>();
        }

        /// <summary>
        /// 更新keywords0
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="ProductSysNo"></param>
        /// <param name="Keywords0"></param>
        /// <returns></returns>
        public int UpdateKeyWords0ByProductSysNo(string companyCode, int? ProductSysNo, string Keywords0)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductKeywords_UpdateKeyWords0ByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", ProductSysNo);
            cmd.SetParameterValue("@Keywords0", Keywords0);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteNonQuery();
        }
        
        public List<ProductKeywordsQueue> GetAllC3Categories(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_GetAllC3Categories");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<ProductKeywordsQueue>();
        }

        public List<ProductKeywordsQueue> GetProductByC3SysNo(string companyCode,int? categoryId)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductKeywords_GetProductByC3SysNo");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@CategoryId", categoryId);
            return cmd.ExecuteEntityList<ProductKeywordsQueue>();
        }

        public ProductKeywordsQueue GetSingleProduct(string companyCode, int? productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductKeywords_GetSingleProduct");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@productSysNo", productSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            return DataMapper.GetEntity<ProductKeywordsQueue>(dt.Rows[0]);
        }

        /// <summary>
        ///得到当前产品的keywords
        /// </summary>
        /// <param name="category"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        //private static string GetProductKeywords0(CategoryKeywords category, ProductKeywordsQueue product)
        //{

        //    //因为要不重复, 所以使用HashSet
        //    var distinctedKeywordsList = new HashSet<string>();
        //    //如果公共关键字不为空
        //    if (!string.IsNullOrEmpty(category.CommonKeywords.Content))
        //    {
        //        distinctedKeywordsList.AddRange(category.CommonKeywords.Content.Split(' '));
        //    }
        //    //得到属性关键字
        //    var propertyInfoList = BatchUpdateItemKeywordsDA.GetPropertyInfo(product.ProductSysNo.Value, product.C3SysNo.Value);
        //    foreach (var propertyInfo in propertyInfoList)
        //    {
        //        var keywords = propertyInfo.ManualInput.IsNullOrEmpty() ? propertyInfo.ValueDescription : propertyInfo.ManualInput;
        //        keywords = (keywords ?? string.Empty).Trim();

        //        if (!keywords.IsNullOrEmpty())
        //        {
        //            distinctedKeywordsList.AddRange(keywords.Split(' '));
        //        }
        //    }

        //    return distinctedKeywordsList.Join(" ");
        //}
        #endregion

        /// <summary>
        /// 加载产品页面关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //public ProductPageKeywords LoadProductPageKeywords(int sysNo)
        //{
        //    DataCommand dc = DataCommandManager.GetDataCommand("Segment_GetSegment");
        //    dc.SetParameterValue("@SysNo", sysNo);
        //    DataTable dt = dc.ExecuteDataTable();
        //    return DataMapper.GetEntity<ProductPageKeywords>(dt.Rows[0]);
        //}

        /// <summary>
        /// 获取页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<UserInfo> GetProductKeywordsEditUserList(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_GetAllLastEditUser");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<UserInfo>();
        }

        /// <summary>
        /// 更新产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        public void UpdateProductPageKeywords(ProductPageKeywords item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_UpdateProductKeywords");
            dc.SetParameterValue<ProductPageKeywords>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量导入产品页面关键字
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="companyCode"></param>
        public void InsertProductKeywordsListBatch(string productID, string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductKeywords_InsertProductKeywordsListBatch");
            dc.SetParameterValue("@ProductID", productID);
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.ExecuteNonQuery();
        }

        public DataTable ReadExcelFileToDataTable(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                try
                {
                    //[Mark]:Move To Config
                    //string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;";

                    DataTable dt = new DataTable();
                    using (OleDbConnection Conn = new OleDbConnection(string.Format(strConn, fileName)))
                    {
                        string SQL = "select * from [Sheet1$]";
                        Conn.Open();
                        OleDbDataAdapter da = new OleDbDataAdapter(SQL, string.Format(strConn, fileName));
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    return null;
                }
                
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 得到某类下所有的属性
        /// </summary>
        /// <param name="GetPropertyByCategory3SysNo"></param>
        /// <returns>DataTable</returns>
        public DataTable GetPropertyByCategory3SysNo(int Category3SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyByCategory3SysNo");
            dc.SetParameterValue("@Category3SysNo", Category3SysNo);
            return dc.ExecuteDataTable();
        }
        /// <summary>
        /// 根据属性得到所有属性值
        /// </summary>
        /// <param name="PropertySysNo"></param>
        /// <returns>DataTable</returns>
        public DataTable GetPropertyValueByPropertySysNo(int PropertySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPropertyValueByPropertySysNo");
            dc.SetParameterValue("@PropertySysNo", PropertySysNo);
            return dc.ExecuteDataTable();
        }
    }
}
