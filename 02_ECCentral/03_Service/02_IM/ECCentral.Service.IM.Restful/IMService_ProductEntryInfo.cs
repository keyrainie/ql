using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.AppService;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.IDataAccess;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {

        //[WebInvoke(UriTemplate = "/Entry/Create", Method = "POST")]
        //public bool InsertEntryInfo(ProductEntryInfo request)
        //{
        //    return ObjectFactory<ProductEntryInfoAppService>.Instance.InsertEntryInfo(request);
        //}

        [WebInvoke(UriTemplate = "/Entry/Update", Method = "PUT")]
        public bool UpdateEntryInfo(ProductEntryInfo entity)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.UpdateEntryInfo(entity);
        }


        [WebInvoke(UriTemplate = "/Entry/Load/{productSysno}", Method = "GET")]
        public ProductEntryInfo LoadProductEntryInfo(string productSysno)
        {
            int temp = 0;
            if (int.TryParse(productSysno, out temp))
            {
                return ObjectFactory<ProductEntryInfoAppService>.Instance.LoadProductEntryInfo(temp);
            }
            return null;

        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/AuditSucess", Method = "PUT")]
        public bool AuditSucess(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.AuditSucess(info);
        }

        /// <summary>
        /// 审核失败
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/AuditFail", Method = "PUT")]
        public bool AuditFail(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.AuditFail(info);
        }

        /// <summary>
        /// 提交商检
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/ToInspection", Method = "PUT")]
        public bool ToInspection(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.ToInspection(info);
        }

        /// <summary>
        /// 商检通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/InspectionSucess", Method = "PUT")]
        public bool InspectionSucess(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.InspectionSucess(info);
        }

        /// <summary>
        /// 商检失败
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/InspectionFail", Method = "PUT")]
        public bool InspectionFail(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.InspectionFail(info);
        }

        /// <summary>
        /// 提交报关
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/ToCustoms", Method = "PUT")]
        public bool ToCustoms(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.ToCustoms(info);
        }

        /// <summary>
        /// 报关成功
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/CustomsSuccess", Method = "PUT")]
        public bool CustomsSuccess(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.CustomsSuccess(info);
        }

        /// <summary>
        /// 报关失败
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Entry/CustomsFail", Method = "PUT")]
        public bool CustomsFail(ProductEntryInfo info)
        {
            return ObjectFactory<ProductEntryInfoAppService>.Instance.CustomsFail(info);
        }

    }
}
