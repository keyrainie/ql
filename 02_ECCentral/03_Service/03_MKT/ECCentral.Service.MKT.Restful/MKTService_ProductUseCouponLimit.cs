using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductUseCouponLimit/GetProductUseCouponLimitByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductUseCouponLimitByQuery(ProductUseCouponLimitQueryFilter query)
        {
            int totalCount;
            var dt = ObjectFactory<IProductUseCouponLimitDA>.Instance.GetProductUseCouponLimitByQuery(query, out totalCount);
          
         
            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/ProductUseCouponLimit/CreateProductUseCouponLimit", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            ObjectFactory<ProductUseCouponLimitAppService>.Instance.CreateProductUseCouponLimit(list);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/ProductUseCouponLimit/ModifyProductUseCouponLimit", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void ModifyProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            ObjectFactory<ProductUseCouponLimitAppService>.Instance.ModifyProductUseCouponLimit(list);
        }
    }
}
