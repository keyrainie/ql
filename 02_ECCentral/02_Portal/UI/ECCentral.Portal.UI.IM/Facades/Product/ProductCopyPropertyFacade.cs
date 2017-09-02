using System;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.IM.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductCopyPropertyFacade
    {
        private readonly RestClient _restClient;

        private const string ProductCopyPropertyRelativeUrl = "/IMService/Product/ProductCopyProperty";

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductCopyPropertyFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public void ProductCopyProperty(ProductCopyPropertyVM data, EventHandler<RestClientEventArgs<ProductCopyPropertyRequestMsg>> callback)
        {
            _restClient.Update(ProductCopyPropertyRelativeUrl, CovertVMtoEntity(data), callback);
        }

        private ProductCopyPropertyRequestMsg CovertVMtoEntity(ProductCopyPropertyVM data)
        {
            var msg = new ProductCopyPropertyRequestMsg
            {
                CanOverrite = data.CanOverrite,
                SourceProductSysNo = data.SourceProductSysNo,
                TargetProductSysNo = data.TargetProductSysNo,
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode,
                OperationUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                }
            };
            return msg;
        }
    }
}
