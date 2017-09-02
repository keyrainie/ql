using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.AppService.Keywords;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful
{

    public partial class MKTService
    {
        private AdvancedKeywordsAppService _advancedKeywordsAppService = ObjectFactory<AdvancedKeywordsAppService>.Instance;

        #region 跳转关键字（AdvancedKeywordsInfo）
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddAdvancedKeywords", Method = "POST")]
        public virtual void AddAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            _advancedKeywordsAppService.AddAdvancedKeywords(item);
        }

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/EditAdvancedKeywords", Method = "PUT")]
        public virtual void EditAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            _advancedKeywordsAppService.EditAdvancedKeywords(item);
        }

        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadAdvancedKeywordsInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual AdvancedKeywordsInfo LoadAdvancedKeywordsInfo(int sysNo)
        {
            return _advancedKeywordsAppService.LoadAdvancedKeywordsInfo(sysNo);
        }

        /// <summary>
        /// 查询跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryAdvancedKeywords", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryAdvancedKeywords(AdvancedKeywordsQueryFilter filter)
        {

            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryAdvancedKeywords(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}
