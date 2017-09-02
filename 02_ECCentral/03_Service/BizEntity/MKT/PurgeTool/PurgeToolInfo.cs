using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
   public class PurgeToolInfo:ICompany,ILanguage
    {
       /// <summary>
       /// URL
       /// </summary>
       public string Url { get; set; }
       /// <summary>
       /// 优先级
       /// </summary>
       public int Priority { get; set; }

       /// <summary>
       /// 清除时间
       /// </summary>
       public DateTime? ClearDate { get; set; }

       public string LanguageCode
       {
           get;
           set;
       }

       public string CompanyCode
       {
           get;
           set;
       }
       public UserInfo User { get; set; }
    }
}
