using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 加载系统账户列表
        /// </summary>
        /// <param name="channelID">渠道ID</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SysAccount/LoadAll/{channelID}", Method = "GET")]
        public List<CustomerBasicInfo> LoadSysAccountList(string channelID)
        {
            return ObjectFactory<AjustSysAccountPointAppService>.Instance.LoadSysAccountList(channelID);
        }

        /// <summary>
        /// 取得系统账户可用积分
        /// </summary>
        /// <param name="customerSysNo">账户系统编号（系统账户也是用户）</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SysAccount/GetVaildScore/{customerSysNo}", Method = "GET")]
        public int LoadSysAccountValidScore(string customerSysNo)
        {
            int _customerSysNo;
            if (!int.TryParse(customerSysNo, out _customerSysNo))
            {
                throw new ArgumentException("Invalid customerSysNo");
            }
            return ObjectFactory<AjustSysAccountPointAppService>.Instance.GetSysAccountValidScore(_customerSysNo);
        }

        /// <summary>
        /// 调整积分
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/SysAccount/AjustPoint", Method = "PUT")]
        public void AjustSysAccountPoint(AdjustPointRequest entity)
        {
            ObjectFactory<AjustSysAccountPointAppService>.Instance.AjustSysAccountPoint(entity);
        }
    }
}
