using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public class LanguageHelper
    {
        /// <summary>
        /// 获取文字性内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetText(string key,string languageCode)
        {
            return _GetResourceMess("text", key, languageCode);
        }

        private static string _GetResourceMess(string messType, string key, string languageCode)
        { 
            string text = ECommerce.Utility.TextResourceManager.GetText(languageCode + "." + messType, key, false);
            if (string.IsNullOrEmpty(text))
            {
                return key;
            }
            return text;
        }
    }
}
