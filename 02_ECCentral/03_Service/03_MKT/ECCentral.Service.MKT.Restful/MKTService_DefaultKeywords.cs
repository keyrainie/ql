using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using System.Data;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private DefaultKeywordsAppService _defaultKeywordsAppService = ObjectFactory<DefaultKeywordsAppService>.Instance;

        #region 默认关键字（DefaultKeywordsInfo）
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddDefaultKeywords", Method = "POST")]
        public virtual void AddDefaultKeywords(DefaultKeywordsInfo item)
        {
            _defaultKeywordsAppService.AddDefaultKeywords(item);
        }

        /// <summary>
        /// 编辑默认关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/EditDefaultKeywords", Method = "PUT")]
        public virtual void EditDefaultKeywords(DefaultKeywordsInfo item)
        {
            _defaultKeywordsAppService.EditDefaultKeywords(item);
        }

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/KeywordsInfo/LoadDefaultKeywordsInfo/{id}")]
        [DataTableSerializeOperationBehavior]
        public virtual DefaultKeywordsInfo LoadDefaultKeywordsInfo(string id)
        {
            int sysNo = int.Parse(id);
            return _defaultKeywordsAppService.LoadDefaultKeywordsInfo(sysNo);
        }

        /// <summary>
        /// 查询默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryDefaultKeywords", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryDefaultKeywords(DefaultKeywordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IDefaultKeywordsQueryDA>.Instance.QueryDefaultKeywords(filter, out totalCount);
            //根据PageType获取对应的名称
            string colName = "PageTypeName";
            dataTable.Columns.Add(colName, typeof(String));
            var codeNamePairs = ObjectFactory<PageTypeAppService>.Instance.GetPageType(filter.CompanyCode, filter.ChannelID, ModuleType.DefaultKeywords);
            foreach (DataRow row in dataTable.Rows)
            {
                var found = codeNamePairs.SingleOrDefault(item => item.Code == row["PageType"].ToString());
                if (found != null)
                {
                    row[colName] = found.Name;
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}
