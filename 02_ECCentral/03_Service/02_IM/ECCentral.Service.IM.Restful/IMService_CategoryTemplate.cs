using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 根据C3SysNo得到类别模板信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryTemplate/GetCategoryTemplateInfoByC3SysNo", Method = "POST")]
        public virtual CategoryTemplateRsp GetCategoryTemplateInfoByC3SysNo(int? C3SysNo)
        {
            return new CategoryTemplateRsp() 
            {
                CategoryTemplatePropertyInfoList = ObjectFactory<ICategoryTemplateQueryDA>.Instance.GetCategoryPropertyListByC3SysNo(C3SysNo),
                CategoryTemplateList = ObjectFactory<ICategoryTemplateQueryDA>.Instance.GetCategoryTemplateListByC3SysNo(C3SysNo)
            };
        }
        /// <summary>
        /// 保存类别模板
        /// </summary>
        /// <param name="info"></param>
         [WebInvoke(UriTemplate = "/CategoryTemplate/SaveCategoryTemplate", Method = "POST")]
        public void SaveCategoryTemplate(List<CategoryTemplateInfo> list) 
        {
            ObjectFactory<CategoryTemplateAppService>.Instance.SaveCategoryTemplate(list);
        }
     }
}
