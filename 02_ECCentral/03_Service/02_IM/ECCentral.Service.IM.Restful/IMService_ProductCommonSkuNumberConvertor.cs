using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.QueryFilter.IM;
using System.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 根据query得到商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductCommonSkuNumberConvertor/GetCommonSkuNumbersByProductIDs", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetCommonSkuNumbersByProductIDs(List<string> list)
        {
           var dataTable= ObjectFactory<IProductCommonSkuNumberConvertorDA>.Instance.GetCommonSkuNumbersByProductIDs(list);
           return new QueryResult() 
           {
               Data=dataTable,
               TotalCount=0
           };
            
        }
        [WebInvoke(UriTemplate = "/ProductCommonSkuNumberConvertor/GetProductIDsByCommonSkuNumbers", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductIDsByCommonSkuNumbers(List<string> list)
        {
            var dataTable = ObjectFactory<IProductCommonSkuNumberConvertorDA>.Instance.GetProductIDsByCommonSkuNumbers(list);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };

        }
    }
}
