using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using System.Collections;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(FreeShippingChargeRuleProcessor))]
    public class FreeShippingChargeRuleProcessor
    {
        /// <summary>
        /// 加载一条免运费规则信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public virtual FreeShippingChargeRuleInfo Load(int sysno)
        {
            if (sysno <= 0) return null;

            return ObjectFactory<IFreeShippingChargeRuleDA>.Instance.Load(sysno);
        }

        /// <summary>
        /// 创建一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity)
        {
            //新建的规则设置状态为“无效”，防止不符合规则的规则立即生效
            entity.Status = FreeShippingAmountSettingStatus.DeActive;

            this.CreateOrUpdatePreCheck(entity);

            return ObjectFactory<IFreeShippingChargeRuleDA>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(FreeShippingChargeRuleInfo entity)
        {
            FreeShippingChargeRuleInfo info = this.Load(entity.SysNo.Value);
            if (info == null)
            {
                ThrowBizException("Res_FreeShippingChargeRuleIsNotExist", entity.SysNo);
            }

            if (info.Status == FreeShippingAmountSettingStatus.Active)
            {
                ThrowBizException("Res_OnlyInvalidStatusCanBeEdit");
            }

            this.CreateOrUpdatePreCheck(entity);

            ObjectFactory<IFreeShippingChargeRuleDA>.Instance.UpdateInfo(entity);
        }

        /// <summary>
        /// 设置一条免运费规则为有效状态
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void Valid(int sysno)
        {
            FreeShippingChargeRuleInfo info = this.Load(sysno);
            if (info == null)
            {
                ThrowBizException("Res_FreeShippingChargeRuleIsNotExist", sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.DeActive)
            {
                ThrowBizException("Res_OnlyInvalidStatusCanSetToBeValidStatus");
            }

            //检查冲突的免运费规则 
            this.CheckConflictRule(info);

            info.Status = FreeShippingAmountSettingStatus.Active;
            ObjectFactory<IFreeShippingChargeRuleDA>.Instance.UpdateStatus(info);
        }

        /// <summary>
        /// 设置一条免运费规则为无效状态
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void Invalid(int sysno)
        {
            FreeShippingChargeRuleInfo info = this.Load(sysno);
            if (info == null)
            {
                ThrowBizException("Res_FreeShippingChargeRuleIsNotExist", sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.Active)
            {
                ThrowBizException("Res_OnlyValidStatusCanSetToBeInvalidStatus");
            }

            info.Status = FreeShippingAmountSettingStatus.DeActive;
            ObjectFactory<IFreeShippingChargeRuleDA>.Instance.UpdateStatus(info);
        }

        /// <summary>
        /// 删除一条无效的免运费规则 
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void Delete(int sysno)
        {
            FreeShippingChargeRuleInfo info = this.Load(sysno);
            if (info == null)
            {
                ThrowBizException("Res_FreeShippingChargeRuleIsNotExist", sysno);
            }

            if (info.Status != FreeShippingAmountSettingStatus.DeActive)
            {
                ThrowBizException("Res_OnlyInvalidStatusCanBeDelete");
            }

            ObjectFactory<IFreeShippingChargeRuleDA>.Instance.Delete(info.SysNo.Value);
        }

        /// <summary>
        /// 新建或更新一条免运费规则前的预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CreateOrUpdatePreCheck(FreeShippingChargeRuleInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (!entity.StartDate.HasValue)
            {
                ThrowBizException("Res_StartDateIsRequired");
            }
            if (!entity.EndDate.HasValue)
            {
                ThrowBizException("Res_EndDateIsRequired");
            }
            if (!entity.AmountSettingType.HasValue)
            {
                ThrowBizException("Res_AmountSettingTypeIsRequired");
            }
            if (!entity.AmountSettingValue.HasValue)
            {
                ThrowBizException("Res_AmountSettingValueIsRequired");
            }
            if (!String.IsNullOrEmpty(entity.Description) && entity.Description.Length > 350)
            {
                ThrowBizException("Res_DescriptionMoreThan350Characters");
            }

            int tempValue;
            if (entity.PayTypeSettingValue != null)
            {
                if (entity.PayTypeSettingValue.Any(x => !int.TryParse(x.ID, out tempValue) || tempValue < 0))
                {
                    ThrowBizException("Res_PayTypeSettingValueIsIncorrect");
                }
            }
            if (entity.ShipAreaSettingValue != null)
            {
                if (entity.ShipAreaSettingValue.Any(x => !int.TryParse(x.ID, out tempValue) || tempValue < 0))
                {
                    ThrowBizException("Res_ShipAreaSettingValueIsIncorrect");
                }
            }

            if (!entity.IsGlobal)
            {
                if (entity.ProductSettingValue == null || entity.ProductSettingValue.Count <= 0)
                {
                    ThrowBizException("Res_UnGlobalModeProductSettingIsRequired");
                }
            }

            if (entity.Status == FreeShippingAmountSettingStatus.Active)
            {
                //检查冲突的免运费规则 
                this.CheckConflictRule(entity);
            }
        }

        /// <summary>
        /// 检查冲突的规则
        /// </summary>
        /// <param name="entity"></param>
        private void CheckConflictRule(FreeShippingChargeRuleInfo entity)
        {
            List<FreeShippingChargeRuleInfo> allValidRules =
                ObjectFactory<IFreeShippingChargeRuleDA>.Instance.GetAllByStatus(FreeShippingAmountSettingStatus.Active);

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

                if (HasElements(entity.PayTypeSettingValue) || HasElements(entity.ShipAreaSettingValue))
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
                                    BakString1 = GetMessageString("Res_ConflictPayType", String.Join(",", intersect.Select(x => x.Name)))
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
                                BakString1 = GetMessageString("Res_ConflictAllPayType")
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
                                        , GetMessageString("Res_ConflictShipArea", String.Join(",", intersect.Select(x => x.Name))));
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
                                    , GetMessageString("Res_ConflictAllShipArea"));
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
                            BakString1 = GetMessageString("Res_ConflictTimeRange", checkItem.StartDate, checkItem.EndDate)
                        });
                    }
                }

                if (conflictList.Count > 0)
                {
                    var conflictRuleItem = conflictList.First();
                    ThrowBizException("Res_ConflictCheckResult", conflictRuleItem.ID, conflictRuleItem.BakString1);
                }
            }
        }

        private bool HasElements(IList list)
        {
            return list != null && list.Count > 0;
        }

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(CommonConst.MessagePath_FreeShippingChargeRule, msgKeyName), args);
        }
    }
}
