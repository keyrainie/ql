using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    public static class CustomerConst
    {
        public const string DomainName = "Customer";
        /// <summary>
        /// 取得积分换算成钱的比率Key ，如果是积分换算成钱，则除以通过此key取得的配置值；如果是钱换算成积分，则乘以通过此key取得的配置值
        /// 注意：此配置在 SO Domain 中也有用到。
        /// </summary>
        public const string Key_PointToMonetyRatio = "PointExChangeRate";
    }
}
