using ECommerce.Entity.Common;
using ECommerce.Entity.Promotion;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.Promotion
{
    public class CouponDA
    {
        /// <summary>
        /// 优惠券查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult QueryCouponList(CouponQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "c.SysNo ASC" : queryFilter.SortFields))
            {
                //Set SQL WHERE Condition:
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo);
                }

                if (queryFilter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.CouponName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.CouponName", DbType.String, "@CouponName", QueryConditionOperatorType.Like, queryFilter.CouponName);
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.CouponCode))
                {
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist,
                        "Select Top 1 1 FROM [OverseaECommerceManagement].[dbo].[CouponCode] as cc with(nolock) WHERE cc.CouponSysNo=c.SysNo AND cc.CouponCode=@CouponCode");
                    command.AddInputParameter("@CouponCode", DbType.String, queryFilter.CouponCode);

                }

                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status);
                }

                if (queryFilter.BeginDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.BeginDate", DbType.DateTime, "@BeginDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.BeginDateFrom.Value.Date);
                }

                if (queryFilter.BeginDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.BeginDate", DbType.DateTime, "@BeginDateTo", QueryConditionOperatorType.LessThan, queryFilter.BeginDateTo.Value.Date.AddDays(1));
                }

                if (queryFilter.EndDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.EndDate", DbType.DateTime, "@EndDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.EndDateFrom.Value.Date);
                }

                if (queryFilter.EndDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.EndDate", DbType.DateTime, "@EndDateTo", QueryConditionOperatorType.LessThan, queryFilter.EndDateTo.Value.Date.AddDays(1));
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                DataTable resultList = command.ExecuteDataTable(3, typeof(CouponStatus));
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return new QueryResult(resultList, queryFilter, totalCount);// { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        public static Coupon Load(int? sysNo)
        {
            Coupon info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCoupon");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dtMaster = ds.Tables[0];
                if (dtMaster.Rows.Count > 0)
                {
                    info = DataMapper.GetEntity<Coupon>(dtMaster.Rows[0]);
                    info.DiscountRules = new List<CouponDiscountRule>();
                    info.BindRule = new CouponBindRule();
                    info.SaleRule = new CouponSaleRule();

                    info.SaleRule.ProductRange = new ProductRelation();
                    info.SaleRule.ProductRange.Products = new List<RelProduct>();

                    info.SaleRule.CustomerRange = new CustomerRelation();
                    info.SaleRule.CustomerRange.Customers = new List<RelCustomer>();

                    info.BindRule.ProductRange = new ProductRelation();
                    info.BindRule.ProductRange.Products = new List<RelProduct>();

                    DataTable dtDiscountRules = ds.Tables[1];
                    if (dtDiscountRules.Rows.Count > 0)
                    {
                        #region Coupon_DiscountRules的数据绑定
                        foreach (DataRow dr in dtDiscountRules.Rows)
                        {
                            CouponDiscountRule rule = new CouponDiscountRule();
                            if (dr["RulesType"].ToString().Trim().ToUpper() == "P")
                            {
                                rule.RulesType = CouponDiscountRuleType.Percentage;
                            }
                            else if (dr["RulesType"].ToString().Trim().ToUpper() == "D")
                            {
                                rule.RulesType = CouponDiscountRuleType.Discount;
                            }
                            rule.Value = decimal.Parse(dr["Value"].ToString().Trim());
                            rule.Amount = decimal.Parse(dr["Amount"].ToString().Trim());
                            info.DiscountRules.Add(rule);
                        }
                        #endregion
                    }


                    DataTable dtBindRule = ds.Tables[2];
                    if (dtBindRule.Rows.Count > 0)
                    {
                        #region Coupon_BindRules的数据绑定
                        DataRow dr = dtBindRule.Rows[0];
                        if (!string.IsNullOrEmpty(dr["BindCondition"].ToString()))
                        {
                            object bindConditionType;
                            EnumCodeMapper.TryGetEnum(dr["BindCondition"], typeof(CouponsBindConditionType), out bindConditionType);
                            info.BindRule.BindCondition = (CouponsBindConditionType)bindConditionType;
                        }

                        if (!string.IsNullOrEmpty(dr["ValidPeriod"].ToString()))
                        {
                            object validPeriod;
                            EnumCodeMapper.TryGetEnum(dr["ValidPeriod"], typeof(CouponValidPeriodType), out validPeriod);
                            info.BindRule.ValidPeriod = (CouponValidPeriodType)validPeriod;
                        }

                        if (!string.IsNullOrEmpty(dr["BindBeginDate"].ToString()))
                        {
                            info.BindRule.BindBeginDate = DateTime.Parse(dr["BindBeginDate"].ToString().Trim());
                        }
                        if (!string.IsNullOrEmpty(dr["BindEndDate"].ToString()))
                        {
                            info.BindRule.BindEndDate = DateTime.Parse(dr["BindEndDate"].ToString().Trim());
                        }

                        if (!string.IsNullOrEmpty(dr["AmountLimit"].ToString()))
                        {
                            info.BindRule.AmountLimit = decimal.Parse(dr["AmountLimit"].ToString().Trim());
                        }

                        if (!string.IsNullOrEmpty(dr["LimitType"].ToString()))
                        {
                            if (dr["LimitType"].ToString().Trim().ToLower() == "A".ToLower())
                            {
                                info.BindRule.ProductRange.ProductRangeType = ProductRangeType.All;
                            }
                            else
                            {
                                info.BindRule.ProductRange.ProductRangeType = ProductRangeType.Limit;
                            }
                        }
                        else
                        {
                            info.BindRule.ProductRange.ProductRangeType = ProductRangeType.All;
                        }
                        #endregion
                    }

                    DataTable dtBindRuleItems = ds.Tables[3];
                    if (dtBindRuleItems.Rows.Count > 0)
                    {

                        #region Coupon_BindRuleItems的数据绑定
                        foreach (DataRow dr in dtBindRuleItems.Rows)
                        {
                            if (dr["RuleItemType"].ToString().Trim().ToUpper() == "I")
                            {
                                RelProduct product = new RelProduct();
                                if (!string.IsNullOrEmpty(dr["ItemDataSysNo"].ToString()))
                                {
                                    product.ProductSysNo = int.Parse(dr["ItemDataSysNo"].ToString());
                                }

                                if (!string.IsNullOrEmpty(dr["ProductID"].ToString()))
                                {
                                    product.ProductID = dr["ProductID"].ToString();
                                }

                                if (!string.IsNullOrEmpty(dr["ProductName"].ToString()))
                                {
                                    product.ProductName = dr["ProductName"].ToString();
                                }

                                if (!string.IsNullOrEmpty(dr["RelationType"].ToString()))
                                {
                                    if (dr["RelationType"].ToString().Trim() == "Y")
                                    {
                                        info.BindRule.ProductRange.RelationType = RelationType.Y;
                                    }
                                    else
                                    {
                                        info.BindRule.ProductRange.RelationType = RelationType.N;
                                    }
                                }
                                if (!string.IsNullOrEmpty(dr["ProductStatus"].ToString()))
                                {
                                    ProductStatus productStatus;
                                    if (Enum.TryParse<ProductStatus>(dr["ProductStatus"].ToString(), out productStatus))
                                    {
                                        product.ProductStatus = EnumHelper.GetDescription(productStatus);
                                    }
                                }
                                info.BindRule.ProductRange.Products.Add(product);
                            }

                        }
                        #endregion
                    }

                    DataTable dtSaleRules_Ex = ds.Tables[4];
                    if (dtSaleRules_Ex.Rows.Count > 0)
                    {
                        #region Coupon_SaleRules_Ex的数据绑定
                        DataRow dr = dtSaleRules_Ex.Rows[0];

                        if (!string.IsNullOrEmpty(dr["OrderAmountLimit"].ToString()))
                        {
                            info.SaleRule.OrderAmountLimit = decimal.Parse(dr["OrderAmountLimit"].ToString().Trim());
                            if (info.SaleRule.OrderAmountLimit == 0m)
                            {
                                info.SaleRule.OrderAmountLimit = null;
                            }
                        }

                        if (!string.IsNullOrEmpty(dr["OrderMaxDiscount"].ToString()))
                        {
                            info.SaleRule.OrderMaxDiscount = decimal.Parse(dr["OrderMaxDiscount"].ToString().Trim());
                        }

                        if (!string.IsNullOrEmpty(dr["CustomerMaxFrequency"].ToString()))
                        {
                            info.SaleRule.CustomerMaxFrequency = int.Parse(dr["CustomerMaxFrequency"].ToString());
                        }
                        if (!string.IsNullOrEmpty(dr["MaxFrequency"].ToString()))
                        {
                            info.SaleRule.MaxFrequency = int.Parse(dr["MaxFrequency"].ToString());
                        }

                        #endregion
                    }
                    DataTable dtSaleRules = ds.Tables[5];
                    if (dtSaleRules.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtMaster.Rows[0]["ProductRangeType"].ToString()))
                        {
                            object productRangeType;
                            EnumCodeMapper.TryGetEnum(dtMaster.Rows[0]["ProductRangeType"], typeof(ProductRangeType), out productRangeType);
                            info.SaleRule.ProductRange.ProductRangeType = (ProductRangeType)productRangeType;
                        }

                        #region Coupon_SaleRules的数据绑定
                        foreach (DataRow dr in dtSaleRules.Rows)
                        {
                            if (dr["Type"].ToString().Trim().ToUpper() == "I")
                            {
                                RelProduct product = new RelProduct();
                                if (!string.IsNullOrEmpty(dr["ProductSysNo"].ToString()))
                                {
                                    product.ProductSysNo = int.Parse(dr["ProductSysNo"].ToString());
                                }

                                if (!string.IsNullOrEmpty(dr["ProductID"].ToString()))
                                {
                                    product.ProductID = dr["ProductID"].ToString();
                                }

                                if (!string.IsNullOrEmpty(dr["ProductName"].ToString()))
                                {
                                    product.ProductName = dr["ProductName"].ToString();
                                }

                                if (!string.IsNullOrEmpty(dr["RelationType"].ToString()))
                                {
                                    if (dr["RelationType"].ToString().Trim() == "Y")
                                    {
                                        info.SaleRule.ProductRange.RelationType = RelationType.Y;
                                    }
                                    else
                                    {
                                        info.SaleRule.ProductRange.RelationType = RelationType.N;
                                    }
                                }
                                if (!string.IsNullOrEmpty(dr["ProductStatus"].ToString()))
                                {
                                    ProductStatus productStatus;
                                    if (Enum.TryParse<ProductStatus>(dr["ProductStatus"].ToString(), out productStatus))
                                    {
                                        product.ProductStatus = EnumHelper.GetDescription(productStatus);
                                    }
                                }
                                info.SaleRule.ProductRange.Products.Add(product);
                            }
                        }

                        #endregion
                    }



                    DataTable dtSaleRulesCustomer = ds.Tables[6];
                    if (dtSaleRulesCustomer.Rows.Count > 0)
                    {

                        #region Coupon_SaleRulesCustomer的数据绑定
                        foreach (DataRow dr in dtSaleRulesCustomer.Rows)
                        {

                            RelCustomer customer = new RelCustomer();
                            //cs.Customer = new SampleObject(int.Parse(dr["CustomerSysNo"].ToString()));
                            customer.CustomerSysNo = int.Parse(dr["CustomerSysNo"].ToString());

                            customer.CustomerID = string.IsNullOrEmpty(dr["CustomerID"].ToString()) ? null : dr["CustomerID"].ToString().Trim();
                            customer.CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? null : dr["CustomerName"].ToString().Trim();
                            info.SaleRule.CustomerRange.Customers.Add(customer);
                            info.SaleRule.CustomerRange.CustomerRangeType = CouponCustomerRangeType.Limit;
                        }
                        #endregion
                    }
                    else
                    {
                        info.SaleRule.CustomerRange.CustomerRangeType = CouponCustomerRangeType.All;
                    }

                    if (info != null
                        && info.BindRule != null
                        && info.BindRule.BindCondition == CouponsBindConditionType.None)
                    {
                        List<CouponCode> codes = LoadCouponCodes(info.SysNo);
                        if (codes != null && codes.Count > 0)
                        {
                            info.CouponCodes = codes;
                            if (codes[0].CodeType == CouponCodeType.Common)
                            {
                                info.GeneralCode = codes[0];
                            }
                            else
                            {
                                string BatchCode = "";
                                for (int i = 0; i < codes.Count; i++)
                                {
                                    if (i == codes.Count - 1)
                                    {
                                        BatchCode += codes[i].Code;
                                    }
                                    else
                                    {
                                        BatchCode += codes[i].Code + "\n";
                                    }
                                }
                                info.ThrowInCodes= BatchCode;
                            }
                        }
                    }

                    if (info != null && (info.Status == CouponStatus.Run
                        || info.Status == CouponStatus.Finish
                        || info.Status == CouponStatus.Stoped))
                    {
                        info.CouponCodes = LoadCouponCodes(info.SysNo);
                    }
                }
            }
            return info;
        }

        public static int? CreateMaster(Coupon info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCouponsMaster");

            cmd.SetParameterValue("@CouponName", info.CouponName);
            cmd.SetParameterValue("@CouponDesc", info.CouponDesc);
            if (info.SaleRule != null && info.SaleRule.ProductRange != null)
            {
                cmd.SetParameterValue("@RulesType", info.SaleRule.ProductRange.ProductRangeType);
            }
            else
            {
                cmd.SetParameterValue("@RulesType", "A");
            }
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@MerchantSysNo", info.MerchantSysNo);
            cmd.SetParameterValue("@BeginDate", info.BeginDate);
            cmd.SetParameterValue("@EndDate", info.EndDate);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);

            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo;
        }

        public static void UpdateMaster(Coupon info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCouponsMaster");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@CouponName", info.CouponName);
            cmd.SetParameterValue("@CouponDesc", info.CouponDesc);
            if (info.SaleRule != null && info.SaleRule.ProductRange != null)
            {
                cmd.SetParameterValue("@RulesType", info.SaleRule.ProductRange.ProductRangeType);
            }
            else
            {
                cmd.SetParameterValue("@RulesType", "A");
            }
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@BeginDate", info.BeginDate);
            cmd.SetParameterValue("@EndDate", info.EndDate);
            cmd.SetParameterValue("@EditUser", info.EditUser);
            cmd.ExecuteNonQuery();

        }

        public static void AddDiscountRule(Coupon info)
        {
            if (info.DiscountRules != null && info.DiscountRules.Count > 0)
            {
                foreach (CouponDiscountRule rule in info.DiscountRules)
                {
                    DataCommand cmd1 = DataCommandManager.GetDataCommand("Coupons_AddDiscountRules");
                    string rulesType = rule.RulesType == CouponDiscountRuleType.Discount ? "D" : "P";
                    cmd1.SetParameterValue("@CouponSysNo", info.SysNo);
                    cmd1.SetParameterValue("@RulesType", rulesType);
                    cmd1.SetParameterValue("@Amount", rule.Amount);
                    cmd1.SetParameterValue("@Value", rule.Value);
                    cmd1.SetParameterValue("@InUser", info.InUser);
                    cmd1.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteDiscountRule(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_DeleteDiscountRules");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void SetCouponSaleRule(Coupon info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_SetSaleRuleEx");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo);
            cmd.SetParameterValue("@OrderAmountLimit", info.SaleRule.OrderAmountLimit);
            cmd.SetParameterValue("@OrderMaxDiscount", info.SaleRule.OrderMaxDiscount);
            cmd.SetParameterValue("@CustomerMaxFrequency", info.SaleRule.CustomerMaxFrequency);
            cmd.SetParameterValue("@MaxFrequency", info.SaleRule.MaxFrequency);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.ExecuteNonQuery();
        }

        public static void AddSeleRulesProductCondition(Coupon info)
        {
            if (info != null
                && info.SaleRule != null
                && info.SaleRule.ProductRange != null
                && info.SaleRule.ProductRange.Products != null
                && info.SaleRule.ProductRange.Products.Count > 0)
            {
                foreach (RelProduct product in info.SaleRule.ProductRange.Products)
                {
                    AddCoupon_SaleRules(info.SysNo.Value, info.InUser
                        , info.SaleRule.ProductRange.RelationType == RelationType.Y ? "Y" : "N"
                        , product);
                }
            }

        }

        public static void DeleteSeleRulesProductCondition(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteSeleRulesProductCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteSeleRulesCustomerCondition(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteCustomerCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void AddCustomerCondition(Coupon info)
        {
            if (info != null
                && info.SaleRule != null
                && info.SaleRule.CustomerRange != null
                && info.SaleRule.CustomerRange.Customers != null
                && info.SaleRule.CustomerRange.Customers.Count > 0)
            {
                string status = "N";
                foreach (RelCustomer cas in info.SaleRule.CustomerRange.Customers)
                {
                    AddCoupon_SaleRulesCustomer(info.SysNo.Value, cas.CustomerSysNo, status, info.InUser);
                }
            }
            else
            {
                AddCoupon_SaleRulesCustomerRank(info.SysNo.Value, info.InUser);
            }


        }

        public static void SetCouponBindRules(Coupon info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_SetBindRules");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo);
            cmd.SetParameterValue("@BindCondition", info.BindRule.BindCondition);
            cmd.SetParameterValue("@ValidPeriod", info.BindRule.ValidPeriod);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.SetParameterValue("@BeginDate", info.BindRule.BindBeginDate);
            cmd.SetParameterValue("@EndDate", info.BindRule.BindEndDate);
            cmd.SetParameterValue("@AmountLimit", info.BindRule.AmountLimit);
            if (info.BindRule.ProductRange != null)
            {
                cmd.SetParameterValue("@LimitType", info.BindRule.ProductRange.ProductRangeType);
            }
            else
            {
                cmd.SetParameterValue("@LimitType", "A");
            }

            cmd.SetParameterValue("@IsAutoBinding", "N");


            cmd.ExecuteNonQuery();
        }

        public static void DeleteBindRulesProductCondition(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteBindRulesProductCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void AddBindRulesProductCondition(Coupon info)
        {
            if (info != null
                && info.BindRule != null
                && info.BindRule.ProductRange != null
                && info.BindRule.ProductRange.Products != null
                && info.BindRule.ProductRange.Products.Count > 0)
            {
                foreach (RelProduct product in info.BindRule.ProductRange.Products)
                {
                    AddCoupon_BindRulesProduct(info.SysNo.Value, info.InUser
                        , info.BindRule.ProductRange.RelationType == RelationType.Y ? "Y" : "N"
                        , product);
                }
            }
        }

        private static void AddCoupon_SaleRules(int couponsSysNo, string username, string relation, RelProduct product)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_SaleRules");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@Type", "I");
            cmd.SetParameterValue("@ProductSysNo", product.ProductSysNo);
            cmd.SetParameterValue("@InUser", username);
            cmd.SetParameterValue("@RelationType", relation);
            cmd.ExecuteNonQuery();
        }

        private static void AddCoupon_SaleRulesCustomerRank(int couponsSysNo, string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_SaleRulesCustomerRank");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@CustomerRank", -1);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
        }

        private static void AddCoupon_SaleRulesCustomer(int couponsSysNo, int? customerSysNo, string status, string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_SaleRulesCustomer");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
        }

        private static void AddCoupon_BindRulesProduct(int couponsSysNo, string username, string relation, RelProduct product)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_BindRulesProductCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@RuleItemType", "I");
            cmd.SetParameterValue("@ItemDataSysNo", product.ProductSysNo);
            cmd.SetParameterValue("@RelationType", relation);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
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

        public static void CreateCouponCode(CouponCode info, string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCouponCode");

            cmd.SetParameterValue("@CouponSysNo", info.CouponSysNo);
            cmd.SetParameterValue("@CouponCode", info.Code);
            cmd.SetParameterValue("@CustomerMaxFrequency", info.CustomerMaxFrequency);
            cmd.SetParameterValue("@TotalCount", info.TotalCount);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量创建
        /// </summary>
        /// <param name="info"></param>
        /// <param name="username"></param>
        public static void BatchCreateCouponCode(string couponCodeXml, Coupon info, string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("BatchCreateCouponCode");

            cmd.SetParameterValue("@CouponSysNo", info.SysNo);
            cmd.SetParameterValue("@CouponCodeXml", couponCodeXml);
            cmd.SetParameterValue("@CustomerMaxFrequency", info.SaleRule.CustomerMaxFrequency);
            cmd.SetParameterValue("@TotalCount", info.SaleRule.MaxFrequency);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
        }

        public static void DelAllCouponCode(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelAllCouponCode");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateStatus(int couponSysNo, CouponStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCouponsStatus");
            cmd.SetParameterValue("@SysNo", couponSysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        public static QueryResult<CouponCode> QueryCouponCodeList(CouponCodeQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCode");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "c.SysNo ASC" : queryFilter.SortFields))
            {
                //Set SQL WHERE Condition:

                if (queryFilter.CouponSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.CouponSysNo", DbType.Int32, "@CouponSysNo", QueryConditionOperatorType.Equal, queryFilter.CouponSysNo);
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.CouponCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.CouponCode", DbType.String, "@CouponCode", QueryConditionOperatorType.Like, queryFilter.CouponCode);
                }

                if (queryFilter.InDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.InDate", DbType.DateTime, "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.InDateFrom.Value.Date);
                }

                if (queryFilter.InDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.InDate", DbType.DateTime, "@InDateTo", QueryConditionOperatorType.LessThan, queryFilter.InDateTo.Value.Date.AddDays(1));
                }

                if (queryFilter.BeginDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.BeginDate", DbType.DateTime, "@BeginDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.BeginDateFrom.Value.Date);
                }

                if (queryFilter.BeginDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.BeginDate", DbType.DateTime, "@BeginDateTo", QueryConditionOperatorType.LessThan, queryFilter.BeginDateTo.Value.Date.AddDays(1));
                }

                if (queryFilter.EndDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.EndDate", DbType.DateTime, "@EndDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.EndDateFrom.Value.Date);
                }

                if (queryFilter.EndDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.EndDate", DbType.DateTime, "@EndDateTo", QueryConditionOperatorType.LessThan, queryFilter.EndDateTo.Value.Date.AddDays(1));
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<CouponCode> resultList = command.ExecuteEntityList<CouponCode>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<CouponCode>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }

        }

        public static CouponStatus GetCouponStatus(int couponSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponsStatus");
            cmd.SetParameterValue("@SysNo", couponSysNo);
            return cmd.ExecuteScalar<CouponStatus>();
        }

        public static List<CouponCode> LoadCouponCodes(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponCodesByCouponSysNo");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            return cmd.ExecuteEntityList<CouponCode>();
        }

        /// <summary>
        /// 优惠券使用记录查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult QueryCouponCodeRedeemLog(CouponCodeRedeemLogFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCodeRedeemLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, filter, string.IsNullOrEmpty(filter.SortFields) ? "Coupon.SysNo ASC" : filter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.MerchantSysNo", DbType.Int32,
                    "@MerchantSysNo", QueryConditionOperatorType.Equal, filter.MerchantSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal, filter.CouponSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.CouponName", DbType.String,
                    "@CouponName", QueryConditionOperatorType.Equal, filter.CouponName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RedeemLog.CouponCode", DbType.String,
                    "@CouponCode", QueryConditionOperatorType.Equal, filter.CouponCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cs.CustomerID", DbType.String,
                    "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.SysNo", DbType.String,
                    "@SOID", QueryConditionOperatorType.Equal, filter.SOID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RedeemLog.Status", DbType.String,
                    "@CouponCodeStatus", QueryConditionOperatorType.Equal, filter.CouponCodeStatus);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "RedeemLog.InDate", DbType.DateTime,
                     "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan
                     , filter.BeginUseDate.HasValue ? filter.BeginUseDate.Value.Date : filter.BeginUseDate
                     , filter.EndUseDate.HasValue ? filter.EndUseDate.Value.Date : filter.EndUseDate);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable resultList = cmd.ExecuteDataTable(new EnumColumnList{
                    {"SOStatus",typeof(SOStatus)},{"RedeemLogStatus",typeof(CouponCodeUsedStatus)}});

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return new QueryResult(resultList, filter, totalCount);// { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        /// <summary>
        /// 优惠券发放记录查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static QueryResult QueryCouponCodeCustomerLog(CouponCodeCustomerLogFilter filter)
        {

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCodeCustomerLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, filter, string.IsNullOrEmpty(filter.SortFields) ? "Coupon.SysNo ASC" : filter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.MerchantSysNo", DbType.Int32,
                    "@MerchantSysNo", QueryConditionOperatorType.Equal, filter.MerchantSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal, filter.CouponSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.CouponName", DbType.String,
                    "@CouponName", QueryConditionOperatorType.Like, filter.CouponName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CustomerLog.CouponCode", DbType.String,
                    "@CouponCode", QueryConditionOperatorType.Equal, filter.CouponCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Customer.SysNo", DbType.Int32,
                   "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Customer.CustomerID", DbType.String,
                    "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);


                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "CustomerLog.GetCouponCodeDate", DbType.DateTime,
                     "@GetCouponCodeDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan,
                     filter.BeginUseDate, filter.EndUseDate);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable resultList = cmd.ExecuteDataTable(new EnumColumnList{
                    {"Status",typeof(CouponStatus)}});

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return new QueryResult(resultList, filter, totalCount);
            }
        }

        /// <summary>
        /// 获取优惠券商家编号
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        public static int GetCouponMerchantSysNo(int couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponMerchantSysNo");
            command.SetParameterValue("@SysNo", couponSysNo);
            return command.ExecuteScalar<int>();
        }
    }

}
