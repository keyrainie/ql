using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECommerce.DataAccess.Promotion;
using ECommerce.Entity.Common;
using ECommerce.Entity.Promotion;
using ECommerce.Enums;
using ECommerce.Service.Product;
using ECommerce.Utility;

namespace ECommerce.Service.Promotion
{
    public class GiftPromotionService
    {
        /// <summary>
        /// 查询赠品促销列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<GiftPromotionListQueryResult> QueryGiftPromotionActivityList(GiftPromotionListQueryFilter queryFilter)
        {
            return GiftPromotionDA.QueryGiftPromotionActivityList(queryFilter);
        }

        /// <summary>
        /// 保存赠品促销活动信息(添加或者修改)
        /// </summary>
        /// <returns></returns>
        public static int SaveGiftPromotionInfo(SalesGiftInfo giftInfo, string editUserName)
        {
            bool isEdit = giftInfo.SysNo.HasValue ? true : false;

            #region [Check操作:]

            if ((giftInfo.ProductRuleList == null || giftInfo.ProductRuleList.Count <= 0) && giftInfo.IsGlobalProduct != true)
            {
                throw new BusinessException("请至少添加一项主商品规则!");
            }
            if (giftInfo.Type == SaleGiftType.Single && giftInfo.ProductRuleList.Count > 1)
            {
                throw new BusinessException("当选择主商品为\"整网商品\"时，无需再指定任何的主商品规则，请删除后再操作!");
            }
            if (giftInfo.GiftRuleList == null || giftInfo.GiftRuleList.Count <= 0)
            {
                throw new BusinessException("请至少添加一项赠品规则!");
            }
            foreach (var giftItem in giftInfo.GiftRuleList)
            {
                if (!giftItem.Count.HasValue || giftItem.Count.Value <= 0 || giftItem.Count.Value > 9999)
                {
                    throw new BusinessException(string.Format("赠品规则中的商品:{0},数量格式不正确，请检查，数量至少是1,最大是9999!", giftItem.ProductSysNo));
                }
            }

            if (!giftInfo.BeginDate.HasValue)
            {
                throw new BusinessException("请设置活动开始时间！");
            }
            if (!giftInfo.EndDate.HasValue)
            {
                throw new BusinessException("请设置活动结束时间！");
            }

            if (
               new DateTime(giftInfo.BeginDate.Value.Year, giftInfo.BeginDate.Value.Month, giftInfo.BeginDate.Value.Day) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                )
            {
                throw new BusinessException("设置的活动生效时间不能小于当前时间！");

            }
            if (giftInfo.BeginDate.Value > giftInfo.EndDate.Value)
            {
                throw new BusinessException("活动开始时间不能大于结束时间，请重新设置！");
            }

            //if (giftInfo.Type != SaleGiftType.Additional && giftInfo.OrderMinAmount.HasValue)
            //{
            //    throw new BusinessException("只有活动类型为\"满额加购\"时才能设置门槛金额！");
            //}
            if (giftInfo.ProductRuleList != null && giftInfo.ProductRuleList.Count > 0)
            {
                foreach (var item in giftInfo.ProductRuleList)
                {
                    var productInfo = ProductService.GetProductBySysNo(item.ProductSysNo.Value);
                    if (null == productInfo || productInfo.SysNo <= 0)
                    {
                        throw new BusinessException(string.Format("主商品规则中，商品编号{0}不是有效的商品！", item.ProductSysNo.Value));
                    }
                }
            }
            foreach (var item in giftInfo.GiftRuleList)
            {
                var productInfo = ProductService.GetProductBySysNo(item.ProductSysNo.Value);
                if (null == productInfo || productInfo.SysNo <= 0)
                {
                    throw new BusinessException(string.Format("赠品规则中，商品编号{0}不是有效的商品！", item.ProductSysNo.Value));
                }
                else if (productInfo.Status != ProductStatus.InActive_Show)
                {
                    throw new BusinessException(string.Format("赠品规则中，只有上架不展示商品才能设置为赠品，商品编号{0}状态不是上架不展示商品！", item.ProductSysNo.Value));
                }
            }


            SalesGiftInfo tempEntity = null;
            int vendorsysno = 1;
            if (isEdit)
            {
                tempEntity = GiftPromotionDA.LoadSalesGiftInfo(giftInfo.SysNo.Value);
                vendorsysno = tempEntity.VendorSysNo.Value;
                giftInfo.VendorSysNo = tempEntity.VendorSysNo.Value;
                if (tempEntity.VendorType == 0)
                {
                    vendorsysno = 1;
                }
                //活动信息：

                if (giftInfo.Status != tempEntity.Status)
                {
                    throw new BusinessException(string.Format("活动[{0}]编辑失败：编辑期间，状态已经发生了变化，请重新刷新处理！", giftInfo.SysNo));
                }

                CheckStatusWhenUpdate(giftInfo, giftInfo.InUserName);
            }

            vendorsysno = giftInfo.SellerSysNo.Value;
            //主商品:
            foreach (var item in giftInfo.ProductRuleList)
            {
                if (item.ProductSysNo.HasValue)
                {
                    int productvendorsysno = GiftPromotionDA.GetVendorSysNoByProductSysNo(item.ProductSysNo.Value);
                    if (productvendorsysno != vendorsysno)
                    {
                        //throw new BizException(string.Format("{0}不能添加其他商家的商品【{1}】", tempEntity.VendorName, item.RelProduct.ProductID));
                        throw new BusinessException(string.Format("{0}不能添加其他商家的商品【{1}】", tempEntity.VendorName, item.ProductID));
                    }
                }
            }


            //赠品:
            foreach (var item in giftInfo.GiftRuleList)
            {
                if (item.ProductSysNo.HasValue)
                {
                    item.VendorSysNo = GiftPromotionDA.GetVendorSysNoByProductSysNo(item.ProductSysNo.Value);
                    if (item.VendorSysNo.Value != vendorsysno)
                    {
                        throw new BusinessException(string.Format("{0}不能添加其他商家的商品【{1}】", tempEntity.VendorName, item.ProductID));
                    }
                }
            }



            #endregion


            using (TransactionScope ts = new TransactionScope())
            {
                //step1.保存活动主信息:
                int sysNo = GiftPromotionDA.SaveGiftPromotionMasterInfo(giftInfo);
                //step2.保存活动主商品信息:
                if (isEdit)
                {
                    GiftPromotionDA.DeleteSaleRules(sysNo);
                }
                if (giftInfo.ProductRuleList != null && giftInfo.ProductRuleList.Count > 0)
                {
                    foreach (SalesGiftMainProductRuleInfo setting in giftInfo.ProductRuleList)
                    {
                        GiftPromotionDA.CreateSaleRules(sysNo, setting);
                    }
                }
                if (isEdit)
                {
                    GiftPromotionDA.UpdateGiftIsGlobal(sysNo, giftInfo.IsGlobalProduct.Value, editUserName);
                }
                //step3.保存活动赠品信息:
                if (isEdit)
                {
                    GiftPromotionDA.DeleteGiftItemRules(sysNo);
                }
                if (giftInfo.GiftRuleList != null && giftInfo.GiftRuleList.Count > 0)
                {
                    int priority = 1;
                    foreach (SalesGiftProductRuleInfo setting in giftInfo.GiftRuleList)
                    {
                        setting.Priority = priority;
                        GiftPromotionDA.CreateGiftItemRules(sysNo, setting);
                        priority++;
                    }
                }
                if (isEdit)
                {
                    GiftPromotionDA.UpdateGiftItemCount(sysNo, SaleGiftGiftItemType.AssignGift, /*赠品池任选总数量:1*/1, editUserName);
                }
                ts.Complete();
            }
            return giftInfo.SysNo.Value;
        }

