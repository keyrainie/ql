using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Xml;
using IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess;
using IPP.Oversea.CN.ContentMgmt.Baidu.Utility;
using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class CategoryEntity
    {
        #region DataMapping Properties

        [DataMapping("CategoryId", DbType.Int32)]
        public int CategoryId { get; set; }

        [DataMapping("C1Name", DbType.String)]
        public string C1Name { get; set; }

        [DataMapping("C2SysNo", DbType.Int32)]
        public int C2SysNo { get; set; }

        [DataMapping("CategoryName", DbType.String)]
        public string CategoryName { get; set; }

        [DataMapping("ProductCount", DbType.Int32)]
        public int ProductCount { get; set; }

        [DataMapping("ProductPrice", DbType.Decimal)]
        public decimal ProductPrice { get; set; }

        #endregion

        public string StorePic
        {
            get
            {
                return ConfigurationManager.AppSettings["StorePic"];
            }
        }

        public void WriteCategory(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("url");

            string loc = string.Format("http://www.newegg.com.cn/SubCategory/{0}.htm?cm_mmc=BaiduShop-_-{1}-_-{2}-_-{3}",
                CategoryId, HttpUtility.UrlEncode(C1Name, System.Text.Encoding.GetEncoding("gb2312")), C2SysNo, CategoryId);

            xmlWriter.WriteElementString("loc", JobHelper.bSubstring(loc, 256));
            xmlWriter.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd"));
            xmlWriter.WriteElementString("changefreq", "daily");
           
            WriteCategoryData(xmlWriter);

            xmlWriter.WriteEndElement();
        }

        private void WriteCategoryData(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("data");

            WriteCategoryDisplay(xmlWriter);

            xmlWriter.WriteEndElement();
        }

        private void WriteCategoryDisplay(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("display");

            xmlWriter.WriteElementString("store", "新蛋商城");
            xmlWriter.WriteElementString("store_pic", StorePic);

            BaiduCategoryEntity category = BaiduBP.GetBaiduCategory(CategoryId);

            if (category != null)
            {
                xmlWriter.WriteElementString("tags", JobHelper.bSubstring(category.CategoryName, 30));
            }
            else
            {
                xmlWriter.WriteElementString("tags", JobHelper.bSubstring("其它", 30));
            }

            xmlWriter.WriteElementString("services", @"正规发票\全国联保\七天退换货");
            xmlWriter.WriteElementString("brand", "");
            xmlWriter.WriteElementString("number", JobHelper.bSubstring(ProductCount.ToString(), 20));
            xmlWriter.WriteElementString("price", JobHelper.bSubstring(ProductPrice.ToString("0.00"), 20));
            
            xmlWriter.WriteEndElement();
        }
    }
}