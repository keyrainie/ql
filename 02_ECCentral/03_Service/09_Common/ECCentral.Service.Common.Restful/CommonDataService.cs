using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Common.Restful
{
    [ServiceContract]
    public partial class CommonDataService
    {
        //将删除本方法，因为渠道必须跟随公司和用户 -Jin
        [WebInvoke(UriTemplate = "/WebChannel/GetAll/{companyCode}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<WebChannel> GetWebChannels(string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetWebChannelList(companyCode);
        }

        [WebInvoke(UriTemplate = "/WebChannel/{companyCode}/{userSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<WebChannel> GetWebChannelsByUser(string companyCode, string userSysNo)
        {
            int no = int.Parse(userSysNo);
            return ObjectFactory<CommonDataAppService>.Instance.GetWebChannelsByUser(companyCode, no);
        }

        [WebInvoke(UriTemplate = "/Company/GetAll", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<Company> GetCompanyList()
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetCompanyList();
        }

        [WebInvoke(UriTemplate = "/Company/{userSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<Company> GetCompanysByUser(string userSysNo)
        {
            int no = int.Parse(userSysNo);
            return ObjectFactory<CommonDataAppService>.Instance.GetCompaniesByUser(no);
        }

        [WebInvoke(UriTemplate = "/ShippingType/GetAll/{companyCode}", Method = "GET")]
        public List<ShippingType> GetShippingTypeList(string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetShippingTypeList(companyCode);
        }

        [WebInvoke(UriTemplate = "/PayType/GetAll/{companyCode}", Method = "GET")]
        public List<PayType> GetPayTypeList(string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetPayTypeList(companyCode);
        }

        [WebInvoke(UriTemplate = "/User/{userAccount}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public int? GetUserSysNo(string userAccount)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetUserSysNo(userAccount);
        }

        [WebInvoke(UriTemplate = "/Stock/GetAll/{companyCode}", Method = "GET")]
        public List<StockInfo> GetStockList(string companyCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetStockList(companyCode);
        }

        [WebInvoke(UriTemplate = "/Department/GetAllEffectiveDepartment/{companyCode}/{languageCode}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<DepartmentInfo> GetAllEffectiveDepartment(string companyCode, string languageCode)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetAllEffectiveDepartment(companyCode, languageCode);
        }

        [WebInvoke(UriTemplate = "/User/GetBizOperationUser", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<UserInfo> GetBizOperationUser(QueryFilter.Common.BizOperationUserQueryFilter filter)
        {
            return ObjectFactory<ICommonDA>.Instance.GetBizOperationUser(filter);
        }

        [WebInvoke(UriTemplate = "/User/GetAllSystemUser/{companyCode}", Method = "GET")]
        public List<UserInfo> GetAllSystemUser(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetAllAuthSystemUser(companyCode);
        }

        [WebInvoke(UriTemplate = "/CurrencyType/GetAll", Method = "GET")]
        public List<CurrencyInfo> GetCurrencyList()
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetCurrencyList();
        }

        [WebInvoke(UriTemplate = "/User/FreightMan/{companyCode}", Method = "GET")]
        public List<UserInfo> GetFreightManList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetFreightManList(companyCode);
        }

        [WebInvoke(UriTemplate = "/User/GetUserInfoList", Method = "POST")]
        public List<UserInfo> GetUserInfoList(UserInfoQueryFilter filter)
        {
            return ObjectFactory<ICommonDA>.Instance.GetUserInfoList(filter);
        }

        [WebInvoke(UriTemplate = "/User/GetCSList/{companyCode}", Method = "GET")]
        public virtual List<UserInfo> GetCustomerServiceList(string companyCode)
        {
            return ObjectFactory<ICommonDA>.Instance.GetCustomerServiceList(companyCode);
        }

        [WebInvoke(UriTemplate = "/Common/GetSystemConfigurationValue/{key}", Method = "GET")]
        public virtual string GetSystemConfigurationValue(string key)
        {
            return ObjectFactory<CommonDataAppService>.Instance.GetSystemConfigurationValue(key,"8601");
        }

        #region 非业务查询简化方法

        delegate DataTable NoBizQueryListHanding<T>(T filter, out int dataCount);

        private QueryResult QueryList<T>(T filter, NoBizQueryListHanding<T> handing) where T : class,new()
        {
            QueryResult result = new QueryResult();
            int dataCount = 0;
            result.Data = handing(filter, out dataCount);
            result.TotalCount = dataCount;
            return result;
        }

        #endregion 非业务查询简化方法
    }
}