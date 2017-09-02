using System;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class OtherDomainDataFacade
    {
        IPage Page;
        private RestClient GetRestClient(string domainName)
        {
            return new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, ConstValue.Key_ServiceBaseUrl), Page);
        }

        public OtherDomainDataFacade(IPage page)
        {
            Page = page;
        }
        public void GetProductInfoByProductSysNoList(List<int> sysNoList, Action<List<ProductInfo>> callback)
        {
            RestClient client = GetRestClient(ConstValue.DomainName_IM);
            client.Query<List<ProductInfo>>("/IMService/Product/GetProductInfoListBySysNoList", sysNoList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    callback(args.Result);
                }
            });
        }

    }
}
