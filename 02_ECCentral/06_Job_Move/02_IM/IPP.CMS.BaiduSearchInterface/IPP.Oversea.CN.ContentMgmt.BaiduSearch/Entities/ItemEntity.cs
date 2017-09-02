using System;
using System.Data;
using System.Web;
using System.Xml;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess;
using IPP.Oversea.CN.ContentMgmt.Baidu.Utility;
using IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess;
using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities.String;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class ItemEntity
    {
        #region DataMapping Properties

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductTitle", DbType.String)]
        public string ProductTitle { get; set; }

        [DataMapping("PromotionTitle", DbType.String)]
        public string PromotionTitle { get; set; }

        [DataMapping("Category", DbType.String)]
        public string Category { get; set; }

        [DataMapping("C1Name", DbType.String)]
        public string C1Name { get; set; }

        [DataMapping("C2Name", DbType.String)]
        public string C2Name { get; set; }

        [DataMapping("C3Name", DbType.String)]
        public string C3Name { get; set; }

        [DataMapping("Brand", DbType.String)]
        public string Brand { get; set; }

        [DataMapping("PicUrl", DbType.String)]
        public string PicUrl { get; set; }

        [DataMapping("ProductUrl", DbType.String)]
        public string ProductUrl { get; set; }

        [DataMapping("FirstOnlineTime", DbType.DateTime)]
        public DateTime FirstOnlineTime { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal CurrentPrice { get; set; }

        [DataMapping("UpdateTime", DbType.DateTime)]
        public DateTime UpdateTime { get; set; }

        [DataMapping("ProductDescLong", DbType.String)]
        public string ProductDescLong { get; set; }

        [DataMapping("Performance", DbType.String)]
        public string Performance { get; set; }

        [DataMapping("ReviewCount", DbType.Int32)]
        public int ReviewCount { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo { get; set; }

        [DataMapping("ReviewUrl", DbType.String)]
        public string ReviewUrl { get; set; }

        [DataMapping("OnlineQty", DbType.Int32)]
        public int OnlineQty { get; set; }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }

        [DataMapping("ManufacturerName", DbType.String)]
        public string ManufacturerName { get; set; }

        #endregion

        public void WriteItem(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("url");
            xmlWriter.WriteElementString("loc", JobHelper.bSubstring(string.Format(
                ProductUrl, C3SysNo, HttpUtility.UrlEncode(Brand, System.Text.Encoding.GetEncoding("gb2312")), ProductID), 256));
            xmlWriter.WriteElementString("lastmod", UpdateTime.ToString("yyyy-MM-dd"));
            xmlWriter.WriteElementString("changefreq", "daily");
            WriteItemData(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        private void WriteItemData(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("data");
            WriteItemDisplay(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        private void WriteItemDisplay(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("display");
            xmlWriter.WriteElementString("title", JobHelper.bSubstring(ProductTitle + NoHtml(PromotionTitle), 150));
            xmlWriter.WriteElementString("realtitle", JobHelper.bSubstring(ProductTitle, 150));
            xmlWriter.WriteElementString("price", JobHelper.bSubstring(CurrentPrice.ToString("0.00"), 40));
            xmlWriter.WriteElementString("brand", GetBrand());
            BaiduCategoryEntity category = BaiduBP.GetBaiduCategory(C3SysNo);
            xmlWriter.WriteElementString("tags", GetItemTags(category));
            xmlWriter.WriteElementString("services", @"正规发票\全国联保\七天退换货");
            xmlWriter.WriteElementString("image", JobHelper.bSubstring(PicUrl, 256));
            xmlWriter.WriteElementString("store", "新蛋商城");
            xmlWriter.WriteElementString("stock", OnlineQty > 0 ? "0" : "1");
            xmlWriter.WriteElementString("description", JobHelper.CleanInvalidXmlChars(JobHelper.bSubstring(NoHtml(ProductDescLong), 2000)));
            xmlWriter.WriteElementString("city", "全国");
            xmlWriter.WriteElementString("comments", ReviewCount.ToString());
            xmlWriter.WriteElementString("commentsurl", JobHelper.bSubstring(string.Format(
                 ReviewUrl, C3SysNo, HttpUtility.UrlEncode(Brand, System.Text.Encoding.GetEncoding("gb2312")), ProductID), 256));
            xmlWriter.WriteElementString("firstclass", JobHelper.bSubstring(C1Name, 80));
            xmlWriter.WriteElementString("secondclass", JobHelper.bSubstring(C2Name, 80));
            xmlWriter.WriteElementString("thirdclass", JobHelper.bSubstring(C3Name, 80));
            xmlWriter.WriteElementString("score", BaiduDA.GetProductScore(ProductSysNo, AppConfig.CompanyCode).ScoreString);
            xmlWriter.WriteEndElement();
        }

        private string GetBrand()
        {
            string result = string.Empty;
            if (StringHelper.IsNullOrEmpty(ManufacturerName))
            {
                result = BriefName;
            }
            else
            {
                result = ManufacturerName;
            }
            return JobHelper.bSubstring(result, 40);
        }

        private string GetItemTags(BaiduCategoryEntity category)
        {
            string result = string.Empty;
            if (!StringHelper.IsNullOrEmpty(category.CategoryName))
            {
                result = ReplaceSlash(C1Name) + @"\" + ReplaceSlash(C2Name) + @"\" + ReplaceSlash(category.CategoryName);
            }
            while (result.Length > 15)
            {
                int index = result.LastIndexOf('\\');
                if (index > -1)
                {
                    result = result.Substring(0, index);
                }
                else
                {
                    result = JobHelper.bSubstring(result, 15);
                }
            }
            if (StringHelper.IsNullOrEmpty(result))
            {
                result = "新蛋";
            }
            return result;
        }

        private static string ReplaceSlash(string categoryName)
        {
            return categoryName.Replace('/', '\\');
        }

        private static string NoHtml(string content)
        {
            if (!StringHelper.IsNullOrEmpty(content))
            {
                return System.Text.RegularExpressions.Regex.Replace(content, "<[^>]*>", "").Replace("&nbsp;", "");
            }
            else
            {
                return "";
            }
        }
    }
}