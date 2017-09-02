using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.MKT.Models.Floor;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class FloorFacade
    {
        private readonly RestClient restClient;
        public IPage Page;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public FloorFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetFloorMaster(string sysNo, EventHandler<RestClientEventArgs<FloorMaster>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Floor/GetFloorMaster/{0}", sysNo);
            restClient.Query(relativeUrl, callback);
        }

        public void GetTemplate(EventHandler<RestClientEventArgs<List<FloorTemplate>>> callback)
        {
            string relativeUrl = "/MKTService/Floor/GetFloorTemplateList";
            restClient.Query<List<FloorTemplate>>(relativeUrl, callback);
        }

        public void Save(FloorVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var master = vm.ConvertVM<FloorVM, FloorMaster>();
            string relativeUrl;
            if (master.SysNo > 0)
            {
                relativeUrl = "/MKTService/Floor/UpdateFloorMaster";
                restClient.Update(relativeUrl, master, callback);
            }
            else
            {
                relativeUrl = "/MKTService/Floor/CreateFloorMaster";
                restClient.Create(relativeUrl, master, callback);
            }
        }

        public void QueryFloor(EventHandler<RestClientEventArgs<List<FloorMaster>>> callback)
        {
            string relativeUrl = "/MKTService/Floor/GetAllFloorMasterList";
            restClient.Query(relativeUrl, callback);
        }

        public void QueryFloor(FloorMasterQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/Floor/QueryFloorMaster";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void DeleteFloor(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Floor/DeleteFloor";
            restClient.Delete(relativeUrl, sysNo, callback);
        }

        public void GetFloorSectionList(string floorMasterSysNo, EventHandler<RestClientEventArgs<List<FloorSection>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Floor/GetFloorSectionList/{0}", floorMasterSysNo);
            restClient.Query(relativeUrl, callback);
        }

        public void CreateFloorSection(FloorSectionVM vm, EventHandler<RestClientEventArgs<int>> callback)
        {
            var data = vm.ConvertVM<FloorSectionVM, FloorSection>();
            string relativeUrl = "/MKTService/Floor/CreateFloorSection";
            restClient.Create(relativeUrl, data, callback);
        }

        public void UpdateFloorSection(FloorSectionVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var data = vm.ConvertVM<FloorSectionVM, FloorSection>();
            string relativeUrl = "/MKTService/Floor/UpdateFloorSection";
            restClient.Update(relativeUrl, data, callback);
        }

        public void GetFloorSectionItemList(string floorSectionSysNo, EventHandler<RestClientEventArgs<List<FloorSectionItem>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Floor/GetFloorSectionItemList/{0}", floorSectionSysNo);
            restClient.Query(relativeUrl, callback);
        }

        public void DeleteFloorSection(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Floor/DeleteFloorSection";
            restClient.Delete(relativeUrl, sysNo, callback);
        }

        public void CreateFloorSectionItem(FloorSectionItem sectionItem, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/MKTService/Floor/CreateFloorSectionItem";
            restClient.Create(relativeUrl, sectionItem, callback);
        }

        public void BtnBatchCreateFloorSectionItem(List<FloorSectionItem> sectionItems, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/MKTService/Floor/BatchCreateFloorSectionItem";
            restClient.Create(relativeUrl, sectionItems, callback);
        }

        public void UpdateFloorSectionItem(FloorSectionItem sectionItem, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Floor/UpdateFloorSectionItem";
            restClient.Update(relativeUrl, sectionItem, callback);
        }

        public void DeleteFloorSectionItem(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Floor/DeleteFloorSectionItem";
            restClient.Delete(relativeUrl, sysNo, callback);
        }

        public void QueryPageCode(string companyCode, string channelID, EventHandler<RestClientEventArgs<List<KeyValuePair<string, string>>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Floor/GetFloorAllPageCodeList/{0}/{1}", companyCode, channelID);
            restClient.Query<Dictionary<int, string>>(relativeUrl, (s, args) =>
            {
                var ls = new List<KeyValuePair<string, string>>();
                foreach (var item in args.Result)
                {
                    ls.Add(new KeyValuePair<string, string>(item.Key.ToString(), item.Value.ToString()));
                }
                callback(s, new RestClientEventArgs<List<KeyValuePair<string, string>>>(ls, Page));
            });
        }

        public void QueryBrand(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            //BrandQueryVM model = new BrandQueryVM();
            //model.Status = ValidStatus.Active;
            //model.AuthorizedStatus = AuthorizedStatus.Active;
            int PageSize = int.MaxValue;
            int PageIndex = 0;
            string SortField = "case when Brand.Priority is null then 1 else 0 end,Brand.Priority";
            BrandQueryFilter filter = new BrandQueryFilter();
            //filter.Status = ValidStatus.Active;
            //filter.AuthorizedStatus = AuthorizedStatus.Active;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Brand/QueryBrand";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    //if (!(args == null || args.Result == null || args.Result.Rows == null))
                    //{
                    //    foreach (var item in args.Result.Rows)
                    //    {
                    //        item.IsChecked = false;
                    //    }
                    //}
                    callback(obj, args);
                }
                );
        }


        public void QueryPageID(string pageCode, string companyCode, EventHandler<RestClientEventArgs<List<KeyValuePair<string, string>>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Floor/GetFloorPageID/{0}/{1}", pageCode, companyCode);
            restClient.Query<Dictionary<int, string>>(relativeUrl, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var ls = new List<KeyValuePair<string, string>>();
                foreach (var item in args.Result)
                {
                    ls.Add(new KeyValuePair<string, string>(item.Key.ToString(), item.Value.ToString()));
                }
                callback(s, new RestClientEventArgs<List<KeyValuePair<string, string>>>(ls, Page));
            });
        }
    }
}
