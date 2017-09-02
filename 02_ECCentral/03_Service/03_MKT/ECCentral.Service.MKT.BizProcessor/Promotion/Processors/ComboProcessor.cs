using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.Service.IBizInteract;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.IM;
using System.Transactions;
using System.Collections;
using ECCentral.BizEntity.Common;
using ECCentral.Service.EventMessage.MKT;
//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;

namespace ECCentral.Service.MKT.BizProcessor.Promotion.Processors
{
    [Export(typeof(IPromotionCalculate))]
    [VersionExport(typeof(ComboProcessor))]
    public class ComboProcessor : CalculateBaseProcessor, IPromotionCalculate
    {
        private IComboDA _da = ObjectFactory<IComboDA>.Instance;

        #region 计算类行为
        /// <summary>
        /// 为SO提供计算当前订单能能享受的所有促销活动结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            List<SOPromotionInfo> promotionInfoList = new List<SOPromotionInfo>();
            //只有零售才享受Combo
            if (soInfo.BaseInfo.IsWholeSale.HasValue && soInfo.BaseInfo.IsWholeSale.Value) return promotionInfoList;
            //1.取得所有的有效状态，并且主商品在SO中存在的Combo List
            List<int> soItemSysNoList = new List<int>(); //SO Item中所有购买的主商品
            List<SOItemInfo> soItemList = new List<SOItemInfo>();
            foreach (SOItemInfo soItem in soInfo.Items)
            {
                if (soItem.ProductType.Value == SOProductType.Product)
                {
                    soItemSysNoList.Add(soItem.ProductSysNo.Value);
                    soItemList.Add(soItem);
                }
            }

            if (soItemSysNoList.Count == 0) return promotionInfoList;
            List<ComboInfo> comboList = _da.GetComboListForCurrentSO(soItemSysNoList);

            GetPromotionListForSO(comboList, soInfo, ref soItemList, ref promotionInfoList);

            return promotionInfoList;
        }

        /// <summary>
        /// 获取促销结果
        /// </summary>
        /// <param name="origComboList"></param>
        /// <param name="soInfo"></param>
        /// <param name="soItemList"></param>
        /// <param name="promotionInfoList"></param>
        protected virtual void GetPromotionListForSO(List<ComboInfo> origComboList, SOInfo soInfo, ref List<SOItemInfo> soItemList, ref List<SOPromotionInfo> promotionInfoList)
        {
            //1.取当前订单商品有效的Combo
            List<ComboInfo> validComboList = GetValidComboList(origComboList, soItemList);
            if (validComboList.Count == 0)
            {
                return;
            }

            //2.按照这些可以参与活动的Combo列表，得到所有的折扣列表: 遵循最大Combo折扣优先原则，而不是客户最大折扣优先原则
            List<ComboApplyInstance> comboApplyList = new List<ComboApplyInstance>();
            foreach (ComboInfo combo in validComboList)
            {
                ComboApplyInstance comboApply = new ComboApplyInstance();
                ComboInfo comboClone = SerializationUtility.DeepClone<ComboInfo>(combo);
                comboApply.ComboSysNo = comboClone.SysNo.Value;
                //获取当前Combo在订单中最多可以存在多少套: 取订单中满足该Combo商品中最小数量
                int maxCount = int.MaxValue;
                foreach (ComboItem comboItem in comboClone.Items)
                {
                    SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == comboItem.ProductSysNo);
                    int curCount = soItem.Quantity.Value / comboItem.Quantity.Value;
                    if (curCount < maxCount)
                    {
                        maxCount = curCount;
                    }
                }
                comboApply.Qty = maxCount;
                decimal totalDiscount = 0.00m;
                foreach (ComboItem comboItem in comboClone.Items)
                {
                    totalDiscount += Math.Round(comboItem.Discount.Value * comboItem.Quantity.Value * maxCount, 2);
                }
                //取Combo Total Discount的绝对值
                comboApply.TotalDiscount = Math.Abs(totalDiscount);
                comboApplyList.Add(comboApply);
            }
            //3.得到折扣从大到小排序的列表
            var comboApplySortList = from p in comboApplyList
                                     orderby p.TotalDiscount descending
                                     select p;

            //4.取最大折扣的Combo
            ComboApplyInstance maxDiscountComboApply = comboApplySortList.First();
            ComboInfo curCombo = validComboList.Find(f => f.SysNo.Value == maxDiscountComboApply.ComboSysNo);


            SOPromotionInfo promotionInfo = GetPromotionInfoForCurrentCombo(curCombo, maxDiscountComboApply, ref soItemList,
                    promotionInfoList.Count + 1, soInfo.SysNo);
            if (promotionInfo != null)
            {

                promotionInfoList.Add(promotionInfo);
            }

