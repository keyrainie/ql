using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Nesoft.ECWeb.DataAccess.Category;
using Nesoft.ECWeb.DataAccess.Topic;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Category;
using Nesoft.ECWeb.Entity.Topic;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess;

namespace SitemapConsole
{
    public class SitemapCore
    {
        private static XmlDocument Doc { get; set; }
        private static string Domain { get; set; }
        private static string RouteConfig { get; set; }

        public static void RunSitemap()
        {
            var urls = new List<string>();

            var exePath = System.AppDomain.CurrentDomain.BaseDirectory;

            Logger.WriteLog(string.Format("运行路径:{0}", exePath));

            Doc = new XmlDocument();
            try
            {
                Doc.Load(Path.Combine(exePath, "config.xml"));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message);
                throw ex;
            }
            var node = Doc.SelectNodes("//domain");
            if (node != null) Domain = node[0].InnerText.Trim();
            node = Doc.SelectNodes("//route");
            if (node != null) RouteConfig = node[0].InnerText.Trim();

            //添加route中的静态url配置
            urls.AddRange(BuildStaticUrls());

            //build 商品详情
            urls.AddRange(BuidlProductDetail());
            //build 新闻详情
            urls.AddRange(BuildTopic());
            //build 新闻列表
            urls.AddRange(BuildTopicList());
            //build 一级类别
            urls.AddRange(BuildTabStore());
            //build 二级类别
            urls.AddRange(BuildMidCategory());
            //build 三级类别
            urls.AddRange(BuildSubCategory());
            //品牌详情
            //urls.AddRange(BuildBrands());
            urls.AddRange(BuildBrandProducts());
            //团购详情页
            urls.AddRange(BuildGroupbuyingDetail());

            //save static url
            urls.ForEach(Console.WriteLine);
            SaveToSitemap(urls);
            CopyTo();
#if DEBUG
            Console.ReadKey();
#endif
        }

        private static IEnumerable<string> BuildGroupbuyingDetail()
        {
            var result = new List<string>();

            var urlTemp = Path.Combine(Domain,"GroupBuying/{0}");

            var cmd = DataCommandManager.GetDataCommand("QueryAllGroupbuyingSysNo");
            var dt = cmd.ExecuteDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(string.Format(urlTemp, dr[0].ToString()));
            }

