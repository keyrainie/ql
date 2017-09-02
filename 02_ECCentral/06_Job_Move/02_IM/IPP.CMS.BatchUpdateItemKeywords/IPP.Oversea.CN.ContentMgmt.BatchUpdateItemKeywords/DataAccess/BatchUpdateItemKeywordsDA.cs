using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.BatchUpdateItemKeywords.Entities;
using Newegg.Oversea.Framework.DataAccess;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using System.Data;
using System.Xml.Linq;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.DataAccess
{
    public class BatchUpdateItemKeywordsDA
    {

        /// <summary>
        /// 更新keywords0
        /// </summary>
        /// <returns></returns>
        public static int UpdateKeyWords0ByProductSysNo(int? ProductSysNo,string Keywords0)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateKeyWords0ByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", ProductSysNo);
            cmd.SetParameterValue("@Keywords0", Keywords0);
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            return cmd.ExecuteNonQuery();
        }

        internal static List<ProductKeywordsQueue> GetProductIDsFromQueue()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductIDsFromQueue");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            return cmd.ExecuteEntityList<ProductKeywordsQueue>();
        }

        internal static List<CategoryKeyWordsEntity> GetCategroyKeywords(IEnumerable<int> categoryIds)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategroyKeywords");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            cmd.SetParameterValue("@C3SysNos", categoryIds.Join(", "));
            return cmd.ExecuteEntityList<CategoryKeyWordsEntity>();
        }

        internal static List<PropertyInfo> GetPropertyInfo(int productSysNo, int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPropertyInfo");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@productSysNo", productSysNo);
            return cmd.ExecuteEntityList<PropertyInfo>();
        }

        internal static List<ProductKeywordsQueue> GetAllCategories()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCategories");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            return cmd.ExecuteEntityList<ProductKeywordsQueue>();
        }

        internal static List<ProductKeywordsQueue> GetProductByC3SysNo(int? categoryId)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductByC3SysNo");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            cmd.SetParameterValue("@CategoryId", categoryId);
            return cmd.ExecuteEntityList<ProductKeywordsQueue>();
        }

        internal static ProductKeywordsQueue GetProduct(int productId)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProduct");
            cmd.SetParameterValue("@CompanyCode", AppConfig.CompanyCode);
            cmd.SetParameterValue("@productSysNo", productId);
            return cmd.ExecuteEntity<ProductKeywordsQueue>();
        }
    }

    public static class Extensions
    {
        public static string Join<T>(this IEnumerable<T> value, string separator)
        {
            return string.Join(separator, value.Select(e => e.ToString()).ToArray());
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> value)
        {
            return new HashSet<T>(value);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static void AddRange<T>(this HashSet<T> value, IEnumerable<T> collection)
        {
            foreach (var li in collection)
            {
                value.Add(li);
            }
        }
    }
}
