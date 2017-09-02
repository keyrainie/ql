using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.BatchUpdateItemKeywords.Entities;
using IPP.ContentMgmt.BatchUpdateItemKeywords.DataAccess;
using Newegg.Oversea.Framework.ExceptionBase;
using System.Configuration;
using System.Threading;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.BizProcess
{
    public class BatchUpdateItemKeywordsBP
    {
        #region rules and const
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;
        #endregion

        public static JobContext jobContext = null;
        /// <summary>
        /// 从Queue中得到所有要更新的产品, 并更新之
        /// </summary>
        public static void UpdateKeywords0Queue()
        {
            WriteLog("开始更新Keywords0");
            var productList = BatchUpdateItemKeywordsDA.GetProductIDsFromQueue();
            var index = 0;
            UpdateProductsKeywords0(productList, ref index);
            WriteLog("更新成功!");
        }

        /// <summary>
        /// 更新所有的keywords
        /// </summary>
        public static void UpdateKeywords0()
        {
            //得到所以categroy3
            var categoryList = BatchUpdateItemKeywordsDA.GetAllCategories();
            var index = 0;           

            //遍历所有categroy每次拿出一个categoryId 找到其下所有product 并执行更新方法
            foreach (var catgory in categoryList)
            {
                var productList = BatchUpdateItemKeywordsDA.GetProductByC3SysNo(catgory.C3SysNo);

                WriteLog("正在更新S3SysNo为{0}的Product..", catgory.C3SysNo);
                UpdateProductsKeywords0(productList, ref index);
                WriteLog("更新S3SysNo为{0}的Product 成功!", catgory.C3SysNo);
            }
        }

        /// <summary>
        /// 更新指定产品的keywords
        /// </summary>
        /// <param name="products"></param>
        /// <param name="index"></param>
        private static void UpdateProductsKeywords0(List<ProductKeywordsQueue> products, ref int index)
        {
            //得到所以产品的cageoryId
            var categoryIds = from e in products
                              where e.C3SysNo.HasValue
                              group e by e.C3SysNo into g
                              select g.Key.Value;
            //根据categoryId从数据中得到catgegoryKeywods 并返回一个dcitionary
            var dictCaregroyKeywords = (from e in BatchUpdateItemKeywordsDA.GetCategroyKeywords(categoryIds)
                                        group e by e.C3SysNo).ToDictionary(e => e.Key.Value, e => e.First());

            for (int i = 0; i < products.Count; i++)
            {
                //执行每2000条停5秒
                if (++index % 2000 == 0)
                {
                    WriteLog("更新条数超过2000条,等待中...!");
                    Thread.Sleep(5 * 1000);
                }
                //如果执行失败不中断任务
                try
                {
                    var product = products[i];
                    UpdateProductKeywords0(product, dictCaregroyKeywords);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("更新失败,错误信息为:" +  ex.Message); 
                }
            }
        }
        /// <summary>
        /// 更新指定产品的keywords0
        /// </summary>
        /// <param name="product"></param>
        /// <param name="dictCaregroyKeywords"></param>
        private static void UpdateProductKeywords0(ProductKeywordsQueue product, Dictionary<int, CategoryKeyWordsEntity> dictCaregroyKeywords)
        {

            var keywords0 = string.Empty;
            //如果Product在CategoryKeywords中没有, 更新keywords0为空字符串
            if (dictCaregroyKeywords.ContainsKey(product.C3SysNo.Value))
            {
                var category = dictCaregroyKeywords[product.C3SysNo.Value];
                keywords0 = GetProductKeywords0(category, product);
            }

            BatchUpdateItemKeywordsDA.UpdateKeyWords0ByProductSysNo(product.ProductSysNo, keywords0);
            if (!keywords0.IsNullOrEmpty())
            {
                Console.WriteLine("成功更新SysNo为{0} 的产品 Keywords0 到 '{1}'! ", product.ProductSysNo, keywords0);
            }
        }

        /// <summary>
        ///得到当前产品的keywords
        /// </summary>
        /// <param name="category"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private static string GetProductKeywords0(CategoryKeyWordsEntity category, ProductKeywordsQueue product)
        {

            //因为要不重复, 所以使用HashSet
            var distinctedKeywordsList = new HashSet<string>();
            //如果公共关键字不为空
            if (!category.CommonKeywords.IsNullOrEmpty())
            {
                distinctedKeywordsList.AddRange(category.CommonKeywords.Split(' '));
            }
            //得到属性关键字
            var propertyInfoList = BatchUpdateItemKeywordsDA.GetPropertyInfo(product.ProductSysNo.Value, product.C3SysNo.Value);
            foreach (var propertyInfo in propertyInfoList)
            {
                var keywords = propertyInfo.ManualInput.IsNullOrEmpty() ? propertyInfo.ValueDescription : propertyInfo.ManualInput;
                keywords = (keywords ?? string.Empty).Trim();

                if (!keywords.IsNullOrEmpty())
                {
                    distinctedKeywordsList.AddRange(keywords.Split(' '));
                }
            }

            return  distinctedKeywordsList.Join(" ");
        }

        public static void WriteLog(string content,params object[] args)
        {
            content = string.Format(content, args);
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }

        internal static void UpdateKeywords0ByProductSysNo(int productId)
        {
            ProductKeywordsQueue product = BatchUpdateItemKeywordsDA.GetProduct(productId);
            int index = 0;
            if (product == null)
            {
                WriteLog("没有找到Product SysNo 为{0}的数据", productId);
                return;
            }

            WriteLog("正在更新的Product SysNo为{0}的keywords", productId);
            UpdateProductsKeywords0(new List<ProductKeywordsQueue> { product }, ref index);
        }
    }
}
