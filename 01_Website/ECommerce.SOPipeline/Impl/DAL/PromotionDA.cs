using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.Utility.DataAccess;
using System.Data;
using ECommerce.Utility;
using ECommerce.Entity.Product;
using ECommerce.Entity.Member;
using ECommerce.Enums;
using ECommerce.Entity;
using ECommerce.SOPipeline.Entity;

namespace ECommerce.SOPipeline.Impl
{
    public class PromotionDA
    {
        /// <summary>
        /// 非常轻量级的获取商品基础信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static SimpleItemEntity GetSimpleItemBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetSimpleItemBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<SimpleItemEntity>();
        }

        /// <summary>
        /// 非常轻量级的批量获取商品基础信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<SimpleItemEntity> GetSimpleItemListBySysNumbers(List<int> productSysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Promotion_GetSimpleItemListBySysNumbers");

            string strWhere = " SysNo IN (";
            foreach (int num in productSysNoList)
            {
                strWhere += num.ToString() + ",";
            }
            strWhere = strWhere.TrimEnd(",".ToCharArray());
            strWhere += ")";

            cmd.CommandText = cmd.CommandText.Replace("#StrWhere#", strWhere);


            return cmd.ExecuteEntityList<SimpleItemEntity>();
        }


        /// <summary>
        /// 根据当前商品，获取以此商品为主商品的套餐信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ComboInfo> GetComboListByMasterProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetComboListByMasterProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            List<ComboInfo> comboList = new List<ComboInfo>();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItems = ds.Tables[1];
            if (dtMaster != null && dtMaster.Rows.Count > 0)
            {
                comboList = DataMapper.GetEntityList<ComboInfo, List<ComboInfo>>(dtMaster.Rows, (row, entity) =>
                {
                    entity.IsShowName = row["IsShow"] == null ? false : row["IsShow"].ToString().Trim().ToUpper() == "Y" ? true : false;
                }
                        );
                List<ComboItem> tempItemList = DataMapper.GetEntityList<ComboItem, List<ComboItem>>(dtItems.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
                foreach (ComboInfo combo in comboList)
                {
                    combo.Items = new List<ComboItem>();
                    foreach (ComboItem item in tempItemList)
                    {
                        if (combo.SysNo == item.ComboSysNo)
                        {
                            combo.Items.Add(item);
                        }
                    }
                }
            }
            return comboList;
        }

        /// <summary>
        /// 商品是否在团购，在团购，返回团购编码，否则返回0
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static int ProductIsGroupBuying(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_ProductIsGroupBuying");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            object obj = cmd.ExecuteScalar();
            if (obj == null)
            {
                return 0;
            }
            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// 商品是否在虚拟团购，在虚拟团购，返回团购编码，否则返回0
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static int ProductIsVirualGroupBuying(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_ProductIsVirualGroupBuying");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            object obj = cmd.ExecuteScalar();
            if (obj == null)
            {
                return 0;
            }
            return int.Parse(obj.ToString());
        }

        /// <summary>
        /// 根据商品编号获取限时、秒杀信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static CountdownInfo GetProductCountdownByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetProductCountdownByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
           
            return cmd.ExecuteEntity<CountdownInfo>();
            
        }

        /// <summary>
        /// 获取商品所有参与的赠品活动, For 商品详细页
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<SaleGiftInfo> GetSaleGiftListByProductSysNo(int productSysNo)
        {
            List<SaleGiftInfo> listResult = new List<SaleGiftInfo>();

            #region 获取指定商品的当前赠品活动:单品，同时购买，厂商赠品
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetSaleGiftByProductSysNo_AssignProduct");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataSet ds1 = cmd.ExecuteDataSet();
            if (ds1 != null && ds1.Tables[0] != null && ds1.Tables[0].Rows.Count > 0)
            {
                List<SaleGiftInfo> list1 = DataMapper.GetEntityList<SaleGiftInfo, List<SaleGiftInfo>>(ds1.Tables[0].Rows, (row, entity) =>
                {
                    switch (row["SaleGiftTypeChar"].ToString().Trim().ToUpper())
                    {
                        case "S":
                            entity.SaleGiftType = ECommerce.Enums.SaleGiftType.Single;
                            break;
                        case "M":
                            entity.SaleGiftType = ECommerce.Enums.SaleGiftType.Multiple;
                            break;
                        case "V":
                            entity.SaleGiftType = ECommerce.Enums.SaleGiftType.Vendor;
                            break;
                        default:
                            entity.SaleGiftType = ECommerce.Enums.SaleGiftType.Single;
                            break;
                    };
                    entity.IsGiftPool = row["IsGiftPoolChar"].ToString().Trim().ToUpper() == "O" ? true : false;
                    entity.IsGlobal = row["IsGlobalChar"].ToString().Trim().ToUpper() == "Y" ? true : false;
                });

                //获取上面所有赠品活动的赠品规则，并一一赋给对应的赠品活动
                List<GiftSaleRule> saleruleList1 = DataMapper.GetEntityList<GiftSaleRule, List<GiftSaleRule>>(ds1.Tables[1].Rows);
                foreach (SaleGiftInfo curSaleRule in list1)
                {
                    List<GiftSaleRule> curSaleRuleList = saleruleList1.FindAll(f => f.PromotionSysNo == curSaleRule.SysNo.Value);
                    curSaleRule.GiftSaleRuleList = curSaleRuleList;
                }

                if (list1 != null)
                {
                    listResult.AddRange(list1);
                }
            }
           
            #endregion

            #region 获取当前有效的赠品活动: 满赠，然后看哪些满足当前商品
            cmd = DataCommandManager.GetDataCommand("Promotion_GetSaleGift_ProductRange");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                List<SaleGiftInfo> listRange = DataMapper.GetEntityList<SaleGiftInfo, List<SaleGiftInfo>>(ds.Tables[0].Rows, (row, entity) =>
                {
                    entity.SaleGiftType = ECommerce.Enums.SaleGiftType.Full;
                    entity.IsGiftPool = row["IsGiftPoolChar"].ToString().Trim().ToUpper() == "O" ? true : false;
                    entity.IsGlobal = row["IsGlobalChar"].ToString().Trim().ToUpper() == "Y" ? true : false;
                });

                List<GiftSaleRule> saleruleList = DataMapper.GetEntityList<GiftSaleRule, List<GiftSaleRule>>(ds.Tables[1].Rows);

                SimpleItemEntity item = GetSimpleItemBySysNo(productSysNo);
                //根据设置的条件规则来判定：全网模式，只要不在排除范围内，则参加活动；
                //非全网模式：首先必须不在排除范围内，然后并且至少有一项属性在包含范围内，则参加活动
                foreach (SaleGiftInfo saleGift in listRange)
                {
                    if (saleGift.IsGlobal)  //全场范围
                    {
                        List<GiftSaleRule> curSaleRuleList = saleruleList.FindAll(f => f.PromotionSysNo == saleGift.SysNo.Value);
                        saleGift.GiftSaleRuleList = curSaleRuleList;

                        if (!curSaleRuleList.Exists(f => f.RelProductSysNo == item.ProductSysNo
                            || f.RelC3SysNo == item.C3SysNo || f.RelBrandSysNo == item.BrandSysNo))
                        {
                            if (saleGift.VendorSysNo.Value == 1)
                            {
                                listResult.Add(saleGift);
                            }
                            else if (saleGift.VendorSysNo.Value == item.MerchantSysNo)
                            {
                                listResult.Add(saleGift);
                            }
                        }
                    }
                    else
                    {
                        bool include = false;
                        List<GiftSaleRule> curSaleRuleList = saleruleList.FindAll(f => f.PromotionSysNo == saleGift.SysNo.Value);
                        saleGift.GiftSaleRuleList = curSaleRuleList;

                        if (curSaleRuleList != null && curSaleRuleList.Count > 0)
                        {
                            foreach (GiftSaleRule curSaleRule in curSaleRuleList)
                            {
                                if (curSaleRule.ComboType.Trim().ToUpper() == "O" &&
                                    (curSaleRule.RelProductSysNo == item.ProductSysNo
                                    || curSaleRule.RelC3SysNo == item.C3SysNo
                                    || curSaleRule.RelBrandSysNo == item.BrandSysNo))
                                {
                                    include = true;
                                    continue;
                                }

                                if (curSaleRule.ComboType.Trim().ToUpper() == "N" &&
                                    (curSaleRule.RelProductSysNo == item.ProductSysNo
                                    || curSaleRule.RelC3SysNo == item.C3SysNo
                                    || curSaleRule.RelBrandSysNo == item.BrandSysNo))
                                {
                                    include = false;
                                    break;
                                }
                            }

                            if (include == true)
                            {
                                listResult.Add(saleGift);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 给每个活动填充指定类型的赠品
            if (listResult.Count > 0)
            {
                string strPromotionSysNoes = string.Empty;
                listResult.ForEach(f => strPromotionSysNoes += f.SysNo.Value.ToString() + ",");
                strPromotionSysNoes = strPromotionSysNoes.TrimEnd(",".ToCharArray());
                strPromotionSysNoes = "(" + strPromotionSysNoes + ")";

                CustomDataCommand cucmd = DataCommandManager.CreateCustomDataCommandFromConfig("Promotion_GetSaleGiftItemByPromotionSysNoList");
                cucmd.CommandText = cucmd.CommandText.Replace("#PromotionSysNoes#", strPromotionSysNoes);
                List<GiftItem> allGiftItemList = cucmd.ExecuteEntityList<GiftItem>();

                List<SaleGiftInfo> needRemoveSaleGiftList = new List<SaleGiftInfo>();
                foreach (SaleGiftInfo saleGift in listResult)
                {
                    List<GiftItem> curGiftItemList = allGiftItemList.FindAll(f => f.PromotionSysNo == saleGift.SysNo.Value);
                    if (curGiftItemList != null && curGiftItemList.Count > 0)
                    {
                        curGiftItemList.ForEach(f => f.IsGiftPool = saleGift.IsGiftPool);
                        saleGift.GiftItemList = curGiftItemList;
                    }
                    else
                    {
                        saleGift.GiftItemList = new List<GiftItem>();
                        needRemoveSaleGiftList.Add(saleGift);
                    }
                }
                if (needRemoveSaleGiftList.Count > 0)
                {
                    needRemoveSaleGiftList.ForEach(f => listResult.Remove(f));
                }
            }
            #endregion

            return listResult;
        }

        /// <summary>
        /// 根据套餐编号获取套餐信息和套餐商品列表
        /// </summary>
        /// <param name="comboSysNo">套餐编号</param>
        /// <returns></returns>
        public static ComboInfo GetComboByComboSysNo(int comboSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetComboByComboSysNo");
            cmd.SetParameterValue("@ComboSysNo", comboSysNo);
            DataSet ds = cmd.ExecuteDataSet();

            ComboInfo combo = new ComboInfo();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItems = ds.Tables[1];
            if (dtMaster != null && dtMaster.Rows.Count > 0)
            {
                combo = DataMapper.GetEntity<ComboInfo>(dtMaster.Rows[0], (row, entity) =>
                {
                    entity.IsShowName = row["IsShow"] == null ? false : row["IsShow"].ToString().Trim().ToUpper() == "Y" ? true : false;
                });
                
                combo.Items = DataMapper.GetEntityList<ComboItem, List<ComboItem>>(dtItems.Rows, (row, entity) =>
                {
                    entity.IsMasterItemB = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
                });
            }
            return combo;
        }


        /// <summary>
        /// 获取当前用户可用的优惠券列表
        /// </summary>
        /// <param name="query">查询信息</param>
        /// <returns></returns>
        public static List<CustomerCouponInfo> GetCanUsingCouponCodeList(int customerSysNo, int customerRank)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCanUsingCouponCodeList");

            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@CustomerRank", customerRank);
            List<CustomerCouponInfo> couponList = cmd.ExecuteEntityList<CustomerCouponInfo>();

            return couponList;
        }

        /// <summary>
        /// 通用型优惠券，获取该Code在全站已使用次数
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static int GetCouponCodeTotalUsedCount(string couponCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponCodeTotalUsedCount");

            cmd.SetParameterValue("@CouponCode", couponCode);
            int count = cmd.ExecuteScalar<int>();
            return count;
        }

        /// <summary>
        /// 投放型优惠券，获取该活动在全站已使用次数
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static int GetCouponTotalUsedCount(int couponSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponTotalUsedCount");

            cmd.SetParameterValue("@CouponSysNo", couponSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count;
        }

        /// <summary>
        /// 获取当前优惠券号码当前Customer已使用的次数
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static int GetCustomerTotalUsedCount(string couponCode, int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCustomerTotalUsedCount");
            cmd.SetParameterValue("@CouponCode", couponCode);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count;
        }

        /// <summary>
        /// 根据优惠券Code获取优惠券活动的整体信息
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static CouponInfo GetComboInfoByCouponCode(string couponCode)
        {            
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponInfoByCouponCode");
            cmd.SetParameterValue("@CouponCode", couponCode);
            DataSet ds = cmd.ExecuteDataSet();
            if (!(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0))
            {
                return null;
            }

            CouponInfo coupon = DataMapper.GetEntity<CouponInfo>(ds.Tables[0].Rows[0]);
            if (ds.Tables[1].Rows.Count > 0)
            {
                coupon.DiscountRuleList = DataMapper.GetEntityList<Coupon_DiscountRules, List<Coupon_DiscountRules>>(ds.Tables[1].Rows);
            }
            else
            {
                coupon.DiscountRuleList = new List<Coupon_DiscountRules>();
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                coupon.SaleRulesList = DataMapper.GetEntityList<Coupon_SaleRules, List<Coupon_SaleRules>>(ds.Tables[2].Rows);
            }
            else
            {
                coupon.SaleRulesList = new List<Coupon_SaleRules>();
            }

            if (ds.Tables[3].Rows.Count > 0)
            {
                coupon.SaleRulesEx = DataMapper.GetEntity<Coupon_SaleRules_Ex>(ds.Tables[3].Rows[0]);
            }
            else
            {
                coupon.SaleRulesEx = null;
            }

            coupon.AssignCustomerList=new List<int>();
            if (ds.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[4].Rows)
                {
                    coupon.AssignCustomerList.Add(int.Parse(row["CustomerSysNo"].ToString()));
                }
            }


            return coupon;
        }


        public static List<OrderAttachment> GetAttachmentListByProductSysNoList(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count <= 0)
            {
                return new List<OrderAttachment>(0);
            }
            string strWhere = " pa.ProductSysNo IN (";
            foreach (int num in productSysNoList)
            {
                strWhere += num.ToString() + ",";
            }
            strWhere = strWhere.TrimEnd(",".ToCharArray());
            strWhere += ")";

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Promotion_GetAttachmentListByProductSysNoList");

            cmd.CommandText = cmd.CommandText.Replace("#StrWhere#", strWhere);


            List<OrderAttachment> list = cmd.ExecuteEntityList<OrderAttachment>((dr, entity) => {
                entity["ProductStoreType"] = dr["ProductStoreType"] == null || string.IsNullOrWhiteSpace(dr["ProductStoreType"].ToString()) ? (int)ProductStoreType.Narmal : int.Parse(dr["ProductStoreType"].ToString().Trim());
                //entity["TariffRate"] = dr["TariffRate"] == null || string.IsNullOrWhiteSpace(dr["TariffRate"].ToString()) ? 0m : decimal.Parse(dr["TariffRate"].ToString().Trim());
                entity["Warranty"] = dr["Warranty"];
            });
            return list;
        }

        public static Area GetAreaBySysNo(int district)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetAreaBySysNo");
            cmd.SetParameterValue("@SysNo", district);
            return cmd.ExecuteEntity<Area>();
        }

        /// <summary>
        /// 获取商家优惠券
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static List<CustomerCouponInfo> GetMerchantCouponCodeList(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetMerchantCouponCodeList");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntityList<CustomerCouponInfo>();
        }

        /// <summary>
        /// 获取优惠券规则
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static List<CouponSaleRules> GetCouponSaleRulesList(string couponCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponSaleRulesList");
            cmd.SetParameterValue("@CouponCode", couponCode);
            return cmd.ExecuteEntityList<CouponSaleRules>();
        }

        /// <summary>
        /// 获取优惠券
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static CustomerCouponInfo GetCouponSaleRules(int customerSysNo, string couponCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponSaleRules");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@CouponCode", couponCode);
            return cmd.ExecuteEntity<CustomerCouponInfo>();
        }
        /// <summary>
        /// 获取优惠券活动列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static List<CouponInfo> GetCouponList(int customerSysNo, int MerchantSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponList");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@MerchantSysNo", MerchantSysNo);
            return cmd.ExecuteEntityList<CouponInfo>();
        }

        /// <summary>
        /// 根据优惠券Code获取优惠券活动的整体信息
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static CouponInfo GetCouponInfo(int CouponSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCouponInfo");
            cmd.SetParameterValue("@CouponSysNo", CouponSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (!(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0))
            {
                return null;
            }

            CouponInfo coupon = DataMapper.GetEntity<CouponInfo>(ds.Tables[0].Rows[0]);
            if (ds.Tables[1].Rows.Count > 0)
            {
                coupon.DiscountRuleList = DataMapper.GetEntityList<Coupon_DiscountRules, List<Coupon_DiscountRules>>(ds.Tables[1].Rows);
            }
            else
            {
                coupon.DiscountRuleList = new List<Coupon_DiscountRules>();
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                coupon.SaleRulesList = DataMapper.GetEntityList<Coupon_SaleRules, List<Coupon_SaleRules>>(ds.Tables[2].Rows);
            }
            else
            {
                coupon.SaleRulesList = new List<Coupon_SaleRules>();
            }

            if (ds.Tables[3].Rows.Count > 0)
            {
                coupon.SaleRulesEx = DataMapper.GetEntity<Coupon_SaleRules_Ex>(ds.Tables[3].Rows[0]);
            }
            else
            {
                coupon.SaleRulesEx = null;
            }

            coupon.AssignCustomerList = new List<int>();
            if (ds.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[4].Rows)
                {
                    coupon.AssignCustomerList.Add(int.Parse(row["CustomerSysNo"].ToString()));
                }
            }


            return coupon;
        }

        /// <summary>
        /// 用户优惠券码关联表
        /// </summary>
        /// <param name="log"></param>
        public static bool InsertCustomerLog(CouponCodeCustomerLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Coupon_InsertCouponCodeCustomerLog");
            command.SetParameterValue("@CouponSysNo", log.CouponSysNo);
            command.SetParameterValue("@CouponCode", log.CouponCode);
            command.SetParameterValue("@CustomerSysNo", log.CustomerSysNo);
            command.SetParameterValue("@UserCodeType", log.UserCodeType);
            command.SetParameterValue("@SOSysNo", log.SOSysNo);
            if (command.ExecuteNonQuery() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 插入优惠券码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertCouponCode(int customerSysNo, CouponCode entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Coupon_InsertCouponCode");
            command.SetParameterValue("@CouponSysNo", entity.CouponSysNo);
            command.SetParameterValue("@CouponCode", entity.Code);
            command.SetParameterValue("@CodeType", entity.CodeType);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@InUser", customerSysNo);
            command.ExecuteNonQuery();
            return (int)command.GetParameterValue("SysNo");
        }

        /// <summary>
        /// 校验是否有重复的优惠券代码
        /// </summary>
        /// <param name="couponcode"></param>
        /// <returns></returns>
        public static bool CheckExistCode(string couponCode, int? couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckExistCode");
            command.SetParameterValue("@CouponCode", couponCode);
            command.SetParameterValue("@CouponSysNo", couponSysNo.HasValue ? couponSysNo.Value : 0);
            return command.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 校验用户是否已领取优惠券
        /// </summary>
        /// <param name="couponcode"></param>
        /// <returns></returns>
        public static bool CheckUserAreadyGetCode(int customerSysNo, int couponSysNo, int LimitCount, out int RemainingNumber
)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckUserAreadyGetCode");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@CouponSysNo", couponSysNo);
            RemainingNumber =LimitCount-command.ExecuteScalar<int>();
            return RemainingNumber <=0;
        }

        /// <summary>
        /// 校验用户是否已领取优惠券
        /// </summary>
        /// <param name="couponcode"></param>
        /// <returns></returns>
        public static int GetCodeNumberByCouponNumber(int couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCodeNumberByCouponNumber");
            command.SetParameterValue("@CouponSysNo", couponSysNo);
            return command.ExecuteScalar<int>();
        }
        /// <summary>
        /// 获取当前用户参与当前优惠券活动的次数
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static int GetCustomerCouponNumber(int couponSysNo, int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_GetCustomerCouponCount");
            cmd.SetParameterValue("@CouponSysNo", couponSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count;
        }

    }
}
