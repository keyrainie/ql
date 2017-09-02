using System.Collections.Generic;

namespace ECCentral.Service.IM.Restful.ResponseMsg
{
    public class ProductCloneRsp
    {
        public List<CloneSuccessProduct> SuccessProductList { get; set; }

        public List<CloneErrorProduct> ErrorProductList { get; set; }
    }

    public class CloneErrorProduct
    {
        public string ProductID { get; set; }

        public string ErrorMsg { get; set; }
    }

    public class CloneSuccessProduct
    {
        public string SourceProductID { get; set; }

        public string TargetProductID { get; set; }
    }
}
