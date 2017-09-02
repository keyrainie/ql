using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/CustomerExtend/SetCollectionPeriodAndRating", Method = "PUT")]
        public void SetCollectionPeriodAndRating(AccountPeriodInfo entity)
        {
            ObjectFactory<CustomerAppService>.Instance.UpdateAccountPeriodInfo(entity);
        }

        //[WebInvoke(UriTemplate = "/CustomerExtend/AdjustCollectionPeriodAndRating", Method = "PUT")]
        //public void AdjustCollectionPeriodAndRating(AccountPeriodInfo entity)
        //{
        //    return ObjectFactory<CustomerExtendAppService>.Instance.AdjustCollectionPeriodAndRating(entity);
        //}

        //[WebInvoke(UriTemplate = "/CustomerExtend/AdjustCreditLimit", Method = "PUT")]
        //public void AdjustCreditLimit(AccountPeriodInfo entity)
        //{
        //    return ObjectFactory<CustomerExtendAppService>.Instance.AdjustCreditLimit(entity);
        //}
    }
}
