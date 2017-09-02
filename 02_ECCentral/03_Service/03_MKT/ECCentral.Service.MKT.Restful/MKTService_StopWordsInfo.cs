using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private StopWordsInfoAppService stopWordsInfoAppService = ObjectFactory<StopWordsInfoAppService>.Instance;

        #region 阻止词（StopWordsInfo）
        /// <summary>
        ///添加阻止词
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/CreateStopWordsInfo", Method = "POST")]
        public virtual void AddStopWords(StopWordsInfo item)
        {
            stopWordsInfoAppService.AddStopWords(item);
        }

        /// <summary>
        ///根据编号编辑阻止词
        /// </summary>
        /// <param name="item"></param>
        //[WebInvoke(UriTemplate = "/KeywordsInfo/EditStopWords", Method = "PUT")]
        //public virtual void EditStopWords(StopWordsInfo item)
        //{
        //    stopWordsInfoAppService.EditStopWords(item);
        //}

        /// <summary>
        ///根据编号列表批量更新阻止词
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchUpdateStopWords", Method = "PUT")]
        public virtual void BatchUpdateStopWords(List<StopWordsInfo> items)
        {
            stopWordsInfoAppService.BatchUpdateStopWords(items);
        }

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadStopWords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual StopWordsInfo LoadStopWords(int sysNo)
        {
            return stopWordsInfoAppService.LoadStopWords(sysNo);
        }

        /// <summary>
        /// 查询阻止词
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryStopWords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryStopWords(StopWordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryStopWords(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}
