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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class ShipTypeFacade
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

        public ShipTypeFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询配送方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryShipTypeList(ShipTypeQueryVM _filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeInfo/QueryShipTypeList";
             var msg = _filter.ConvertVM<ShipTypeQueryVM, ShipTypeQueryFilter>();
             restClient.QueryDynamicData(relativeUrl, msg, callback);
        }
        /// <summary>
        /// 新增配送方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void CreateShipType(ShipTypeInfoVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeInfo/Create";
            var msg = _viewInfo.ConvertVM<ShipTypeInfoVM, ShippingType>();
            if (_viewInfo.ShipTypeEnum.HasValue && (_viewInfo.ShipTypeEnum.Value == ShippingTypeEnum.SelfGetInCity || _viewInfo.ShipTypeEnum.Value == ShippingTypeEnum.SelfGetInStock))
            {
                msg.IsGetBySelf = 1;
            }
            else
            {
                msg.IsGetBySelf = 0;
            }
            restClient.Create(relativeUrl, msg, callback);
        }
        public void LoadShipType(int? sysNo, EventHandler<RestClientEventArgs<ShipTypeInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeInfo/Load/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<ShippingType>(relativeUrl, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        ShipTypeInfoVM _viewModel=null;
                        ShippingType entity=args.Result;
                        if (entity == null)
                        {
                            _viewModel = new ShipTypeInfoVM();
                        }
                        else
                        {
                          _viewModel=entity.Convert<ShippingType, ShipTypeInfoVM>();
                        }
                        callback(obj, new RestClientEventArgs<ShipTypeInfoVM>(_viewModel, restClient.Page));
                    });
            }
        }
        /// <summary>
        /// 更新配送方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateShipType(ShipTypeInfoVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeInfo/Update";
            var msg = _viewInfo.ConvertVM<ShipTypeInfoVM, ShippingType>();

            if (_viewInfo.ShipTypeEnum.HasValue && (_viewInfo.ShipTypeEnum.Value == ShippingTypeEnum.SelfGetInCity || _viewInfo.ShipTypeEnum.Value == ShippingTypeEnum.SelfGetInStock))
            {
                msg.IsGetBySelf = 1;
            }
            else
            {
                msg.IsGetBySelf = 0;
            }
            restClient.Update(relativeUrl, msg, callback);
        }
        //public void LoadWarehouse(string companyCode, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/CommonService/ShipTypeInfo/LoadWarehouse";
        //    restClient.Query(relativeUrl, companyCode, callback);
        //}
        /// <summary>
        /// 查询配送方式-产品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryShipTypeProductList(ShipTypeProductQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            string relativeUrl = "/CommonService/ShipTypeProductInfo/QueryShipTypeProductList";

            var msg = filter.ConvertVM<ShipTypeProductQueryFilterVM, ShipTypeProductQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result,this.Page));
            });
        }
        /// <summary>
        /// 删除配送方式-产品
        /// </summary>
        /// <param name="sysnoList"></param>
        /// <param name="callback"></param>
        public void VoidShipTypeProduct(List<int?> sysnoList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeProductInfo/Void";
            restClient.Delete(relativeUrl,sysnoList,callback);
        }
        /// <summary>
        /// 新增配送方式-产品
        /// </summary>
        /// <param name="ShipTypeInfo"></param>
        /// <param name="callback"></param>
        public void CreateShipTypeProduct(ShipTypeProductInfoVM ShipTypeInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeProductInfo/Create";
            ShipTypeProductInfo msg = ShipTypeInfo.ConvertVM<ShipTypeProductInfoVM, ShipTypeProductInfo>();
            msg.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            restClient.Create(relativeUrl, msg, callback);
        }
        /// <summary>
        /// 查询配送方式-地区（非）
        /// </summary>
        /// <param name="_filter"></param>
        /// <param name="callback"></param>
        public void QueryShipTypeAreaUnList(ShipTypeAreaUnQueryFilterVM _filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaUnInfo/QueryShipTypeAreaUnList";

            var msg = _filter.ConvertVM<ShipTypeAreaUnQueryFilterVM, ShipTypeAreaUnQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }
        /// <summary>
        /// 删除配送方式-地区（非）
        /// </summary>
        /// <param name="sysnoList"></param>
        /// <param name="callback"></param>
        public void VoidShipTypeAreaUn(List<int> sysnoList,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaUnInfo/Void";
            restClient.Delete(relativeUrl, sysnoList,callback);
        }
        /// <summary>
        ///新增配送方式-地区(非)
        ///<summary>
        public void CreateShipTypeAreaUn(ShipTypeAreaUnInfoVM _viewInfo, EventHandler<RestClientEventArgs<ErroDetail>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaUnInfo/Create";
            var msg = _viewInfo.ConvertVM<ShipTypeAreaUnInfoVM, ShipTypeAreaUnInfo>();
            restClient.Create(relativeUrl, msg, callback);
        }
        /// <summary>
        ///查询配送方式-地区-价格(非)
        ///<summary>
        public void QueryShipTypeAreaPriceList(ShipTypeAreaPriceQueryFilterVM _filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaPriceInfo/QueryShipTypeAreaPriceList";

            var msg = _filter.ConvertVM<ShipTypeAreaPriceQueryFilterVM, ShipTypeAreaPriceQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }
        /// <summary>
        /// 新增配送方式-地区-价格（非）
        /// </summary>
        /// <param name="_viewInfo"></param>
        /// <param name="callback"></param>
        public void CreateShipTypeAreaPrice(ShipTypeAreaPriceInfoVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaPriceInfo/Create";
            ShipTypeAreaPriceInfo entity = new ShipTypeAreaPriceInfo();
            //entity.AreaSysNoList = _viewInfo.AreaSysNoList;
            entity = _viewInfo.ConvertVM<ShipTypeAreaPriceInfoVM, ShipTypeAreaPriceInfo>();
            
            restClient.Create(relativeUrl, entity, callback);
        }
        /// <summary
        /// 删除配送方式-地区-价格（非）
        /// </summary>
        /// <param name="sysnoList"></param>
        /// <param name="callback"></param>
        public void VoidShipTypeAreaPrice(List<int> sysnoList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaPriceInfo/Void";
            restClient.Delete(relativeUrl, sysnoList, callback);
        }
        /// <summary>
        /// 更新配送方式-地区-价格（非）
        /// </summary>
        /// <param name="sysno"></param>
        /// <param name="callback"></param>
        public void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfoVM entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ShipTypeAreaPriceInfo/Update";
            var msg = entity.ConvertVM<ShipTypeAreaPriceInfoVM, ShipTypeAreaPriceInfo>();
            restClient.Update(relativeUrl, msg, callback);
        }
    }
}
