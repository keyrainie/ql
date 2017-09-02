using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IShipTypeSMSDA
    {
        /// <summary>
        /// 创建配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         ShipTypeSMS Create(ShipTypeSMS entity);

        /// <summary>
        /// 更新配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        void Update(ShipTypeSMS entity);

        /// <summary>
        /// 加载配送方式提示短信
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ShipTypeSMS Load(int sysNo);

        ShipTypeSMS Load(int shipTypeSysNo, SMSType smsType, string WebChannelID);

    }
}
