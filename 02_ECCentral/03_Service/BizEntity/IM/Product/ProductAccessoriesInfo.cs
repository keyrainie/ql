using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
  public class ProductAccessoriesInfo:ICompany,ILanguage
    {


      
      public int SysNo { get; set; }

      /// <summary>
      /// 查询功能名称 
      /// </summary>
      public string AccessoriesQueryName { get; set; }

      /// <summary>
      /// 背景图片Url
      /// </summary>
      public string BackPictureBigUrl { get; set; }

      /// <summary>
      /// 状态
      /// </summary>
      public ValidStatus Status { get; set; }

      /// <summary>
      /// 是否树形结构
      /// </summary>
      public BooleanEnum IsTreeQuery { get; set; }

      public string CompanyCode
      {
          get;
          set;
      }

      public string LanguageCode
      {
          get;
          set;
      }

      public UserInfo User { get; set; }
    }
}
