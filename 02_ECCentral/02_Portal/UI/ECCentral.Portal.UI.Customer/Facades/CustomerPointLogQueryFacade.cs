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
using ECCentral.Portal.UI.Customer.Views;
using ECCentral.Portal.UI.Customer.Models;
using System.Collections.Generic;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerPointLogQueryFacade
    {
        private readonly RestClient restClient;


        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerPointLogQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerPointLogQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }



        public void Query(CustomerPointLogQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CustomerPointLogQueryFilter filter = new CustomerPointLogQueryFilter();
            filter = model.ConvertVM<CustomerPointLogQueryVM, CustomerPointLogQueryFilter>();
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/CustomerService/Point/QueryPointLog";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                if (filter.ResultType == 1)
                {
                    if (args.Result.Rows != null)
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            //在ECC中手动为顾客加积分并选择积分类型为“商品评论送积分”，由于并没有创建一条商品评论，无法得到商品编号
                            //这里通过服务端返回-9999来作特殊处理 at【PoseidonTong 2014-07-30 18:42:42】
                            if (item.PointLogType == (int)AdjustPointType.Remark
                                && item.ProductID != "-9999")
                            {
                                item.Memo = string.Format("{0}{1}", ResCustomerPointLogQuery.GridList_ProductID, item.ProductID);
                            }
                        }
                    }
                }
                callback(obj, args);
            });
        }
    }
}
