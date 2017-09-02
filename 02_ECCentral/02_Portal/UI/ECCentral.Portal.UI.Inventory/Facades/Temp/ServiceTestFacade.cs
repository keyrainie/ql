using System;
using System.Text;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ServiceTestFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }
        public ServiceTestFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Test(Action<string> callback)
        {
            StringBuilder testResult = new StringBuilder();

            string relativeUrl = "/InventoryService/Inventory/TestInventoryService";


            restClient.Query<Object>(relativeUrl, (obj, args) =>
            {
                if (callback != null)
                {
                    if (args.FaultsHandle())
                    {
                        
                        callback(args.Error.ToString());
                    }
                    if (!args.FaultsHandle())
                    {
                        callback(args.Result.ToString());                        
                    }
                }                
            });            
        }     
    }
}
