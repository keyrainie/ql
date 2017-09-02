using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ProductRecommendAppService))]
    public class ProductRecommendAppService
    {
        private ProductRecommendProcessor _processor = ObjectFactory<ProductRecommendProcessor>.Instance;

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create(ProductRecommendInfo entity)
        {
            _processor.Create(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update(ProductRecommendInfo entity)
        {
            _processor.Update(entity);
        }

        /// <summary>
        /// 加载
        /// </summary>
        public virtual ProductRecommendInfo Load(int sysNo)
        {
            return _processor.Load(sysNo);
        }

        /// <summary>
        /// 将商品推荐置为无效
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        public virtual void Deactive(int sysNo)
        {
            _processor.Deactive(sysNo);
        }

        /// <summary>
        /// 加载专卖店对应的位置信息
        /// </summary>
        /// <param name="pageID">专卖店对应页面ID</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public virtual List<ProductRecommendLocation> GetBrandPosition(int pageID, string companyCode, string channelID)
        {
            return _processor.GetBrandPosition(pageID, companyCode, channelID);
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public virtual ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return ExternalDomainBroker.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }
    }
}
