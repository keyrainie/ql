using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using IPP.Oversea.CN.ContentMgmt.Baidu.ServiceAdapter;
using IPP.Oversea.CN.ContentMgmt.Baidu.Utility;
using IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess;
using Newegg.Oversea.Framework.Utilities.String;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess
{
    public class TopItemBP
    {
        public List<TopItemEntity> AllItemList { get; set; }

        public List<AdminEntity> AllCategoryList { get; set; }

        public void Process()
        {
            string companyCode = ConfigurationManager.AppSettings["CompanyCode"] ?? "8601";
            AllItemList = TopItemDA.GetAllTopItemList(companyCode);
            AllCategoryList = TopItemDA.GetCategoryList(companyCode);
            ProcessCategory(companyCode);
            ProcessPM(companyCode);
        }

        private void ProcessPM(string companyCode)
        {
            List<TopItemEntity> tempList = new List<TopItemEntity>();
            Dictionary<int, TopItemEntity> dic = new Dictionary<int, TopItemEntity>();
            foreach (TopItemEntity itemEntity in AllItemList)
            {
                if (!dic.ContainsKey(itemEntity.ProductSysNo))
                {
                    dic.Add(itemEntity.ProductSysNo, itemEntity);
                    tempList.Add(itemEntity);
                }
            }
            if (tempList == null || tempList.Count == 0)
            {
                return;
            }
            foreach (var item in tempList.GroupBy(p => p.UserSysNo).ToList())
            {
                if (item.Key == 0)
                {
                    continue;
                }
                JobHelper.SendMail(new MailEntity()
                {
                    To = item.ToList().First().EmailAddress,
                    Body = GetEmailHtmlBody(item.ToList()),
                    Subject = "置顶商品无货通知【PM】"
                });
            }
        }

        private void ProcessCategory(string companyCode)
        {
            foreach (AdminEntity category in AllCategoryList)
            {
                category.EmailAddress = TopItemDA.GetEmailAddressByCategorySysNo(category.CategorySysNo, companyCode);
                List<TopItemEntity> itemList = AllItemList.Where(p => p.CategorySysNo == category.CategorySysNo).ToList();
                if (itemList == null || itemList.Count == 0)
                {
                    continue;
                }
                JobHelper.SendMail(new MailEntity()
                {
                    To = category.EmailAddress,
                    Body = GetEmailHtmlBody(itemList),
                    Subject = string.Format("置顶商品无货通知【{0}】", category.CategoryName),
                    IsBodyHtml = true
                });
            }
        }

        private string GetEmailHtmlBody(List<TopItemEntity> itemList)
        {
            string mailTemplate = string.Empty;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "DataAccess\\Configuration\\Template\\MailTemplate.txt";
            string trTemplate = @"<tr>
              <td>商品编号：</td>
              <td><a href='http://www.newegg.com.cn/Product/@ProductID.htm'>@ProductID</td>
              <td>商品名称：</td>
              <td>@ProductName</td>
              </tr>";
            using (StreamReader sr = new StreamReader(path))
            {
                mailTemplate = sr.ReadToEnd();
            }
            StringBuilder trBuilder = new StringBuilder();
            foreach (TopItemEntity item in itemList)
            {
                string tr = trTemplate.Replace("@ProductID", item.ProductID).Replace("@ProductName", item.ProductName);
                trBuilder.Append(tr);
            }
            return mailTemplate.Replace("@TRList", trBuilder.ToString());
        }

        private static string NoHtml(string content)
        {
            if (!StringHelper.IsNullOrEmpty(content))
            {
                return System.Text.RegularExpressions.Regex.Replace(content, "<[^>]*>", "");
            }
            else
            {
                return "";
            }
        }
    }
}
