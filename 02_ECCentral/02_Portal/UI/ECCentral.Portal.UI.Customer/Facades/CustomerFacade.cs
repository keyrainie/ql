using System;
using System.Collections.Generic;
using System.Threading;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 顾客维护
        private CustomerInfo CovertVMtoEntity(CustomerVM data)
        {
            CustomerInfo msg = data.ScoreInfo.ConvertVM<ScoreVM, CustomerInfo>();
            msg.IsBadCustomer = data.BasicInfo.IsBadCustomer;
            msg.SysNo = data.SysNo;
            msg.BasicInfo = data.BasicInfo.ConvertVM<CustomerBasicVM, CustomerBasicInfo>();
            msg.BasicInfo.DwellAreaSysNo = string.IsNullOrEmpty(data.BasicInfo.DwellAreaSysNo) ? 0 : msg.BasicInfo.DwellAreaSysNo;
            msg.PasswordInfo.Password = data.BasicInfo.Pwd;
            msg.AccountPeriodInfo = data.AccountPeriodInfo.ConvertVM<CustomerAccountPeriodVM, AccountPeriodInfo>();
            msg.VipCardNo = data.ScoreInfo.CardNo;
            msg.CustomersType = data.BasicInfo.CustomersType;
            msg.CompanyCode = data.BasicInfo.CompanyCode;
            msg.BasicInfo.CompanyCustomer = data.BasicInfo.CompanyCustomer;
            msg.WebChannel = new WebChannel() { ChannelID = data.BasicInfo.ChannelID };
            return msg;
        }

        public void CreateCustomer(CustomerVM data, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/Create";
            restClient.Create<CustomerInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 批量导入用户
        /// </summary>
        /// <param name="customerList"></param>
        /// <param name="callback"></param>
        public void BatchImportCustomers(CustomerBatchImportInfo importInfo, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/BatchImportCustomers";
            restClient.Create(relativeUrl, importInfo, callback);
        }

        public void UpdateCustomer(CustomerVM data, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/Update";
            restClient.Update<CustomerInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        public void GetCustomerBySysNo(int sysNo, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/Customer/Load/{0}", sysNo);
            restClient.Query<CustomerInfo>(relativeUrl, callback);
        }

        public void ManualAdjustUserExperience(ExperienceVM vm, EventHandler<RestClientEventArgs<CustomerExperienceLog>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/ManualAdjustUserExperience";
            CustomerExperienceLog msg = vm.ConvertVM<ExperienceVM, CustomerExperienceLog>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void ManaulSetVipRank(ScoreVM vm, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/ManaulSetVipRank";
            CustomerInfo msg = vm.ConvertVM<ScoreVM, CustomerInfo>();
            msg.SysNo = vm.CustomerSysNo;
            restClient.Update<CustomerInfo>(relativeUrl, msg, callback);
        }

        public void VoidCustomer(CustomerBasicVM vm, EventHandler<RestClientEventArgs<CustomerBasicInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/Void";
            CustomerBasicInfo msg = vm.ConvertVM<CustomerBasicVM, CustomerBasicInfo>();
            CustomerInfo cutomer = new CustomerInfo();
            cutomer.BasicInfo = msg;
            cutomer.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<CustomerBasicInfo>(relativeUrl, cutomer, callback);
        }

        public void CancelConfrimEmail(CustomerBasicVM vm, EventHandler<RestClientEventArgs<CustomerBasicInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/CancelConfirmEmail";
            CustomerBasicInfo msg = vm.ConvertVM<CustomerBasicVM, CustomerBasicInfo>();
            CustomerInfo customer = new CustomerInfo();
            customer.BasicInfo = msg;
            customer.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<CustomerBasicInfo>(relativeUrl, customer, callback);
        }

        public void CancelConfrimPhone(CustomerBasicVM vm, EventHandler<RestClientEventArgs<CustomerBasicInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/CancelConfirmPhone";
            CustomerBasicInfo msg = vm.ConvertVM<CustomerBasicVM, CustomerBasicInfo>();
            CustomerInfo customer = new CustomerInfo();
            customer.BasicInfo = msg;
            customer.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<CustomerBasicInfo>(relativeUrl, customer, callback);
        }

        public void MaintainMaliceUser(CustomerBasicVM vm, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/MaintainMaliceUser";
            CustomerInfo msg = new CustomerInfo();
            msg.SysNo = vm.CustomerSysNo;
            msg.IsBadCustomer = vm.IsBadCustomer;
            msg.OperateLog = new CustomerOperateLog();
            msg.OperateLog.Memo = vm.BadCustomerMemo;
            msg.OperateLog.CustomerSysNo = vm.CustomerSysNo;
            restClient.Update<CustomerInfo>(relativeUrl, msg, callback);
        }

        public void SetCollectionPeriodAndRating(CustomerAccountPeriodVM vm, EventHandler<RestClientEventArgs<AccountPeriodInfo>> callback)
        {
            string relativeUrl = "/CustomerService/CustomerExtend/SetCollectionPeriodAndRating";
            AccountPeriodInfo msg = vm.ConvertVM<CustomerAccountPeriodVM, AccountPeriodInfo>();
            restClient.Update<AccountPeriodInfo>(relativeUrl, msg, callback);
        }

        #endregion

        #region 代理信息

        public void CreateAgent(AgentInfoVM vm, EventHandler<RestClientEventArgs<AgentInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Agent/Create";
            AgentInfo msg = vm.ConvertVM<AgentInfoVM, AgentInfo>();
            ////  msg.LanguageCode = CPApplication.Current.LanguageCode;
            restClient.Create<AgentInfo>(relativeUrl, msg, callback);
        }

        public void UpdateAgent(AgentInfoVM vm, EventHandler<RestClientEventArgs<AgentInfo>> callback)
        {
            string relativeUrl = "/CustomerService/Agent/Update";
            AgentInfo msg = vm.ConvertVM<AgentInfoVM, AgentInfo>();
            ////  msg.LanguageCode = CPApplication.Current.LanguageCode;
            restClient.Update<AgentInfo>(relativeUrl, msg, callback);
        }

        #endregion 代理信息

        #region 收货地址信息

        public void CreateShippingAddress(ShippingAddressVM data, Action<ShippingAddressVM> callback)
        {
            string relativeUrl = "/CustomerService/ShippingAddress/Create";
            ShippingAddressInfo msg = data.ConvertVM<ShippingAddressVM, ShippingAddressInfo>();
            restClient.Create<ShippingAddressInfo>(relativeUrl, msg, (sender, e) =>
            {
                if (e.FaultsHandle())
                {
                    viewPage.Context.Window.Alert(ECCentral.Portal.UI.Customer.Resources.ResShippingAddressInfoDetail.Message_SaveFailed
                        , Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                    return;
                }
                ShippingAddressVM vm = e.Result.Convert<ShippingAddressInfo, ShippingAddressVM>();
                callback(vm);
            });
        }

        public void UpdateShippingAddress(ShippingAddressVM data, EventHandler<RestClientEventArgs<ShippingAddressInfo>> callback)
        {
            string relativeUrl = "/CustomerService/ShippingAddress/Update";
            ShippingAddressInfo msg = data.ConvertVM<ShippingAddressVM, ShippingAddressInfo>();
            restClient.Update<ShippingAddressInfo>(relativeUrl, msg, callback);
        }

        public void QueryShippingAddress(string customerSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/ShippingAddress/Query/{0}", customerSysNo);
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        #endregion 收货地址信息

        #region 增值税信息

        public void CreateValueAddedTaxInfo(ValueAddedTaxInfoVM data, Action<ValueAddedTaxInfoVM> callback)
        {
            string relativeUrl = "/CustomerService/ValueAddedTaxInfo/Create";
            ValueAddedTaxInfo msg = data.ConvertVM<ValueAddedTaxInfoVM, ValueAddedTaxInfo>();
            restClient.Create<ValueAddedTaxInfo>(relativeUrl, msg, (sender, e) =>
            {
                if (e.FaultsHandle())
                {
                    viewPage.Context.Window.Alert(ECCentral.Portal.UI.Customer.Resources.ResValueAddedTaxInvoiceDetail.Message_SaveFailed
                        , Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                    return;
                }
                ValueAddedTaxInfoVM vm = e.Result.Convert<ValueAddedTaxInfo, ValueAddedTaxInfoVM>();
                callback(vm);
            });
        }

        public void UpdateValueAddedTaxInfo(ValueAddedTaxInfoVM data, EventHandler<RestClientEventArgs<ValueAddedTaxInfo>> callback)
        {
            string relativeUrl = "/CustomerService/ValueAddedTaxInfo/Update";
            ValueAddedTaxInfo msg = data.ConvertVM<ValueAddedTaxInfoVM, ValueAddedTaxInfo>();
            // msg.SetCommonInfo();

            restClient.Update<ValueAddedTaxInfo>(relativeUrl, msg, callback);
        }

        public void QueryValueAddedTaxInfo(string valueAddedTaxSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/ValueAddedTaxInfo/Query/{0}", valueAddedTaxSysNo);
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        #endregion 增值税信息

        #region 更改积分有效期

        public void UpdateCustomerPointExpiringDate(CustomerPointExpiringDateVM data, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/CustomerService/Point/UpdateExpiringDate";
            PointObtainLog adjust = new PointObtainLog();
            adjust.ExpireDate = data.PointExpiringDate;
            adjust.SysNo = data.SysNo;
            restClient.Update(relativeUrl, adjust, callback);
        }
        #endregion 更改积分有效期

        #region 顾客头像

        public void BatchShowAvatar(List<int> customerSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            BatchUpdateAvatarStatus(customerSysNoList, AvtarShowStatus.Show, callback);
        }

        public void BatchNotShowAvatar(List<int> customerSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            BatchUpdateAvatarStatus(customerSysNoList, AvtarShowStatus.NotShow, callback);
        }

        private void BatchUpdateAvatarStatus(List<int> customerSysNoList, AvtarShowStatus showStatus, EventHandler<RestClientEventArgs<object>> callback)
        {
            BatchUpdateAvatarStatusReq request = new BatchUpdateAvatarStatusReq();
            request.CustomerSysNoList = customerSysNoList;
            request.AvtarImageStatus = showStatus;

            string relativeUrl = "/CustomerService/Customer/BatchUpdateAvatarStatus";
            restClient.Update(relativeUrl, request, callback);
        }

        #endregion 顾客头像


        #region 社团

        public void GetSociety(Action<List<KeyValuePair<string, string>>> callback)
        {
            string relativeUrl = "/CustomerService/Customer/GetSociety";
            restClient.Query<List<SocietyInfo>>(relativeUrl, (obj, args) =>
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