            return result;
        }

        private static IEnumerable<string> BuildBrands()
        {
            var result = new List<string>();

            var urlTemp = Path.Combine(Domain,"brandzone/{0}");

            var cmd = DataCommandManager.GetDataCommand("QueryAllBrandSysNo");
            var dt = cmd.ExecuteDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(string.Format(urlTemp, dr[0].ToString()));
            }

            return result;
        }

        private static IEnumerable<string> BuildBrandProducts()
        {
            var result = new List<string>();

            var urlTemp = Path.Combine(Domain, "BrandProducts/{0}");

            var cmd = DataCommandManager.GetDataCommand("QueryAllSearchProductBrands");
            var dt = cmd.ExecuteDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(string.Format(urlTemp, dr[0].ToString()));
            }

            return result;
        }

        private static IEnumerable<string> BuildTopicList()
        {
            var urlTemp = Path.Combine(Domain, "Topic/TopicList/{0}");
            var result = new List<string>();
            result.Add(string.Format(urlTemp, 1));
            result.Add(string.Format(urlTemp, 1001));
            return result;
        }

        private static IEnumerable<string> BuildMidCategory()
        {
            var urlTemp = Path.Combine(Domain, "MidCategory/{0}");
            var result = new List<string>();

            var cates = CategoryDA.QueryCategories().Where(p => p.CategoryType == CategoryType.Category);

            foreach (CategoryInfo item in cates)
            {
                result.Add(string.Format(urlTemp, item.CategoryID));
            }

            return result;
        }

        private static IEnumerable<string> BuildSubCategory()
        {
            var urlTemp = Path.Combine(Domain, "SubStore/{0}");
            var result = new List<string>();

            var cates = CategoryDA.QueryCategories().Where(p => p.CategoryType == CategoryType.SubCategory);

            foreach (CategoryInfo item in cates)
            {
                result.Add(string.Format(urlTemp, item.CategoryID));
            }

            return result;
        }

        private static IEnumerable<string> BuildTabStore()
        {
            var urlTemp = Path.Combine(Domain, "TabStore/{0}");
            var result = new List<string>();

            var cates = CategoryDA.QueryCategories().Where(p => p.CategoryType == CategoryType.TabStore);

            foreach (CategoryInfo item in cates)
            {
                result.Add(string.Format(urlTemp, item.CategoryID));
            }

            return result;
        }

        private static IEnumerable<string> BuildTopic()
        {
            var urlTemp = Path.Combine(Domain, "Topic/TopicDetail/{0}");
            var result = new List<string>();

            var filter = new NewsQueryFilter
            {
                PageInfo = new PageInfo
                {
                    PageIndex = 1,
                    PageSize = 10000
                }
            };

            var news = TopicDA.QueryNewsInfo(filter);

            foreach (NewsInfo item in news.ResultList)
            {
                result.Add(string.Format(urlTemp, item.SysNo));
            }

            return result;
        }

        private static IEnumerable<string> BuidlProductDetail()
        {
            var urlTemp = Path.Combine(Domain, "Product/Detail/{0}");

            var cmd = DataCommandManager.GetDataCommand("QueryAllProductSysNo");
            var dt = cmd.ExecuteDataTable();

            var result = new List<string>();
            int sysNo;

            foreach (DataRow row in dt.Rows)
            {
                if (int.TryParse(row[0].ToString(), out sysNo))
                {
                    result.Add(string.Format(urlTemp, sysNo));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据Route.config中的route配置读取静态的url
        /// </summary>
        /// <returns></returns>
        private static List<string> BuildStaticUrls()
        {
            var result = new List<string>();
            var doc = new XmlDocument();
            doc.Load(RouteConfig);
            var staticUrlNodes = doc.SelectNodes("//map/route");
            if (staticUrlNodes != null)
            {
                foreach (XmlNode node in staticUrlNodes)
                {
                    string url = "";
                    string needssl = "";
                    if (node.Attributes != null)
                    {
                        url = node.Attributes["url"].Value;
                        if (node.Attributes["needssl"] != null)
                        {
                            needssl = node.Attributes["needssl"].Value;
                        }
                    }
                    var realUrl = Path.Combine(Domain, url);
                    if (!url.Contains("{") && !Regex.IsMatch(url,"(?<=\\/?)Pay\\/", RegexOptions.IgnoreCase)
  && !String.Equals(needssl, "1", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.Add(realUrl);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获得config中sitemap的保存位置
        /// </summary>
        /// <returns></returns>
        private static string GetSavePath()
        {
            var nodes = Doc.SelectNodes("//path/value");
            if (nodes != null && nodes.Count > 0)
            {
                return nodes[0].InnerText;
            }
            return "Sitemap.xml";
        }
        /// <summary>
        /// 将生成的sitemap 拷贝到其他目录
        /// </summary>
        private static void CopyTo()
        {
            var result = new List<string>();
            var nodes = Doc.SelectNodes("//copyTo/value");
            if (nodes != null)
            {
                result.AddRange(from XmlNode node in nodes select node.InnerText);
            }

            result.ForEach(p =>
            {
                Console.WriteLine("将生成的sitemap 拷贝至 {0}", p);
                File.Copy(GetSavePath(), p, true);
            });
        }
        /// <summary>
        /// 保存生成的sitemap
        /// </summary>
        /// <param name="urls"></param>
        private static void SaveToSitemap(List<string> urls)
        {
            var doc = new XmlDocument();

            var urlset = doc.CreateElement("urlset");
            urlset.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            doc.AppendChild(urlset);

            foreach (var url in urls)
            {
                var urlNode = doc.CreateElement("url");
                var loc = doc.CreateElement("loc");
                loc.InnerText = url;
                urlNode.AppendChild(loc);

                var lastmod = doc.CreateElement("lastmod");
                lastmod.InnerText = DateTime.Now.ToString();
                urlNode.AppendChild(lastmod);

                var changefreq = doc.CreateElement("changefreq");
                changefreq.InnerText = "Daily";
                urlNode.AppendChild(changefreq);

                urlset.AppendChild(urlNode);
            }
            Console.WriteLine("将sitemap保存至{0}", GetSavePath());
            doc.Save(GetSavePath());
        }

    }
}
