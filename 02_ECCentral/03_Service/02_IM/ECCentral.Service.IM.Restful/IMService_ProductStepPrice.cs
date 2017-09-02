using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.IM.Restful.RequestMsg;
using ECCentral.Service.Utility.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    { 
        /// <summary>
        /// 创建商品阶梯价格
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/CreateProductStepPrice", Method = "POST")]
        public int CreateProductStepPrice(ProductStepPriceInfo entity)
        {
            return ProductStepPriceProcessor.CreateProductStepPrice(entity);
        }

        [WebInvoke(UriTemplate = "/Product/GetProductStepPrice", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductStepPrice(ProductStepPriceReq filter)
        {
            int totalCount;
            var dataTable = ProductStepPriceProcessor.GetProductStepPrice(filter.VendorSysNo,
                filter.ProductSysNo,
                filter.PagingInfo.PageIndex,
                filter.PagingInfo.PageSize,
                out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Product/DeleteProductStepPrice", Method = "POST")]
        public int DeleteProductStepPrice(List<int> sysNos)
        {
            return ProductStepPriceProcessor.DeleteProductStepPrice(sysNos);
        }

        [WebInvoke(UriTemplate = "/Product/GetProductStepPricebyProductSysNo", Method = "POST") ]
        public List<ProductStepPriceInfo> GetProductStepPricebyProductSysNo(int ProductSysNo)
        {
          var list= ProductStepPriceProcessor.GetProductStepPricebyProductSysNo(ProductSysNo);
          return list;
        }
    }
}
