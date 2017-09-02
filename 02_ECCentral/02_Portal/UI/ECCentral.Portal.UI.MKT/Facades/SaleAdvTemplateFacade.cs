using System;
using System.Linq;
using System.Collections.Generic;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class SaleAdvTemplateFacade
    {
        private readonly RestClient restClient;
        private readonly IPage viewPage;

        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public SaleAdvTemplateFacade(IPage page)
        {
            this.viewPage = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetActiveCodeNames(string companyCode, string channelID, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/GetActiveCodeNames/" + CPApplication.Current.CompanyCode + "/" + channelID;
            restClient.Query(relativeUrl, callback);

        }

        public void Query(SaleAdvQueryVM vm, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SaleAdvTemplateQueryFilter filter = vm.ConvertVM<SaleAdvQueryVM, SaleAdvTemplateQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                SortBy = sortField
            };
            string relativeUrl = "/MKTService/SaleAdvTemplate/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<SaleAdvertisementVM>> callback)
        {
            string relativeUrl = string.Format("/MKTService/SaleAdvTemplate/{0}", sysNo);
            restClient.Query<SaleAdvertisement>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = args.Result.Convert<SaleAdvertisement, SaleAdvertisementVM>();
                vm.Name = args.Result.Name.Content;

                vm.IsTableType = args.Result.Type == ShowType.Table;
                vm.IsImageTextType = args.Result.Type == ShowType.ImageText;
                if (vm.Items != null)
                {
                    vm.Items.ForEach(p =>
                    {
                        p.Groups = vm.Groups;
                    });
                }
                RestClientEventArgs<SaleAdvertisementVM> e = new RestClientEventArgs<SaleAdvertisementVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void Create(SaleAdvertisementVM vm, EventHandler<RestClientEventArgs<SaleAdvertisementVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/Create";
            var msg = vm.ConvertVM<SaleAdvertisementVM, SaleAdvertisement>();
            msg.Name.Content = vm.Name;
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<SaleAdvertisement>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vm.SysNo = args.Result.SysNo;
                RestClientEventArgs<SaleAdvertisementVM> e = new RestClientEventArgs<SaleAdvertisementVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void Update(SaleAdvertisementVM vm, EventHandler<RestClientEventArgs<SaleAdvertisementVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/Update";
            var msg = vm.ConvertVM<SaleAdvertisementVM, SaleAdvertisement>();
            msg.Name.Content = vm.Name;
            restClient.Update<SaleAdvertisement>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                RestClientEventArgs<SaleAdvertisementVM> e = new RestClientEventArgs<SaleAdvertisementVM>(vm, this.viewPage);

                callback(obj, e);
            });
        }

        public void Lock(int sysNo, EventHandler<RestClientEventArgs<SaleAdvertisement>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/Lock";

            restClient.Update<SaleAdvertisement>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void UnLock(int sysNo, EventHandler<RestClientEventArgs<SaleAdvertisement>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/UnLock";

            restClient.Update<SaleAdvertisement>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CreateSaleAdvGroup(SaleAdvGroupVM vm, EventHandler<RestClientEventArgs<SaleAdvGroupVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/CreateSaleAdvGroup";
            var msg = vm.ConvertVM<SaleAdvGroupVM, SaleAdvertisementGroup>();
            restClient.Create<SaleAdvertisementGroup>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vm = args.Result.Convert<SaleAdvertisementGroup, SaleAdvGroupVM>();
                RestClientEventArgs<SaleAdvGroupVM> e = new RestClientEventArgs<SaleAdvGroupVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void UpdateSaleAdvGroup(SaleAdvGroupVM vm, EventHandler<RestClientEventArgs<SaleAdvGroupVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/UpdateSaleAdvGroup";
            var msg = vm.ConvertVM<SaleAdvGroupVM, SaleAdvertisementGroup>();

            restClient.Update<SaleAdvertisementGroup>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vm = args.Result.Convert<SaleAdvertisementGroup, SaleAdvGroupVM>();
                RestClientEventArgs<SaleAdvGroupVM> e = new RestClientEventArgs<SaleAdvGroupVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void DeleteSaleAdvGroup(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/DeleteSaleAdvGroup";

            restClient.Delete(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CreateSaleAdvItem(SaleAdvItemVM vm, EventHandler<RestClientEventArgs<SaleAdvItemVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/CreateSaleAdvItem";
            var msg = vm.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>();
            restClient.Create<SaleAdvertisementItem>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vm.SysNo = args.Result.SysNo;
                RestClientEventArgs<SaleAdvItemVM> e = new RestClientEventArgs<SaleAdvItemVM>(vm, this.viewPage);

                callback(obj, e);
            });
        }

        public void BatchCreateSaleAdvItem(SaleAdvItemVM vm, EventHandler<RestClientEventArgs<BatchResultRsp>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/BatchCreateSaleAdvItem";
            var msg = vm.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>();
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdateSaleAdvItem(SaleAdvItemVM vm, EventHandler<RestClientEventArgs<SaleAdvItemVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/UpdateSaleAdvItem";
            var msg = vm.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>();
            restClient.Update<SaleAdvertisementItem>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                RestClientEventArgs<SaleAdvItemVM> e = new RestClientEventArgs<SaleAdvItemVM>(vm, this.viewPage);

                callback(obj, e);
            });
        }

        public void BatchUpdateSaleAdvItem(List<SaleAdvItemVM> vmList, EventHandler<RestClientEventArgs<List<SaleAdvertisementItem>>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/BatchUpdateSaleAdvItem";
            List<SaleAdvertisementItem> msg = new List<SaleAdvertisementItem>();
            vmList.ForEach(p =>
            {
                msg.Add(p.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>());
            });

            restClient.Update<List<SaleAdvertisementItem>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void UpdateSaleAdvItemStatus(SaleAdvItemVM vm, EventHandler<RestClientEventArgs<SaleAdvItemVM>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/UpdateSaleAdvItemStatus";
            var msg = vm.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>();
            restClient.Update<SaleAdvertisementItem>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                RestClientEventArgs<SaleAdvItemVM> e = new RestClientEventArgs<SaleAdvItemVM>(vm, this.viewPage);

                callback(obj, e);
            });
        }

        public void BatchUpdateSaleAdvItemStatus(List<SaleAdvItemVM> vmList, EventHandler<RestClientEventArgs<List<SaleAdvertisementItem>>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/BatchUpdateSaleAdvItemStatus";
            List<SaleAdvertisementItem> msg = new List<SaleAdvertisementItem>();
            vmList.ForEach(p =>
            {
                msg.Add(p.ConvertVM<SaleAdvItemVM, SaleAdvertisementItem>());
            });

            restClient.Update<List<SaleAdvertisementItem>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void DeleteSaleAdvItem(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/DeleteSaleAdvItem";

            restClient.Delete(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void BatchDeleteSaleAdvItem(List<SaleAdvItemVM> vmList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/SaleAdvTemplate/BatchDeleteSaleAdvItem";
            List<int> msg = new List<int>();
            vmList.ForEach(p =>
            {
                msg.Add(p.SysNo.Value);
            });

            restClient.Delete(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
    }
}
