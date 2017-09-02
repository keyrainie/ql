using System.ServiceModel.Web;

using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 创建Pending
        /// </summary>
        /// <param name="info">创建请求信息</param>
        /// <returns>创建后的实体</returns>
        [WebInvoke(UriTemplate = "/SO/CreatePending", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SOPending CreateSOPending(SOPending info)
        {
            return info;
        }

        /// <summary>
        /// 打开Pending
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/OpenPending", Method = "PUT")]
        public void OpenSOPending(int soSysNo)
        {
            ObjectFactory<SOPendingAppService>.Instance.OpenSOPending(soSysNo);
        }

        /// <summary>
        /// 关闭Pending
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/ClosePending", Method = "PUT")]
        public void CloseSOPending(int soSysNo)
        {
            ObjectFactory<SOPendingAppService>.Instance.CloseSOPending(soSysNo);
        }

        /// <summary>
        /// 改单Pending
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/UpdatePending", Method = "PUT")]
        public void UpdateSOPending(int soSysNo)
        {
            ObjectFactory<SOPendingAppService>.Instance.UpdateSOPending(soSysNo);
        }
    }
}
