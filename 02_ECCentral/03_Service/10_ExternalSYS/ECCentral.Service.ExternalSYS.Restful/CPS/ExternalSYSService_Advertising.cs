using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService;
using ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Advertising/AdvertisingQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryAdvertising(AdvertisingQueryFilter query)
        {
         
            int totalCount;
            var dt = ObjectFactory<IAdvertisingQueryDA>.Instance.Query(query, out totalCount);
            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 加载
        /// </summary>
        [WebGet(UriTemplate = "/Advertising/{sysNo}")]
        public virtual AdvertisingInfo LoadAdvertising(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new ECCentral.BizEntity.BizException("请传入有效的编号！");
            }
            return ObjectFactory<AdvertisingAppService>.Instance.Load(id);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Advertising/Create", Method = "POST")]
        public int? CreateAdvertising(AdvertisingInfo info)
        {
            return ObjectFactory<AdvertisingAppService>.Instance.CreateAdvertising(info);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Advertising/Update", Method = "PUT")]
        public void UpdateAdvertising(AdvertisingInfo info)
        {
            ObjectFactory<AdvertisingAppService>.Instance.UpdateAdvertising(info);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Advertising/Delete", Method = "DELETE")]
        public void DeleteAdvertising(int SysNo)
        {
            ObjectFactory<AdvertisingAppService>.Instance.DeleteAdvertising(SysNo);
        }
    }
}
