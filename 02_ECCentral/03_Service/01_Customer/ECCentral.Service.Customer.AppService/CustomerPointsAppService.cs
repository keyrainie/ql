using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CustomerPointsAppService))]
    public class CustomerPointsAppService
    {
        public virtual void ConfirmCustomerPointsAddReques(CustomerPointsAddRequest info)
        {
            ObjectFactory<PointProcessor>.Instance.VerifyCustomerAddPointRequset(info);
        }
        public virtual CustomerPointsAddRequest CreateCustomerPointsAddRequest(CustomerPointsAddRequest newInfo)
        {
            return ObjectFactory<PointProcessor>.Instance.CreateCustomerAddPointRequset(newInfo);
        }

        public virtual BizEntity.SO.SOInfo GetSoDetail(int SOSysNo,string sysAccount)
        {
            return ObjectFactory<PointProcessor>.Instance.GetSoDetail(SOSysNo, sysAccount);
        }
        /// <summary>
        /// 调整客户积分有效期
        /// </summary>
        public virtual void UpateExpiringDate(PointObtainLog entity)
        {
            ObjectFactory<PointProcessor>.Instance.UpateExpiringDate(entity.SysNo.Value, entity.ExpireDate.Value);
        }
        /// <summary>
        /// 调整顾客积分
        /// </summary>
        public virtual void AdjustCustomerPoint(AdjustPointRequest entity)
        {
            ObjectFactory<PointProcessor>.Instance.Adjust(entity);
        }
        /// <summary>
        /// 为顾客加积分
        /// </summary>
        public virtual void AddCustomerPoint(AdjustPointRequest entity)
        {
            ObjectFactory<PointProcessor>.Instance.Adjust(entity);
        }
    }
}
