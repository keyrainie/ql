using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.RMA;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class OtherDomainQueryFacade
    {
        private IPage m_CurrentPage;

        private readonly RestClient Common_restClient;

        private RestClient GetRestClient(string domainName)
        {
            string baseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl");
            RestClient restClient = new RestClient(baseUrl, m_CurrentPage);
            return restClient;
        }

        /// <summary>
        /// CommonServier服务基地址
        /// </summary>
        private string CommonServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public OtherDomainQueryFacade(IPage page)
        {
            m_CurrentPage = page;
            Common_restClient = new RestClient(CommonServiceBaseUrl, page);
        }

        public OtherDomainQueryFacade():this(CPApplication.Current.CurrentPage)
        {
        
        }

        #region Customer

        /// <summary>
        /// 获取用户配送地区列表
        /// </summary>
        /// <param name="customerSysNo">用户Id</param>
        /// <param name="callback">回调函数</param>
        public void QueryCustomerShippingAddress(int customerSysNo, EventHandler<RestClientEventArgs<List<ShippingAddressInfo>>> callback)
        {
            GetRestClient(ConstValue.DomainName_Customer).Query<List<ShippingAddressInfo>>(string.Format("/CustomerService/ShippingAddress/Query/{0}",customerSysNo), callback);
        }

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCustomerShippingAddress(ShippingAddressInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            GetRestClient(ConstValue.DomainName_Customer).Update("/CustomerService/ShippingAddress/Update", info, callback);
        }

        #endregion

        #region RMA
        
        /// <summary>
        /// 获取订单相关RMA
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryRMARequest(RMARequestQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            GetRestClient(ConstValue.DomainName_RMA).QueryDynamicData("/RMAService/Request/Query", query, callback);
        }

        /// <summary>
        /// 取得退款原因列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetRefundReaons(Action<List<CodeNamePair>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetRefundReaons";
            GetRestClient(ConstValue.DomainName_RMA).Query<List<CodeNamePair>>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<CodeNamePair> reasonList = args.Result ?? new List<CodeNamePair>();

                    reasonList.Insert(0, new CodeNamePair()
                    {
                        Name = ResCommonEnum.Enum_Select
                    });
                    if (callback != null)
                    {
                        callback(reasonList);
                    }
                }
            });
        }

        #endregion

        #region Invoice
        
        /// <summary>
        /// 取得订单的
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void GetSOIncomeBySOSysNo(int soSysNo, Action<ECCentral.BizEntity.Invoice.SOIncomeInfo> callback)
        {
            string url = String.Format("/InvoiceService/SOIncome/GetValid/{0}", soSysNo);

            GetRestClient(ConstValue.DomainName_Invoice).Query<ECCentral.BizEntity.Invoice.SOIncomeInfo>(url, (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    callback(args.Result);
                }
            });
        }
        #endregion

        #region MKT

        public void GetGiftByMasterProducts(DateTime orderDate, List<int> prodcutSysNoList, Action<dynamic> callback)
        {
            string url = "MKTService/SaleGift/GetGiftItemByMasterProducts";
            ECCentral.Service.MKT.Restful.RequestMsg.GetGiftItemByMasterProductsReq request = new Service.MKT.Restful.RequestMsg.GetGiftItemByMasterProductsReq
            {
                BeginDateTime = orderDate.AddMonths(1),
                EndDateTime = orderDate.AddMonths(-1),
                MasterProductSysNoList = prodcutSysNoList
            };
            GetRestClient(ConstValue.DomainName_Invoice).QueryDynamicData(url, request, (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    callback(args.Result);
                }
            });
        }

        #endregion

        #region IM

        /// <summary>
        /// 获取订单相关RMA
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryProductRequest(ProductQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            query.CompanyCode = CPApplication.Current.CompanyCode;
            GetRestClient(ConstValue.DomainName_IM).QueryDynamicData("/IMService/Product/QueryProduct", query, callback);
        }

        public void QueryCategoryC1ByProductID(string productID, EventHandler<RestClientEventArgs<ECCentral.BizEntity.IM.CategoryInfo>> callback)
        {
            GetRestClient(ConstValue.DomainName_IM).Query<ECCentral.BizEntity.IM.CategoryInfo>("/IMService/Category/GetProductC1CategoryDomain/" + productID, callback);
        }

        #endregion

        #region Common_Service

        /// <summary>
        /// 取得投递员列表，应用场景是绑定下拉列表框
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetFreightManList(bool needAll, Action<List<UserInfo>> callback)
        {
            string relativeUrl = string.Format("/CommonService/User/FreightMan/{0}", CPApplication.Current.CompanyCode);
            Common_restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> freightManList = args.Result;
                if (needAll)
                {
                    freightManList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                callback(freightManList);
            });
        }

        #endregion Common_Service

        #region Common

        public void GetFreightMen(bool needAll, Action<List<CodeNamePair>> callback)
        {
            string relativeUrl = string.Format("/CommonService/User/FreightMan/{0}", CPApplication.Current.CompanyCode);
            GetRestClient(ConstValue.DomainName_Common).Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> freightManList = args.Result;
                if (needAll)
                {
                    freightManList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                List<CodeNamePair> list = new List<CodeNamePair>();

                freightManList.ForEach(x => {
                    list.Add(new CodeNamePair()
                    {
                         Code=x.SysNo.ToString()
                         ,Name=x.UserName
                    });
                });

                callback(list);
            });
        }

        #endregion

    }
}