            //5.轮询调用剩下的订单商品
            GetPromotionListForSO(validComboList, soInfo, ref soItemList, ref promotionInfoList);

        }

        /// <summary>
        /// 获取现存订单中有效的Combo列表
        /// </summary>
        /// <param name="origComboList">第一次从DB中取得所有的有效状态，并且主商品在SO中存在的Combo列表</param>
        /// <param name="soItemList"></param>
        /// <returns></returns>
        protected virtual List<ComboInfo> GetValidComboList(List<ComboInfo> origComboList, List<SOItemInfo> soItemList)
        {
            List<ComboInfo> validComboList = new List<ComboInfo>();
            //根据SO中Item，遍历comboList（条件:combo中所有的商品都包含在SO Items中），得到最终可以参与活动的Combo列表
            foreach (ComboInfo combo in origComboList)
            {
                bool includeAll = true;
                foreach (ComboItem comboItem in combo.Items)
                {
                    //订单中的数量必须大于等于Combo中对应商品的数量则该Combo有效，否则该Combo也是无效的
                    if (soItemList.Find(f => f.ProductSysNo == comboItem.ProductSysNo
                        && f.Quantity >= comboItem.Quantity) == null)
                    {
                        includeAll = false;
                        break;
                    }
                }
                if (includeAll)
                {
                    validComboList.Add(combo);
                }
            }
            return validComboList;
        }


        /// <summary>
        /// 根据当前Combo，得到该Combo在订单中的SOPromotionInfo
        /// </summary>
        /// <param name="curCombo"></param>
        /// <param name="comboApply"></param>
        /// <param name="soItemList"></param>
        /// <param name="priority"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        protected virtual SOPromotionInfo GetPromotionInfoForCurrentCombo(ComboInfo curCombo,
            ComboApplyInstance comboApply, ref List<SOItemInfo> soItemList, int priority, int? soSysNo)
        {
            SOPromotionInfo promotionInfo = null;

            int maxCount = comboApply.Qty;

            if (maxCount > 0)
            {
                //说明当前订单剩余商品还可以满足这个Combo
                promotionInfo = new SOPromotionInfo();
                promotionInfo.PromotionType = SOPromotionType.Combo;
                promotionInfo.Combo = curCombo;
                promotionInfo.PromotionSysNo = curCombo.SysNo.Value;
                promotionInfo.PromotionName = curCombo.Name != null ? curCombo.Name.Content : null;
                promotionInfo.DiscountAmount = -Math.Abs(comboApply.TotalDiscount);
                promotionInfo.GainPoint = 0;
                promotionInfo.Priority = priority;
                promotionInfo.SOSysNo = soSysNo;
                promotionInfo.Time = maxCount;
                if (promotionInfo.Time > 0)
                {
                    promotionInfo.Discount = promotionInfo.DiscountAmount / promotionInfo.Time;
                }

                StringBuilder promotionNote = new StringBuilder();

                promotionInfo.SOPromotionDetails = new List<SOPromotionDetailInfo>();
                foreach (ComboItem comboItem in curCombo.Items)
                {
                    SOPromotionDetailInfo promotionDetail = new SOPromotionDetailInfo();
                    promotionDetail.DiscountAmount = Math.Abs(Math.Round(comboItem.Discount.Value * comboItem.Quantity.Value * maxCount, 2));
                    promotionDetail.GainPoint = 0;
                    promotionDetail.MasterProductQuantity = comboItem.Quantity.Value * maxCount;
                    promotionDetail.MasterProductSysNo = comboItem.ProductSysNo.Value;
                    //promotionDetail.MasterProductType = SOProductType.Product;
                    promotionInfo.SOPromotionDetails.Add(promotionDetail);

                    promotionNote.AppendFormat("{0},{1},-{2};", promotionDetail.MasterProductQuantity, promotionDetail.MasterProductSysNo, promotionDetail.DiscountAmount);

                    //最重要的一点：要从soItemList中减掉这些已经做了折扣的商品及数量
                    //如果数量没减完，则在soItemList中保留该Item，但是数量要减掉；如果数量减完，则从soItemList中Remove掉改Item
                    List<SOItemInfo> needRemoveSOItemList = new List<SOItemInfo>();
                    foreach (SOItemInfo soItem in soItemList)
                    {
                        if (soItem.ProductSysNo == comboItem.ProductSysNo)
                        {
                            soItem.OriginalPrice = soItem.OriginalPrice + comboItem.Discount; //有折扣总价减去折扣 Bug:89610
                            if (soItem.Quantity > comboItem.Quantity.Value * maxCount)
                            {
                                soItem.Quantity = soItem.Quantity - comboItem.Quantity.Value * maxCount;
                            }
                            else
                            {
                                needRemoveSOItemList.Add(soItem);
                            }
                        }
                    }
                    foreach (SOItemInfo soItem in needRemoveSOItemList)
                    {
                        soItemList.Remove(soItem);
                    }
                }
                promotionInfo.Note = promotionNote.ToString();
            }
            return promotionInfo;
        }

        #endregion

        #region 维护类行为
        /// <summary>
        /// 加载Combo所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ComboInfo Load(int? sysNo)
        {
            ComboInfo info = _da.Load(sysNo.Value);
            if (info == null)
            {
                //throw new BizException("套餐不存在！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_NotExsistComb"));
            }
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            info.InUser = userfullname;
            return info;
        }

        /// <summary>
        /// 批量创建Combo
        /// </summary>
        /// <param name="comboList"></param>
        /// <returns></returns>
        public virtual List<ComboInfo> BatchCreateCombo(List<ComboInfo> comboList)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            #region Check
            if (comboList != null && comboList.Count > 0)
            {
                ComboInfo combo = comboList[0];
                List<string> errorList = CheckBasicIsPass(combo);
                if (errorList.Count > 0)
                {
                    throw new BizException(errorList.Join("\r\n"));
                }

                errorList = CheckComboItemIsPass(combo);
                if (errorList.Count > 0)
                {
                    throw new BizException(errorList.Join("\r\n"));
                }

                if (!CheckPriceIsPass(combo))
                {
                    //errorList.Add("差价大于成本（销售价格和 + 总折扣 < 成本价格和），请先提交审核！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenPriceDiffrenceIsHigher"));
                }
                if (errorList.Count > 0)
                {
                    throw new BizException(errorList.Join("\r\n"));
                }

                foreach (ComboItem i in combo.Items)
                {
                    if (!CheckMarginIsPass(i))
                    {
                        // errorList.Add(string.Format("商品{0}毛利率小于最低毛利率，请提交审核！", item.ProductID));
                        errorList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenRateOfMarginIsHigher"), i.ProductID));
                    }
                }
                if (errorList.Count > 0)
                {
                    throw new BizException(errorList.Join("\r\n"));
                }
            }
            #endregion

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                comboList.ForEach(p =>
                {
                    p.SysNo = CreateCombo(p);
                });

                scope.Complete();
            }

            return comboList;
        }

        /// <summary>
        /// 批量生效Combo
        /// </summary>
        /// <param name="comboList"></param>
        /// <returns></returns>
        public virtual List<ComboInfo> BatchUpdateCombo(List<ComboInfo> comboList)
        {
            StringBuilder sbMsg = new StringBuilder();
            string createtypeMsg = string.Empty;
            string returnSysNo = string.Empty;

            ComboInfo combo = comboList[0];
            //只需验证一个Combo即可，因为所有的ComboItem都是相同的
            foreach (ComboItem item in combo.Items)
            {
                if (item.IsMasterItemB == null || !item.IsMasterItemB.Value) { continue; }
                int sysNo = _da.CheckComboExits(combo.Name.Content, item.ProductSysNo.Value);
                if (sysNo > 0)
                {
                    combo.SysNo = sysNo;

                    List<string> errorList = CheckBasicIsPass(combo);
                    if (errorList.Count > 0)
                    {
                        throw new BizException(errorList.Join("\r\n"));
                    }

                    errorList = CheckComboItemIsPass(combo);
                    if (errorList.Count > 0)
                    {
                        throw new BizException(errorList.Join("\r\n"));
                    }

                    if (!CheckPriceIsPass(combo))
                    {
                        //errorList.Add("差价大于成本（销售价格和 + 总折扣 < 成本价格和），请先提交审核！");
                        errorList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenPriceDiffrenceIsHigher"));
                    }
                    if (errorList.Count > 0)
                    {
                        throw new BizException(errorList.Join("\r\n"));
                    }

                    foreach (ComboItem i in combo.Items)
                    {
                        if (!CheckMarginIsPass(i))
                        {
                            // errorList.Add(string.Format("商品{0}毛利率小于最低毛利率，请提交审核！", item.ProductID));
                            errorList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenRateOfMarginIsHigher"), item.ProductID));
                        }
                    }
                    if (errorList.Count > 0)
                    {
                        throw new BizException(errorList.Join("\r\n"));
                    }

                    //如果差价大于成本（销售价格和 + 总折扣 < 成本价格和），自动提交审核！
                    //UpdateStatus(combo.SysNo, ComboStatus.Active);

                }
                else
                {
                    //throw new BizException("该销售规则不存在！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_NotExsistTheSaleRule"));
                }

                foreach (var _cb in comboList)
                {
                    ComboItem _cbi = _cb.Items.Where(oi => oi.IsMasterItemB == true).ToList<ComboItem>()[0];
                    sysNo = _da.CheckComboExits(_cb.Name.Content, _cbi.ProductSysNo.Value);
                    UpdateStatus(sysNo, ComboStatus.Active);
                }
            }
            //if (!string.IsNullOrEmpty(returnSysNo))
            //{
            //    createtypeMsg = "\r\n规则编号：" + returnSysNo + " 规则名:" + combo.Name
            //                              + " 该规则下的商品差价小于0，已提交审核。";
            //    sbMsg.AppendLine(createtypeMsg);
            //}
            //combo.CreateType = sbMsg.ToString();

            return comboList;
        }


        /// <summary>
        /// 创建Combo Master
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateCombo(ComboInfo info)
        {
            List<string> errorList = CheckBasicIsPass(info);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }

            //创建的时候不检查Item的信息

            //errorList = CheckComboItemIsPass(info);

            //if (errorList.Count > 0)
            //{
            //    throw new BizException(errorList.Join("\r\n"));
            //}
            if (info.Status == null)
            {
                info.Status = ComboStatus.Deactive;
            }

            TransactionScopeFactory.TransactionAction(() =>
            {
                info.SysNo = _da.CreateMaster(info);
                foreach (ComboItem item in info.Items)
                {
                    item.ComboSysNo = info.SysNo;
                    _da.AddComboItem(item);
                }

                //将套餐信息写入到促销引擎中，写入时促销活动是停止状态，审核通过后变成运行状态。
                //ObjectFactory<ComboPromotionEngine>.Instance.SaveComboActivity(info);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.ComboCreate.ToEnumDesc(), BizLogType.ComboCreate, info.SysNo.Value, info.CompanyCode);


            return info.SysNo;
        }

        /// <summary>
        /// 更新Combo Master，包含：更新主信息，更新状态：
        /// 无效->有效,无效->待审核，有效->无效，有效->待审核,待审核->无效，待审核->有效
        /// 其中无效->有效需要Check RequiredSaleRule4UpdateValidate
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateCombo(ComboInfo info)
        {
            List<string> errorList = CheckBasicIsPass(info);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }
            #region 2012-11-13 update
            //修改内容:在销售规则有商品时才走对商品check的逻辑
            #endregion

            if (info.Items != null && info.Items.Count > 0)
            {
                errorList = CheckComboItemIsPass(info);
            }

            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }

            errorList = CheckValidateWhenChangeStatus(info);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }
            TransactionScopeFactory.TransactionAction(() =>
            {
               _da.UpdateMaster(info);
               _da.DeleteComboAllItem(info.SysNo.Value);
               foreach (ComboItem item in info.Items)
               {
                   item.ComboSysNo = info.SysNo;
                   _da.AddComboItem(item);
               }
               //更新活动
               //ObjectFactory<ComboPromotionEngine>.Instance.SaveComboActivity(info);

               // 发送待办消息
               switch (info.TargetStatus)
               {
                   // 待审核
                   case ComboStatus.WaitingAudit:
                       EventPublisher.Publish<ComboSaleSubmitMessage>(new ComboSaleSubmitMessage
                       {
                           ComboSaleSysNo = info.SysNo.Value,
                           ComboSaleName = info.Name.Content,
                           CurrentUserSysNo = ServiceContext.Current.UserSysNo
                       });
                       break;
                   // 有效
                   case ComboStatus.Active:
                       EventPublisher.Publish<ComboSaleActiveMessage>(new ComboSaleActiveMessage
                       {
                           ComboSaleSysNo = info.SysNo.Value,
                           ComboSaleName = info.Name.Content,
                           CurrentUserSysNo = ServiceContext.Current.UserSysNo
                       });
                       break;
                   // 无效
                   case ComboStatus.Deactive:
                       EventPublisher.Publish<ComboSaleAuditRefuseMessage>(new ComboSaleAuditRefuseMessage
                       {
                           ComboSaleSysNo = info.SysNo.Value,
                           ComboSaleName = info.Name.Content,
                           CurrentUserSysNo = ServiceContext.Current.UserSysNo
                       });
                       break;
               }
           });

            ExternalDomainBroker.CreateOperationLog(BizLogType.ComboUpdate.ToEnumDesc(), BizLogType.ComboUpdate, info.SysNo.Value, info.CompanyCode);

        }

        /// <summary>
        /// 仅仅更新状态，不做任何检查，主要是为外部系统提供服务
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="targetStatus"></param>
        public virtual void UpdateStatus(int? sysNo, ComboStatus targetStatus)
        {
            TransactionScopeFactory.TransactionAction(() =>
            {
                _da.UpdateStatus(sysNo, targetStatus);
                //更新活动状态 
                //ObjectFactory<ComboPromotionEngine>.Instance.UpdateComboActivityStatus(sysNo.Value, targetStatus);

                // 发送待办消息
                switch (targetStatus)
                {
                    // 待审核
                    case ComboStatus.WaitingAudit:
                        EventPublisher.Publish<ComboSaleSubmitMessage>(new ComboSaleSubmitMessage
                        {
                            ComboSaleSysNo = sysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        break;
                    // 有效
                    case ComboStatus.Active:
                        EventPublisher.Publish<ComboSaleActiveMessage>(new ComboSaleActiveMessage
                        {
                            ComboSaleSysNo = sysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        break;
                    // 无效
                    case ComboStatus.Deactive:
                        EventPublisher.Publish<ComboSaleAuditRefuseMessage>(new ComboSaleAuditRefuseMessage
                        {
                            ComboSaleSysNo = sysNo.Value,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                        break;
                }
            });
        }


        public virtual void ApproveCombo(int? sysNo, ComboStatus targetStatus)
        {
            //Check审核人与创建人不能相同
            if (sysNo == null)
            {
                //throw new BizException("更新失败，参数有误！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Combo", "Combo_ArgsError"));
            }
            ComboInfo oldEntity = _da.Load(sysNo.Value);
            //if (oldEntity.Status == ComboStatus.WaitingAudit && oldEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    throw new BizException(string.Format("{0} 创建人与审核人不能相同", oldEntity.Name));
            //}

            TransactionScopeFactory.TransactionAction(() =>
            {
                this.UpdateStatus(sysNo, targetStatus);
                //更新活动状态 
                //ObjectFactory<ComboPromotionEngine>.Instance.UpdateComboActivityStatus(sysNo.Value, targetStatus);
            });
        }

        public virtual void BatchDelete(List<int> sysNoList)
        {
            if (sysNoList != null && sysNoList.Count > 0)
            {
                TransactionScopeFactory.TransactionAction(() =>
                {
                    sysNoList.ForEach(p =>
                    {
                        _da.UpdateStatus(p, ComboStatus.Deactive);
                        //更新活动状态 
                        //ObjectFactory<ComboPromotionEngine>.Instance.UpdateComboActivityStatus(p, ComboStatus.Deactive);
                    });
                });
            }
        }
        #endregion

        #region 验证Check
        /// <summary>
        /// 创建、更新，数据验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual List<string> CheckBasicIsPass(ComboInfo entity)
        {
            List<string> errList = new List<string>();
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(entity.Name.Content))
            {
                //errList.Add("规则名不能为空!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_RuleNameIsNotNull"));
            }
            if (entity.Name.Content.Trim().Length < 0 || entity.Name.Content.Trim().Length > 12)
            {
                // errList.Add("规则描述长度只能为12个字符!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_LessThanTwelveChar"));
            }
            if (entity.Priority < 0 || entity.Priority > 9999)
            {
                //errList.Add("优先级只能在0-9999范围之内的整数!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MustInRange"));
            }

            //if (entity.Items == null || entity.Items.Count < 2)
            //{
            //    //errList.Add("请至少添加2个商品！");
            //    errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MoreThanTwoProduct"));
            //}
            bool hasMain = entity.Items.Where(p=>p.IsMasterItemB==true).Count()>0;
            bool hasSub = entity.Items.Where(p => p.IsMasterItemB != true).Count() >0;
            if (!hasMain||!hasSub)
            {
                 //errList.Add("至少需要一个主商品和子商品");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_AtLeast1MainAnd1ChildProduct"));
            }

            return errList;
        }

        /// <summary>
        /// 更新保存时需要检查状态变化
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual List<string> CheckValidateWhenChangeStatus(ComboInfo entity)
        {
            List<string> errList = new List<string>();

            if (entity.Status.Value == ComboStatus.Deactive
                && entity.TargetStatus.Value == ComboStatus.Active)
            {
                if (!CheckPriceIsPass(entity))
                {
                    //errList.Add("差价大于成本（销售价格和 + 总折扣 < 成本价格和），请先提交审核！");
                    errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenPriceDiffrenceIsHigher"));
                }
                if (entity.Items.Find(f => f.IsMasterItemB.HasValue && f.IsMasterItemB.Value) == null)
                {
                    //throw new BizException("套餐必须有一个主商品！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MustOneMainGoods"));
                }

            }
            if (entity.Status.Value == ComboStatus.WaitingAudit && entity.TargetStatus.Value == ComboStatus.Active)
            {   //bug 127:要求添加验证
                //throw new BizException("待审核规则必须【审核通过】之后才能变为有效状态！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ActiveNeedAfterAuditPass"));
            }
            return errList;
        }

        /// <summary>
        /// 对Combo Item进行检查,本方法对添加ComboItem时也有用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual List<string> CheckComboItemIsPass(ComboInfo info)
        {
            List<string> errList = new List<string>();

            if (info.Items.Count > 1 && info.Items.FindAll(f => f.IsMasterItemB.Value).Count > 1)
            {
                //errList.Add("一个组合中必须有且只能有1个主商品！");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_OnlyOneProductInCombo"));
                return errList;
            }

            var grouplist = from p in info.Items
                            group p by p.ProductSysNo into g
                            select new { g.Key, num = g.Count() };

            var maxcount = (from p in grouplist
                            select p.num).Max();
            if (maxcount > 1)
            {
                // errList.Add("捆绑商品中存在重复的商品！");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_HasSameProductInCombo"));
                return errList;
            }
            decimal price = 0m;
            //每个商品的价格check
            foreach (ComboItem item in info.Items)
            {
                ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
                if (item.Discount > 0)
                {
                    //errList.Add(string.Format("商品{0}折扣只能为小于等于0的整数！", product.ProductID));
                    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_DisCountMustIntLessZero"), product.ProductID));
                }

                if (!item.Quantity.HasValue || item.Quantity.Value <= 0)
                {
                    //errList.Add(string.Format("商品{0}数量必须为大于0的整数！", product.ProductID));
                    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_QuntityMustIntAndThanZero"), product.ProductID));
                }

                item.MerchantName = product.Merchant != null ? product.Merchant.MerchantName : "";
                item.MerchantSysNo = product.Merchant != null ? product.Merchant.MerchantID : null;

                //确认用折扣后价格比较，折后价不能小于成本，会员价，批发价，限时抢购当前价
                decimal discountPrice = Math.Abs(item.Discount.Value);

                if (discountPrice > item.ProductCurrentPrice && item.ProductCurrentPrice > 0)
                {
                    //errList.Add(string.Format("商品{0}捆绑销售折扣小于成本价！", product.ProductID));
                    //errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessThanCost"), product.ProductID));
                    //errList.Add(string.Format("捆绑销售折扣大于当前价(商品： {0}------当前价：{1}) ", product.ProductID, product.ProductPriceInfo.CurrentPrice));
                    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_DiscountMoreThanCurrentPrice"), product.ProductID, product.ProductPriceInfo.CurrentPrice));
                }

                if (product.ProductPriceInfo.ProductWholeSalePriceInfo != null
                    && product.ProductPriceInfo.ProductWholeSalePriceInfo.Count() > 0)
                {
                    ProductWholeSalePriceInfo P1 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L1);
                    if (P1 != null && P1.Price < discountPrice)
                    {
                        //errList.Add(string.Format("商品{0}捆绑销售折扣小于团购价1！", product.ProductID));
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice1")
                                    , product.ProductID
                                    , discountPrice
                                    , P1.Price));
                    }
                    ProductWholeSalePriceInfo P2 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L2);
                    if (P2 != null && P2.Price < discountPrice)
                    {
                        //errList.Add(string.Format("商品{0}捆绑销售折扣小于团购价2！", product.ProductID));
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice2")
                                    , product.ProductID
                                    , discountPrice
                                    , P2.Price));
                    }
                    ProductWholeSalePriceInfo P3 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L3);
                    if (P3 != null && P3.Price < discountPrice)
                    {
                        //errList.Add(string.Format("商品{0}捆绑销售折扣小于团购价3！", product.ProductID));
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice3")
                                    , product.ProductID
                                    , discountPrice
                                    , P3.Price));
                    }
                }

                List<CountdownInfo> countDownList = ObjectFactory<CountdownProcessor>.Instance.GetCountDownByProductSysNo(item.ProductSysNo.Value);
                if (countDownList != null && countDownList.Count > 0)
                {
                    // || f.Status == CountdownStatus.Ready
                    CountdownInfo countdown = countDownList.Find(f => f.Status == CountdownStatus.Running);
                    if (countdown != null)
                    {
                        if (countdown.CountDownCurrentPrice.HasValue && countdown.CountDownCurrentPrice.Value < discountPrice)
                        {
                            //errList.Add(string.Format("商品{0}捆绑销售折扣小于就绪或运行中的限时抢购的最低价！", product.ProductID));
                            errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessThanRunning"), product.ProductID));
                        }
                    }
                }

                if (product.ProductPriceInfo.ProductRankPrice != null && product.ProductPriceInfo.ProductRankPrice.Count > 0)
                {
                    decimal? rankPrice = (from p in product.ProductPriceInfo.ProductRankPrice
                                          where p.Status == ProductRankPriceStatus.Active
                                          select p.RankPrice).Min();
                    if (rankPrice != null && rankPrice < discountPrice)
                    {
                        // errList.Add(string.Format("商品{0}捆绑销售折扣小于会员价！", product.ProductID));
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessThanVIP"), product.ProductID));
                    }
                }
            }

            if (!CheckItemMerchantIsSame(info))
            {
                //errList.Add("捆绑商品中存在供应商不同！");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_DifferentSupplierInCombo"));
                return errList;
            }

            return errList;
        }

        /// <summary>
        /// 检查Combo中商品是否是同一个Merchant，主要用于批量创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool CheckItemMerchantIsSame(ComboInfo info)
        {
            bool result = true;
            if (info.Items != null && info.Items.Count > 0)
            {
                var grouplist = from p in info.Items
                                group p by p.MerchantSysNo into g
                                select new { g.Key, num = g.Count() };

                if (grouplist.Count() > 1)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        #region 2012-11-02 Update  Method：CheckPriceIsPass
        /*
         *修改原因:需求变更
         *修改内容：设置产品的价格和+折扣<成本价格和  变为待审核状态.--这是以前的逻辑，现在改成只要有一个商品的毛利小于0，即变成待审核 
         */
        #endregion

        //设置产品的价格和+折扣<成本价格和  变为待审核状态
        //提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno，
        //然后检查商品对应捆绑规则是否有低于成本价的情况，有的就将其变为待审核(status=1)！
        /// <summary>
        /// 检查条件：如果（1）Combo当前是有效状态（2）价格和+折扣和 小于 成本价格和 ，价格检查不通过
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool CheckPriceIsPass(ComboInfo entity)
        {
            bool result = true;


            if (entity.Items != null && entity.Items.Count > 0)
            {
                foreach (ComboItem item in entity.Items)
                {
                    decimal totalPrice = 0.00m; //商品总价=单价*数量
                    decimal totalDiscount = 0.00m; //折扣==折扣*数量 折扣是负数
                    decimal totalCost = 0.00m; // 成本=成本价*数量
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
                    totalPrice += Math.Round(product.ProductPriceInfo.CurrentPrice.Value * item.Quantity.Value, 2);
                    totalDiscount += Math.Round(item.Discount.Value * item.Quantity.Value, 2);
                    totalCost += Math.Round(product.ProductPriceInfo.UnitCost * item.Quantity.Value, 2);
                    if (totalPrice + totalDiscount < totalCost)
                    {
                        return false;
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// 根据毛利率提交审核
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CheckMarginIsPass(ComboItem item)
        {
            decimal minMarin = 0;
            decimal minMarinH = 0;
            //SaleRuleDA.GetMinMarginByProductSysNo(item.ProductSysNo, item.CompanyCode, ref minMarin, ref minMarinH);

            //if (item.ProductUnitCost != 0)
            //{
            //    //计算毛利率
            //    decimal margin = GetMargin(item);
            //    //M1H < 毛利率 < M1 时，需要提交TL审核
            //    if (margin <= minMarin && minMarinH < margin)
            //    {
            //        return false;
            //    }
            //    //毛利率 < M1H 时，需要提交PMD审核（Type=P）
            //    if (minMarinH >= margin)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

        #endregion

        /// <summary>
        /// 用于计算能存在的Combo组合对象
        /// </summary>
        public class ComboApplyInstance
        {
            public int ComboSysNo { get; set; }
            public int Qty { get; set; }
            /// <summary>
            /// 总折扣，正数，取Combo Total Discount的绝对值
            /// </summary>
            public decimal TotalDiscount { get; set; }
        }

        /// <summary>
        /// 提供一个接口供商品价格管理模块来调用，传入商品sysno
        /// 然后检查商品对应捆绑规则是否有低于成本价的情况(价格和+折扣 《 成本价格和)，有的就将其变为待审核(status=1)
        /// </summary>
        /// <param name="productSysNo"></param>
        public virtual void CheckComboPriceAndSetStatus(int productSysNo)
        {
            //1.根据productSysNo得到所有包含了该Product的所有状态的Combo
            List<ComboInfo> comboList = _da.GetComboListByProductSysNo(productSysNo);
            if (comboList.Count == 0) return;
            //2.循环处理Combo，调用CheckIsPass函数，判断是否： 价格和+折扣和<成本价格和，有则将其变为待审核(status=1)，并记录LOG，发送邮件
            comboList = comboList.Where(o => o.Status == ComboStatus.Active).ToList<ComboInfo>();
            foreach (ComboInfo combo in comboList)
            {
                #region 需处理套餐与DIY的数据，即combo.RefernceType=1，2
                if (combo.ReferenceType != 1 && combo.ReferenceType != 2) { continue; }
                #endregion
                if (!CheckPriceIsPass(combo))
                {
                    if (combo.Status == ComboStatus.Active) //有效状态改为待审核，无效和待审核不做处理
                    {
                        _da.UpdateStatus(combo.SysNo, ComboStatus.WaitingAudit);
                    }
                    if (combo.RequestSysNo > 0)
                    {
                        combo.Status = ComboStatus.Deactive;
                        _da.SyncSaleRuleStatus(combo.RequestSysNo, combo.Status);
                    }
                    //string note = "捆绑销售编号 " + combo.SysNo.Value.ToString() + ",status=" + combo.Status.ToDisplayText() + "状态改为待审核。";
                    string note = ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboMKTSysNo") + combo.SysNo.Value.ToString() + ",status=" + combo.Status.ToDisplayText() + ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_StatusTransToCheckPending");
                    ExternalDomainBroker.CreateOperationLog(note, BizLogType.Basic_Product_Price_Update_Verify, combo.SysNo.Value, combo.CompanyCode);
                    SendMail(combo);
                    ExternalDomainBroker.CreateOperationLog(BizLogType.ComboCheckPriceAndSetStatus.ToEnumDesc(), BizLogType.ComboCheckPriceAndSetStatus, combo.SysNo.Value, combo.CompanyCode);

                }
            }


        }

        public virtual List<ComboInfo> GetActiveAndWaitingComboListByProductSysNo(List<int> sysNos)
        {
            List<ComboInfo> resultList = new List<ComboInfo>();

            foreach (var sysno in sysNos)
            {
                //根据productSysNo得到所有包含了该Product的所有状态的Combo
                resultList.AddRange(_da.GetComboListByProductSysNo(sysno));
            }
            resultList = resultList.Where(o => o.Status == ComboStatus.Active || o.Status == ComboStatus.WaitingAudit).ToList<ComboInfo>();
            return resultList;
        }

        public virtual List<string> CheckOptionalAccessoriesItemAndDiys(List<int> sysNos)
        {
            List<string> resultMsg = new List<string>();

            List<OptionalAccessoriesItem> oaItemList = ObjectFactory<OptionalAccessoriesProcessor>.Instance.GetActiveAndWaitingItemListByProductSysNo(sysNos);

            if (oaItemList.Count() > 0)
            {
                string masterItemID = string.Empty;
                foreach (var item in oaItemList)
                {
                    masterItemID = ObjectFactory<IOptionalAccessoriesDA>.Instance.Load(item.OptionalAccessoriesSysNo.Value).Items
                                        .Where(o => o.IsMasterItemB.Value).Select(i => i.ProductID).Join(",");
                    //resultMsg.Add(string.Format("{2}已存在于随心配编号{0}中，折扣为{1}元，主商品为{3} ",
                    //        item.OptionalAccessoriesSysNo, item.Discount, item.ProductID, masterItemID));
                    resultMsg.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExisitOptionalAccessorie"),
                            item.OptionalAccessoriesSysNo, item.Discount, item.ProductID, masterItemID));
                }
            }

            if (resultMsg.Count() > 0)
            {
                //resultMsg.Add("请确认是否继续。");
                resultMsg.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ConfirmIsGoOn"));
            }
            return resultMsg;
        }

        /// <summary>
        /// 发送邮件通知PM Combo的因商品调价，状态已改
        /// </summary>
        /// <param name="combo"></param>
        protected virtual void SendMail(ComboInfo combo)
        {
            UserInfo user = ExternalDomainBroker.GetUserInfoBySysNo(combo.CreateUserSysNo.Value);
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("ComboSysNo", combo.SysNo.Value.ToString());
            keyValueVariables.Add("ComboName", combo.Name.Content);
            keyValueVariables.Add("PMUser", user.UserDisplayName);
            EmailHelper.SendEmailByTemplate(user.EmailAddress,
                "MKT_Combo_ChangeStatusForChangeProductPrice", keyValueVariables, true);
        }
    }
}
