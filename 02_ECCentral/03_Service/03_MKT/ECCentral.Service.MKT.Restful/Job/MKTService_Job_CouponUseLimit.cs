using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.Restful
{
    /// <summary>
    /// MKT Job所调用的服务接口
    /// </summary>
    public partial class MKTService
    {
        #region Private
        private ProductUseCouponLimitAppService _service =
            ObjectFactory<ProductUseCouponLimitAppService>.Instance;
        #endregion

        #region Method
        [WebInvoke(UriTemplate = "/CouponUseLimit/GetLimitProductList", Method = "POST")]
        public virtual List<ProductJobLimitProductInfo> GetLimitProductList(string datacommandname)
        {
            return _service.GetLimitProductList(datacommandname);
        }

        [WebInvoke(UriTemplate = "/CouponUseLimit/DeleteLimitProductByProductSysNo", Method = "PUT")]
        public virtual String DeleteLimitProductByProductSysNo(List<ProductJobLimitProductInfo> Products)
        {
            return _service.DeleteProductUseCouponLimit(Products);
        }

        //[WebInvoke(UriTemplate = "/CouponUseLimit/GetLimitProductByProductSysNo", Method = "GET")]
        //public virtual ProductJobLimitProductInfo GetLimitProductByProductSysNo(int productSysNo)
        //{
        //    return _service.GetLimitProductByProductSysNo(productSysNo);
        //}

        //[WebInvoke(UriTemplate = "/CouponUseLimit/GetLimitProductByProductSysNo", Method = "POST")]
        //public virtual void CreateLimitProduct(ProductJobLimitProductInfo entity)
        //{
        //    _service.CreateLimitProduct(entity);
        //}
        #endregion
    }
}