        public static SalesGiftInfo LoadGiftPromotionInfo(int sysNo)
        {
            SalesGiftInfo info = GiftPromotionDA.LoadSalesGiftInfo(sysNo);
            //if (info.ProductRuleList != null)
            //{
            //foreach (SalesGiftMainProductRuleInfo setting in info.ProductRuleList)
            //{
            //    if (setting.RelBrand != null && setting.RelBrand.SysNo.HasValue)
            //    {
            //        var brandName = ExternalDomainBroker.GetBrandInfoBySysNo(setting.RelBrand.SysNo.Value);
            //        if (brandName != null && brandName.BrandNameLocal != null)
            //        {
            //            setting.RelBrand.Name = brandName.BrandNameLocal.Content;
            //        }
            //    }
            //    if (setting.RelC3 != null && setting.RelC3.SysNo.HasValue)
            //    {
            //        var categoryName = ExternalDomainBroker.GetCategory3Info(setting.RelC3.SysNo.Value);
            //        if (categoryName != null && categoryName.CategoryName != null)
            //        {
            //            setting.RelC3.Name = categoryName.CategoryName.Content;
            //        }
            //    }
            //    if (setting.RelProduct != null && setting.RelProduct.ProductSysNo.HasValue)
            //    {
            //        ProductInfo product = ExternalDomainBroker.GetProductInfo(setting.RelProduct.ProductSysNo.Value);
            //        if (product != null)
            //        {
            //            setting.RelProduct.ProductName = product.ProductName;
            //            setting.RelProduct.ProductID = product.ProductID;
            //            //获取商品可用库存，代销库存，毛利率等接口
            //            ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(product.SysNo);
            //            setting.RelProduct.AvailableQty = inventory.AvailableQty;
            //            setting.RelProduct.ConsignQty = inventory.ConsignQty;
            //            setting.RelProduct.VirtualQty = inventory.VirtualQty;

            //            setting.RelProduct.UnitCost = product.ProductPriceInfo.UnitCost;
            //            setting.RelProduct.CurrentPrice = product.ProductPriceInfo.CurrentPrice;
            //        }
            //    }
            //}

            //foreach (SalesGiftMainProductRuleInfo setting in info.ProductRuleList)
            //{
            //    if (setting.RelProduct.ProductSysNo.HasValue)
            //    {
            //        ProductInfo product = ExternalDomainBroker.GetProductInfo(setting.RelProduct.ProductSysNo.Value);
            //        if (product != null)
            //        {
            //            int minBuyQty = setting.RelProduct.MinQty.HasValue ? (setting.RelProduct.MinQty.Value == 0 ? 1 : setting.RelProduct.MinQty.Value) : 1;
            //            setting.RelProduct.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_SaleItemGrossMarginRate(product,
            //                minBuyQty, sysNo.Value, info);
            //        }
            //    }
            //}
            // }

            //if (info.GiftRoleList != null)
            //{
            //    foreach (SalesGiftProductRuleInfo giftItem in info.GiftRoleList)
            //    {
            //        ProductInfo product = ExternalDomainBroker.GetProductInfo(giftItem.ProductSysNo.Value);
            //        if (product == null) continue;
            //        giftItem.ProductName = product.ProductName; ;
            //        giftItem.ProductID = product.ProductID;
            //        //获取商品可用库存，代销库存，毛利率等接口
            //        ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(product.SysNo);
            //        if (inventory == null) continue;
            //        giftItem.AvailableQty = inventory.AvailableQty;
            //        giftItem.ConsignQty = inventory.ConsignQty;
            //        giftItem.VirtualQty = inventory.VirtualQty;
            //        giftItem.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_GiftItemGrossMarginRate(product, info.DisCountType.Value);
            //        giftItem.UnitCost = product.ProductPriceInfo.UnitCost;
            //        giftItem.CurrentPrice = product.ProductPriceInfo.CurrentPrice;
            //    }
            //}
            if (info == null)
            {
                throw new BusinessException(string.Format("找不到相关的促销赠品信息，编号：{0}", sysNo));
            }
            return info;
        }

