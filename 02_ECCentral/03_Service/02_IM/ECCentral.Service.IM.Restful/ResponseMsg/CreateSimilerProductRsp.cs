using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful.ResponseMsg
{
    public class CreateSimilerProductRsp
    {
        public List<ProductInfo> SuccessProductList { get; set; }

        public List<ErrorProduct> ErrorProductList { get; set; }
    }

    public class ErrorProduct
    {
        public string ProductTitle { get; set; }

        public string ErrorMsg { get; set; }
    }
}
