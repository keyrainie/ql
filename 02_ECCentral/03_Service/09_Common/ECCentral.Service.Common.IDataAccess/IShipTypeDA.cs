using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IShipTypeDA
    {
        /// <summary>
        /// 获取配送方式的SysNo
        /// </summary>
        /// <returns></returns>
        int GetShipTypeSequence();

        /// <summary>
        /// 检查创建配送方式是否存在
        /// </summary>
        /// <param name="item"></param>
        bool GetShipTypeforCreate(ShippingType item);

        /// <summary>
        /// 创建配送方式
        /// </summary>
        /// <param name="item"></param>
        void CreateShipType(ShippingType item);

        /// <summary>
        /// 更新配送方式
        /// </summary>
        /// <param name="item"></param>
        void UpdateShipType(ShippingType item);
        /// <summary>
        /// 加载配送方式
        /// </summary>
        /// <param name="sysNo"></param>
        ShippingType LoadShipType(int sysNo);

    }
}
