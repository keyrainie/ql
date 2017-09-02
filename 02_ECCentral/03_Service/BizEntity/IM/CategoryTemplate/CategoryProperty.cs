using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.IM
{
    public class CategoryTemplatePropertyInfo
    {
       /// <summary>
       /// 系统编号
       /// </summary>
       public int? SysNo { get; set; }

       /// <summary>
       /// 属性描述
       /// </summary>
       public LanguageContent PropertyDescription { get; set; }
    
    }
}
