using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private readonly ProductPayTypeAppService _productPayTypeService = ObjectFactory<ProductPayTypeAppService>.Instance;

        /// <summary>
        /// 查询商品支付方式
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductPayType/QueryProductPayType", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductPayType(ProductPayTypeQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductPayTypeQueryDA>.Instance.QueryProductPayType(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询支付方式列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductPayType/QueryPayTypeList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<PayTypeInfo> GetProductPayTypeList()
        {
            return ObjectFactory<IProductPayTypeQueryDA>.Instance.GetProductPayTypeList();
        }

        /// <summary>
        /// 批量添加支付方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductPayType/BatchCreateProductPayType", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual void BatchCreateProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            _productPayTypeService.BatchCreateProductPayType(productPayTypeInfo);
        }

        /// <summary>
        /// 批量中止支付方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductPayType/BathAbortProductPayType", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual void BathAbortProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            _productPayTypeService.BathAbortProductPayType(productPayTypeInfo.ProductPayTypeIds, productPayTypeInfo.EditUser);
        }
    }
}
