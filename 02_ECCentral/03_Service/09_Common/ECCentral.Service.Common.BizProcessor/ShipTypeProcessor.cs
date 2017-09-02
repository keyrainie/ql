using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ShipTypeProcessor))]
    public class ShipTypeProcessor
    {
        private IShipTypeDA shipTypeDADA = ObjectFactory<IShipTypeDA>.Instance;

        /// <summary>
        /// 创建配送方式
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateShipType(ShippingType item)
        {
            if (shipTypeDADA.GetShipTypeforCreate(item))
                throw new BizException(string.Format("配送方式ID为{0}的数据已存在！",item.ShipTypeID));
            else
            {
                //int sysNo = shipTypeDADA.GetShipTypeSequence();
                //item.SysNo = sysNo;
                //item.ShipTypeID = sysNo.ToString();
                shipTypeDADA.CreateShipType(item);
            }
        }

        /// <summary>
        /// 更新配送方式
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateShipType(ShippingType item)
        {
            shipTypeDADA.UpdateShipType(item);
        }
        /// <summary>
        /// 加载配送方式
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual ShippingType LoadShipType(int sysNo)
        {
           return  shipTypeDADA.LoadShipType(sysNo);
        }

    }
}
