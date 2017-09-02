using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.SO.Models;
using System.Collections.Generic;
using ECCentral.QueryFilter.SO;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOLogisticsFacade
    {


        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);

        public SOLogisticsFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public SOLogisticsFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void QueryDiffSODelivery(SODeliveryDiffSearchVM queryView,Action<List<SODeliveryDiffSearchDataVM>,int> action )
        {
            SODeliveryDiffFilter filter = queryView == null ? null : EntityConverter<SODeliveryDiffSearchVM, SODeliveryDiffFilter>.Convert(queryView);

            restClient.QueryDynamicData("/SOService/SODeliveryDiff/Query", filter, (sender, e) => {
                if (!e.FaultsHandle())
                {
                    if (e.Result != null && action != null)
                    {
                        List<SODeliveryDiffSearchDataVM> dataVMList = DynamicConverter<SODeliveryDiffSearchDataVM>.ConvertToVMList(e.Result.Rows);
                        action(dataVMList, e.Result.TotalCount);
                    }
                }

            });
        }


        public void MarkDeliveryExp(DeliveryExpMarkEntity entity, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/SOService/DeliveryExp/Mark";

            restClient.Update<object>(relativeUrl, entity, callback);

 
        }


    }
}
