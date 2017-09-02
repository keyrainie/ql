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

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 得到所有尺寸
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ImageSize/GetAllImageSize", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAllImageSize(ImageSizeQueryFilter query)
        {
         
            int totalCount;
            var dataTable = ObjectFactory<IImageSizeDA>.Instance.GetAllImageSize(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 创建新尺寸
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ImageSize/CreateImageSize", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateImageSize(ImageSizeInfo info)
        {
            ObjectFactory<ImageSizeAppService>.Instance.CreateImageSize(info);
        }

        /// <summary>
        /// 删除时更新标识位 逻辑删除
        /// </summary>
        /// <param name="SysNo"></param>
        [WebInvoke(UriTemplate = "/ImageSize/DeleteImageSize", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteImageSize(int SysNo)
        {
            ObjectFactory<ImageSizeAppService>.Instance.UpdateImageSizeFlag(SysNo);
        }
    }
}
