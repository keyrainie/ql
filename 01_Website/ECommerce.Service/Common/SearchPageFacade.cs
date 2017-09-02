using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using ECommerce.DataAccess.Common;
using ECommerce.Entity;
using ECommerce.Entity.Common;

namespace ECommerce.Facade
{
    public static class SearchPageFacade
    {
        public static string PageSize = "pageSize";
        public static string PageNumber = "page";
        public static string SortKey = "sort";
        public static string TagKey = "tag";

        #region  搜索分页参数
        //solr小类id
        public static string Enid = "enid";
        public static string SubCategoryID = "subCategoryID";
        public static string Keyword = "keyword";
        public static string WithInKeyword = "withInKeyword";
        //N值
        public static string N = "N";
        public static string QueryFilter = "q";
        public static string Ep = "ep";
        //自定义价格开始
        public static string Pf = "pf";
        //自定义价格结束
        public static string Pt = "pt";
        #endregion

        /// <summary>
        /// 构造QueryString字符串，返回保留原始QueryString中的所有参数，并根据resetKey和resetValue参数来添加或者更新某些QueryString参数值
        /// </summary>
        /// <param name="pageQueryCollection">页面原始的QueryString</param>
        /// <param name="resetKeyAndValues">需要重设或需要添加的QueryString键值对</param>
        /// <returns>QueryString字符串</returns>
        public static string BuildQueryString(NameValueCollection pageQueryCollection, params object[] resetKeyAndValues)
        {
            NameValueCollection validQueryCollection = new NameValueCollection();

            foreach (string key in pageQueryCollection)
            {
                if (pageQueryCollection[key] != null && !validQueryCollection.AllKeys.Any(x => x.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    validQueryCollection.Add(key, pageQueryCollection[key]);
                }
            }

            bool findKey = false;
            string theKey = string.Empty;
            if (validQueryCollection.AllKeys.Length > 0)
            {
                for (int keyIndex = 0; keyIndex < validQueryCollection.AllKeys.Length; keyIndex++)
                {
                    theKey = validQueryCollection.AllKeys[keyIndex];
                    if (resetKeyAndValues != null && resetKeyAndValues.Length > 0)
                    {
                        string resetKey, resetValue;
                        for (int i = 0; i < resetKeyAndValues.Length; i++)
                        {
                            resetKey = resetKeyAndValues[i++].ToString();
                            if (i < resetKeyAndValues.Length)
                            {
                                resetValue = resetKeyAndValues[i].ToString();
                                if (theKey.Equals(resetKey, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    findKey = true;
                                    validQueryCollection.Set(theKey, resetValue);
                                }
                                if (!findKey && !validQueryCollection.AllKeys.Any(x => x.Equals(resetKey, StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    validQueryCollection.Add(resetKey, resetValue);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (resetKeyAndValues != null && resetKeyAndValues.Length > 0)
                {
                    string resetKey, resetValue;
                    for (int i = 0; i < resetKeyAndValues.Length; i++)
                    {
                        resetKey = resetKeyAndValues[i++].ToString();
                        if (i < resetKeyAndValues.Length)
                        {
                            resetValue = resetKeyAndValues[i].ToString();
                            validQueryCollection.Add(resetKey, resetValue);
                        }
                    }
                }
            }

            string returnQueryString = string.Empty;
            foreach (string validKey in validQueryCollection)
            {
                if (!string.IsNullOrWhiteSpace(validQueryCollection[validKey]))
                {
                    returnQueryString += string.Format("&{0}={1}", validKey, validQueryCollection[validKey]);
                }
            }
            returnQueryString = returnQueryString.TrimStart('&');
            if (!string.IsNullOrWhiteSpace(returnQueryString))
            {
                returnQueryString = '?' + returnQueryString;
            }

            return HttpContext.Current.Request.FilePath+ returnQueryString;
        }

    }
}
