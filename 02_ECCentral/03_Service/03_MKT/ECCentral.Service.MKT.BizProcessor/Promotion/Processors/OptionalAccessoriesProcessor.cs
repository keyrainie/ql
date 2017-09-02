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
using ECCentral.Service.Utility;
//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;

namespace ECCentral.Service.MKT.BizProcessor.Promotion.Processors
{
    [Export(typeof(IPromotionCalculate))]
    [VersionExport(typeof(OptionalAccessoriesProcessor))]
    public class OptionalAccessoriesProcessor : CalculateBaseProcessor
    {
        private IOptionalAccessoriesDA _da = ObjectFactory<IOptionalAccessoriesDA>.Instance;
        //private OptionalAccessoriesPromotionEngine _oaPromotionEngine = ObjectFactory<OptionalAccessoriesPromotionEngine>.Instance;

        #region 维护类行为
        /// <summary>
        /// 加载OptionalAccessories所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual OptionalAccessoriesInfo Load(int? sysNo)
        {
            OptionalAccessoriesInfo info = _da.Load(sysNo.Value);
            if (info == null)
            {
                //throw new BizException("随心配不存在！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_NotExsistOptionalAccessories"));
            }

            #region 获取待审核规则的商品毛利Check信息，供页面显示
            if (info.Status == ComboStatus.WaitingAudit)
            {
                info.DisplayApproveMsg = new List<string>();
                foreach (var item in info.Items)
                {
                    if (CheckMarginIsPass(item))
                    {
                        //info.DisplayApproveMsg.Add(string.Format("{0} 商品折扣价毛利低于PMCC毛利标准", item.ProductID));
                        info.DisplayApproveMsg.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_DiscountLowerPMCC"), item.ProductID));
                    }
                }
            }
            #endregion

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            return info;
        }

        public virtual List<OptionalAccessoriesItem> GetActiveAndWaitingItemListByProductSysNo(List<int> sysNos)
        {
            List<OptionalAccessoriesItem> resultList = new List<OptionalAccessoriesItem>();

            foreach (var sysno in sysNos)
            {
                //根据productSysNo得到所有包含了该Product的所有状态的Combo
                resultList.AddRange(_da.GetOptionalAccessoriesItemListByProductSysNo(sysno, 0));
            }
            return resultList.Where(o => !o.IsMasterItemB.Value).ToList();
        }

        /// <summary>
        /// 创建OptionalAccessories
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            List<string> errorList = CheckBasicIsPass(info);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }

