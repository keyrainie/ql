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
using System.Collections.Generic;
using ECCentral.Portal.UI.Customer.Views;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.BizEntity.Customer.Society;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryCustomer(CustomerQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void GetWebChannels(EventHandler<RestClientEventArgs<List<KeyValuePair<string, string>>>> callback)
        {
            string relativeUrl = "/CommonService/WebChannel/GetAll/" + CPApplication.Current.CompanyCode;

            restClient.Query<List<WebChannel>>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    list.Add(new KeyValuePair<string, string>(string.Empty, ResCommonEnum.Enum_All));
                    foreach (var item in args.Result)
                    {
                        list.Add(new KeyValuePair<string, string>(item.ChannelID, item.ChannelName));
                    }

                    RestClientEventArgs<List<KeyValuePair<string, string>>> eventArgs = new RestClientEventArgs<List<KeyValuePair<string, string>>>(list, restClient.Page);
                    callback(obj, eventArgs);
                });
        }

        /// <summary>
        /// 查询恶意用户操作历史
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void QueryMaliceCustomer(int customerSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CustomerQueryFilter filter = new CustomerQueryFilter();
            filter.PagingInfo = null;

            string relativeUrl = "/CustomerService/Customer/QueryMaliceCustomerLog";
            restClient.QueryDynamicData(relativeUrl, customerSysNo, callback);
        }

        /// <summary>
        /// 更改用户权限
        /// </summary>
        /// <param name="CustomerSysNo"></param>
        /// <param name="AvtarImageStatus"></param>
        public void UpdateCustomerRights(CustomerRightMaintainView vm, EventHandler<RestClientEventArgs<List<CustomerRight>>> callback)
        {
            CustomerRightReq msg = new CustomerRightReq();
            msg.CustomerSysNo = vm.CustomerSysNo;
            vm.RightList.ForEach(item =>
            {
                if (item.ItemChecked)
                    msg.RightList.Add(item.ConvertVM<CustomerRightVM, CustomerRight>());
            });
            string relativeUrl = "/CustomerService/Customer/UpdateCustomerRights";
            restClient.Update<List<CustomerRight>>(relativeUrl, msg, callback);
        }

        /// <summary>
        /// 添加用户权限
        /// </summary>
        /// <param name="CustomerSysNo"></param>
        /// <param name="AvtarImageStatus"></param>
        //public void CreateCustomerRight(CustomerRight right, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/CustomerService/Customer/CreateCustomerRight";
        //    restClient.Create(relativeUrl, right, callback);
        //}

        /// 获取用户所有的权限
        /// </summary>
        /// <param name="right"></param>
        public void LoadCustomerRight(int customerSysNo, EventHandler<RestClientEventArgs<List<CustomerRight>>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/LoadAllCustomerRight";
            restClient.Query<List<CustomerRight>>(relativeUrl, customerSysNo, callback);
        }

        /// <summary>
        /// 获取第三方账号信息
        /// </summary>
        /// <param name="customerSysNoList"></param>
        public void GetThirdPartyUserInfo(List<int?> customerSysNoList,EventHandler<RestClientEventArgs<List<ThirdPartyUser>>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/GetThirdPartyUserInfo";
            restClient.Query<List<ThirdPartyUser>>(relativeUrl, customerSysNoList, callback);
        }

        /// <summary>
        /// 获取客户密保问题
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="callback"></param>
        public void GetSecurityQuestion(int customerSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/GetSecurityQuestion";
            restClient.QueryDynamicData(relativeUrl, customerSysNo, callback);
        }
    }
}
