using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Activation;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.Restful.RequestMsg;
using System.ServiceModel.Web;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Customer.AppService;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Customer.Restful
{

    public partial class CustomerService
    {
        #region     [顾客加积分申请]

        #region [Query Methods]

        [WebInvoke(UriTemplate = "/Points/QueryPointsAddRequest", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryCustomerPointAddRequest(CustomerPointsAddRequestFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<ICustomerPointsAddRequestQueryDA>.Instance.Query(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/Points/QueryPointsAddRequestItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryCustomerPointAddRequestItem(CustomerPointsAddRequestFilter request)
        {
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<ICustomerPointsAddRequestQueryDA>.Instance.QueryRequestItems(request)
            };
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/Points/GetSoDetail/{soSysNo}/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public SOInfo GetSoDetail(string soSysNo)
        {
            int SoSysNo = int.TryParse(soSysNo, out SoSysNo) ? SoSysNo : 0;
            return ObjectFactory<CustomerPointsAppService>.Instance.GetSoDetail(SoSysNo, "");
        }

        #endregion

        #region [Action Methods]
        [WebInvoke(UriTemplate = "/Point/CreateCustomerPointsAddRequest", Method = "PUT")]

        public CustomerPointsAddRequest CreateCustomerPointsAddRequest(CustomerPointsAddRequest request)
        {
            return ObjectFactory<CustomerPointsAppService>.Instance.CreateCustomerPointsAddRequest(request);
        }

        [WebInvoke(UriTemplate = "/Point/ConfirmCustomerPointsAddRequest", Method = "PUT")]
        public void ConfirmCustomerPointsAddRequest(CustomerPointsAddRequest request)
        {
            ObjectFactory<CustomerPointsAppService>.Instance.ConfirmCustomerPointsAddReques(request);
        }

        #endregion

        #endregion

        /// <summary>
        /// 查询积分历史
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Point/QueryPointLog", Method = "POST")]

        public QueryResult QueryCustomerPointLog(ECCentral.QueryFilter.Customer.CustomerPointLogQueryFilter request)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<ICustomerQueryDA>.Instance.QueryCustomerPointLog(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 调整客户积分有效期
        /// </summary>
        [WebInvoke(UriTemplate = "/Point/UpdateExpiringDate", Method = "PUT")]
        public virtual void UpateExpiringDate(PointObtainLog entity)
        {
            ObjectFactory<CustomerPointsAppService>.Instance.UpateExpiringDate(entity);
        }

        /// <summary>
        /// 调整顾客积分
        /// </summary>
        [WebInvoke(UriTemplate = "/Point/AdjustCustomerPoint", Method = "PUT")]
        public virtual void AdjustCustomerPoint(AdjustPointRequest entity)
        {
            ObjectFactory<CustomerPointsAppService>.Instance.AdjustCustomerPoint(entity);
        }

        /// <summary>
        /// 为顾客加积分
        /// </summary>
        [WebInvoke(UriTemplate = "/Point/AddCustomerPoint", Method = "PUT")]
        public virtual void AddCustomerPoint(AdjustPointRequest entity)
        {
            ObjectFactory<CustomerPointsAppService>.Instance.AddCustomerPoint(entity);
        }

        /// <summary>
        /// 批量修改积分
        /// </summary>
        [WebInvoke(UriTemplate = "/Point/BatchAdjustPoint", Method = "POST")]
        public void BatchAdjustPoint(List<CustomerPointsAddRequest> request)
        {
            ICustomerBizInteract service = ObjectFactory<ICustomerBizInteract>.Instance;
            request.ForEach(e => {
                service.AdjustPoint(e);
            });
        }

    }
}
