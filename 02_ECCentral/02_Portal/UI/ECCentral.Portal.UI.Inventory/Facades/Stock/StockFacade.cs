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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class StockFacade
    {

        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
        IPage Page;
        public StockFacade(IPage page)
        {
            Page = page;
            restClient = new RestClient(serviceBaseUrl, page);
        }


        /// <summary>
        /// 创建渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public void CreateStock(StockInfoVM vm, Action callback)
        {
            restClient.Query<StockInfo>("/InventoryService/Stock/Create", vm.ConvertVM<StockInfoVM, StockInfo>(), (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    if (args.Result != null && args.Result.SysNo > 0)
                    {
                        vm.SysNo = args.Result.SysNo;
                        this.Page.Context.Window.Alert("渠道仓库信息创建成功", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    }
                    else
                    {
                        Page.Context.Window.Alert("渠道仓库信息添加失败", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    }
                }
                callback();
            });
        }

        /// <summary>
        /// 更新渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public void UpdateStock(StockInfoVM vm, Action callback)
        {
            restClient.Query<StockInfo>("/InventoryService/Stock/Update", vm.ConvertVM<StockInfoVM, StockInfo>(), (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Page.Context.Window.Alert("渠道仓库信息修改成功", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                }
                callback();
            });
        }

        /// <summary>
        /// 获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public void GetStockInfo(int stockSysNo, Action<StockInfoVM> callback)
        {
            restClient.Query<StockInfo>(String.Format("/InventoryService/Stock/Get/{0}", stockSysNo), (obj, args) =>
            {
                StockInfoVM vm = null;
                if (!args.FaultsHandle() && callback != null)
                {
                    if (args.Result != null)
                    {
                        vm = args.Result.Convert<StockInfo, StockInfoVM>();
                    }
                }
                callback(vm);
            });
        }
    }
}
