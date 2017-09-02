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
        private ThesaurusInfoAppService thesaurusInfoAppService = ObjectFactory<ThesaurusInfoAppService>.Instance;

        #region 同义词（ThesaurusInfo）
        /// <summary>
        /// 查询同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryThesaurusKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryThesaurusKeywords(ThesaurusKeywordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryThesaurusKeywords(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        ///添加同义词
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddThesaurusWords", Method = "POST")]
        public virtual void AddThesaurusWords(ThesaurusInfo item)
        {
            thesaurusInfoAppService.AddThesaurusWords(item);
        }

        /// <summary>
        ///根据编号编辑同义词
        /// </summary>
        /// <param name="item"></param>
        //[WebInvoke(UriTemplate = "/KeywordsInfo/EditThesaurusWords", Method = "PUT")]
        //public virtual void EditThesaurusWords(ThesaurusInfo item)
        //{
        //    thesaurusInfoAppService.EditThesaurusWords(item);
        //}

        /// <summary>
        ///根据编号列表批量更新同义词
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchUpdateThesaurusInfo", Method = "PUT")]
        public virtual void BatchUpdateThesaurusWords(List<ThesaurusInfo> items)
        {
            thesaurusInfoAppService.BatchUpdateThesaurusWords(items);
        }

        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadThesaurusWords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual ThesaurusInfo LoadThesaurusWords(int sysNo)
        {
            return thesaurusInfoAppService.LoadThesaurusWords(sysNo);
        }
        #endregion
    }
}
