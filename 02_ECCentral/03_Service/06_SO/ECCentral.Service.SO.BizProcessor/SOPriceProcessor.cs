using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOPriceProcessor))]
    public class SOPriceProcessor
    {
        ISOPriceDA SOPriceDA = ObjectFactory<ISOPriceDA>.Instance;


        /// <summary>
        /// 根据订单编号取得订单拆分的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo)
        {
            return SOPriceDA.GetSOPriceBySOSysNo(soSysNo);
        }
    }
}