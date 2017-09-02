using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(GroupBuyingAppService))]
    public class GroupBuyingAppService
    {
        /// <summary>
        /// 创建团购
        /// </summary>
        public virtual GroupBuyingInfo Create(GroupBuyingInfo item)
        {
            item.InUser = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            item.CurrentSellCount = item.CurrentSellCount.HasValue ? item.CurrentSellCount.Value : 0;

            return ObjectFactory<GroupBuyingProcessor>.Instance.Create(item);
        }
        /// <summary>
        /// 更新团购
        /// </summary>
        public virtual GroupBuyingInfo Update(GroupBuyingInfo item)
        {
            item.EditUser = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            return ObjectFactory<GroupBuyingProcessor>.Instance.Update(item);
        }
        /// <summary>
        /// 加载一个团购信息
        /// </summary>
        public virtual GroupBuyingInfo Load(int sysNo)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.Load(sysNo);
        }
        public virtual Dictionary<int, string> GetGroupBuyingTypes()
        {
            Dictionary<int, string> result = ObjectFactory<GroupBuyingProcessor>.Instance.GetGroupBuyingTypes();
            return result;
        }
        public virtual Dictionary<int, string> GetGroupBuyingAreas()
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetGroupBuyingAreas();
        }
        public virtual Dictionary<int, string> GetGroupBuyingVendors()
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetGroupBuyingVendors();
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Void(List<int> sysNoList)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.Void(sysNoList);
        }
        /// <summary>
        /// 中止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Stop(List<int> sysNoList)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.Stop(sysNoList);
        }

        public virtual void SubmitAudit(int sysNo)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.SubmitAudit(sysNo);
        }

        public virtual void CancelAudit(int sysNo)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.CancelAudit(sysNo);
        }

        public virtual void AuditApprove(int sysNo, string reasonStr)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.AuditApprove(sysNo, reasonStr);
        }

        public virtual void AuditRefuse(int sysNo, string reasonStr)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.AuditRefuse(sysNo, reasonStr);
        }

        public virtual List<GroupBuySaveInfo> LoadMarginRateInfo(GroupBuyingInfo groupBuyInfo)
        {
            List<GroupBuySaveInfo> list = new List<GroupBuySaveInfo>();
            List<ProductInfo> products = new List<ProductInfo>();
            if (groupBuyInfo.IsByGroup.Value)
            {
                products = ExternalDomainBroker.GetProductsInSameGroupWithProductSysNo(groupBuyInfo.ProductSysNo.Value);
            }
            else
            {
                products.Add(ExternalDomainBroker.GetProductInfo(groupBuyInfo.ProductSysNo.Value));
            }

            foreach (ProductInfo p in products)
            {
                GroupBuySaveInfo msgInfo = new GroupBuySaveInfo();
                msgInfo.ProductSysNo = p.SysNo;
                msgInfo.ProductID = p.ProductID;
                decimal originalPrice = groupBuyInfo.OriginalPrice ?? 0;//ObjectFactory<GroupBuyingProcessor>.Instance.GetProductOriginalPrice(p.SysNo, groupBuyInfo.IsByGroup ?? false ? "Y" : "N", groupBuyInfo.CompanyCode);
                decimal unitCost = p.ProductPriceInfo.UnitCost == 0 ? p.ProductPriceInfo.VirtualPrice : p.ProductPriceInfo.UnitCost;

                if (groupBuyInfo.PriceRankList.Count > 0 && groupBuyInfo.PriceRankList[0] != null && groupBuyInfo.PriceRankList[0].DiscountValue.HasValue)
                {
                    msgInfo.Price1 = Math.Round(groupBuyInfo.PriceRankList[0].DiscountValue.Value, 2);

                    msgInfo.SpareMoney1 = Math.Round(originalPrice - msgInfo.Price1.Value, 2);
                    if (originalPrice > 0)
                    {
                        msgInfo.Discount1 = Math.Round(msgInfo.Price1.Value / originalPrice, 2) * 10;
                    }
                    else
                    {
                        msgInfo.Discount1 = -1 * 10;
                    }
                    if (msgInfo.Price1 > 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10)) > 0)
                        {
                            var rate1 = (msgInfo.Price1 - unitCost) / msgInfo.Price1 * 100;
                            msgInfo.MarginRate1 = Math.Round(rate1.Value, 2);
                            msgInfo.MarginDollar1 = Math.Round(msgInfo.Price1.Value - unitCost, 2);
                        }
                    }
                    if (msgInfo.Price1 == 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10m)) > 0)
                        {
                            msgInfo.MarginRate1 = -100;
                            msgInfo.Discount1 = 0.0m;
                            msgInfo.MarginDollar1 = Math.Round(msgInfo.Price1.Value - unitCost, 2);
                        }
                    }
                }
                if (groupBuyInfo.PriceRankList.Count > 1 && groupBuyInfo.PriceRankList[1] != null && groupBuyInfo.PriceRankList[1].DiscountValue.HasValue)
                {
                    msgInfo.Price2 = Math.Round(groupBuyInfo.PriceRankList[1].DiscountValue.Value, 2);
                    if ((msgInfo.Price2 ?? 0) == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_GroupBuyingPrice2NotNullOr0"));
                    }
                    msgInfo.SpareMoney2 = Math.Round(originalPrice - msgInfo.Price2.Value, 2);
                    if (originalPrice > 0)
                    {
                        msgInfo.Discount2 = Math.Round(msgInfo.Price2.Value / originalPrice, 2) * 10;
                    }
                    else
                    {
                        msgInfo.Discount2 = -1 * 10;
                    }
                    if (msgInfo.Price2 > 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10)) > 0)
                        {
                            var rate2 = (msgInfo.Price2 - unitCost) / msgInfo.Price2 * 100;
                            msgInfo.MarginRate2 = Math.Round(rate2.Value, 2);
                            msgInfo.MarginDollar2 = Math.Round(msgInfo.Price2.Value - unitCost, 2);
                        }
                    }
                    if (msgInfo.Price2 == 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10m)) > 0)
                        {
                            msgInfo.MarginRate2 = -1;
                            msgInfo.MarginDollar2 = Math.Round(msgInfo.Price2.Value - unitCost, 2);
                        }
                    }
                }
                if (groupBuyInfo.PriceRankList.Count > 2 && groupBuyInfo.PriceRankList[2] != null && groupBuyInfo.PriceRankList[2].DiscountValue.HasValue)
                {
                    msgInfo.Price3 = Math.Round(groupBuyInfo.PriceRankList[2].DiscountValue.Value, 2);
                    if ((msgInfo.Price3 ?? 0) == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_GroupBuyingPrice2NotNullOr0"));
                    }
                    msgInfo.SpareMoney3 = Math.Round(originalPrice - msgInfo.Price3.Value, 2);
                    if (originalPrice > 0)
                    {
                        msgInfo.Discount3 = Math.Round(msgInfo.Price3.Value / originalPrice, 2) * 10;
                    }
                    else
                    {
                        msgInfo.Discount3 = -1 * 10;
                    }
                    if (msgInfo.Price3 > 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10)) > 0)
                        {
                            var rate3 = (msgInfo.Price3 - unitCost) / msgInfo.Price3 * 100;
                            msgInfo.MarginRate3 = Math.Round(rate3.Value, 2);
                            msgInfo.MarginDollar3 = Math.Round(msgInfo.Price3.Value - unitCost, 2);
                        }
                    }
                    if (msgInfo.Price3 == 0)
                    {
                        if ((p.ProductPriceInfo.CurrentPrice - (p.ProductPriceInfo.Point / 10m)) > 0)
                        {
                            msgInfo.MarginRate3 = -1;
                            msgInfo.MarginDollar3 = Math.Round(msgInfo.Price3.Value - unitCost, 2);
                        }
                    }
                }

                list.Add(msgInfo);
            }

            return list;
        }

        public virtual List<object> GetProductOriginalPrice(int productSysNo, string isByGroup, string companyCode)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetProductOriginalPriceList(productSysNo, isByGroup, companyCode);
        }

        #region "check随心配在团购最低阶梯价的毛利率"

        /// <summary>
        /// 得到随心配的商品在团购里的毛利率
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual string GetProductPromotionMarginByGroupBuying(GroupBuyingInfo info)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetProductPromotionMarginByGroupBuying(info);
        }
        #endregion


        #region Job相关
        public virtual List<GroupBuyingInfo> GetGroupBuyingList(int groupBuyingSysNo, int companyCode)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetGroupBuyingList(groupBuyingSysNo, companyCode);
        }
        #endregion

        public GroupBuyingCategoryInfo CreateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.CreateGroupBuyingCategory(entity);
        }

        public GroupBuyingCategoryInfo UpdateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.UpdateGroupBuyingCategory(entity);
        }

        public List<GroupBuyingCategoryInfo> GetAllGroupBuyingCategory()
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetAllGroupBuyingCategory();
        }

        public virtual string BatchReadGroupbuyingFeedback(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var bl = ObjectFactory<GroupBuyingProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<int, BizException>(items, (p) =>
            {
                bl.ReadGroupBuyingFeedback(p);
            });

            return resutl.PromptMessage;
        }

        public virtual string BatchHandleGroupbuyingBusinessCooperation(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var bl = ObjectFactory<GroupBuyingProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<int, BizException>(items, (p) =>
            {
                bl.HandleGroupbuyingBusinessCooperation(p);
            });

            return resutl.PromptMessage;
        }

        public void HandleGroupbuyingBusinessCooperation(int sysNo)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.HandleGroupbuyingBusinessCooperation(sysNo);
        }

        public string BatchAuditPassGroupBuyingSettlement(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var bl = ObjectFactory<GroupBuyingProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<int, BizException>(items, (p) =>
            {
                bl.AuditPassGroupBuyingSettlement(p);
            });

            return resutl.PromptMessage;           
        }

        public string BatchVoidGroupBuyingTicket(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var bl = ObjectFactory<GroupBuyingProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<int, BizException>(items, (p) =>
            {
                bl.VoidGroupBuyingTicket(p);
            });

            return resutl.PromptMessage;  
        }
    }
}
