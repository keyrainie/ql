using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(ShipTypeSMSService))]
    public class ShipTypeSMSService
    {
        /// <summary>
        /// 创建配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShipTypeSMS Create(ShipTypeSMS entity)
        {
            return ObjectFactory<ShipTypeSMSProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(ShipTypeSMS entity)
        {
            ObjectFactory<ShipTypeSMSProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 加载配送方式提示短信
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShipTypeSMS Load(int sysNo)
        {
            return ObjectFactory<ShipTypeSMSProcessor>.Instance.Load(sysNo);
        }

    }
}