        public static void SubmitAudit(int promotionSysNo, string currentUserName)
        {
            SalesGiftInfo info = LoadGiftPromotionInfo(promotionSysNo);
            if (info.GiftRuleList != null && info.GiftRuleList.Count > 0)
            {
                foreach (var item in info.GiftRuleList)
                {
                    var productInfo = ProductService.GetProductBySysNo(item.ProductSysNo.Value);
                    if (null == productInfo || productInfo.SysNo <= 0)
                    {
                        throw new BusinessException(string.Format("赠品规则中，商品编号{0}不是有效的商品！", item.ProductSysNo.Value));
                    }
                    else if (productInfo.Status != ProductStatus.InActive_Show)
                    {
                        throw new BusinessException(string.Format("赠品规则中，只有上架不展示商品才能设置为赠品，商品编号{0}状态不是上架不展示商品！", item.ProductSysNo.Value));
                    }
                }                
            }
            else
            {
                throw new BusinessException("请至少添加一项赠品规则!");
            }
            if (CheckGiftCompleted(info))
            {
                SaleGiftStatus resultStatus = info.Status.Value;
                string errorDescription = null;
                string successDescription = string.Empty;
                if (!CheckAndOperateStatus(PSOperationType.SubmitAudit, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
                {
                    throw new BusinessException(errorDescription);
                }
                //List<string> errorMsg = CheckGiftRules(info);
                //if (errorMsg.Count > 0)
                //{
                //    throw new BusinessException(string.Join(";", errorMsg));
                //}
                resultStatus = SaleGiftStatus.WaitingAudit;

                GiftPromotionDA.UpdateStatus(promotionSysNo, resultStatus, currentUserName);
            }
            else
            {
                string errorMsg = string.Format("赠品活动[{0}]信息不完整,无法提交审核!\r\n", promotionSysNo);
                throw new BusinessException(errorMsg);
            }
        }

        public static void Void(int promotionSysNo, string currentUserName)
        {
            SalesGiftInfo info = LoadGiftPromotionInfo(promotionSysNo);
            if (info == null)
            {
                throw new BusinessException(string.Format("活动[{0}]信息加载失败!", promotionSysNo));

            }

            SaleGiftStatus resultStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Void, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
            {
                throw new BusinessException(errorDescription);
            }
            GiftPromotionDA.UpdateStatus(promotionSysNo, resultStatus, currentUserName);
        }

        public static void Stop(int promotionSysNo, string currentUserName)
        {
            SalesGiftInfo info = LoadGiftPromotionInfo(promotionSysNo);
            if (info == null)
            {
                throw new BusinessException(string.Format("活动[{0}]信息加载失败!", promotionSysNo));
            }

            SaleGiftStatus resultStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Stop, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
            {
                throw new BusinessException(errorDescription);

            }
            GiftPromotionDA.UpdateStatus(promotionSysNo, resultStatus, currentUserName);
        }

        public static Tuple<int, int, string> BatchSubmitAudit(List<int> promotionSysNoList, string currentUserName)
        {
            int sucCount = 0;
            int failCount = 0;
            StringBuilder errorMsg = new StringBuilder();
            if (null != promotionSysNoList && promotionSysNoList.Count > 0)
            {
                foreach (var sysNo in promotionSysNoList)
                {
                    try
                    {
                        SubmitAudit(sysNo, currentUserName);
                        sucCount++;
                    }
                    catch (BusinessException bizEx)
                    {
                        errorMsg.AppendLine(string.Format("{0}", bizEx.Message));
                        failCount++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new BusinessException("请选择至少一个活动!");
            }
            return new Tuple<int, int, string>(sucCount, failCount, errorMsg.ToString());
        }
        public static Tuple<int, int, string> BatchAbandon(List<int> promotionSysNoList, string currentUserName)
        {
            int sucCount = 0;
            int failCount = 0;
            StringBuilder errorMsg = new StringBuilder();
            if (null != promotionSysNoList && promotionSysNoList.Count > 0)
            {
                foreach (var sysNo in promotionSysNoList)
                {
                    try
                    {
                        Void(sysNo, currentUserName);
                        sucCount++;
                    }
                    catch (BusinessException bizEx)
                    {
                        errorMsg.AppendLine(string.Format("{0}", bizEx.Message));
                        failCount++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new BusinessException("请选择至少一个活动!");
            }
            return new Tuple<int, int, string>(sucCount, failCount, errorMsg.ToString());
        }
        public static Tuple<int, int, string> BatchStop(List<int> promotionSysNoList, string currentUserName)
        {
            int sucCount = 0;
            int failCount = 0;
            StringBuilder errorMsg = new StringBuilder();
            if (null != promotionSysNoList && promotionSysNoList.Count > 0)
            {
                foreach (var sysNo in promotionSysNoList)
                {
                    try
                    {
                        Stop(sysNo, currentUserName);
                        sucCount++;
                    }
                    catch (BusinessException bizEx)
                    {
                        errorMsg.AppendLine(string.Format("活动编号{0}", bizEx.Message));
                        failCount++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new BusinessException("请选择至少一个活动!");
            }
            return new Tuple<int, int, string>(sucCount, failCount, errorMsg.ToString());
        }



        #region [Private Methods]
        private static void CheckStatusWhenUpdate(SalesGiftInfo info, string userfullname)
        {
            SaleGiftStatus resultStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Edit, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
            {
                throw new BusinessException(errorDescription);
            }
            //如果当前状态是就绪状态，那么Check后应该是Init状态，所以需要更改为Init状态：就绪状态下一旦Upadate了，就要更新状态为Init

            if (resultStatus != info.Status)
            {
                GiftPromotionDA.UpdateGiftPromotionStatus(info.SysNo.Value, resultStatus, userfullname);
            }
        }

        /// <summary>
        /// 检查当前状态的下当前操作是否符合业务逻辑。同时也通过out参数返回操作后的状态
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="curStatus">当前状态</param>
        /// <param name="resultStatus">操作后的状态</param>
        /// <returns>当前状态下本操作是否正确</returns>
        private static bool CheckAndOperateStatus(PSOperationType operation, int? sysNo, SaleGiftStatus? curStatus,
            out SaleGiftStatus resultStatus, out string errorDescription)
        {
            resultStatus = curStatus.HasValue ? curStatus.Value : SaleGiftStatus.Origin;
            errorDescription = null;
            bool checkPassResult = true;

            switch (operation)
            {
                case PSOperationType.Edit:
                    if (curStatus != SaleGiftStatus.Origin)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]编辑失败:只有初始状态可以进行编辑！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());
                    }
                    break;
                case PSOperationType.SubmitAudit:
                    if (curStatus != SaleGiftStatus.Origin)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]提交审核失败:只有初始状态可以提交审核！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.WaitingAudit;
                    break;
                case PSOperationType.CancelAudit:
                    if (curStatus != SaleGiftStatus.WaitingAudit && curStatus != SaleGiftStatus.Ready)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]撤回失败:只有待审核状态和就绪状态可以取消审核！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.Origin;
                    break;
                case PSOperationType.AuditApprove:
                    if (curStatus != SaleGiftStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]审核通过失败:只有待审核状态可以审核通过！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.Ready;
                    break;
                case PSOperationType.AuditRefuse:
                    if (curStatus != SaleGiftStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]审核拒绝失败:只有待审核状态可以进行审核拒绝！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.Void;
                    break;
                case PSOperationType.Stop:
                    if (curStatus != SaleGiftStatus.Run)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]提前终止失败:只有运行状态可以进行提前终止！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.Stoped;
                    break;
                case PSOperationType.Void:
                    //if (curStatus != SaleGiftStatus.Init && curStatus != SaleGiftStatus.WaitingAudit && curStatus != SaleGiftStatus.Ready)
                    //{
                    if (curStatus != SaleGiftStatus.Origin)
                    {
                        checkPassResult = false;
                        errorDescription = string.Format("活动[{0}]作废失败:只有初始、待审核、就绪状态可以进行作废！本活动当前状态:{1}！", sysNo, curStatus.Value.GetDescription());

                        break;
                    }
                    resultStatus = SaleGiftStatus.Void;
                    break;
            }

            return checkPassResult;
        }

        /// <summary>
        /// 判断提交审核
        /// </summary>
        /// <param name="promotionSysNo"></param>
        /// <param name="companyCode"></param>
        private static bool CheckGiftCompleted(SalesGiftInfo info)
        {
            int saleRulesCount = 0;
            if ((info.Type == SaleGiftType.Full)
                && info.IsGlobalProduct.HasValue && info.IsGlobalProduct.Value)
            {
                saleRulesCount = 1;
            }
            else
            {
                saleRulesCount = info.ProductRuleList != null ? info.ProductRuleList.Count : 0;
            }
            int giftRulesCount = info.GiftRuleList != null ? info.GiftRuleList.Count : 0;

            bool result = saleRulesCount != 0 && giftRulesCount != 0;
            return result;
        }

        //private static List<String> CheckGiftRules(SalesGiftInfo info)
        //{
        //    List<SalesGiftInfo> giftInfoListTmp = new List<SalesGiftInfo>();
        //    List<String> msgTmp = new List<String>();
        //    foreach (var item in info.GiftRuleList)
        //    {
        //        giftInfoListTmp.AddRange(GiftPromotionDA.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Run));
        //        giftInfoListTmp.AddRange(GiftPromotionDA.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Ready));
        //        giftInfoListTmp.AddRange(GiftPromotionDA.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.WaitingAudit));

        //        if (info.Type == SaleGiftType.Additional)
        //        //|| info.Type == SaleGiftType.Additional)
        //        {
        //            if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && p.Type != SaleGiftType.Additional).ToList().Count > 0)
        //            {
        //                msgTmp.Add(string.Format(@"""满额加购赠品"" ({0}) 在其他赠品活动中存在有效的重复的记录！", item.ProductID));
        //            }
        //        }
        //        else
        //        {
        //            if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && (p.Type == SaleGiftType.Additional)).ToList().Count > 0)
        //            //|| p.Type == SaleGiftType.Additional)).ToList().Count > 0)
        //            {
        //                msgTmp.Add(string.Format(@"赠品 ({0}) 在""满额加购赠品""活动中存在有效的重复的记录！", item.ProductID));
        //            }
        //        }
        //    }
        //    return msgTmp;
        //}
        #endregion
    }
}
