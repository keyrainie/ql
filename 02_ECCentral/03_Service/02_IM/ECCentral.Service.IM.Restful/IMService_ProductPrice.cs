using System.ServiceModel.Web;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 更新商品价格信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductPriceInfo", Method = "PUT")]
        public void UpdateProductPriceInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceAppService>.Instance.UpdateProductPriceInfo(productInfo);
        }

        /// <summary>
        /// 商品价格提交审核
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/AuditRequestProductPrice", Method = "PUT")]
        public void AuditRequestProductPrice(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceAppService>.Instance.AuditRequestProductPrice(productInfo);
        }

        /// <summary>
        /// 撤销审核商品价格
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/CancelAuditProductPriceRequest", Method = "PUT")]
        public void CancelAuditProductPriceRequest(ProductInfo productInfo)
        {
            ObjectFactory<ProductPriceAppService>.Instance.CancelAuditProductPriceRequest(productInfo);
        }
    }
}
