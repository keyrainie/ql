using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.SO.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 获取全部分期付款支付方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/GetAllInstalmentPayTypeSysNos", Method = "GET")]
        public List<int> GetAllInstalmentPayTypeSysNos()
        {
           return ObjectFactory<SOInstalmentAppService>.Instance.GetAllInstalmentPayTypeSysNos();
        }

        /// <summary>
        ///获取招商银行在线分期支付方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/GetOnlinePayTypeSysNos", Method = "GET")]
        public List<int> GetOnlinePayTypeSysNos()
        {
            return ObjectFactory<SOInstalmentAppService>.Instance.GetOnlinePayTypeSysNos();
        }
        
        /// <summary>
        /// 新建分期付款信息
        /// </summary>
        [WebInvoke(UriTemplate = "/SO/SaveSOInstallmentWhenCreateSO", Method = "POST")]
        public void SaveSOInstallmentWhenCreateSO(SOInstallmentInfo info)
        {
            ObjectFactory<SOInstalmentAppService>.Instance.SaveSOInstallmentWhenCreateSO(info);
        }

        /// <summary>
        /// 更新分期付款信息
        /// </summary>
        [WebInvoke(UriTemplate = "/SO/UpdateSOInstallmentWithoutCreditCardInfo", Method = "POST")]
        public void UpdateSOInstallmentWithoutCreditCardInfo(SOInstallmentInfo info)
        {
            ObjectFactory<SOInstalmentAppService>.Instance.UpdateSOInstallmentWithoutCreditCardInfo(info);
        }

        /// <summary>
        /// 保存分期付款信息
        /// </summary>
        [WebInvoke(UriTemplate = "/SO/SaveSOInstallment", Method = "POST")]
        public void SaveSOInstallment(SOInstallmentInfo info)
        {
            ObjectFactory<SOInstalmentAppService>.Instance.SaveSOInstallment(info);
        }
    }
}
