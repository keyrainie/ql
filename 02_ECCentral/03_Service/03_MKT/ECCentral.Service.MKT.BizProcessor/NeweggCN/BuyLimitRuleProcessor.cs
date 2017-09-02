using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(BuyLimitRuleProcessor))]
    public class BuyLimitRuleProcessor
    {
        private IBuyLimitRuleDA _daBuyLimitRule = ObjectFactory<IBuyLimitRuleDA>.Instance;

        public virtual BuyLimitRule Load(int sysNo)
        {
            var result = _daBuyLimitRule.Load(sysNo);
            if (result == null)
            {
                //throw new BizException(string.Format("系统编号为{0}的限购规则不存在。", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_NotExistThisLimitRule"), sysNo));
            }
            if (result.LimitType == LimitType.SingleProduct)
            {
                var productInfo = ExternalDomainBroker.GetProductInfo(result.ItemSysNo);
                if (productInfo != null)
                {
                    result.UIProductID = productInfo.ProductID;
                }
            }
            return result;
        }

        public virtual void Insert(BuyLimitRule data)
        {
            CheckData(data);
            _daBuyLimitRule.Insert(data);
        }

        public virtual void Update(BuyLimitRule data)
        {
            CheckData(data);
            _daBuyLimitRule.Update(data);
        }

        #region Helpers

        private void CheckData(BuyLimitRule data)
        {
            int sysNo = data.SysNo ?? 0;
            if (data.LimitType == LimitType.Combo)
            {
                //1.验证套餐是否存在
                var combo = ObjectFactory<ComboProcessor>.Instance.Load(data.ItemSysNo);
                if (combo == null)
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_NotExistThisGroupBuy"), data.ItemSysNo));
                }
                //2.验证套餐是否已存在规则设置
                //CheckExistsRule(LimitType.Combo, "该套餐已存在相关限购规则，请不要重复设置。"
                //    , sysNo, data.ItemSysNo);
                CheckExistsRule(LimitType.Combo, ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_ExisitThisLimitRuleForGroupBuy")
                    , sysNo, data.ItemSysNo);
            }
            else
            {
                //1.验证商品是否存在
                var product = ExternalDomainBroker.GetProductInfo(data.ItemSysNo);
                if (product == null)
                {
                    //throw new BizException("商品不存在。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_NotExistProduct"));
                }
                //2.验证商品或其所属商品组是否已存在规则设置
                var productInGroup = ExternalDomainBroker.GetProductsInSameGroupWithProductSysNo(data.ItemSysNo);
                List<int> productSysNos = new List<int>(0);
                if (productInGroup != null && productInGroup.Count > 0)
                {
                    productSysNos = productInGroup.Select(item => item.SysNo).ToList();
                }
                else
                {
                    productSysNos.Add(data.ItemSysNo);
                }
                //CheckExistsRule(LimitType.SingleProduct, "该单品或其所属商品组已存在相关限购规则，请不要重复设置。"
                //    , sysNo, productSysNos.ToArray());
                CheckExistsRule(LimitType.SingleProduct, ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_ExisitThisLimitRuleForProduct")
                    , sysNo, productSysNos.ToArray());
            }
            if (data.EndDate <= data.BeginDate)
            {
                //throw new BizException("结束时间必须大于开始时间。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_EndDateNeedMoreThanStartDate"));
            }
            if (data.EndDate.HasValue && data.EndDate.Value <= DateTime.Now)
            {
                //throw new BizException("结束时间必须大于当前时间。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_EndDateNeedMoreThanCurrentDate"));
            }
            if (data.MinQty < 0)
            {
                //throw new BizException("限购下限必须大于等于零。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_LowerLimitNeedMoreThan0"));
            }
            if (data.MaxQty < 0)
            {
                //throw new BizException("限购上限必须大于等于零。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_UpperLimitNeedMoreThan0"));
            }
            if (data.MaxQty < data.MinQty)
            {
                //throw new BizException("限购上限必须大于等于限购下限");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_UpperLimitNeedMoreThanLowerLimit"));
            }

            if (data.OrderTimes < 0)
            {
                //throw new BizException("当日限购次数必须大于等于零。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.BuyLimit", "BuyLimit_LimitCountNeedMoreThan0InCurrentDay"));
            }
        }

        private void CheckExistsRule(LimitType limitType, string existsMessage, int excludeSysNo, params int[] productSysNos)
        {
            bool exists = _daBuyLimitRule.CheckExistsRule(limitType, excludeSysNo, productSysNos);
            if (exists)
            {
                throw new BizException(existsMessage);
            }
        }

        #endregion
    }
}
