//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动申请单据
// 子系统名		        商品价格变动申请单据业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductPriceRequestAppService))]
    public class ProductPriceRequestAppService
    {
        /// <summary>
        /// 审核商品价格变动单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void AuditProductPriceRequest(List<ProductPriceRequestInfo> entity)
        {
            var errorDesc = "";
            var sucessCount = 0;
            var faileCount = 0;
            foreach (var item in entity)
            {
                try
                {
                    ObjectFactory<ProductPriceRequestProcessor>.Instance.AuditProductPriceRequest(item);
                    sucessCount++;
                }
                catch (BizException ex)
                {
                    errorDesc += ex.Message + "\r\n";
                    faileCount++;
                }

            }
            errorDesc += ResouceManager.GetMessageString("IM.ProductPriceRequest", "AuditSuccess");
            errorDesc += sucessCount.ToString() + ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            errorDesc += "," + ResouceManager.GetMessageString("IM.ProductPriceRequest", "AuditFail");
            errorDesc += faileCount.ToString() + ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            if (!String.IsNullOrEmpty(errorDesc))
            {
                throw new BizException(errorDesc);
            }
        }

        /// <summary>
        /// 根据SysNO获取商品价格变动单据
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        /// <returns></returns>
        public ProductPriceRequestInfo GetProductPriceRequestInfoBySysNo(int auditProductPriceSysNo)
        {
            var result = ObjectFactory<ProductPriceRequestProcessor>.Instance.GetProductPriceRequestInfoBySysNo(auditProductPriceSysNo);
            return result;
        }

        /// <summary>
        /// 添加商品价格审核列表添加其他信息
        /// </summary>
        /// <param name="productPriceRequest"></param>
        public virtual void AddOtherData(DataTable productPriceRequest)
        {
            ObjectFactory<ProductPriceRequestProcessor>.Instance.AddOtherData(productPriceRequest);
        }

        /// <summary>
        /// 添加商品价格审核列表添加其他信息
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        /// <param name="productSysNo"></param>
        public List<ProductPromotionDiscountInfo> GetProductPromotionDiscountInfoList(int auditProductPriceSysNo, ref int productSysNo)
        {
            if (auditProductPriceSysNo <= 0)
            {
                return new List<ProductPromotionDiscountInfo>();
            }
             productSysNo =
                ObjectFactory<ProductPriceRequestProcessor>.Instance.GetProductSysNoByAuditProductPriceSysNo(
                    auditProductPriceSysNo);
            if(productSysNo>0)
            {
                var source = ExternalDomainBroker.GetProductPromotionDiscountInfoList(productSysNo);
                return source;
            }
            return new List<ProductPromotionDiscountInfo>();
        }
    }
}
