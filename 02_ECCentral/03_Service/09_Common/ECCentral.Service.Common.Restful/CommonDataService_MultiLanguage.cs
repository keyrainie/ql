using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Common;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.BizEntity;
//using ECCentral.BizEntity.MultiLanguage;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        /// <summary>
        /// 获取业务对象的多语言数据列表
        /// </summary>
        /// <param name="dataContract"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/MultiLanguage/GetMultiLanguageBizEntityList", Method = "POST")]
        public virtual List<MultiLanguageBizEntity> GetMultiLanguageBizEntityList(MultiLanguageDataContract dataContract)
        {
            List<MultiLanguageBizEntity>  list =ObjectFactory<MultiLanguageAppService>.Instance.GetMultiLanguageBizEntityList(dataContract);
            return list; 
        }

        /// <summary>
        /// 设置指定语言的业务对象数据
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/MultiLanguage/SetMultiLanguageBizEntity", Method = "PUT")]
        public virtual void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            ObjectFactory<MultiLanguageAppService>.Instance.SetMultiLanguageBizEntity(entity);
        }
    }
}