            errorList = CheckOptionalAccessoriesItemIsPass(info, false);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }
            if (info.Status == null)
            {
                info.Status = ComboStatus.Deactive;
            }

            TransactionScopeFactory.TransactionAction(() =>
            {
                info.SysNo = _da.CreateMaster(info);
                foreach (OptionalAccessoriesItem item in info.Items)
                {
                    item.OptionalAccessoriesSysNo = info.SysNo;
                    _da.AddOptionalAccessoriesItem(item);
                }
                //将数据保存到PromotionEngine配置库中
                //_oaPromotionEngine.SaveActivity(info);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.OptionalAccessories_Create.ToEnumDesc(), BizLogType.OptionalAccessories_Create, info.SysNo.Value, info.CompanyCode);


            return info.SysNo;
        }

        /// <summary>
        /// 更新OptionalAccessories Master，包含：更新主信息，更新状态：
        /// 无效->有效,无效->待审核，有效->无效，有效->待审核,待审核->无效，待审核->有效
        /// 其中无效->有效需要Check RequiredSaleRule4UpdateValidate
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            List<string> errorList = CheckBasicIsPass(info);
            if (errorList.Count > 0)
            {
                throw new BizException(errorList.Join("\r\n"));
            }

            errorList = CheckOptionalAccessoriesItemIsPass(info, false);
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
                _da.DeleteOptionalAccessoriesAllItem(info.SysNo.Value);
                foreach (OptionalAccessoriesItem item in info.Items)
                {
                    item.OptionalAccessoriesSysNo = info.SysNo;
                    _da.AddOptionalAccessoriesItem(item);
                }
                //将数据保存到PromotionEngine配置库中
                //_oaPromotionEngine.SaveActivity(info);

                ExternalDomainBroker.CreateOperationLog(
               String.Format("{0}{1}SysNo:{2}| 规则描述:{3}| 优先级:{4} | 状态:{5}",
               DateTime.Now.ToString(), "更新随心配"
               , info.SysNo, info.Name, info.Priority
               , info.Status == ComboStatus.Active ? "有效"
               : info.Status == ComboStatus.Deactive ? "无效" : "待审核")
               , BizEntity.Common.BizLogType.OptionalAccessories_Update
               , info.SysNo.Value, info.CompanyCode);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.OptionalAccessories_Update.ToEnumDesc(), BizLogType.OptionalAccessories_Update, info.SysNo.Value, info.CompanyCode);

        }

        /// <summary>
        /// 调用Combo更新状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="targetStatus"></param>
        public virtual void ApproveOptionalAccessories(int? sysNo, ComboStatus targetStatus)
        {
            //Check审核人与创建人不能相同
            if (sysNo == null)
            {
                //throw new BizException("更新失败，参数有误！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo","Combo_ArgsError"));
            }
            OptionalAccessoriesInfo oldEntity = _da.Load(sysNo.Value);
            //if (oldEntity.Status == ComboStatus.WaitingAudit && oldEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    throw new BizException(string.Format("{0} 创建人与审核人不能相同", oldEntity.Name));
            //}

            TransactionScopeFactory.TransactionAction(() =>
            {
                ObjectFactory<ComboProcessor>.Instance.UpdateStatus(sysNo, targetStatus);
                //更新PromotionEngine配置中活动的状态
                //_oaPromotionEngine.UpdateActivityStatus(sysNo.Value, targetStatus);

                ExternalDomainBroker.CreateOperationLog(
                String.Format("{0}{1}SysNo:{2}| 状态:{3}",
                DateTime.Now.ToString(), "审核状态", sysNo
                , targetStatus == ComboStatus.Active ? "有效"
                : targetStatus == ComboStatus.Deactive ? "无效" : "待审核")
                , BizEntity.Common.BizLogType.OptionalAccessories_Approve
                , sysNo.Value, "8601");//[Mark][Alan.X.Luo 硬编码]
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
                    //更新PromotionEngine配置中活动的状态
                    //_oaPromotionEngine.UpdateActivityStatus(p, ComboStatus.Deactive);

                    ExternalDomainBroker.CreateOperationLog(
                    String.Format("{0}{1}SysNo:{2}",
                    DateTime.Now.ToString(), "删除", p)
                    , BizEntity.Common.BizLogType.OptionalAccessories_Del
                    , p, "8601");//[Mark][Alan.X.Luo 硬编码]
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
        protected virtual List<string> CheckBasicIsPass(OptionalAccessoriesInfo entity)
        {
            List<string> errList = new List<string>();
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(entity.Name.Content))
            {
                //errList.Add("规则名不能为空!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_RuleNameIsNotNull"));
            }
            if (entity.Name.Content.Trim().Length < 0 || entity.Name.Content.Trim().Length > 500)
            {
                // errList.Add("规则描述长度不能超过500个字符!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_LessCharLength"));
            }
            if (entity.Priority < 0 || entity.Priority > 9999)
            {
                //errList.Add("优先级只能在0-9999范围之内的整数!");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MustInRange"));
            }

            if (entity.Items == null || entity.Items.Count < 2)
            {
                //errList.Add("请至少添加2个商品！");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MoreThanTwoProduct"));
            }

            return errList;
        }

        /// <summary>
        /// 更新保存时需要检查状态变化
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual List<string> CheckValidateWhenChangeStatus(OptionalAccessoriesInfo entity)
        {
            List<string> errList = new List<string>();

            if (entity.TargetStatus.Value == ComboStatus.Active)
            {
                //if (!CheckPriceIsPass(entity))
                //{
                //    //errList.Add("差价大于成本（销售价格和 + 总折扣 < 成本价格和），请先提交审核！");
                //    errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenPriceDiffrenceIsHigher"));
                //}

                foreach (OptionalAccessoriesItem item in entity.Items)
                {
                    item.OptionalAccessoriesSysNo = entity.SysNo;
                    if (CheckMarginIsPass(item))
                    {
                        // errList.Add(string.Format("商品{0}毛利率小于最低毛利率，请提交审核！", item.ProductID));
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ExamineFirstWhenRateOfMarginIsHigher"), item.ProductID));
                    }
                }

                if (entity.Items.Find(f => f.IsMasterItemB.HasValue && f.IsMasterItemB.Value) == null)
                {
                    //throw new BizException("套餐必须有一个主商品！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_MustOneMainGoods"));
                }

            }
            return errList;
        }

        /// <summary>
        /// 对OptionalAccessories Item进行检查,本方法对添加OptionalAccessoriesItem时也有用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual List<string> CheckOptionalAccessoriesItemIsPass(OptionalAccessoriesInfo info, bool isCreate)
        {
            List<string> errList = new List<string>();

            if (!isCreate && info.Items.Count > 1 && info.Items.FindAll(f => f.IsMasterItemB.Value).Count < 1)
            {
                //errList.Add("一个随心配中必须有至少1个主商品！");
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_LeastOneMasterProduct"));
                return errList;
            }
            if (!isCreate && info.Items.Count > 1 && info.Items.FindAll(f => !f.IsMasterItemB.Value).Count < 1)
            {
                errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_LeastOneSubProduct"));
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

            //每个商品的价格check
            foreach (OptionalAccessoriesItem item in info.Items)
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

                if (item.IsMasterItemB.Value && !isCreate)
                {
                    List<OptionalAccessoriesItem> itemListTmp = _da.GetOptionalAccessoriesItemListByProductSysNo(item.ProductSysNo.Value, item.SysNo == null ? 0 : item.SysNo.Value)
                                                    .Where(o => o.IsMasterItemB.Value).ToList();
                    itemListTmp.RemoveAll(obj => obj.OptionalAccessoriesSysNo == info.SysNo);
                    if (itemListTmp.Count() > 0)
                    {
                        //商品已存在于其他随心配规则的主商品中
                        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_HaveSameMasterProduct"), item.ProductID));
                    }
                }

                item.MerchantName = product.Merchant != null ? product.Merchant.MerchantName : "";
                item.MerchantSysNo = product.Merchant != null ? product.Merchant.MerchantID : null;


                #region 折扣后价格比较，会员价，批发价，限时抢购当前价

                //if (product.ProductPriceInfo.ProductWholeSalePriceInfo != null
                //    && product.ProductPriceInfo.ProductWholeSalePriceInfo.Count > 0)
                //{
                //    ProductWholeSalePriceInfo P1 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L1);
                //    if (P1 != null && P1.Price > discountPrice)
                //    {
                //        //errList.Add(string.Format("商品{0}捆绑销售折扣价格小于团购价1！", product.ProductID));
                //        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice1"), product.ProductID));
                //    }
                //    ProductWholeSalePriceInfo P2 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L2);
                //    if (P2 != null && P2.Price > discountPrice)
                //    {
                //        //errList.Add(string.Format("商品{0}捆绑销售折扣价格小于团购价2！", product.ProductID));
                //        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice2"), product.ProductID));
                //    }
                //    ProductWholeSalePriceInfo P3 = product.ProductPriceInfo.ProductWholeSalePriceInfo.FirstOrDefault(f => f.Level == WholeSaleLevelType.L3);
                //    if (P3 != null && P3.Price > discountPrice)
                //    {
                //        //errList.Add(string.Format("商品{0}捆绑销售折扣价格小于团购价3！", product.ProductID));
                //        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessWholeSalePrice3"), product.ProductID));
                //    }
                //}

                //List<CountdownInfo> countDownList = ObjectFactory<CountdownProcessor>.Instance.GetCountDownByProductSysNo(item.ProductSysNo.Value);
                //if (countDownList != null && countDownList.Count > 0)
                //{
                //    CountdownInfo countdown = countDownList.Find(f => f.Status == CountdownStatus.Running || f.Status == CountdownStatus.Ready);
                //    if (countdown != null)
                //    {
                //        if (countdown.CountDownCurrentPrice.HasValue && countdown.CountDownCurrentPrice.Value > discountPrice)
                //        {
                //            //errList.Add(string.Format("商品{0}捆绑销售折扣价格小于就绪或运行中的限时抢购的最低价！", product.ProductID));
                //            errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessThanRunning"),product.ProductID));
                //        }
                //    }
                //}

                //if (product.ProductPriceInfo.ProductRankPrice != null && product.ProductPriceInfo.ProductRankPrice.Count > 0)
                //{
                //    decimal? rankPrice = (from p in product.ProductPriceInfo.ProductRankPrice
                //                         select p.RankPrice).Min();
                //    if (rankPrice != null && rankPrice > discountPrice)
                //    {
                //       // errList.Add(string.Format("商品{0}捆绑销售折扣价格小于会员价！", product.ProductID));
                //        errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ComboPriceLessThanVIP"),product.ProductID));
                //    }
                //}
                #endregion
            }

            //if (!CheckItemMerchantIsSame(info))
            //{
            //    //errList.Add("捆绑商品中存在供应商不同！");
            //    errList.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_DifferentSupplierInCombo"));
            //    return errList;
            //}

            return errList;
        }

        public virtual List<string> CheckSaleRuleItemAndDiys(List<int> sysNos)
        {
            List<string> resultMsg = new List<string>();

            List<ComboInfo> comboList = ObjectFactory<ComboProcessor>.Instance.GetActiveAndWaitingComboListByProductSysNo(sysNos);

            if (comboList.Count() > 0)
            {
                string masterItemID = string.Empty;
                foreach (var combo in comboList)
                {
                    masterItemID = combo.Items.Where(i => i.IsMasterItemB.Value).Select(i => i.ProductID).Join(",");
                    foreach (var item in combo.Items.Where(i => !i.IsMasterItemB.Value))
                    {
                        if (sysNos.Contains(item.ProductSysNo.Value))
                        {
                            //resultMsg.Add(string.Format("{2}已存在于销售规则编号{0}中，折扣为{1}元，主商品为{3} ",
                            //    combo.SysNo, item.Discount, item.ProductID, masterItemID));
                            resultMsg.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Combo", "OptionalAccessories_AlreadyExsistSaleRule"),
                               combo.SysNo, item.Discount, item.ProductID, masterItemID));
                        }
                    }
                }
            }

            if (resultMsg.Count() > 0) { 
                //resultMsg.Add("请确认是否继续。"); 
                resultMsg.Add(ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_ConfirmIsGoOn")); 
            }
            return resultMsg;
        }

        /// <summary>
        /// 检查Combo中商品是否是同一个Merchant，主要用于批量创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool CheckItemMerchantIsSame(OptionalAccessoriesInfo info)
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

        //设置产品的价格和+折扣<成本价格和  变为待审核状态
        //提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno，
        //然后检查商品对应捆绑规则是否有低于成本价的情况，有的就将其变为待审核(status=1)！
        /// <summary>
        /// 检查条件：如果（1）Combo当前是有效状态（2）价格和+折扣和 小于 成本价格和 ，价格检查不通过
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool CheckPriceIsPass(OptionalAccessoriesInfo entity)
        {
            bool result = true;

            decimal totalPrice = 0.00m;
            decimal totalDiscount = 0.00m;
            decimal totalCost = 0.00m;
            if (entity.Items != null && entity.Items.Count > 0)
            {
                foreach (OptionalAccessoriesItem item in entity.Items)
                {
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
                    totalPrice += Math.Round(product.ProductPriceInfo.CurrentPrice.Value * item.Quantity.Value, 2);
                    totalDiscount += Math.Round(item.Discount.Value * item.Quantity.Value, 2);
                    totalCost += Math.Round(product.ProductPriceInfo.UnitCost * item.Quantity.Value, 2);
                }
                if (totalPrice + totalDiscount < totalCost)
                {
                    return false;
                }
            }

            return result;
        }

        /// <summary>
        /// Check毛利率提交审核
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CheckMarginIsPass(OptionalAccessoriesItem item)
        {
            string returnMsgStr = string.Empty;
            var productInfo = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
            ProductPriceRequestInfo priceMsg = new ProductPriceRequestInfo()
            {
                CurrentPrice = productInfo.ProductPriceInfo.CurrentPrice,
                UnitCost = productInfo.ProductPriceInfo.UnitCost,
                Point = productInfo.ProductPriceInfo.Point,
                Category = productInfo.ProductBasicInfo.ProductCategoryInfo
            };
            var _discount = this.GetOptionalAccessoriesDiscountByItem(item);
            List<ProductPromotionMarginInfo> marginList = ObjectFactory<IIMBizInteract>.Instance.GetProductPromotionMargin(
                                                            priceMsg, item.ProductSysNo.Value, "", _discount, ref returnMsgStr);
            //return marginList != null && marginList.Where(m => m.PromotionType == PromotionType.OptionalAccessories).Count() > 0;
            return !string.IsNullOrEmpty(returnMsgStr);
        }

        #endregion

        /// <summary>
        /// 获取随心配Item的折扣
        /// </summary>
        /// <param name="queryEntity"></param>
        /// <returns></returns>
        private decimal GetOptionalAccessoriesDiscountByItem(OptionalAccessoriesItem _item)
        {
            List<OptionalAccessoriesItem> _itemListTmp = null;
            decimal _totalGrossTmp = 0m;
            int cnt = 0;

            #region 获取商品所在随心配毛利
            //当前Item所在规则中的所有Item
            _itemListTmp = _da.GetOptionalAccessoriesItemListByOASysNo(_item.OptionalAccessoriesSysNo.Value);

            //当前规则的主商品数量
            cnt = _itemListTmp.Where(o => o.IsMasterItemB.Value).Count();
            if (cnt > 0)
            {
                if (_item.IsMasterItemB.Value)
                {
                    _totalGrossTmp = _itemListTmp.Where(o => !o.IsMasterItemB.Value)
                                    .Select(o => o.Discount.Value * (1 - o.DiscountPercent.Value)).Sum() / cnt;
                }
                else
                {
                    _totalGrossTmp = _itemListTmp.Where(o => o.ProductSysNo == _item.ProductSysNo)
                                    .Select(o => o.Discount.Value * o.DiscountPercent.Value).Sum();
                }
            }
            #endregion

            return Math.Abs(_totalGrossTmp);
        }

        #region 外部Service将访问

        /// <summary>
        /// 获取商品所有随心配的折扣
        /// </summary>
        /// <param name="queryEntity"></param>
        /// <returns></returns>
        public List<ProductPromotionDiscountInfo> GetOptionalAccessoriesDiscountListByProductSysNo(int productSysNo)
        {
            List<ProductPromotionDiscountInfo> oaDiscountList = new List<ProductPromotionDiscountInfo>();
            List<OptionalAccessoriesItem> _itemList = null;
            List<OptionalAccessoriesItem> _itemListTmp = null;
            decimal _totalGrossTmp = 0m;
            int cnt = 0;

            #region 获取商品所在所有随心配毛利率
            _itemList = _da.GetOptionalAccessoriesItemListByProductSysNo(productSysNo);

            foreach (OptionalAccessoriesItem _oai in _itemList)
            {
                //当前Item所在规则中的所有Item
                _itemListTmp = _da.GetOptionalAccessoriesItemListByOASysNo(_oai.OptionalAccessoriesSysNo.Value);

                //当前规则的主商品数量
                cnt = _itemListTmp.Where(o => o.IsMasterItemB.Value).Count();
                if (cnt > 0)
                {
                    if (_oai.IsMasterItemB.Value)
                    {
                        _totalGrossTmp = _itemListTmp.Where(o => !o.IsMasterItemB.Value)
                                        .Select(o => o.Discount.Value * (1 - o.DiscountPercent.Value)).Sum() / cnt;
                    }
                    else
                    {
                        _totalGrossTmp = _itemListTmp.Where(o => o.ProductSysNo == _oai.ProductSysNo)
                                        .Select(o => o.Discount.Value * o.DiscountPercent.Value).Sum();
                    }

                    oaDiscountList.Add(new ProductPromotionDiscountInfo()
                    {
                        PromotionType = PromotionType.OptionalAccessories,
                        Discount = Math.Abs(_totalGrossTmp),
                        ReferenceSysNo = _oai.OptionalAccessoriesSysNo.Value
                    });
                }
            }
            #endregion

            return oaDiscountList;
        }
        #endregion

        /// <summary>
        /// 发送邮件通知PM Combo的因商品调价，状态已改
        /// </summary>
        /// <param name="combo"></param>
        protected virtual void SendMail(OptionalAccessoriesInfo oa)
        {
            UserInfo user = ExternalDomainBroker.GetUserInfoBySysNo(oa.CreateUserSysNo.Value);
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("ComboSysNo", oa.SysNo.Value.ToString());
            keyValueVariables.Add("ComboName", oa.Name.Content);
            keyValueVariables.Add("PMUser", user.UserDisplayName);
            EmailHelper.SendEmailByTemplate(user.EmailAddress,
                "MKT_Combo_ChangeStatusForChangeProductPrice", keyValueVariables, true);
        }
    }
}
