using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
   public class CategoryTemplateInfo:ICompany,ILanguage
    {

       public int? C3SysNo { get; set; }

       public int? TargetType { get { return 0; } }

       public int? TargetSysNo { get; set; }

       public CategoryTemplateType TemplateType { get; set; }

       public string Templates { get; set; }

       public string CategoryTemplateProperty { get; set; }
       public UserInfo User { get; set; }

       public DateTime LastEditTime { get; set; }
       

       #region ILanguage Members

       public string LanguageCode
       {
           get;
           set;
       }

       #endregion

       #region ICompany Members

       public string CompanyCode
       {
           get;
           set;
       }

       #endregion
    }
}
