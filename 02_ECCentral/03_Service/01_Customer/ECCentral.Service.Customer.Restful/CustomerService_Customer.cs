using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.Customer.Society;

namespace ECCentral.Service.Customer.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class CustomerService
    {
        #region Query

        /// <summary>
        /// 查询顾客
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult Query(CustomerQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICustomerQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 顾客查询服务,主要通过系统编号，名称，邮件等主要信息查询
        /// </summary>
        [WebInvoke(UriTemplate = "/Customer/Picker", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult SimpleQuery(CustomerSimpleQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICustomerQueryDA>.Instance.SimpleQuery(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 获取第三方账户信息
        /// </summary>
        /// <param name="customerSysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/GetThirdPartyUserInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public List<ThirdPartyUser> GetThirdPartyUserInfo(List<int?> customerSysNoList)
        {
            return ObjectFactory<ICustomerQueryDA>.Instance.GetThirdPartyUserInfo(customerSysNoList);
        }
        #endregion Query

        #region BatchUpdateAvatarStatus

        /// <summary>
        /// 批量设置顾客头像状态
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/Customer/BatchUpdateAvatarStatus", Method = "PUT")]
        public void BatchUpdateAvatarStatus(BatchUpdateAvatarStatusReq request)
        {
            ObjectFactory<CustomerAppService>.Instance.BatchUpdateAvatarStatus(request.CustomerSysNoList, request.AvtarImageStatus);
        }

        #endregion BatchUpdateAvatarStatus

        #region BatchImportCustomer

        /// <summary>
        /// 批量导入顾客
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/Customer/BatchImportCustomers", Method = "POST")]
        public string BatchImportCustomer(CustomerBatchImportInfo batchInfo)
        {
            return ObjectFactory<CustomerBatchImportAppService>.Instance.BatchImportCustomer(batchInfo);
        }

        #endregion BatchImportCustomer

        #region Maitain Customer

        [WebInvoke(UriTemplate = "/Customer/Create", Method = "POST")]
        public CustomerInfo CreateCustomer(CustomerInfo customer)
        {
            return ObjectFactory<CustomerAppService>.Instance.CreateCustomer(customer);
        }

        [WebInvoke(UriTemplate = "/Customer/Update", Method = "PUT")]
        public void UpdateCustomer(CustomerInfo customer)
        {
            ObjectFactory<CustomerAppService>.Instance.UpdateCustomer(customer);
        }

        /// <summary>
        /// 根据系统编号加载顾客详细信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/Load/{customerSysNo}", Method = "GET")]
        public CustomerInfo LoadCustomer(string customerSysNo)
        {
            int sysNo = int.Parse(customerSysNo);
            return ObjectFactory<CustomerAppService>.Instance.GetCustomerBySysNo(sysNo);
        }

        /// <summary>
        /// 作废顾客
        /// </summary>
        /// <param name="customerSysNo"></param>
        [WebInvoke(UriTemplate = "/Customer/Void", Method = "PUT")]
        public void AbandonCustomer(CustomerInfo customer)
        {
            ObjectFactory<CustomerAppService>.Instance.AbandonCustomer(customer);
        }

        /// <summary>
        /// 取消验证email
        /// </summary>
        /// <param name="customerSysNo"></param>
        [WebInvoke(UriTemplate = "/Customer/CancelConfirmEmail", Method = "PUT")]
        public void CancelConfirmEmail(CustomerInfo customer)
        {
            ObjectFactory<CustomerAppService>.Instance.CancelConfirmEmail(customer);
        }

        /// <summary>
        /// 取消验证phone
        /// </summary>
        /// <param name="customerSysNo"></param>
        [WebInvoke(UriTemplate = "/Customer/CancelConfirmPhone", Method = "PUT")]
        public void CancelConfirmPhone(CustomerInfo customer)
        {
            ObjectFactory<CustomerAppService>.Instance.CancelConfirmPhone(customer);
        }

        ///<summary>
        ///手工设置VIP等级
        ///</summary>
        ///<param name="customer"></param>
        [WebInvoke(UriTemplate = "/Customer/ManaulSetVipRank", Method = "PUT")]
        public void ManaulSetVipRank(CustomerInfo customer)
        {
            ObjectFactory<CustomerAppService>.Instance.ManaulSetVipRank(customer.SysNo.Value, customer.VIPRank.Value, customer.CompanyCode);
        }

        /// <summary>
        /// 手工调整经验值
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/ManualAdjustUserExperience", Method = "PUT")]
        public void ManualAdjustUserExperience(CustomerExperienceLog adjustInfo)
        {
            ObjectFactory<CustomerAppService>.Instance.AdjustExperience(adjustInfo);
        }

        /// <summary>
        /// 修改顾客已购买金额
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/AdjustOrderedAmount", Method = "PUT")]
        public CustomerInfo AdjustOrderedAmount(CustomerInfo customer)
        {
            return null;
        }

        /// <summary>
        /// 维护恶意顾客
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Customer/MaintainMaliceUser", Method = "PUT")]
        public void MaintainMaliceUser(CustomerInfo entity)
        {
            ObjectFactory<CustomerAppService>.Instance.MaintainMaliceUser(entity);
        }

        #endregion Maitain Customer

        #region 代理信息

        [WebInvoke(UriTemplate = "/Agent/Create", Method = "POST")]
        public AgentInfo CreateAgent(AgentInfo agent)
        {
            return ObjectFactory<CustomerAppService>.Instance.CreateAgent(agent);
        }

        [WebInvoke(UriTemplate = "/Agent/Update", Method = "PUT")]
        public AgentInfo UpdateAgent(AgentInfo agent)
        {
            return ObjectFactory<CustomerAppService>.Instance.UpdateAgent(agent);
        }

        [WebInvoke(UriTemplate = "/Agent/Load/{customerSysNo}", Method = "GET")]
        public AgentInfo LoadAgent(string customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.GetAgentByCustomerSysNo(int.Parse(customerSysNo));
        }

        #endregion 代理信息

        #region Shipping Address

        [WebInvoke(UriTemplate = "/ShippingAddress/Create", Method = "POST")]
        public ShippingAddressInfo CreateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            return ObjectFactory<CustomerAppService>.Instance.CreateShippingAddress(shippingAddress);
        }

        [WebInvoke(UriTemplate = "/ShippingAddress/Update", Method = "PUT")]
        public void UpdateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            ObjectFactory<CustomerAppService>.Instance.UpdateShippingAddress(shippingAddress);
        }

        [WebInvoke(UriTemplate = "/ShippingAddress/Query/{customerSysNo}", Method = "GET")]
        public List<ShippingAddressInfo> QueryShippingAddress(string customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.QueryShippingAddress(int.Parse(customerSysNo));
        }

        #endregion Shipping Address

        #region 增值税信息

        [WebInvoke(UriTemplate = "/ValueAddedTaxInfo/Create", Method = "POST")]
        public ValueAddedTaxInfo CreateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            return ObjectFactory<CustomerAppService>.Instance.CreateValueAddedTaxInfo(vat);
        }

        [WebInvoke(UriTemplate = "/ValueAddedTaxInfo/Update", Method = "PUT")]
        public void UpdateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            ObjectFactory<CustomerAppService>.Instance.UpdateValueAddedTaxInfo(vat);
        }

        [WebInvoke(UriTemplate = "/ValueAddedTaxInfo/Query/{customerSysNo}", Method = "GET")]
        public List<ValueAddedTaxInfo> QueryValueAddedTaxInfo(string customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.QueryValueAddedTaxInfo(int.Parse(customerSysNo));
        }

        #endregion 增值税信息

        /// <summary>
        /// 查询恶意用户操作历史
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/QueryMaliceCustomerLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryMaliceCustomerLog(int customerSysNo)
        {
            var dataTable = ObjectFactory<ICustomerQueryDA>.Instance.QueryMaliceCustomerLog(customerSysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = dataTable.Rows.Count
            };
        }

        /// <summary>
        /// 更新用户权限
        /// </summary>
        /// <param name="right"></param>
        [WebInvoke(UriTemplate = "/Customer/UpdateCustomerRights", Method = "PUT")]
        public void UpdateCustomerRights(CustomerRightReq request)
        {
            ObjectFactory<CustomerAppService>.Instance.UpdateCustomerRight(request.CustomerSysNo, request.RightList);
        }

        /// <summary>
        /// 获取用户所有权限
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Customer/LoadAllCustomerRight", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CustomerRight> LoadAllCustomerRight(int customerSysNo)
        {
            return ObjectFactory<CustomerAppService>.Instance.LoadAllCustomerRight(customerSysNo);
        }

        /// <summary>
        /// 设置客户为恶意用户
        /// </summary>
        /// <param name="right"></param>
        [WebInvoke(UriTemplate = "/Customer/SetMaliceCustomer/{customerSysNo}/{memo}/{soSysNo}", Method = "PUT")]
        public void SetMaliceCustomer(string customerSysNo, string memo, string soSysNo)
        {
            ObjectFactory<CustomerAppService>.Instance.SetMaliceCustomer(int.Parse(customerSysNo), true, memo, int.Parse(soSysNo));
        }

        /// <summary>
        /// 获取客户安全密保问题
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="companydCode"></param>
        [WebInvoke(UriTemplate = "/Customer/GetSecurityQuestion", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetSecurityQuestion(int customerSysNo)
        {
            return new QueryResult()
            {
                Data = ObjectFactory<ICustomerQueryDA>.Instance.GetSecurityQuestion(customerSysNo),
                TotalCount = 0
            };
        }
               /// <summary>
        /// 获取社团信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="companydCode"></param>
         [WebInvoke(UriTemplate = "/Customer/GetSociety", Method = "GET")]
         public List<SocietyInfo> GetSociety()
         {
             var result = ObjectFactory<ICustomerQueryDA>.Instance.GetSocieties();
             return result;
         }
    }
}