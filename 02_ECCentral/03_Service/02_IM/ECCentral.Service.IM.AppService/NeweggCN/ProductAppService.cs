using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.IM.BizProcessor;

namespace ECCentral.Service.IM.AppService
{
    partial class ProductAppService
    {
        /// <summary>
        /// 获取分仓列表 [Ray.L.Xing 泰隆优选不存在多渠道 故将StockInfo 改为WarehouseInfo 返回]
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ExternalDomainBroker.GetWarehouseList(companyCode);         
        }
    }
}