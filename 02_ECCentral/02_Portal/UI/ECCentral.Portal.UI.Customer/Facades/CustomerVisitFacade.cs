using System;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Service.Customer.Restful.RequestMsg;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerVisitFacade
    {
        private readonly RestClient restClient;

        private string serviceBaseUrl;
        public CustomerVisitFacade(IPage page)
        {
            serviceBaseUrl = page.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Customer, ConstValue.Key_ServiceBaseUrl);
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public CustomerVisitFacade()
            : this(CPApplication.Current.CurrentPage)
        {
        }
        public void Query(CustomerVisitView view, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CustomerVisitQueryFilter filter = ECCentral.Portal.Basic.Utilities.EntityConverter<CustomerVisitQueryVM, CustomerVisitQueryFilter>.Convert(view.QueryInfo, (v, f) =>
            {
                if (v.CustomerRank.HasValue) f.CustomerRank = (int)v.CustomerRank;
                f.SeachType = v.SeachType.HasValue ? (int)v.SeachType : (int)VisitSeachType.WaitingVisit;
            });
            string relativeUrl = "/CustomerService/Visit/Query";

            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void GetCustomerInfo(int customSysNo, Action<CustomerMaster> action)
        {
            new CustomerFacade().GetCustomerBySysNo(customSysNo, (sender, e) =>
            {
                if (e.FaultsHandle())
                    return;
                CustomerMaster customer = new CustomerMaster();
                customer.SysNo = customSysNo;
                customer.Balance = e.Result.ValidPrepayAmt;
                customer.CellPhone = e.Result.BasicInfo.Phone + " " + e.Result.BasicInfo.CellPhone;
                customer.ConfirmedTotalAmt = e.Result.ConfirmedTotalAmt;
                customer.CustomerID = e.Result.BasicInfo.CustomerID;
                customer.CustomerName = e.Result.BasicInfo.CustomerName;
                customer.CustomersType = e.Result.CustomersType;
                customer.Email = e.Result.BasicInfo.Email;
                customer.Gender = e.Result.BasicInfo.Gender.HasValue ? (Gender)e.Result.BasicInfo.Gender : customer.Gender;
                customer.Phone = e.Result.BasicInfo.Phone;
                customer.Point = e.Result.ValidScore;
                customer.Rank = e.Result.Rank.HasValue ? (CustomerRank)e.Result.Rank : customer.Rank;
                customer.RegisterTime = e.Result.BasicInfo.RegisterTime;
                customer.TotalSOMoney = e.Result.ConfirmedTotalAmt;
                customer.IsVip = e.Result.VIPRank != null;
                if (action != null)
                {
                    action(customer);
                }
            });
        }

        public void ExportCustomerVisit(CustomerVisitQueryVM view, ColumnSet[] columns)
        {
            restClient.ExportFile("/CustomerService/Visit/Query", view, columns);
        }

        #region 回访日志维护
        public void QueryVisitLog(bool visitForOrder, int customerSysNo, System.Action<List<VisitLogVM>> action)
        {
            if (customerSysNo > 0)
            {
                string relativeUrl = String.Format("/CustomerService/Visit/Load/{0}/{1}", visitForOrder ? "OrderVisitLogs" : "CustomerVisitLogs", customerSysNo);

                restClient.Query<List<VisitLog>>(relativeUrl, (sender, e) =>
                {
                    if (e.FaultsHandle())
                        return;
                    List<VisitLogVM> vm = EntityConverter<List<VisitLog>, List<VisitLogVM>>.Convert(e.Result);
                    if (action != null)
                    {
                        action(vm);
                    }
                });
            }
        }
        public void GetVisitDetailByVisitSysNo(int visitSysNo, System.Action<CustomerVisitDetailView> action)
        {
            if (visitSysNo > 0)
            {
                string relativeUrl = String.Format("/CustomerService/Visit/Load/{0}", visitSysNo);

                restClient.Query<CustomerVisitDetailReq>(relativeUrl, (sender, e) =>
                {
                    if (e.FaultsHandle())
                        return;
                    CustomerVisitDetailView detail = EntityConverter<CustomerVisitDetailReq, CustomerVisitDetailView>.Convert(e.Result);
                    if (action != null)
                    {
                        action(detail);
                    }
                });
            }
        }

        public void AddLog(CustomerVisitMaintainView view, System.Action action)
        {
            string relativeUrl = "/CustomerService/Visit/AddLog";
            view.Log.CompanyCode = CPApplication.Current.CompanyCode;
            VisitLog log = EntityConverter<VisitLogVM, VisitLog>.Convert(view.Log);
            restClient.Create<VisitLog>(relativeUrl, log, (sender, e) =>
            {
                if (e.FaultsHandle())
                    return;
                if (action != null)
                {
                    action();
                }
            });
        }

        public void GetVisitOrderByCustomerSysNo(int customerSysNo, System.Action<List<VisitOrderVM>> action)
        {
            if (customerSysNo > 0)
            {
                string relativeUrl = string.Format("/CustomerService/Visit/LoadSoSysNo/{0}/{1}", customerSysNo, CPApplication.Current.CompanyCode);
                restClient.Query<List<VisitOrder>>(relativeUrl, (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        List<VisitOrderVM> vm = EntityConverter<List<VisitOrder>, List<VisitOrderVM>>.Convert(args.Result);
                        if (action != null)
                        {
                            action(vm);
                        }
                    });
            }

        }
        #endregion
    }
}
