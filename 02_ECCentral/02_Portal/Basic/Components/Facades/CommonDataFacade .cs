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
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;

namespace ECCentral.Portal.Basic.Components.Facades
{
    public class CommonDataFacade
    {
        private IPage m_CurrentPage;

        private RestClient GetRestClient(string domainName)
        {
            string baseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl");
            RestClient restClient = new RestClient(baseUrl, m_CurrentPage);
            return restClient;
        }

        public CommonDataFacade(IPage page)
        {
            m_CurrentPage = page;
        }

        /// <summary>
        /// 查询所属渠道
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetWebChannelList(bool needAll, EventHandler<RestClientEventArgs<List<WebChannelVM>>> callback)
        {
            string relativeUrl = "/CommonService/WebChannel/GetAll/" + CPApplication.Current.CompanyCode;

            GetRestClient("Common").Query<List<WebChannel>>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<WebChannelVM> vm = args.Result.Convert<WebChannel, WebChannelVM>();
                    if (needAll)
                    {
                        vm.Insert(0, new WebChannelVM()
                        {
                            ChannelName = ResCommonEnum.Enum_All
                        });
                    }
                    RestClientEventArgs<List<WebChannelVM>> eventArgs = new RestClientEventArgs<List<WebChannelVM>>(vm, m_CurrentPage);
                    callback(obj, eventArgs);
                });
        }

        /// <summary>
        /// 查询所属公司
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetCompanyList(bool needAll, EventHandler<RestClientEventArgs<List<CompanyVM>>> callback)
        {
            string relativeUrl = "/CommonService/Company/GetAll";

            GetRestClient("Common").Query<List<Company>>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<CompanyVM> vm = args.Result.Convert<Company, CompanyVM>();
                    if (needAll)
                    {
                        vm.Insert(0, new CompanyVM()
                        {
                            CompanyName = ResCommonEnum.Enum_All
                        });
                    }
                    RestClientEventArgs<List<CompanyVM>> eventArgs = new RestClientEventArgs<List<CompanyVM>>(vm, m_CurrentPage);
                    callback(obj, eventArgs);
                });
        }

        /// <summary>
        /// 查询配送方式
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetShippingTypeList(bool needAll, EventHandler<RestClientEventArgs<List<ShippingType>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/ShippingType/GetAll/{0}", CPApplication.Current.CompanyCode);
            GetRestClient("Common").Query<List<ShippingType>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<ShippingType> shippingTypeList = args.Result;
                if (needAll)
                {
                    shippingTypeList.Insert(0, new ShippingType()
                    {
                        ShippingTypeName = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<ShippingType>> eventArgs = new RestClientEventArgs<List<ShippingType>>(shippingTypeList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 查询送货 区间（上午 下午）
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetDeliveryTimeRange(bool needAll, EventHandler<RestClientEventArgs<List<DeliveryTimeRange>>> callback)
        {
            string relativeUrl = "/CommonService/DeliveryTimeRange/GetAll";

            GetRestClient("Common").Query<List<DeliveryTimeRange>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<DeliveryTimeRange> deliveryTimeRange = args.Result;
                if (needAll)
                {
                    deliveryTimeRange.Insert(0, new DeliveryTimeRange()
                    {
                        DeliveryTimeRangeName = ResCommonEnum.Enum_All
                    });
                }

                RestClientEventArgs<List<DeliveryTimeRange>> eventArgs = new RestClientEventArgs<List<DeliveryTimeRange>>(deliveryTimeRange, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 获取所有仓库列表
        /// </summary>
        /// <param name="needAll">是否需要添加“--所有--”项</param>
        /// <param name="callback">数据返回回调函数</param>
        public void GetStockList(bool needAll, EventHandler<RestClientEventArgs<List<StockInfo>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/Stock/GetAll/{0}", CPApplication.Current.CompanyCode);

            GetRestClient("Common").Query<List<StockInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<StockInfo> stockList = args.Result;
                if (needAll)
                {
                    stockList.Insert(0, new StockInfo()
                    {
                        StockName = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<StockInfo>> eventArgs = new RestClientEventArgs<List<StockInfo>>(stockList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 获取客服列表，用于简单列表显示，应用场景为下拉菜单
        /// </summary>
        /// <param name="depId">部门Id</param>
        /// <param name="callback">数据返回回调函数</param>
        public void GetUserInfoByDepartmentId(int depId, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            UserInfoQueryFilter filter = new UserInfoQueryFilter();
            filter.DepartmentId = depId;
            filter.Status = BizOperationUserStatus.Active;
            filter.PagingInfo = new PagingInfo
            {
                PageIndex = 0,
                PageSize = 9999
            };
            string relativeUrl = "/CommonService/User/GetUserInfoList";
            GetRestClient(ConstValue.DomainName_Common).Query<List<UserInfo>>(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 获取客服列表，用于简单列表显示，应用场景为下拉菜单
        /// </summary>
        /// <param name="depId">部门Id</param>
        /// <param name="callback">数据返回回调函数</param>
        public void GetAllCS(string companyCode, EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            string relativeUrl = "/CustomerService/CS/GetAllCS";
            GetRestClient("Customer").Query<List<CSInfo>>(relativeUrl, companyCode, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<List<CSInfo>> eventArgs = new RestClientEventArgs<List<CSInfo>>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 取得订单审核人列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetCustomerServiceList(string companyCode, Action<List<UserInfo>> callback)
        {
            string relativeUrl = String.Format("/CommonService/User/GetCSList/{0}", companyCode);
            GetRestClient("Common").Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    if (args.Result != null && callback != null)
                    {
                        callback(args.Result);
                    }
                }
            });
        }

        public void GetReasonCodePathList(List<ReasonCodeEntity> list, EventHandler<RestClientEventArgs<List<ReasonCodeEntity>>> callback)
        {
            string relativeUrl = "/CommonService/ReasonCode/GetReasonCodePathList";
            GetRestClient("Common").Query<List<ReasonCodeEntity>>(relativeUrl, list, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<List<ReasonCodeEntity>> eventArgs = new RestClientEventArgs<List<ReasonCodeEntity>>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void GetReasonCodeBySysNo(int sysNo, EventHandler<RestClientEventArgs<ReasonCodeEntity>> callback)
        {
            string relativeUrl = string.Format("/CommonService/ReasonCode/GetReasonCodeBySysNo/{0}", sysNo);
            GetRestClient("Common").Query<ReasonCodeEntity>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<ReasonCodeEntity> eventArgs = new RestClientEventArgs<ReasonCodeEntity>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void GetReasonCodePath(int sysNo,string companyCode, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = string.Format("/CommonService/ReasonCode/GetReasonCodePath/{0}/{1}", sysNo,companyCode);
            GetRestClient("Common").Query<string>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<string> eventArgs = new RestClientEventArgs<string>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void GetAllEffectiveDepartment(string companyCode, string languageCode, EventHandler<RestClientEventArgs<List<DepartmentInfo>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/Department/GetAllEffectiveDepartment/{0}/{1}", companyCode, languageCode);
            GetRestClient("Common").Query<List<DepartmentInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<List<DepartmentInfo>> eventArgs = new RestClientEventArgs<List<DepartmentInfo>>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void GetAllSystemUser(string companyCode, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            GetRestClient("Common").Query<List<UserInfo>>("/CommonService/User/GetAllSystemUser/" + companyCode, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RestClientEventArgs<List<UserInfo>> eventArgs = new RestClientEventArgs<List<UserInfo>>(args.Result, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        #region 社团

        public void GetSociety(Action<List<KeyValuePair<string, string>>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/GetSociety";
            GetRestClient("Common").Query<List<SocietyInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>(null, ResLinkableDataPicker.ComboBox_PleaseSelect));
                foreach (var item in args.Result)
                {
                    list.Add(new KeyValuePair<string, string>(item.SysNo.ToString(), item.SocietyName));
                }

                callback(list);
            });
        }
        #endregion
    }
}