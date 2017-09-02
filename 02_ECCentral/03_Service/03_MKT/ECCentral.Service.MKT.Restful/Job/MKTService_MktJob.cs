using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/MktJob/GetCommentProductListSysNo", Method = "GET")]
        public List<int> GetCommentProductListSysNo()
        {
            return new List<int>() { 1, 2, 3, 4, 5, 6, 8 };
        }

        [WebInvoke(UriTemplate = "/MktJob/UpdateProductReview", Method = "PUT")]
        public void UpdateProductReview(string ProductSysNoAndEditUser)
        {
            //拆分系统编号与编辑用户
            string[] split = ProductSysNoAndEditUser.Split('*');
        }
    }
}
