using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{

    public partial class MKTService
    {
        private SegmentInfoAppService segmentInfoAppService = ObjectFactory<SegmentInfoAppService>.Instance;

        #region 中文词库

        /// <summary>
        /// 查询中文词库
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QuerySegment", Method = "POST")]//, ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySegment(SegmentQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QuerySegment(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/DeleteSegmentInfos", Method = "PUT")]
        public virtual void DeleteSegmentInfos(List<int> items)
        {
            segmentInfoAppService.DeleteSegmentInfos(items);
        }

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/SetSegmentInfosInvalid", Method = "PUT")]
        public virtual void SetSegmentInfosInvalid(List<SegmentInfo> items)
        {
            segmentInfoAppService.SetSegmentInfosInvalid(items);
        }

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/SetSegmentInfosValid", Method = "PUT")]
        public virtual void SetSegmentInfosValid(List<SegmentInfo> items)
        {
            segmentInfoAppService.SetSegmentInfosValid(items);
        }

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddSegmentInfo", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public virtual void AddSegmentInfo(SegmentInfo info)
        {
            segmentInfoAppService.AddSegmentInfo(info);
        }

        /// <summary>
        /// 更新中文词库
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/UpdateSegmentInfo", Method = "PUT")]
        public virtual void UpdateSegmentInfo(SegmentInfo info)
        {
            segmentInfoAppService.UpdateSegmentInfo(info);
        }

        /// <summary>
        /// 加载分类关键字信息，包括通用关键字和属性关键字列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadSegmentInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual SegmentInfo LoadSegmentInfo(int sysNo)
        {
            return segmentInfoAppService.LoadSegmentInfo(sysNo);
        }

        /// <summary>
        /// 上传批量添加中文词库
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchImportSegment", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public virtual void BatchImportSegment(string uploadFileInfo)
        {
            segmentInfoAppService.BatchImportSegment(uploadFileInfo);
        }
        #endregion
    }
}
