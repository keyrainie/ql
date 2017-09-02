using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(CouponsAppService))]
    public class CouponsAppService
    {
        private CouponsProcessor _processor = ObjectFactory<CouponsProcessor>.Instance;

        /// <summary>
        /// 获取优惠券所有信息，包含了组装好的其它Domain的描述、计算类信息，但是不Load优惠券代码信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CouponsInfo Load(int? sysNo)
        {
            CouponsInfo info = _processor.Load(sysNo);
            //在APPService中，Load 是为了Restful所调用，所以需要组装CouponsInfo中其它Domain中的描述类信息

            
            #region 商品范围条件
            if (info.ProductCondition != null)
            {
                //组装品牌的名称信息在UI上显示
                if (info.ProductCondition.RelBrands != null
                    && info.ProductCondition.RelBrands.BrandList != null
                    && info.ProductCondition.RelBrands.BrandList.Count > 0)
                {
                    foreach (SimpleObject obj in info.ProductCondition.RelBrands.BrandList)
                    {
                        obj.Name = ExternalDomainBroker.GetBrandInfoBySysNo(obj.SysNo.Value).BrandNameLocal.Content;
                                              
                    }
                }
                //组装商品类别的名称信息在UI上显示
                if (info.ProductCondition.RelCategories != null
                    && info.ProductCondition.RelCategories.CategoryList != null
                    && info.ProductCondition.RelCategories.CategoryList.Count > 0)
                {
                    foreach (SimpleObject obj in info.ProductCondition.RelCategories.CategoryList)
                    {
                        obj.Name = ExternalDomainBroker.GetCategory3Info(obj.SysNo.Value).CategoryName.Content;  
                    }
                }

                //组装商品的ID、名称、毛利率在UI上显示
                if (info.ProductCondition.RelProducts != null
                    && info.ProductCondition.RelProducts.ProductList != null
                    && info.ProductCondition.RelProducts.ProductList.Count > 0)
                {
                    foreach (RelProductAndQty prd in info.ProductCondition.RelProducts.ProductList)
                    {
                        ProductInfo product = ExternalDomainBroker.GetProductInfo(prd.ProductSysNo.Value);
                        prd.ProductName = product.ProductBasicInfo.ProductTitle.Content;
                        prd.ProductID = product.ProductID;
                        if (info.CouponChannelType == CouponsMKTType.MKTPM )
                        {
                            prd.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetCouponGrossMarginRateForPM(product, sysNo.Value);
                        }
                        else
                        {
                            prd.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetCouponGrossMarginRate(product);                           
                        }
                    }
                }
            }
            #endregion

            #region 客户范围条件
            if (info.CustomerCondition != null)
            {
                //地区信息的名称在Portal端组装（因为客户端本身也要加载所有地区信息）
                //客户等级的名称在Portal端组装（因为客户端本身也要加载所有等级信息）

                //服务端组装Customer的基础信息
                if (info.CustomerCondition.RelCustomers != null
                    && info.CustomerCondition.RelCustomers.CustomerIDList != null
                    && info.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
                {
                    foreach (CustomerAndSend cas in info.CustomerCondition.RelCustomers.CustomerIDList)
                    {

                    }
                }
            }
            #endregion


            info.IsExistThrowInTypeCouponCode = _processor.CheckExistThisTypeCouponCode(info.SysNo, CouponCodeType.ThrowIn);

            return info;
        }

        /// <summary>
        /// 获取指定商品的毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal? GetCouponGrossMarginRate(int productSysNo)
        {
            var product = ExternalDomainBroker.GetProductInfo(productSysNo);

            return ObjectFactory<GrossMarginProcessor>.Instance.GetCouponGrossMarginRate(product);
        }

        /// <summary>
        /// Load 活动基本信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CouponsInfo LoadMaster(int? sysNo)
        {
            return _processor.LoadMaster(sysNo);
        }

        /// <summary>
        /// 根据优惠券SysNo 获取对应优惠券的折扣活动信息
        /// </summary>
        /// <param name="sysNo">优惠券系统SysNo</param>
        /// <returns></returns>
        public virtual CouponsInfo GetCouponsInfoByCouponCodeSysNo(int couponCodeSysNo)
        {
            return _processor.GetCouponsInfoByCouponCodeSysNo(couponCodeSysNo);
        }

        /// <summary>
        /// 创建优惠券主信息，因为优惠券是分步进行创建的
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateMaster(CouponsInfo info)
        {
            return _processor.CreateMaster(info).Value;
        }

        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        public virtual CouponsInfo UpdateMaster(CouponsInfo info)
        {            
            _processor.UpdateMaster(info);
            CouponsInfo result = Load(info.SysNo);
            return result;
        }

        /// <summary>
        /// 设置商品范围
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetProductCondition(CouponsInfo info)
        {
            _processor.SetProductCondition(info);
        }

        /// <summary>
        /// 设置折扣规则
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetDiscountRule(CouponsInfo info)
        {
            _processor.SetDiscountRule(info);
        }

        /// <summary>
        /// 设置优惠券活动规则，包含：订单条件，使用频率条件和每单折扣上限；设置客户发放规则
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetSaleRuleEx(CouponsInfo info)
        {
            _processor.SetSaleRuleEx(info);
        }

        /// <summary>
        /// 设置客户范围
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetCustomerCondition(CouponsInfo info)
        {
            _processor.SetCustomerCondition(info);
        }
        /// <summary>
        /// 新增优惠券
        /// </summary>
        /// <param name="info"></param>
        public virtual void CreateCouponCode(CouponCodeSetting info)
        {
            _processor.CreateCouponCode(info);
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="couponCodeSysNoList"></param>
        public virtual void DelCouponCode(List<int?> couponCodeSysNoList)
        {
            _processor.DelCouponCode(couponCodeSysNoList);
        }
        /// <summary>
        /// 删除全部优惠券
        /// </summary>
        /// <param name="couponsSysNo"></param>
        public virtual void DelAllCouponCode(int? couponsSysNo)
        {
            _processor.DelAllCouponCode(couponsSysNo);
        }

        #region 全局行为
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void SubmitAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.SubmitAudit(sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void CancelAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.CancelAudit(sysNoList,out successRecords,out failureRecords);
        }

        /// <summary>
        /// 审核，包含审核通过和审核拒绝
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="auditType"></param>
        public virtual void Audit(List<int?> sysNoList, PromotionAuditType auditType, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.Audit(sysNoList, auditType, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Void(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.Void(sysNoList,out successRecords,out failureRecords);
        }

        /// <summary>
        /// 手动提前中止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void ManualStop(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.ManualStop(sysNoList, out successRecords, out failureRecords);
        }



        #endregion

    }
}
