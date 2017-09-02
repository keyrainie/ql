using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using System.ComponentModel.Composition;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.Service.IBizInteract;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.BizProcessor 
{

    [VersionExport(typeof(BatchCreateSaleGiftProcessor))]
    public class BatchCreateSaleGiftProcessor 
    {
        private IBatchCreateSaleGiftDA _BatchCreateSaleGiftDA = ObjectFactory<IBatchCreateSaleGiftDA>.Instance;

        private ISaleGiftDA _SaleGiftDA = ObjectFactory<ISaleGiftDA>.Instance;

        private ISaleGiftQueryDA _SaleGiftQueryDA = ObjectFactory<ISaleGiftQueryDA>.Instance;

        /// <summary>
        /// 从IMDomain获取商品及赠品的商品信息，主要是状态信息。
        /// </summary>
        private void InitProductInfosFromIM(SaleGiftBatchInfo saleGiftBatchInfo)
        {
            List<int> productSysNos = new List<int>();

            if (saleGiftBatchInfo.ProductItems1 != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.ProductItems1)
                {
                    productSysNos.Add(item.ProductSysNo.Value);
                }

            }

            if (saleGiftBatchInfo.ProductItems2 != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.ProductItems2)
                {
                    productSysNos.Add(item.ProductSysNo.Value);
                }

            }

            if (saleGiftBatchInfo.Gifts != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.Gifts)
                {
                    productSysNos.Add(item.ProductSysNo.Value);
                }

            }

            List<ProductInfo> products = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNos);

            //将商品的相关信息，状态、价格信息等赋值给Item.
            RetryFunc<ProductItemInfo,int> copyItemInfo= (ProductItemInfo item)=>{
                ProductInfo swapItem = products.Find(p => p.SysNo == item.ProductSysNo);
                    if (swapItem != null)
                    {
                        item.ProductStatus = swapItem.ProductStatus;
                        item.ProductID = swapItem.ProductID;
                        item.ProductName = swapItem.ProductName;
                        item.CurrentPrice=swapItem.ProductPriceInfo.CurrentPrice.Value;
                        item.C3SysNo = swapItem.ProductBasicInfo.ProductCategoryInfo.SysNo;
                        item.BrandSysNo = swapItem.ProductBasicInfo.ProductBrandInfo.SysNo;
                    }

                return 0;
            };

            

            if (saleGiftBatchInfo.ProductItems1 != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.ProductItems1)
                {
                    copyItemInfo(item);
                }

            }

            if (saleGiftBatchInfo.ProductItems2 != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.ProductItems2)
                {
                    copyItemInfo(item);
                }

            }

            if (saleGiftBatchInfo.Gifts != null)
            {
                foreach (ProductItemInfo item in saleGiftBatchInfo.Gifts)
                {
                    copyItemInfo(item);
                }

            }


        }

        /// <summary>
        /// 批量创建赠品。
        /// </summary>
        /// <param name="saleGiftBatchInfo"></param>
        public void BatchCreateSaleGift(SaleGiftBatchInfo saleGiftBatchInfo)
        {
            if (saleGiftBatchInfo == null)
            {
                return;
            }


            //从IMDomain获取商品及赠品的商品信息，主要是状态信息。
            InitProductInfosFromIM(saleGiftBatchInfo);

            //厂商赠品的赠品不能作为其它赠品活动的赠品
            List<string> errorMsgList = this.CheckVendorGiftRules(saleGiftBatchInfo);
            if (errorMsgList.Count > 0)
            {
                throw new BizException(errorMsgList.Join(Environment.NewLine));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (saleGiftBatchInfo.Gifts == null || saleGiftBatchInfo.Gifts.Count <= 0 || saleGiftBatchInfo.Gifts.Count > 8)
                    {
                        //throw new Exception("赠品必须设置1-8之间");
                        throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GiftNeedBeteew1And8"));

                    }
                    if (saleGiftBatchInfo.SaleGiftType==SaleGiftType.Single)
                    {
                        #region 单品买赠
                        if (saleGiftBatchInfo.ProductItems1 == null || saleGiftBatchInfo.ProductItems1.Count <= 0 || saleGiftBatchInfo.ProductItems1.Count > 30)
                        {
                            //throw new BizException("商品必须设置1-30之间");
                            throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_QuantityNeedBeteew1And30"));
                        }
                        foreach (ProductItemInfo p in saleGiftBatchInfo.ProductItems1)
                        {
                            List<ProductItemInfo> list = new List<ProductItemInfo>();
                            list.Add(p);
                            CreateGiftAndRules(saleGiftBatchInfo, list);//根据购买的商品，赠品信息创建赠品规则信息。单品买赠及厂商赠品是，商品为单个，同时买赠时商品为多个。
                        }
                        #endregion
                    }
                    else if (saleGiftBatchInfo.SaleGiftType == SaleGiftType.Vendor)
                    {
                        #region 厂商赠品
                        if (saleGiftBatchInfo.ProductItems1 == null || saleGiftBatchInfo.ProductItems1.Count <= 0 || saleGiftBatchInfo.ProductItems1.Count > 30)
                        {
                            //throw new BizException("商品数量必须设置1-30之间");
                            throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_QuantityNeedBeteew1And30"));
                        }
                        foreach (ProductItemInfo p in saleGiftBatchInfo.ProductItems1)
                        {
                            List<ProductItemInfo> list = new List<ProductItemInfo>();
                            list.Add(p);
                            CreateGiftAndRules(saleGiftBatchInfo, list);//根据购买的商品，赠品信息创建赠品规则信息。单品买赠及厂商赠品是，商品为单个，同时买赠时商品为多个。
                        }
                        #endregion
                    }
                    else
                    {
                        #region 同时购买
                        if (saleGiftBatchInfo.ProductItems1 == null || saleGiftBatchInfo.ProductItems1.Count <= 0 || saleGiftBatchInfo.ProductItems1.Count > 30)
                        {
                            //throw new BizException("左侧商品数量必须设置1-30之间");
                            throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_LeftQuantityNeedBeteew1And30"));
                        }
                        if (saleGiftBatchInfo.ProductItems2 == null || saleGiftBatchInfo.ProductItems2.Count < 0 || saleGiftBatchInfo.ProductItems2.Count > 30)
                        {
                            //throw new BizException("右侧商品数量必须设置1-30个");
                            throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RightQuantityNeedBeteew1And30"));
                        }
      
                        //是组合捆绑还是交叉捆绑
                        if (saleGiftBatchInfo.CombineType==SaleGiftCombineType.Assemble)
                        {
                            if (saleGiftBatchInfo.ProductItems2.Count > 11)
                            {
                                //throw new BizException("当组合类型为组合捆绑右侧商品最多只能设置11个");
                                throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RightQuantityNeed11"));
                            }
                            foreach (ProductItemInfo p in saleGiftBatchInfo.ProductItems1)
                            {
                                List<ProductItemInfo> collection = new List<ProductItemInfo>();
                                collection.AddRange(saleGiftBatchInfo.ProductItems2);
                                collection.Add(p);
                                CreateGiftAndRules(saleGiftBatchInfo, collection);//根据购买的商品，赠品信息创建赠品规则信息。单品买赠及厂商赠品是，商品为单个，同时买赠时商品为多个。
                            }
                        }
                        else
                        {
                            if (saleGiftBatchInfo.ProductItems2.Count > 30)
                            {
                                //throw new BizException("右侧商品必须设置1-30之间");
                                throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RightQuantityNeedBeteew1And30"));
                            }
                            foreach (ProductItemInfo left in saleGiftBatchInfo.ProductItems1)
                            {
                                foreach (ProductItemInfo right in saleGiftBatchInfo.ProductItems2)
                                {
                                    List<ProductItemInfo> collection = new List<ProductItemInfo>();
                                    collection.Add(right);
                                    collection.Add(left);
                                    CreateGiftAndRules(saleGiftBatchInfo, collection);//根据购买的商品，赠品及赠品规则信息。单品买赠及厂商赠品是，商品为单个，同时买赠时商品为多个。
                                }
                            }
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    throw new BizException(ex.Message);
                }

                ts.Complete();
            }

            //return saleGiftBatchInfo;
        }

        private void ValidatePromotion(SaleGiftBatchInfo promotionInfo)
        {
            if (promotionInfo.RuleName==null||string.IsNullOrEmpty(promotionInfo.RuleName.Content))
            {
                //throw new BizException("规则名称不能为空！");
                throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RuleNameRequired"));
            }

            if (promotionInfo.BeginDate == null)
            {
                //throw new BizException("请输入开始时间！");
                throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_NeedStartDate"));
            }

            if (promotionInfo.EndDate == null)
            {
                //throw new BizException("请输入结束时间！");
                throw new Exception(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_NeedEndDate"));
            }


        }

        /// <summary>
        /// 根据购买的商品，赠品信息创建赠品规则信息。单品买赠及厂商赠品时，商品为单个，同时买赠时商品为多个。
        /// </summary>
        /// <param name="promotionInfo"></param>
        /// <param name="productList"></param>
        private void CreateGiftAndRules(SaleGiftBatchInfo promotionInfo, List<ProductItemInfo> productList)
        {
            PSOrderCondition orderCondition = new PSOrderCondition();
            orderCondition.OrderMinAmount=0M;
            //Create Gift Master
            int? giftMasterSysNo = _SaleGiftDA.CreateMaster(new SaleGiftInfo
            {
               Title=promotionInfo.RuleName,
               Description=promotionInfo.RuleDescription,
               Type=promotionInfo.SaleGiftType,
               Status=promotionInfo.Status,
               BeginDate=promotionInfo.BeginDate,
               EndDate=promotionInfo.EndDate,
               OrderCondition=orderCondition,
               PromotionLink=promotionInfo.PromotionLink,
               InUser=promotionInfo.InUser,
               DisCountType=promotionInfo.RebateCaculateMode,
               VendorSysNo = promotionInfo.VendorSysNo,
               CompanyCode=promotionInfo.CompanyCode

            });
            //Generate SaleRules
            UpdateSaleRules(new BatchCreateSaleGiftSaleRuleInfo
            {
                IsGlobal =true,
                ProductList = productList,
                PromotionSysNo = giftMasterSysNo.Value,
                InUser=promotionInfo.InUser,
                CompanyCode=promotionInfo.CompanyCode
            });

            //Create Gifts
            CreateGiftRules(new BatchCreateGiftRuleInfo
            {
                GiftComboType = promotionInfo.IsSpecifiedGift?SaleGiftGiftItemType.AssignGift:SaleGiftGiftItemType.GiftPool,
                ProductList = promotionInfo.Gifts,
                SumCount = promotionInfo.TotalQty,
                PromotionSysNo = giftMasterSysNo.Value,
                IsSpecial =0,
                InUser=promotionInfo.InUser,
                CompanyCode=promotionInfo.CompanyCode
            });
        }

        /// <summary>
        /// 检查赠品信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void CheckGiftRules(BatchCreateGiftRuleInfo info)
        {
            if (info == null || info.ProductList == null || info.ProductList.Count == 0) return;
            var products =
                (from s in info.ProductList where (new decimal?[] { 0, 999999 }).Contains(s.CurrentPrice) select s).Select
                    (p => p.ProductID).ToArray();
            if (products.Length > 0)
            {
                //string errorMsg = "当前价格为0或999999的商品不能作为赠品 商品编号为" + products.Join(",");
                string errorMsg = ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GoodsNotAsGift") + products.Join(",");
                throw new BizException(errorMsg);
            }
        }

        #region 创建赠品信息
        private BatchCreateGiftRuleInfo CreateGiftRules(BatchCreateGiftRuleInfo info)
        {
            string msg = string.Empty;
            int special = 0;//是否特殊规则1是0否
            SaleGiftInfo g = _SaleGiftDA.Load(info.PromotionSysNo);
            int vendorsysno = g.VendorSysNo.Value;
            if (g.VendorType == 0)
            {
                vendorsysno = 1;
            }
            CheckGiftRules(info);

            foreach (ProductItemInfo entity in info.ProductList)
            {
                //只有状态为2(不展示)的赠品才允许插入
                if (entity.ProductStatus == ProductStatus.InActive_UnShow)
                {
                    int count = _BatchCreateSaleGiftDA.CheckGiftRulesForVendor(entity.ProductSysNo, g.Type==SaleGiftType.Vendor?true:false,info.CompanyCode);

                    if (count > 0)
                    {
                        //msg += string.Format("赠品{0}[#:{1}]存在有效记录 ", entity.ProductName, entity.ProductID);
                        msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AlreadyExsistActiceRecord"), entity.ProductName, entity.ProductID);
                        continue;
                    }

                    //判断是否为附件
                    bool isAttachment = ExternalDomainBroker.CheckIsAttachment(entity.ProductSysNo.Value);
                    if (isAttachment)
                    {
                        //msg += string.Format("商品{0}[#:{1}]已经设置成附件,不能设置为赠品 ", entity.ProductName,entity.ProductID);
                        msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AccessoryCannotBeGift"), entity.ProductName, entity.ProductID);
                        continue;
                    }


                    //如果是赠品池，把赠送数量置为NULL
                    if (info.GiftComboType == SaleGiftGiftItemType.GiftPool)
                    {
                        entity.HandselQty = null;
                    }
                    _BatchCreateSaleGiftDA.CreateGiftRules(entity, info);
                }
                else
                {
                    //msg += string.Format("赠品{0}[#:{1}]必须为不展示状态 ", entity.ProductName, entity.ProductID);
                    msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AccessoryCannotBeGift"), entity.ProductName, entity.ProductID);
                }
                //检查不能添加其他商家的商品
                if (entity.ProductSysNo.HasValue)
                {
                    int productvendorsysno = _da.GetVendorSysNoByProductSysNo(entity.ProductSysNo.Value);
                    if (productvendorsysno != vendorsysno)
                    {
                        //msg += string.Format("赠品信息中{0}不能添加其他商家的商品【{1}】 ", g.VendorName, entity.ProductID);
                        msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AccessoryCannotBeGift"), entity.ProductName, entity.ProductID);
                    }
                }

            }
            if (msg != string.Empty)
            {
                throw new BizException(msg);
            }
            else
            {

                _BatchCreateSaleGiftDA.UpdateItemGiftCouontGiftRules(info.PromotionSysNo, (info.GiftComboType==SaleGiftGiftItemType.AssignGift ? null : info.SumCount), info.GiftComboType.Value, info.CompanyCode, info.InUser, special);
            }
            return info;
        }
        #endregion

        private ISaleGiftDA _da = ObjectFactory<ISaleGiftDA>.Instance;
        #region 更新规则信息
        private void UpdateSaleRules(BatchCreateSaleGiftSaleRuleInfo info)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    bool isCheckRule = false;//是否验证,至少有一条包含规则
                    List<ProductItemInfo> list = _SaleGiftQueryDA.GetSaleRules(info.PromotionSysNo, info.CompanyCode);
                    if (list.Count != 0)
                    {
                        isCheckRule = true;
                    }

                    SaleGiftInfo tempEntity = _da.Load(info.PromotionSysNo);
                    int vendorsysno = tempEntity.VendorSysNo.Value;
                    if (tempEntity.VendorType == 0)
                    {
                        vendorsysno = 1;
                    }

                    
                    //先清空数据
                    _BatchCreateSaleGiftDA.DeleteSaleRules(info.PromotionSysNo.ToString());
                    string msg = string.Empty;
                    //再添加
                    if (info.ProductList != null && info.ProductList.Count > 0)
                    {
                        foreach (ProductItemInfo entity in info.ProductList)
                        {

                            //只有状态为1(上架)的商品才允许插入
                            if (entity.ProductStatus != ProductStatus.Active)
                            {
                                //msg += string.Format("商品{0}[#:{1}]必须为上架状态 ", entity.ProductName, entity.ProductID);
                                msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ProductNeedOnline"), entity.ProductName, entity.ProductID);
                            }
                            //如果是厂商赠品必须是排重的
                            SaleGiftInfo gift = _SaleGiftDA.Load(info.PromotionSysNo);
                            if (gift.Type == SaleGiftType.Vendor)
                            {
                                if (_BatchCreateSaleGiftDA.CheckIsVendorGift(entity.ProductSysNo.Value, info.CompanyCode) > 0)
                                {
                                    //msg += string.Format("厂商主商品{0}[#:{1}]存在有效的重复的记录 ", entity.ProductName, entity.ProductID);
                                    msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_PrimaryProductExsisit"), entity.ProductName, entity.ProductID);
                                }
                            }
                            //检查不能添加其他商家的商品
                            if (entity.ProductSysNo.HasValue)
                            {
                                int productvendorsysno = _da.GetVendorSysNoByProductSysNo(entity.ProductSysNo.Value);
                                if (productvendorsysno != vendorsysno)
                                {
                                   //msg +=string.Format("规则信息中{0}不能添加其他商家的商品【{1}】 ", tempEntity.VendorName, entity.ProductID);
                                    msg += string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_CanntAddOtherMerchantProduct"), entity.ProductName, entity.ProductID);
                                }
                            }
                            _BatchCreateSaleGiftDA.CreateSaleRules(info, entity);
                            isCheckRule = true;
                        }
                    }
                   
                    if (msg != string.Empty)
                    {
                        throw new BizException(msg);
                    }
                    //更新是否全网
                    _BatchCreateSaleGiftDA.UpdateGiftIsGlobal(info.PromotionSysNo, "N", info.CompanyCode, info.InUser);
                    //是否有包含商品
                    if (isCheckRule)
                    {
                        CheckIsHaveInclude(false, info.PromotionSysNo);
                    }
                }
                catch (Exception ex)
                {
                    throw new BizException(ex.Message);
                }
                ts.Complete();
            }

        }



        /// <summary>
        /// 判断是否有包含的商品
        /// </summary>
        /// <param name="isGlobal"></param>
        /// <param name="promotionSysNo"></param>
        private void CheckIsHaveInclude(bool isGlobal, int promotionSysNo)
        {
            //赠品批量创建的商品都为全网商品，不需要check.
        }


        /// <summary>
        /// Check 厂商赠品的赠品不能作为其它赠品活动的赠品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private List<String> CheckVendorGiftRules(SaleGiftBatchInfo info)
        {
            List<SaleGiftInfo> giftInfoListTmp = new List<SaleGiftInfo>();
            List<String> msgTmp = new List<String>();
            foreach (var item in info.Gifts)
            {
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Run));
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Ready));
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.WaitingAudit));

                if (info.SaleGiftType == SaleGiftType.Vendor)
                {
                    if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && p.Type != SaleGiftType.Vendor).ToList().Count > 0)
                    {
                        //msgTmp.Add(string.Format("厂商赠品 ({0}) 在其他赠品活动中存在有效的重复的记录！", ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        msgTmp.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ExsisitActivityInOther"), ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        continue;
                    }
                }
                else
                {
                    if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && p.Type == SaleGiftType.Vendor).ToList().Count > 0)
                    {
                        //msgTmp.Add(string.Format("赠品 ({0}) 在厂商赠品活动中存在有效的重复的记录！", ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        msgTmp.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ExsisitActivityInMerchant"), ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        continue;
                    }
                }
            }
            return msgTmp;
        }

        #endregion
         
    }
}
