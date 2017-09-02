using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful.ResponseMsg
{
   public class CategoryTemplateRsp
    {
       public List<CategoryTemplateInfo> CategoryTemplateList { get; set; }

       public List<CategoryTemplatePropertyInfo> CategoryTemplatePropertyInfoList { get; set; }

       public string LastEidtUserName
       {
           get
           {
               if (CategoryTemplateList != null && CategoryTemplateList.Count > 0)
               {
                   
                   return CategoryTemplateList.OrderBy(s => s.LastEditTime).First().User.UserDisplayName;
               }
               else
               {
                   return string.Empty;
               }
           }

       }

       public DateTime? LastEditDate
       {
           get
           {
               if (CategoryTemplateList != null && CategoryTemplateList.Count > 0)
               {

                   return CategoryTemplateList.OrderBy(s => s.LastEditTime).First().LastEditTime;
               }
               else
               {
                   return null;
               }
           }
       }
    }
}
