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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.QueryFilter.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.SAP;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Invoice.Restful.RequestMsg;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Facades
{
    public class SAPFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public SAPFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }


        public void QueryVendor(SAPVendorQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SAPVendorQueryFilter filter = queryVM.ConvertVM<SAPVendorQueryVM, SAPVendorQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/QueryVendor";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }


        public void QueryCompany(SAPCompanyQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SAPCompanyQueryFilter filter = queryVM.ConvertVM<SAPCompanyQueryVM, SAPCompanyQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/QueryCompany";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void QueryIPPUser(SAPIPPUserQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SAPIPPUserQueryFilter filter = queryVM.ConvertVM<SAPIPPUserQueryVM, SAPIPPUserQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/QueryIPPUser";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void UpdateIPPUser(SAPIPPUserVM vm, Action callback)
        {
            SAPIPPUserInfo data = vm.ConvertVM<SAPIPPUserVM, SAPIPPUserInfo>();
            string relativeUrl = "/InvoiceService/SAP/UpdateIPPUser";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void CreateSAPVendor(SAPVendorVM vm, Action callback)
        {
            SAPVendorInfo data = vm.ConvertVM<SAPVendorVM, SAPVendorInfo>();
            if (string.IsNullOrEmpty(data.SAPVendorID))
            {
                data.SAPVendorID = "cv1" + data.VendorSysNo.ToString();
            }
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/CreateSAPVendor";
            restClient.Create<SAPVendorInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void PreCheckSAPCompany(SAPCompanyVM vm, Action<SAPCompanyResp> callback)
        {
            SAPCompanyInfo data = vm.ConvertVM<SAPCompanyVM, SAPCompanyInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/PreCheckSAPCompany";
            restClient.Query<SAPCompanyResp>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void CreateSAPCompany(SAPCompanyVM vm, int alertFlag, Action callback)
        {
            SAPCompanyInfo entity = vm.ConvertVM<SAPCompanyVM, SAPCompanyInfo>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            SAPCompanyReq data = new SAPCompanyReq();
            data.SAPCompany = entity;
            data.AlertFlag = alertFlag;
            string relativeUrl = "/InvoiceService/SAP/CreateSAPCompany";
            restClient.Create<SAPCompanyInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void CreateSAPIPPUser(SAPIPPUserVM vm, Action callback)
        {
            SAPIPPUserInfo data = vm.ConvertVM<SAPIPPUserVM, SAPIPPUserInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SAP/CreateSAPIPPUser";
            restClient.Create<SAPIPPUserInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }
    }
}
