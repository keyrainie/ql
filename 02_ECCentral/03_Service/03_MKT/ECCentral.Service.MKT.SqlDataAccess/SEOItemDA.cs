using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ISEOItemDA))]
    public class SEOItemDA : ISEOItemDA
    {
        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SEOItem LoadSEOInfo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SEO_GetSEOMetadata");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable<ADStatus>("Status");
            SEOItem item = DataMapper.GetEntity<SEOItem>(dt.Rows[0]);
            return item;
        }

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="item"></param>
        public SEOItem AddSEOInfo(SEOItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SEO_InsertSEOMetadata");
            dc.SetParameterValue<SEOItem>(item);
            dc.ExecuteNonQuery();
            item.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return item;
        }

        /// <summary>
        /// 判断是否存在该需要创建的对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSEOItem(SEOItem item)
        {
            DataCommand dc;
            if (item.SysNo > 0)
            {
                dc = DataCommandManager.GetDataCommand("SEO_CheckSEOItemByUpdate");
            }
            else
            {
                dc = DataCommandManager.GetDataCommand("SEO_CheckSEOItem");
            }

            dc.SetParameterValue<SEOItem>(item);
            return dc.ExecuteScalar() != null;
        }

        /// <summary>
        /// 添加SEO维护 log
        /// </summary>
        /// <param name="item"></param>
        public void CreateCategoryMetadataLog(CategoryMetadataLog item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SEO_InsertSEOMetadataLog");
            dc.SetParameterValue<CategoryMetadataLog>(item);
            dc.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="item"></param>
        public void UpdateSEOInfo(SEOItem item)
        {

            DataCommand dc = DataCommandManager.GetDataCommand("SEO_UpdateSEOMetadata");
            dc.SetParameterValue<SEOItem>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 扩展生效
        /// </summary>
        /// <param name="item"></param>
        public List<SEOItem> GetCategoryMetadataByC3SysNo(SEOItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SEO_GetSEOMetadataByC3SysNo");
            dc.SetParameterValue("@PageType", item.PageType);
            dc.SetParameterValue("@C3SysNo", item.PageID);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            return dc.ExecuteEntityList<SEOItem>();
        }


        public string GetVendorName(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetVendorName");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetBrandNameSpe(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetBrandNameSpe");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetSaleAdvertisement(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetSaleAdvertisement");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetBrandName(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetBrandName");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetCNameFromCategory1(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetCNameFromCategory1");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetCNameFromCategory2(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetCNameFromCategory2");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }

        public string GetCNameFromCategory3(SEOItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SEO_GetCNameFromCategory3");
            cmd.SetParameterValue<SEOItem>(item);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();

            return string.Empty;
        }


        /// <summary>
        /// 根据Seo SysNo 的商品范围
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public List<SeoProductItem> GetProductRangeBySeoSysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRangeBySeoSysNo");
            cmd.SetParameterValue("@CategoryMetadataInfoSysNo", SysNo);
            return cmd.ExecuteEntityList<SeoProductItem>();
        }
        /// <summary>
        /// 根据Seo SysNo 的类别返回
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public List<SeoCategory> GetCategoryRangeBySeoSysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryRangeBySeoSysNo");
            cmd.SetParameterValue("@CategoryMetadataInfoSysNo", SysNo);
            return cmd.ExecuteEntityList<SeoCategory>();
        }


        public void DeletetProductRangeBySysNo(int? SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeletetProductRangeBySysNo");
            cmd.SetParameterValue("@CategoryMetadataInfoSysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        public bool IsExistsProductByProductId(string productId)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsProductByProductId");
            cmd.SetParameterValue("@productId", productId);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag")>0;
        }

        public bool IsExistsCategory(int? CategorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategory");
            cmd.SetParameterValue("@CategorySysNo", CategorySysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag") > 0;
        }

        public void CreateProductRange(int? seoSysNo,string UserName,string productId = null, int? CategorySysNo = null)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductRange");
            cmd.SetParameterValue("@CategoryMetadataInfoSysNo", seoSysNo);
            cmd.SetParameterValue("@ProductId", productId);
            cmd.SetParameterValue("@C3SysNo", CategorySysNo);
            cmd.SetParameterValue("@InUser", UserName);
            cmd.ExecuteNonQuery();
       }
    }
}
