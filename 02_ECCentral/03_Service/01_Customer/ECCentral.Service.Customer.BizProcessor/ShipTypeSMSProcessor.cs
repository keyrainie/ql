using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(ShipTypeSMSProcessor))]
    public class ShipTypeSMSProcessor
    {
        /// <summary>
        /// 创建配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShipTypeSMS Create(ShipTypeSMS entity)
        {
            if (Load(entity.ShipTypeSysNo.Value, entity.SMSType.Value, "") != null)
                throw new BizException(ResouceManager.GetMessageString("CUSTOMER.ShipTypeSMS", "Exists_Add"));
            return ObjectFactory<IShipTypeSMSDA>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新配送方式提示短信
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(ShipTypeSMS entity)
        {
            var old = Load(entity.ShipTypeSysNo.Value, entity.SMSType.Value, entity.WebChannel.ChannelID);
            if (old != null && old.SysNo != entity.SysNo)
                throw new BizException(ResouceManager.GetMessageString("CUSTOMER.ShipTypeSMS", "Exists_Update"));
            ObjectFactory<IShipTypeSMSDA>.Instance.Update(entity);
        }

        /// <summary>
        /// 加载配送方式提示短信
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShipTypeSMS Load(int sysNo)
        {
            ShipTypeSMS entity = new ShipTypeSMS();

            return entity;
        }

        public virtual ShipTypeSMS Load(int shipTypeSysNo, SMSType smsType, string WebChannelID)
        {
            return ObjectFactory<IShipTypeSMSDA>.Instance.Load(shipTypeSysNo, smsType, WebChannelID); ;
        }

        public virtual string GetSMSContent(string webChannelID, string languageCode, int shipTypeSysNo, SMSType Type)
        {
            ShipTypeSMS entity = ObjectFactory<ShipTypeSMSProcessor>.Instance.Load(shipTypeSysNo, Type, webChannelID);
            if (entity == null || entity.SMSContent == null || string.IsNullOrEmpty(entity.SMSContent.Content))
                throw new BizException(ResouceManager.GetMessageString("CUSTOMER.ShipTypeSMS", "ContentIsNull"));
            return entity.SMSContent.Content;
        }

    }
}
