using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.AppService
{
    /// <summary>
    /// 订单投诉服务
    /// </summary>
    [VersionExport(typeof(SOComplainAppService))]
    public class SOComplainAppService
    {
        SOComplainProcessor ComplainProcessor;
        public SOComplainAppService()
        {
            ComplainProcessor = ObjectFactory<SOComplainProcessor>.Instance;
        }

        /// <summary>
        /// 创建投诉信息
        /// </summary>
        /// <param name="info"></param>
        public virtual SOComplaintInfo AddSOComplaintInfo(SOComplaintCotentInfo info)
        {
            return ComplainProcessor.Create(info);
        }

        /// <summary>
        /// 修改投诉信息
        /// </summary>
        /// <param name="info"></param>
        public virtual SOComplaintInfo UpdateSOComplaintInfo(SOComplaintInfo info)
        {
             return ComplainProcessor.Update(info);
        }

        public virtual SOComplaintInfo GetInfo(int sysNo)
        {
            return ComplainProcessor.GetInfo(sysNo);
        }

        /// <summary>
        /// 分配投诉
        /// </summary>
        /// <param name="infoList"></param>
        public virtual void BatchAssignSOComplaintInfo(List<SOComplaintProcessInfo> infoList)
        {
            ComplainProcessor.BatchAssign(infoList);
        }

        /// <summary>
        /// 取消投诉分配
        /// </summary>
        /// <param name="infoList"></param>
        public virtual void BathCancelAssignSOComplaintInfo(List<SOComplaintProcessInfo> infoList)
        {
            ComplainProcessor.BatchCancelAssign(infoList);
        }
        
        /// <summary>
        /// 发送投诉处理邮件
        /// </summary>
        /// <param name="info"></param>
        public virtual void SendMail(int complainSysNo)
        {
            ComplainProcessor.SendMain(complainSysNo);
        }

        public ProductDomain GetProductDomain(string productID)
        {
            return ComplainProcessor.GetProductDomain(productID);
        }
    }
}
