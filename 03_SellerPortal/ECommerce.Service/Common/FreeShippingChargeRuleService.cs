using ECommerce.DataAccess.Common;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.Service.Common
{
    public class FreeShippingChargeRuleService
    {
        /// <summary>
        /// Query查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult QueryRules(FreeShippingChargeRuleQueryFilter filter)
        {
            int count = 0;
            DataTable dt = FreeShippingChargeRuleDA.Query(filter, out count);
            return new QueryResult(dt, filter, count);
        } 



        /// <summary>
        /// 加载一条免运费规则信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static FreeShippingChargeRuleInfoResult Load(int sysno, int SellerSysNo)
        {
            if (sysno <= 0) return null;

            return FreeShippingChargeRuleDA.Load(sysno, SellerSysNo);
        }

        /// <summary>
        /// 创建一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity)
        {
            //新建的规则设置状态为“无效”，防止不符合规则的规则立即生效
            entity.Status = FreeShippingAmountSettingStatus.DeActive;
            entity.IsGlobal = false;

            CreateOrUpdatePreCheck(entity);

            return FreeShippingChargeRuleDA.Create(entity);
        }

        /// <summary>
        /// 更新一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        public static void Update(FreeShippingChargeRuleInfo entity)
        {
            FreeShippingChargeRuleInfo info = Load(entity.SysNo.Value, entity.SellerSysNo.Value);
            if (info == null)
            {
                throw new BusinessException(LanguageHelper.GetText("编号为{0}的免运费规则不存在！"), entity.SysNo);
            }

            if (info.Status == FreeShippingAmountSettingStatus.Active)
            {
                throw new BusinessException("只有“无效”状态的免运费规则才能编辑！");
            }
            entity.IsGlobal = false;
            CreateOrUpdatePreCheck(entity);

            FreeShippingChargeRuleDA.UpdateInfo(entity);
        }

        /// <summary>
        /// 设置一条免运费规则为有效状态
        /// </summary>
        /// <param name="sysno"></param>
        public static void Valid(int sysno, int SellerSysNo, int UserSysNo)
        {
            FreeShippingChargeRuleInfo info = Load(sysno, SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(LanguageHelper.GetText("编号为{0}的免运费规则不存在！"), sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.DeActive)
            {
                throw new BusinessException("只有“无效”状态的免运费规则才能设置为有效！");
            }

            //检查冲突的免运费规则 
            CheckConflictRule(info);

            info.Status = FreeShippingAmountSettingStatus.Active;
            FreeShippingChargeRuleDA.UpdateStatus(info, UserSysNo);
        }

        /// <summary>
        /// 设置一条免运费规则为无效状态
        /// </summary>
        /// <param name="sysno"></param>
        public static void Invalid(int sysno, int SellerSysNo, int UserSysNo)
        {
            FreeShippingChargeRuleInfo info = Load(sysno, SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(LanguageHelper.GetText("编号为{0}的免运费规则不存在！"), sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.Active)
            {
                throw new BusinessException("只有“有效”状态的免运费规则才能设置为无效！");
            }

            info.Status = FreeShippingAmountSettingStatus.DeActive;
            FreeShippingChargeRuleDA.UpdateStatus(info, UserSysNo);
        }

        /// <summary>
        /// 删除一条无效的免运费规则 
        /// </summary>
        /// <param name="sysno"></param>
        public static void Delete(int sysno, int SellerSysNo)
        {
            FreeShippingChargeRuleInfo info = Load(sysno, SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(LanguageHelper.GetText("编号为{0}的免运费规则不存在！"), sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.DeActive)
            {
                throw new BusinessException("只有“无效”状态的免运费规则才能删除！");
            }

            FreeShippingChargeRuleDA.Delete(info.SysNo.Value);
        }


        #region check

        /// <summary>
        /// 新建或更新一条免运费规则前的预检查
        /// </summary>
        /// <param name="entity"></param>
        protected static void CreateOrUpdatePreCheck(FreeShippingChargeRuleInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (!entity.StartDate.HasValue)
            {
                throw new BusinessException("开始日期不能为空！");
            }
            if (!entity.EndDate.HasValue)
            {
                throw new BusinessException("结束日期不能为空！");
            }
            if (entity.StartDate.Value > entity.EndDate.Value)
            {
                throw new BusinessException("开始日期不能大于结束日期！");
            }
            if (!entity.AmountSettingType.HasValue)
            {
                throw new BusinessException("免运费条件金额类型不能为空！");
            }
            if (!entity.AmountSettingValue.HasValue)
            {
                throw new BusinessException("免运费条件的门槛金额不能为空！");
            }
            if (!String.IsNullOrEmpty(entity.Description) && entity.Description.Length > 350)
            {
                throw new BusinessException("免运费规则描述不能超过350个字符！");
            }

            int tempValue;
            if (entity.PayTypeSettingValue != null)
            {
                if (entity.PayTypeSettingValue.Any(x => !int.TryParse(x.ID, out tempValue) || tempValue < 0))
                {
                    throw new BusinessException("支付类型不正确！");
                }
            }
            if (entity.ShipAreaSettingValue != null)
            {
                if (entity.ShipAreaSettingValue.Any(x => !int.TryParse(x.ID, out tempValue) || tempValue < 0))
                {
                    throw new BusinessException("配送区域不正确！");
                }
            }

            if (entity.ProductSettingValue != null)
            {
                if (entity.ProductSettingValue == null || entity.ProductSettingValue.Count <= 0)
                {
                    throw new BusinessException("请添加至少一个免运费商品！");
                }
            }

            if (!entity.IsGlobal)
            {
                if (entity.ProductSettingValue == null || entity.ProductSettingValue.Count <= 0)
                {
                    throw new BusinessException("非全网模式必须设置免运费商品！");
                }
            }

            if (entity.Status == FreeShippingAmountSettingStatus.Active)
            {
                //检查冲突的免运费规则 
                CheckConflictRule(entity);
            }
        }

        /// <summary>
        /// 检查冲突的规则
        /// </summary>
        /// <param name="entity"></param>
        private static void CheckConflictRule(FreeShippingChargeRuleInfo entity)
        {
            List<FreeShippingChargeRuleInfo> allValidRules =
                FreeShippingChargeRuleDA.GetAllByStatus(FreeShippingAmountSettingStatus.Active, entity.SellerSysNo.Value);

            allValidRules.RemoveAll(x => x.SysNo == entity.SysNo);
            if (allValidRules != null && allValidRules.Count > 0)
            {
                List<FreeShippingChargeRuleInfo> checkList = new List<FreeShippingChargeRuleInfo>();
                List<SimpleObject> conflictList = new List<SimpleObject>();
                // 1. 找出有相同时间段的规则
                foreach (var ruleItem in allValidRules)
                {
                    if (entity.StartDate >= ruleItem.StartDate && entity.EndDate <= ruleItem.EndDate)
                    {
                        checkList.Add(ruleItem);
                    }
                    else if (entity.StartDate <= ruleItem.StartDate && entity.EndDate >= ruleItem.StartDate)
                    {
                        checkList.Add(ruleItem);
                    }
                    else if (entity.StartDate <= ruleItem.EndDate && entity.EndDate >= ruleItem.EndDate)
                    {
                        checkList.Add(ruleItem);
                    }
                }
                #region  旧逻辑 现在不用
                /*if (HasElements(entity.PayTypeSettingValue) || HasElements(entity.ShipAreaSettingValue))
                {
                    SimpleObjectEqualityComparer comparer = new SimpleObjectEqualityComparer();

                    for (var index = checkList.Count - 1; index >= 0; index--)
                    {
                        var rule = checkList[index];
                        bool conflict = false;
                        SimpleObject conflictObject = null;

                        //2.  检查这些规则是否和当前规则有相同的支付方式
                        if (HasElements(entity.PayTypeSettingValue) && HasElements(rule.PayTypeSettingValue))
                        {
                            var intersect = entity.PayTypeSettingValue.Intersect(rule.PayTypeSettingValue, comparer);
                            if (intersect != null && intersect.Count() > 0)
                            {
                                conflict = true;
                                //冲突的支付方式：{0} ！
                                conflictObject = new SimpleObject()
                                {
                                    ID = rule.SysNo.ToString(),
                                    BakString1 = string.Format("冲突的支付方式：{0} ！", String.Join(",", intersect.Select(x => x.Name)))
                                };
                            }
                        }
                        else
                        {
                            //没有配置支付方式表示不限定配送方式，相当于entity和rule具有相同的支付方式设置，支付方式冲突
                            conflict = true;
                            conflictObject = new SimpleObject()
                            {
                                ID = rule.SysNo.ToString(),
                                BakString1 = "支付方式冲突 ！"
                            };
                        }

                        //2.  检查这些规则是否和当前规则有相同的配送区域
                        if (HasElements(entity.ShipAreaSettingValue) && HasElements(rule.ShipAreaSettingValue))
                        {
                            var intersect = entity.ShipAreaSettingValue.Intersect(rule.ShipAreaSettingValue, comparer);
                            if (intersect != null && intersect.Count() > 0)
                            {
                                if (conflict)
                                {
                                    //冲突的配送区域：{0} ！
                                    conflictObject.BakString1 = string.Format("{0} {1}", conflictObject.BakString1
                                        , string.Format("冲突的配送区域：{0} ！", String.Join(",", intersect.Select(x => x.Name))));
                                }
                            }
                            else
                            {
                                conflict = false;
                                conflictObject = null;
                            }
                        }
                        else
                        {
                            if (conflict)
                            {
                                //冲突的配送区域
                                conflictObject.BakString1 = string.Format("{0} {1}", conflictObject.BakString1
                                    , "配送区域冲突！");
                            }
                        }

                        if (conflict)
                        {
                            conflictList.Add(conflictObject);
                            break;
                        }
                    }
                }
                else
                {
                    if (checkList.Count > 0)
                    {
                        var checkItem = checkList.First();
                        //冲突的时间范围：{1:yyyy-MM-dd} - {2:yyyy-MM-dd} ！
                        conflictList.Add(new SimpleObject()
                        {
                            ID = checkItem.SysNo.ToString(),
                            BakString1 = string.Format("冲突的时间范围：{0:yyyy-MM-dd} - {1:yyyy-MM-dd} ！", checkItem.StartDate, checkItem.EndDate)
                        });
                    }
                }*/
                #endregion
                if (checkList.Count > 0)
                {
                    SimpleObjectEqualityComparer comparer = new SimpleObjectEqualityComparer();
                    foreach (var checkitem in checkList)
                    {
                        bool conflict = false;
                        SimpleObject conflictObject = null;
                        var Intersect = checkitem.ProductSettingValue.Intersect(entity.ProductSettingValue, comparer);
                        if (Intersect != null && Intersect.Count() > 0)
                        {
                            conflict = true;
                            conflictObject = new SimpleObject()
                            {
                                ID = checkitem.SysNo.ToString(),
                                BakString1 = string.Format("冲突的商品：{0} ！", String.Join(",", Intersect.Select(x => x.Name)))
                            };
                        }
                        if (conflict)
                        {
                            conflictList.Add(conflictObject);
                            break;
                        }
                    }
                }
                if (conflictList.Count > 0)
                {
                    var conflictRuleItem = conflictList.First();
                    throw new BusinessException("存在冲突的免运费规则：编号为{0}，{1} ", conflictRuleItem.ID, conflictRuleItem.BakString1);
                }
            }
        }

        private static bool HasElements(IList list)
        {
            return list != null && list.Count > 0;
        }
        #endregion

    }
}
