using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private readonly InternetKeywordAppService _internetKeywordService = ObjectFactory<InternetKeywordAppService>.Instance;

        /// <summary>
        /// 创建外网搜索
        /// </summary>
        /// <param name="keywordInfos"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InternetKeyword/CreateKeyword", Method = "POST")]
        public void CreateKeyword(List<InternetKeywordInfo> keywordInfos)
        {
             _internetKeywordService.CreateKeyword(keywordInfos);
        }

        /// <summary>
        /// 修改外网搜索状态
        /// </summary>
        /// <param name="internetKeywords"></param>
        [WebInvoke(UriTemplate = "/InternetKeyword/ModifyOpenAPIStatus", Method = "PUT")]
        public void ModifyKeywordStatus(List<InternetKeywordInfo> internetKeywords)
        {
            _internetKeywordService.ModifyKeywordStatus(internetKeywords);
        }
       

        /// <summary>
        /// 查询外网搜索
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InternetKeyword/QueryKeyword", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryKeyword(InternetKeywordQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IInternetKeywordQueryDA>.Instance.QueryKeyword(msg, out totalCount);
            AddOtherData(ds);
            return new QueryResult
                       {
                Data = ds,
                TotalCount = totalCount
            };
        }

        #region 查询添加其他信息

        private void AddOtherData(DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return;
            AddotherColumns(table);
            var rows = (from e in table.AsEnumerable() select e).ToList();
            rows.ForEach(v =>
                    {
                        var value = v.Field<IsDefaultStatus>("Status");
                        var setValue = value == IsDefaultStatus.Deactive ? IsDefaultStatus.Active : IsDefaultStatus.Deactive;
                        v.SetField("SetStatus", setValue);
              
                    });
        }

        private void AddotherColumns(DataTable product)
        {
            product.Columns.Add("SetStatus").DefaultValue = IsDefaultStatus.Active;
           
        }
        #endregion
    }
}
