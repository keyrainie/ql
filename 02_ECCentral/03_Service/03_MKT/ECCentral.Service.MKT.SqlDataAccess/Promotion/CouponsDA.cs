using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess.Promotion
{
    [VersionExport(typeof(ICouponsDA))]
    public class CouponsDA:ICouponsDA
    {
        public CouponsInfo Load(int? sysNo)
        {
            CouponsInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCouponsInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            if (dtMaster.Rows.Count > 0)
            {
                info = DataMapper.GetEntity<CouponsInfo>(dtMaster.Rows[0]);

                DataTable dtBindRule = ds.Tables[1];

                if (info.BindRule == null) info.BindRule = new CouponBindRule();
                if (dtBindRule.Rows.Count > 0)
                {
                    #region Coupon_BindRules的数据绑定
                    DataRow dr = dtBindRule.Rows[0];
                    info.IsAutoBinding = string.IsNullOrEmpty(dr["IsAutoBinding"].ToString()) ? false :
                        dr["IsAutoBinding"].ToString().Trim() == "Y" ? true : false;
                    info.IsSendMail = string.IsNullOrEmpty(dr["IsSendMail"].ToString()) ? false :
                        dr["IsSendMail"].ToString().Trim() == "Y" ? true : false;

                    if (!string.IsNullOrEmpty(dr["BindCondition"].ToString()))
                    {
                        object bindConditionType;
                        EnumCodeMapper.TryGetEnum(dr["BindCondition"], typeof(CouponsBindConditionType), out bindConditionType);
                        info.BindCondition = (CouponsBindConditionType)bindConditionType;
                    }

                    if (!string.IsNullOrEmpty(dr["ValidPeriod"].ToString()))
                    {
                        object validPeriod;
                        EnumCodeMapper.TryGetEnum(dr["ValidPeriod"], typeof(CouponsValidPeriodType), out validPeriod);
                        info.ValidPeriod = (CouponsValidPeriodType)validPeriod;
                    }
                    if (!string.IsNullOrEmpty(dr["BindingDate"].ToString()))
                    {
                        info.BindingDate = DateTime.Parse(dr["BindingDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dr["BindBeginDate"].ToString()))
                    {
                        info.CustomBindBeginDate = DateTime.Parse(dr["BindBeginDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dr["BindEndDate"].ToString() ))
                    {
                        info.CustomBindEndDate = DateTime.Parse(dr["BindEndDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dr["Status"].ToString()) )
                    {
                        info.BindingStatus = dr["Status"].ToString().Trim();
                    }
                     
                    //if (dr["InDate"] != null)
                    //{
                    //    info.BindingInDate = DateTime.Parse(dr["InDate"].ToString().Trim());
                    //}
                    //if (dr["EditDate"] != null)
                    //{
                    //    info.BindingInDate = DateTime.Parse(dr["EditDate"].ToString().Trim());
                    //}
                    //if (dr["InUser"] != null)
                    //{
                    //    info.BindingInUser = dr["InUser"].ToString().Trim();
                    //}
                    //if (dr["EditUser"] != null)
                    //{
                    //    info.BindingEditUser = dr["EditUser"].ToString().Trim();
                    //}

                    if (!string.IsNullOrEmpty(dr["AmountLimit"].ToString()))
                    {
                        info.BindRule.AmountLimit = decimal.Parse(dr["AmountLimit"].ToString().Trim());
                    }

                    if (!string.IsNullOrEmpty(dr["LimitType"].ToString()))
                    {
                        if (dr["LimitType"].ToString().Trim().ToLower() == "A".ToLower())
                        {
                            info.BindRule.ProductRangeType = ProductRangeType.All;
                        }
                        else
                        {
                            info.BindRule.ProductRangeType = ProductRangeType.Limit;
                        }
                    }
                    else
                    {
                        info.BindRule.ProductRangeType = ProductRangeType.All;
                    }

                    #endregion
                }

                DataTable dtDiscountRules = ds.Tables[2];
                if (dtDiscountRules.Rows.Count > 0)
                {
                    #region Coupon_DiscountRules的数据绑定 
                    foreach (DataRow dr in dtDiscountRules.Rows)
                    {
                        if (dr["RulesType"].ToString().Trim().ToUpper() == "Z")
                        {
                            if (info.PriceDiscountRule == null) info.PriceDiscountRule = new List<PSPriceDiscountRule>();

                            PSPriceDiscountRule priceRule = new PSPriceDiscountRule();
                            priceRule.DiscountType = PSDiscountTypeForProductPrice.ProductPriceDiscount;
                            priceRule.DiscountValue = decimal.Parse(dr["Value"].ToString().Trim());
                            priceRule.MaxQty = int.Parse(dr["Quantity"].ToString().Trim());
                            info.PriceDiscountRule.Add(priceRule);
                        }
                        if (dr["RulesType"].ToString().Trim().ToUpper() == "F")
                        {
                            if (info.PriceDiscountRule == null) info.PriceDiscountRule = new List<PSPriceDiscountRule>();

                            PSPriceDiscountRule priceRule = new PSPriceDiscountRule();
                            priceRule.DiscountType = PSDiscountTypeForProductPrice.ProductPriceFinal;
                            priceRule.DiscountValue = decimal.Parse(dr["Value"].ToString().Trim());
                            priceRule.MaxQty = int.Parse(dr["Quantity"].ToString().Trim());
                            info.PriceDiscountRule.Add(priceRule);
                        }
                        if (dr["RulesType"].ToString().Trim().ToUpper() == "P")
                        {
                            if (info.OrderAmountDiscountRule == null) info.OrderAmountDiscountRule = new PSOrderAmountDiscountRule ();
                            if (info.OrderAmountDiscountRule.OrderAmountDiscountRank == null) info.OrderAmountDiscountRule.OrderAmountDiscountRank = new List<OrderAmountDiscountRank>();
                            OrderAmountDiscountRank rank = new OrderAmountDiscountRank();
                            rank.DiscountType = PSDiscountTypeForOrderAmount.OrderAmountPercentage;
                            rank.DiscountValue = decimal.Parse(dr["Value"].ToString().Trim());
                            rank.OrderMinAmount = decimal.Parse(dr["Amount"].ToString().Trim());
                            info.OrderAmountDiscountRule.OrderAmountDiscountRank.Add(rank);
                        }
                        if (dr["RulesType"].ToString().Trim().ToUpper() == "D")
                        {
                            if (info.OrderAmountDiscountRule == null) info.OrderAmountDiscountRule = new PSOrderAmountDiscountRule();
                            if (info.OrderAmountDiscountRule.OrderAmountDiscountRank == null) info.OrderAmountDiscountRule.OrderAmountDiscountRank = new List<OrderAmountDiscountRank>();
                            OrderAmountDiscountRank rank = new OrderAmountDiscountRank();
                            rank.DiscountType = PSDiscountTypeForOrderAmount.OrderAmountDiscount;
                            rank.DiscountValue = decimal.Parse(dr["Value"].ToString().Trim());
                            rank.OrderMinAmount = decimal.Parse(dr["Amount"].ToString().Trim());
                            info.OrderAmountDiscountRule.OrderAmountDiscountRank.Add(rank);
                        }                        

                    }
                    #endregion
                }

                DataTable dtSaleRules = ds.Tables[3];
                if (dtSaleRules.Rows.Count > 0)
                {
                    #region Coupon_SaleRules的数据绑定
                    foreach (DataRow dr in dtSaleRules.Rows)
                    {
                        if (dr["Type"].ToString().Trim().ToUpper() == "I")
                        {
                            if (info.ProductCondition == null) info.ProductCondition = new PSProductCondition();
                            if (info.ProductCondition.RelProducts == null) info.ProductCondition.RelProducts = new RelProduct();
                            if (info.ProductCondition.RelProducts.ProductList == null) info.ProductCondition.RelProducts.ProductList = new List<RelProductAndQty>();

                            info.ProductCondition.RelProducts.IsIncludeRelation = string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true : 
                                dr["RelationType"].ToString().Trim() == "Y" ?true : false;
                            
                            RelProductAndQty productAndQty = new RelProductAndQty();
                            productAndQty.MinQty = 1;
                            productAndQty.ProductSysNo = int.Parse(dr["ProductSysNo"].ToString().Trim());
                            info.ProductCondition.RelProducts.ProductList.Add(productAndQty);
                        }
                        if (dr["Type"].ToString().Trim().ToUpper() == "C")
                        {
                            if (info.ProductCondition == null) info.ProductCondition = new PSProductCondition();
                            if (info.ProductCondition.RelCategories == null) info.ProductCondition.RelCategories = new RelCategory3();
                            if (info.ProductCondition.RelCategories.CategoryList == null) info.ProductCondition.RelCategories.CategoryList = new List<SimpleObject>();

                            info.ProductCondition.RelCategories.IsIncludeRelation = string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true :
                                dr["RelationType"].ToString().Trim() == "Y" ? true : false;

                           info.ProductCondition.RelCategories.CategoryList.Add(new SimpleObject(int.Parse(dr["C3SysNo"].ToString().Trim())));
                        }
                        if (dr["Type"].ToString().Trim().ToUpper() == "B")
                        {
                            if (info.ProductCondition == null) info.ProductCondition = new PSProductCondition();
                            if (info.ProductCondition.RelBrands == null) info.ProductCondition.RelBrands = new RelBrand();
                            if (info.ProductCondition.RelBrands.BrandList == null) info.ProductCondition.RelBrands.BrandList = new List<SimpleObject>();

                            info.ProductCondition.RelBrands.IsIncludeRelation = string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true :
                                dr["RelationType"].ToString().Trim() == "Y" ? true : false;

                            info.ProductCondition.RelBrands.BrandList.Add(new SimpleObject(int.Parse(dr["BrandSysNo"].ToString().Trim())));
                        }
                        if (dr["Type"].ToString().Trim().ToUpper() == "R")
                        {
                            if (info.CustomerCondition == null) info.CustomerCondition = new PSCustomerCondition();
                            if (info.CustomerCondition.RelCustomerRanks == null) info.CustomerCondition.RelCustomerRanks = new RelCustomerRank();
                            if (info.CustomerCondition.RelCustomerRanks.CustomerRankList == null) info.CustomerCondition.RelCustomerRanks.CustomerRankList = new List<SimpleObject>();

                            info.CustomerCondition.RelCustomerRanks.IsIncludeRelation =  string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true :
                                dr["RelationType"].ToString().Trim() == "Y" ? true : false;

                            info.CustomerCondition.RelCustomerRanks.CustomerRankList.Add(new SimpleObject(int.Parse(dr["CustomerRank"].ToString().Trim())));
                        }
                        if (dr["Type"].ToString().Trim().ToUpper() == "A")
                        {
                            if (info.CustomerCondition == null) info.CustomerCondition = new PSCustomerCondition();
                            if (info.CustomerCondition.RelAreas == null) info.CustomerCondition.RelAreas = new RelArea();
                            if (info.CustomerCondition.RelAreas.AreaList == null) info.CustomerCondition.RelAreas.AreaList = new List<SimpleObject>();

                            info.CustomerCondition.RelAreas.IsIncludeRelation =  string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true  :
                                dr["RelationType"].ToString().Trim() == "Y" ? true : false ;

                            info.CustomerCondition.RelAreas.AreaList.Add(new SimpleObject(int.Parse(dr["AreaSysNo"].ToString().Trim())));
                        }
                    }

                    #endregion
                }

                DataTable dtSaleRules_Ex = ds.Tables[4];
                if (dtSaleRules_Ex.Rows.Count > 0)
                {
                    if (info.OrderAmountDiscountRule == null) info.OrderAmountDiscountRule = new PSOrderAmountDiscountRule();
                    #region Coupon_SaleRules_Ex的数据绑定
                    DataRow dr = dtSaleRules_Ex.Rows[0];
                    if (info.OrderCondition == null) info.OrderCondition = new PSOrderCondition();
                     
                    if(!string.IsNullOrEmpty(dr["OrderAmountLimit"].ToString()))
                    {
                        info.OrderCondition.OrderMinAmount = decimal.Parse(dr["OrderAmountLimit"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dr["PayTypeSysNo"].ToString()))
                    {
                        info.OrderCondition.PayTypeSysNoList = new  List<int>  { int.Parse(dr["PayTypeSysNo"].ToString()) };
                    }
                    if (!string.IsNullOrEmpty(dr["ShippingTypeSysNo"].ToString()))
                    {
                        info.OrderCondition.ShippingTypeSysNoList = new  List<int>  { int.Parse(dr["ShippingTypeSysNo"].ToString()) };
                    }
                    if (!string.IsNullOrEmpty(dr["OrderMaxDiscount"].ToString()))
                    {                      
                        info.OrderAmountDiscountRule.OrderMaxDiscount = decimal.Parse(dr["OrderMaxDiscount"].ToString().Trim());
                    }

                    if (info.UsingFrequencyCondition == null) info.UsingFrequencyCondition = new PSActivityFrequencyCondition(); 
                    if (!string.IsNullOrEmpty(dr["CustomerMaxFrequency"].ToString()))
                    {
                        info.UsingFrequencyCondition.CustomerMaxFrequency = int.Parse(dr["CustomerMaxFrequency"].ToString());
                    }
                    if (!string.IsNullOrEmpty(dr["MaxFrequency"].ToString()))
                    {
                        info.UsingFrequencyCondition.MaxFrequency = int.Parse(dr["MaxFrequency"].ToString());
                    }
                    if (!string.IsNullOrEmpty(dr["UsedFrequency"] .ToString()))
                    {
                        info.UsingFrequencyCondition.UsedFrequency = int.Parse(dr["UsedFrequency"].ToString());
                    }
                    
                    if (info.CustomerCondition == null) info.CustomerCondition = new PSCustomerCondition();
                    info.CustomerCondition.NeedEmailVerification =  string.IsNullOrEmpty(dr["NeedEmailVerification"].ToString()) ? default(bool?) : dr["NeedEmailVerification"].ToString().Trim() == "Y" ? true : false;
                    info.CustomerCondition.NeedMobileVerification = string.IsNullOrEmpty(dr["NeedMobileVerification"].ToString()) ? default(bool?) : dr["NeedMobileVerification"].ToString().Trim() == "Y" ? true : false;
                    info.CustomerCondition.InvalidForAmbassador = string.IsNullOrEmpty(dr["InvalidForAmbassador"].ToString()) ? default(bool?) : dr["InvalidForAmbassador"].ToString().Trim() == "Y" ? true : false;
                    info.IsAutoUse = dr["IsAutoUse"].ToString().Trim() == "Y" ? true : false;
                    
                    #endregion
                }

                DataTable dtSaleRulesCustomer = ds.Tables[5];
                if (dtSaleRulesCustomer.Rows.Count > 0)
                {
                    #region Coupon_SaleRulesCustomer的数据绑定
                    if (info.CustomerCondition == null) info.CustomerCondition = new PSCustomerCondition();
                    if (info.CustomerCondition.RelCustomers == null) info.CustomerCondition.RelCustomers = new RelCustomer();
                    if (info.CustomerCondition.RelCustomers.CustomerIDList == null) info.CustomerCondition.RelCustomers.CustomerIDList = new List<CustomerAndSend>();
                    foreach (DataRow dr in dtSaleRulesCustomer.Rows)
                    {
                        CustomerAndSend cs = new CustomerAndSend();
                        //cs.Customer = new SampleObject(int.Parse(dr["CustomerSysNo"].ToString()));
                        cs.CustomerSysNo = int.Parse(dr["CustomerSysNo"].ToString());
                        cs.SendStatus =  string.IsNullOrEmpty(dr["Status"].ToString()) ? null : dr["Status"].ToString().Trim();
                        cs.CustomerID = string.IsNullOrEmpty(dr["CustomerID"].ToString()) ? null : dr["CustomerID"].ToString().Trim();
                        cs.CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? null : dr["CustomerName"].ToString().Trim();
                        info.CustomerCondition.RelCustomers.CustomerIDList.Add(cs);
                    }
                    #endregion
                }

                DataTable dtBindRulesProduct = ds.Tables[6];
                if (dtBindRulesProduct.Rows.Count > 0)
                {
                    #region Coupon_BindRulesProduct的数据绑定
                    
                    if (info.BindRule.RelProducts == null) info.BindRule.RelProducts = new RelProduct();
                    if (info.BindRule.RelProducts.ProductList == null) info.BindRule.RelProducts.ProductList = new List<RelProductAndQty>();
                    foreach (DataRow dr in dtBindRulesProduct.Rows)
                    {
                        info.BindRule.RelProducts.IsIncludeRelation = string.IsNullOrEmpty(dr["RelationType"].ToString()) ? true :
                                dr["RelationType"].ToString().Trim() == "Y" ? true : false;

                        RelProductAndQty productAndQty = new RelProductAndQty();
                        productAndQty.MinQty = 1;
                        productAndQty.ProductSysNo = int.Parse(dr["ItemDataSysNo"].ToString().Trim());
                        info.BindRule.RelProducts.ProductList.Add(productAndQty);

                    }
                    #endregion
                }
            }
            return info;
        }

        /// <summary>
        /// 根据优惠券SysNo 获取对应优惠券的折扣活动信息
        /// </summary>
        /// <param name="sysNo">优惠券系统SysNo</param>
        /// <returns></returns>
        public CouponsInfo GetCouponsInfoByCouponCodeSysNo(int couponCodeSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponsSysNoByCouponCodeSysNo");
            command.SetParameterValue("@SysNo", couponCodeSysNo);
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                int couponSysNo = Convert.ToInt32(dt.Rows[0]["CouponSysNo"]);
                string couponCode = dt.Rows[0]["CouponCode"].ToString();
                CouponsInfo couponsInfo = Load(command.ExecuteScalar<int>());
                couponsInfo.CouponCodeSetting = new CouponCodeSetting();
                couponsInfo.CouponCodeSetting.CouponCode = couponCode;
                return couponsInfo;
            }
            return new CouponsInfo();
        }

        public void UpdateStatus(int couponSysNo, CouponsStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCouponsStatus");
            cmd.SetParameterValue("@SysNo", couponSysNo);
            cmd.SetParameterValue("@Status", status);
             cmd.SetParameterValue("@EditUser", userName); 
            cmd.ExecuteNonQuery();
        }

        public void Audit(int couponSysNo, CouponsStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AuditCoupons");
            cmd.SetParameterValue("@SysNo", couponSysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@AuditUser", userName);
            cmd.ExecuteNonQuery();
        }

        public bool CheckCouponCodeIsHave(int? couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckCouponCodeIsHave");
            command.SetParameterValue("@CouponSysNo", couponSysNo);
            int count= command.ExecuteScalar<int>();
            return count > 0;
        }

        /************* 局部处理 *******************/

        /// <summary>
        /// Load 活动基本信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CouponsInfo LoadMaster(int? sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCouponsMaster");
            cmd.SetParameterValue("@SysNo", sysNo);
            CouponsInfo info = cmd.ExecuteEntity<CouponsInfo>();
            return info;
        }

        public int? CreateMaster(CouponsInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCouponsMaster");
 
            cmd.SetParameterValue("@CouponName", info.Title.Content);
            cmd.SetParameterValue("@CouponDesc", info.UserDescription);
            cmd.SetParameterValue("@CouponType", info.CouponRuleType);
            cmd.SetParameterValue("@ChannelType", info.CouponChannelType);
            cmd.SetParameterValue("@RulesType", info.ProductRangeType); 
            cmd.SetParameterValue("@BeginDate", info.StartTime);
            cmd.SetParameterValue("@EndDate", info.EndTime);
            cmd.SetParameterValue("@EIMSSysNo", info.EIMSSysNo.HasValue ? info.EIMSSysNo : null);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            
            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo;
        }

        public void UpdateMaster(CouponsInfo info)
        {
            //object couponRuleType = "";
            //bool b = EnumCodeMapper.TryGetCode(info.CouponRuleType, out couponRuleType);

            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCouponsMaster");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@CouponName", info.Title.Content);
            cmd.SetParameterValue("@CouponDesc", info.UserDescription);
            cmd.SetParameterValue("@CouponType", info.CouponRuleType);
            cmd.SetParameterValue("@ChannelType", info.CouponChannelType);
            cmd.SetParameterValue("@RulesType", info.ProductRangeType);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@BeginDate", info.StartTime);
            cmd.SetParameterValue("@EndDate", info.EndTime);
            cmd.SetParameterValue("@EIMSSysNo", info.EIMSSysNo.HasValue ? info.EIMSSysNo : null);
            cmd.SetParameterValue("@EditUser", info.EditUser); 
            cmd.ExecuteNonQuery(); 

        }
                 

       /// <summary>
        /// 设置优惠券规则:设置订单条件，使用频率条件和每单折扣上限
        /// </summary>
        /// <param name="info"></param>
        public void SetSaleRuleEx(CouponsInfo info)
        {
            int? paytypeSysNo = null;
            int? shippingtypeSysNo = null;
            decimal? orderMaxDiscount = null;
            int? customerMaxFrequency=null;
            int? maxFrequency=null;
            if (info.OrderCondition != null)
            {
                if (info.OrderCondition.PayTypeSysNoList != null && info.OrderCondition.PayTypeSysNoList.Count > 0)
                {
                    paytypeSysNo = info.OrderCondition.PayTypeSysNoList[0];
                }
                if (info.OrderCondition.ShippingTypeSysNoList != null && info.OrderCondition.ShippingTypeSysNoList.Count > 0)
                {
                    shippingtypeSysNo = info.OrderCondition.ShippingTypeSysNoList[0];
                }
                if (info.OrderAmountDiscountRule != null)
                {
                    orderMaxDiscount = info.OrderAmountDiscountRule.OrderMaxDiscount;
                }
                if (info.UsingFrequencyCondition != null)
                {
                    customerMaxFrequency = info.UsingFrequencyCondition.CustomerMaxFrequency;
                    maxFrequency = info.UsingFrequencyCondition.MaxFrequency;
                }
            }

            DataCommand cmd = DataCommandManager.GetDataCommand("SetSaleRuleEx");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo);
            cmd.SetParameterValue("@OrderAmountLimit", info.OrderCondition.OrderMinAmount);
            cmd.SetParameterValue("@PayTypeSysNo", paytypeSysNo);
            cmd.SetParameterValue("@ShippingTypeSysNo", shippingtypeSysNo);
            cmd.SetParameterValue("@OrderMaxDiscount", orderMaxDiscount);
            cmd.SetParameterValue("@CustomerMaxFrequency", customerMaxFrequency);
            cmd.SetParameterValue("@MaxFrequency", maxFrequency);
            cmd.SetParameterValue("@IsAutoUse", info.IsAutoUse.Value ? "Y" : "N");
            cmd.SetParameterValue("@EditUser", info.InUser);  
            cmd.SetParameterValue("@InUser", info.InUser);  
            cmd.ExecuteNonQuery();

        }

        public void SetAmountDiscountRule(CouponsInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_DeleteDiscountRules");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo.Value);
            cmd.ExecuteNonQuery();

            if (info.OrderAmountDiscountRule != null && info.OrderAmountDiscountRule.OrderAmountDiscountRank != null)
            {
                foreach (OrderAmountDiscountRank rule in info.OrderAmountDiscountRule.OrderAmountDiscountRank)
                {
                    DataCommand cmd1 = DataCommandManager.GetDataCommand("Coupons_AddDiscountRules");
                    string rulesType = rule.DiscountType == PSDiscountTypeForOrderAmount.OrderAmountDiscount ? "D" : "P";
                    cmd1.SetParameterValue("@CouponSysNo", info.SysNo);
                    cmd1.SetParameterValue("@RulesType", rulesType);
                    cmd1.SetParameterValue("@Amount", rule.OrderMinAmount);
                    cmd1.SetParameterValue("@Value", rule.DiscountValue);
                    cmd1.SetParameterValue("@Quantity", DBNull.Value);
                    cmd1.SetParameterValue("@ProductSysNo", DBNull.Value);
                    cmd1.SetParameterValue("@InUser", info.InUser);
                    cmd1.ExecuteNonQuery();
                }
            }

            if (info.PriceDiscountRule != null && info.PriceDiscountRule.Count>0)
            {
                foreach (PSPriceDiscountRule rule in info.PriceDiscountRule)
                {
                    DataCommand cmd1 = DataCommandManager.GetDataCommand("Coupons_AddDiscountRules");
                    string rulesType = "Z";
                    switch (rule.DiscountType.Value)
                    {
                        case  PSDiscountTypeForProductPrice.ProductPriceDiscount:
                            rulesType = "Z";
                            break;
                        case  PSDiscountTypeForProductPrice.ProductPriceFinal:
                            rulesType = "F";
                            break;                        
                        default:
                            rulesType = "Z";
                            break;
                    }
                    cmd1.SetParameterValue("@CouponSysNo", info.SysNo);
                    cmd1.SetParameterValue("@RulesType", rulesType);
                    cmd1.SetParameterValue("@Amount", DBNull.Value);
                    cmd1.SetParameterValue("@Value", rule.DiscountValue);
                    cmd1.SetParameterValue("@Quantity", rule.MaxQty);
                    cmd1.SetParameterValue("@ProductSysNo", DBNull.Value);
                    cmd1.SetParameterValue("@InUser", info.InUser);
                    cmd1.ExecuteNonQuery();
                }
            }
        }

        public void SetCustomerNotifyRule(CouponsInfo info)
        {
            //object validPeriod;
            //EnumCodeMapper.TryGetCode(info.ValidPeriod,out validPeriod);
            //object bindCondition;
            //EnumCodeMapper.TryGetCode(info.BindCondition, out bindCondition);

            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_SetBindRules");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo.Value);
            cmd.SetParameterValue("@IsAutoBinding", info.IsAutoBinding.Value?"Y":"N");
            cmd.SetParameterValue("@BindingDate", info.BindingDate);
            cmd.SetParameterValue("@IsSendMail", info.IsSendMail.Value?"Y":"N");
            cmd.SetParameterValue("@BindCondition", info.BindCondition);
            cmd.SetParameterValue("@ValidPeriod", info.ValidPeriod);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.SetParameterValue("@BeginDate", info.CustomBindBeginDate);
            cmd.SetParameterValue("@EndDate", info.CustomBindEndDate); 
            cmd.SetParameterValue("@AmountLimit", info.BindRule == null ? null : info.BindRule.AmountLimit);
            if (info.BindRule == null || info.BindRule.ProductRangeType == ProductRangeType.All)
            {
                cmd.SetParameterValue("@LimitType", "A");
            }
            else
            {
                cmd.SetParameterValue("@LimitType", "I");
            }
            //cmd.SetParameterValue("@LimitType", info.BindRule == null ? ProductRangeType.All : info.BindRule.ProductRangeType); 
            cmd.ExecuteNonQuery();
            AddBindRulesProductCondition(info);
        }
               
        public bool IsExistEMIS(int emisSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistEMIS"); 
            cmd.SetParameterValue ("@SysNo",emisSysNo);
            object result = cmd.ExecuteScalar();
            if (result is DBNull || result == null)
            {
                return false;
            }
            else
            {
                return (int)result == 1;
            }
        }
        
        public virtual void SetProductCondition(CouponsInfo info)
        {
             DeleteProductCondition(info.SysNo);

            if (info.ProductCondition.RelBrands != null
                && info.ProductCondition.RelBrands.BrandList != null
                && info.ProductCondition.RelBrands.BrandList.Count > 0)
            {
                foreach (SimpleObject obj in info.ProductCondition.RelBrands.BrandList)
                {
                    AddCoupon_SaleRules(info.SysNo.Value, "B", null, obj.SysNo, null, null, null,
                        info.ProductCondition.RelBrands.IsIncludeRelation.Value  ? "Y" : "N", info.InUser);
                }
            }
            if (info.ProductCondition.RelCategories != null
                && info.ProductCondition.RelCategories.CategoryList != null
                && info.ProductCondition.RelCategories.CategoryList.Count > 0)
            {
                foreach (SimpleObject obj in info.ProductCondition.RelCategories.CategoryList)
                {
                    AddCoupon_SaleRules(info.SysNo.Value, "C", obj.SysNo, null, null, null, null,
                        info.ProductCondition.RelCategories.IsIncludeRelation.Value ? "Y" : "N", info.InUser);
                }
            }

            if (info.ProductCondition.RelProducts != null
                && info.ProductCondition.RelProducts.ProductList != null
                && info.ProductCondition.RelProducts.ProductList.Count > 0)
            {
                foreach (RelProductAndQty prd in info.ProductCondition.RelProducts.ProductList)
                {
                     AddCoupon_SaleRules(info.SysNo.Value, "I", null, null, prd.ProductSysNo, null, null,
                        info.ProductCondition.RelProducts.IsIncludeRelation.Value ? "Y" : "N", info.InUser);
                }
            }
        }

        public virtual void SetCustomerCondition(CouponsInfo info)
        {
            DeleteCustomerCondition(info.SysNo);

            if (info.CustomerCondition.RelAreas != null
                && info.CustomerCondition.RelAreas.AreaList != null
                && info.CustomerCondition.RelAreas.AreaList.Count > 0)
            {
                foreach (SimpleObject obj in info.CustomerCondition.RelAreas.AreaList)
                {
                    AddCoupon_SaleRules(info.SysNo.Value, "A", null, null, null, null, obj.SysNo, "Y", info.InUser);
                }
            }

            if (info.CustomerCondition.RelCustomerRanks != null
                && info.CustomerCondition.RelCustomerRanks.CustomerRankList != null
                && info.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 0)
            {
                foreach (SimpleObject obj in info.CustomerCondition.RelCustomerRanks.CustomerRankList)
                {
                    AddCoupon_SaleRules(info.SysNo.Value, "R", null, null, null, obj.SysNo, null, "Y", info.InUser);
                }
            }

            if (info.CustomerCondition.RelCustomers != null
                && info.CustomerCondition.RelCustomers.CustomerIDList != null
                && info.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
            {
                foreach (CustomerAndSend cas in info.CustomerCondition.RelCustomers.CustomerIDList)
                {
                    AddCoupon_SaleRulesCustomer(info.SysNo.Value, cas.CustomerSysNo, info.InUser);
                }
            }

            DataCommand cmd = DataCommandManager.GetDataCommand("SetCustomerConditionEx");
            cmd.SetParameterValue("@CouponSysNo", info.SysNo);
            cmd.SetParameterValue("@NeedEmailVerification", info.CustomerCondition.NeedEmailVerification.Value ? "Y" : "N");
            cmd.SetParameterValue("@NeedMobileVerification", info.CustomerCondition.NeedMobileVerification.Value ? "Y" : "N");
            cmd.SetParameterValue("@InvalidForAmbassador", info.CustomerCondition.InvalidForAmbassador.Value ? "Y" : "N");
            cmd.ExecuteNonQuery();

        }
                
        private void DeleteProductCondition(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        private void AddCoupon_SaleRules(int couponsSysNo, string type, int? c3SysNo, 
            int? brandSysNo,int? productSysNo,int? customerRank, int? areaSysNo, string relation, string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_SaleRules");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@Type", type);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CustomerRank", customerRank);
            cmd.SetParameterValue("@AreaSysNo", areaSysNo);
            cmd.SetParameterValue("@InUser", username);
            cmd.SetParameterValue("@RelationType", relation);
            cmd.ExecuteNonQuery();
             
        }

        private void DeleteCustomerCondition(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteCustomerCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        private void AddCoupon_SaleRulesCustomer(int couponsSysNo, int? customerSysNo,  string username)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_SaleRulesCustomer");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();

        }


        #region 优惠券编码相关

        public void CreateCouponCode(CouponCodeSetting info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCouponCode");

            cmd.SetParameterValue("@CouponSysNo", info.CouponsSysNo);
            cmd.SetParameterValue("@CouponCode", info.CouponCode);
            cmd.SetParameterValue("@CodeType", info.CouponCodeType);
            cmd.SetParameterValue("@DueInvertRate", info.DueInvertRate);
            cmd.SetParameterValue("@CustomerMaxFrequency", info.CCCustomerMaxFrequency);
            cmd.SetParameterValue("@TotalCount", info.CCMaxFrequency);
            cmd.SetParameterValue("@InUser", info.InUser); 
            cmd.ExecuteNonQuery();            
        }

        public void CreateCouponCodeForSend(CouponCodeSetting info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCouponCodeForSend");

            cmd.SetParameterValue("@CouponSysNo", info.CouponsSysNo);
            cmd.SetParameterValue("@CouponCode", info.CouponCode);
            cmd.SetParameterValue("@CodeType", info.CouponCodeType);
            cmd.SetParameterValue("@DueInvertRate", info.DueInvertRate);
            cmd.SetParameterValue("@CustomerMaxFrequency", info.CCCustomerMaxFrequency);
            cmd.SetParameterValue("@TotalCount", info.CCMaxFrequency);
            cmd.SetParameterValue("@BeginDate", info.StartTime);
            cmd.SetParameterValue("@EndDate", info.EndTime);
            cmd.SetParameterValue("@InUser", info.InUser);
            cmd.ExecuteNonQuery();
        }

        public void DelCouponCode(List<int?> couponCodeSysNoList)
        {
            CustomDataCommand  cmd = DataCommandManager.CreateCustomDataCommandFromConfig("DelCouponCode");
            string sql = cmd.CommandText;
            string inList = "";
            foreach (int? syso in couponCodeSysNoList)
            {
                inList += syso.Value.ToString() + ",";
            }

            inList=inList.TrimEnd(",".ToCharArray());
            
            sql = sql.Replace("#CouponCodeSysNoList#", inList);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

        }

        public void DelAllCouponCode(int? couponsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelAllCouponCode");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.ExecuteNonQuery();
        }

        public bool CheckExistThisTypeCouponCode(int? couponSysNo, CouponCodeType codeType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Coupons_CheckExistThisTypeCouponCode");
            cmd.SetParameterValue("@CouponSysNo", couponSysNo);
            cmd.SetParameterValue("@CodeType", codeType);
            int count = cmd.ExecuteScalar<int>();
            return count > 0;
        }

        /// <summary>
        /// 校验是否有重复的优惠券代码
        /// </summary>
        /// <param name="couponcode"></param>
        /// <returns></returns>
        public virtual bool CheckExistCode(string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckExistCode");
            command.SetParameterValue("@CouponCode", couponCode);
            return command.ExecuteScalar<int>()>0;
        }

        /// <summary>
        /// 根据优惠券编号检查优惠券是否有效，不管优惠券是否有效都要返回优惠券代码
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public virtual bool CheckCouponIsValidAndGetCode(int couponSysNo, out string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckCouponIsValidAndGetCode");
            command.SetParameterValue("@CouponCodeSysNo", couponSysNo);
            DataSet ds= command.ExecuteDataSet();
            DataTable dt1 = ds.Tables[0];
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                couponCode = dt1.Rows[0][0].ToString().Trim();
            }
            else
            {
                couponCode = "";
            }
            DataTable dt2 = ds.Tables[1];
            int count = int.Parse(dt2.Rows[0][0].ToString());
            return count > 0;

        }
        #endregion





        #region 促销引擎相关

        public CouponCodeSetting GetActivedCouponCodeInfoByCode(string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetActivedCouponCodeInfoByCode");
            command.SetParameterValue("@CouponCode", couponCode);
            return command.ExecuteEntity<CouponCodeSetting>();
        }

        public CouponCodeSetting GetCouponCodeInfoByCode(string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponCodeInfoByCode");
            command.SetParameterValue("@CouponCode", couponCode);
            return command.ExecuteEntity<CouponCodeSetting>();
        }

        public DataRow GetCouponsUsedCount(int? customerSysNo, int? couponsSysNo, string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponsUsedCount");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@CouponsSysNo", couponsSysNo);
            command.SetParameterValue("@CouponCode", couponCode);
            DataRow dr = command.ExecuteDataRow();
            return dr;

        }

        public bool CheckExistCustomerIssueLog(int? customerSysNo, string couponCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckExistCustomerIssueLog");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@CouponCode", couponCode);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public void CouponCodeApply(int couponSysNo, string couponCode, int customerSysNo,
            int shoppingCartSysNo, int soSysNo, decimal redeemAmount, string username)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CouponCodeApply");
            command.SetParameterValue("@CouponSysNo", couponSysNo);
            command.SetParameterValue("@CouponCode", couponCode);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@ShoppingCartSysNo", shoppingCartSysNo);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@RedeemAmount", redeemAmount);
            command.SetParameterValue("@InUser", username);

            int count = command.ExecuteNonQuery();
        }

        public void CouponCodeCancel(string couponCode, int soSysNo, int shoppingCartSysNo, string username)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CouponCodeCancel");
            command.SetParameterValue("@CouponCode", couponCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ShoppingCartSysNo", shoppingCartSysNo);
            command.SetParameterValue("@EditUser", username);

            int count = command.ExecuteNonQuery();
        }

        #endregion


        /// <summary>
        /// 获取订单产生的优惠卷信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPromotionCodeLog");
            command.SetParameterValue("@SOSysNo", soSysNo);
            PromotionCode_Customer_Log log = command.ExecuteEntity<PromotionCode_Customer_Log>();

            return log;
        }


        public List<ProductPromotionDiscountInfo> GetCouponAmount(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetCouponAmount");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntityList<ProductPromotionDiscountInfo>();
        }

        #region 限定商家相关
        /// <summary>
        /// 根据蛋卷获取商家限定信息
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        public List<RelVendor> GetVendorSaleRulesByCouponSysNo(int couponSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetVendorSaleRulesByCouponSysNo");
            cmd.SetParameterValue("@CouponSysNo", couponSysNo);
            return cmd.ExecuteEntityList<RelVendor>();
        }
        /// <summary>
        /// 创建商家限定
        /// </summary>
        /// <param name="info"></param>
        public void CreateVendorSaleRules(CouponsInfo info, int SellerSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("CreateVendorSaleRules");
            cmd.SetParameterValue("@CouponSysNo",info.SysNo);
            cmd.SetParameterValue("@Type", "S");
            cmd.SetParameterValue("@RelationType","Y");
            cmd.SetParameterValue("@UserName", info.InUser);
            cmd.SetParameterValue("@SellerSysNo",SellerSysNo);
            cmd.ExecuteNonQuery();
            
        }
        
        #endregion

        #region 赠送优惠券相关
        /// <summary>
        /// 查询下单时有效或者已完成的商家优惠券活动
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<CouponsInfo> QueryCoupons(int merchantSysNo, DateTime soDateTime)
        {
            //加载优惠券活动主信息
            var cmd = DataCommandManager.GetDataCommand("QueryCouponsByMerchant");
            cmd.SetParameterValue("@MerchantSysNo", merchantSysNo);
            cmd.SetParameterValue("@SODateTime", soDateTime);
            DataSet ds = cmd.ExecuteDataSet();
            //加载
            DataTable dtMaster = ds.Tables[0];
            List<CouponsInfo> coupons = DataMapper.GetEntityList<CouponsInfo, List<CouponsInfo>>(dtMaster.Rows);
            foreach (CouponsInfo couponsInfo in coupons)
            {
                var dtBindRule = ds.Tables[1];

                foreach (DataRow dataRowBR in dtBindRule.Rows)
                {
                    if (dataRowBR["CouponSysNo"].ToString() != couponsInfo.SysNo.ToString())
                    {
                        continue;
                    }
                    #region Coupon_BindRules的数据绑定
                    couponsInfo.IsAutoBinding = string.IsNullOrEmpty(dataRowBR["IsAutoBinding"].ToString()) ? false :
                        dataRowBR["IsAutoBinding"].ToString().Trim() == "Y" ? true : false;
                    couponsInfo.IsSendMail = string.IsNullOrEmpty(dataRowBR["IsSendMail"].ToString()) ? false :
                        dataRowBR["IsSendMail"].ToString().Trim() == "Y" ? true : false;

                    if (!string.IsNullOrEmpty(dataRowBR["BindCondition"].ToString()))
                    {
                        object bindConditionType;
                        EnumCodeMapper.TryGetEnum(dataRowBR["BindCondition"], typeof(CouponsBindConditionType), out bindConditionType);
                        couponsInfo.BindCondition = (CouponsBindConditionType)bindConditionType;
                    }

                    if (!string.IsNullOrEmpty(dataRowBR["ValidPeriod"].ToString()))
                    {
                        object validPeriod;
                        EnumCodeMapper.TryGetEnum(dataRowBR["ValidPeriod"], typeof(CouponsValidPeriodType), out validPeriod);
                        couponsInfo.ValidPeriod = (CouponsValidPeriodType)validPeriod;
                    }
                    if (!string.IsNullOrEmpty(dataRowBR["BindingDate"].ToString()))
                    {
                        couponsInfo.BindingDate = DateTime.Parse(dataRowBR["BindingDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dataRowBR["BindBeginDate"].ToString()))
                    {
                        couponsInfo.CustomBindBeginDate = DateTime.Parse(dataRowBR["BindBeginDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dataRowBR["BindEndDate"].ToString()))
                    {
                        couponsInfo.CustomBindEndDate = DateTime.Parse(dataRowBR["BindEndDate"].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(dataRowBR["Status"].ToString()))
                    {
                        couponsInfo.BindingStatus = dataRowBR["Status"].ToString().Trim();
                    }

                    //if (dr["InDate"] != null)
                    //{
                    //    info.BindingInDate = DateTime.Parse(dr["InDate"].ToString().Trim());
                    //}
                    //if (dr["EditDate"] != null)
                    //{
                    //    info.BindingInDate = DateTime.Parse(dr["EditDate"].ToString().Trim());
                    //}
                    //if (dr["InUser"] != null)
                    //{
                    //    info.BindingInUser = dr["InUser"].ToString().Trim();
                    //}
                    //if (dr["EditUser"] != null)
                    //{
                    //    info.BindingEditUser = dr["EditUser"].ToString().Trim();
                    //}
                    if (couponsInfo.BindRule == null) couponsInfo.BindRule = new CouponBindRule();
                    if (!string.IsNullOrEmpty(dataRowBR["AmountLimit"].ToString()))
                    {
                        couponsInfo.BindRule.AmountLimit = decimal.Parse(dataRowBR["AmountLimit"].ToString().Trim());
                    }

                    if (!string.IsNullOrEmpty(dataRowBR["LimitType"].ToString()))
                    {
                        if (dataRowBR["LimitType"].ToString().Trim().ToLower() == "A".ToLower())
                        {
                            couponsInfo.BindRule.ProductRangeType = ProductRangeType.All;
                        }
                        else
                        {
                            couponsInfo.BindRule.ProductRangeType = ProductRangeType.Limit;
                        }
                    }
                    else
                    {
                        couponsInfo.BindRule.ProductRangeType = ProductRangeType.All;
                    }
                    #endregion
                }

                DataTable dtBindRuleItems = ds.Tables[2];
                if (couponsInfo.BindRule.RelProducts == null) couponsInfo.BindRule.RelProducts = new RelProduct();
                if (couponsInfo.BindRule.RelProducts.ProductList == null) couponsInfo.BindRule.RelProducts.ProductList = new List<RelProductAndQty>();
                foreach (DataRow dataRowBRI in dtBindRuleItems.Rows)
                {
                    if (dataRowBRI["CouponSysNo"].ToString() != couponsInfo.SysNo.ToString())
                    {
                        continue;
                    }
                    #region Coupon_BindRuleItems的数据绑定
                    couponsInfo.BindRule.RelProducts.IsIncludeRelation = string.IsNullOrEmpty(dataRowBRI["RelationType"].ToString()) ? true :
                                dataRowBRI["RelationType"].ToString().Trim() == "Y" ? true : false;
                    RelProductAndQty productAndQty = new RelProductAndQty();
                    productAndQty.MinQty = 1;
                    productAndQty.ProductSysNo = int.Parse(dataRowBRI["ItemDataSysNo"].ToString().Trim());
                    couponsInfo.BindRule.RelProducts.ProductList.Add(productAndQty);
                    #endregion
                }

                DataTable dtSaleRulesCustomer = ds.Tables[3];
                foreach (DataRow dataRowSRC in dtSaleRulesCustomer.Rows)
                {
                    if (dataRowSRC["CouponSysNo"].ToString() != couponsInfo.SysNo.ToString())
                    {
                        continue;
                    }
                    #region Coupon_SaleRulesCustomer的数据绑定
                    if (couponsInfo.CustomerCondition == null) couponsInfo.CustomerCondition = new PSCustomerCondition();
                    if (couponsInfo.CustomerCondition.RelCustomers == null) couponsInfo.CustomerCondition.RelCustomers = new RelCustomer();
                    if (couponsInfo.CustomerCondition.RelCustomers.CustomerIDList == null) couponsInfo.CustomerCondition.RelCustomers.CustomerIDList = new List<CustomerAndSend>();

                    CustomerAndSend cs = new CustomerAndSend();
                    //cs.Customer = new SampleObject(int.Parse(dr["CustomerSysNo"].ToString()));
                    cs.CustomerSysNo = int.Parse(dataRowSRC["CustomerSysNo"].ToString());
                    cs.SendStatus = string.IsNullOrEmpty(dataRowSRC["Status"].ToString()) ? null : dataRowSRC["Status"].ToString().Trim();
                    cs.CustomerID = string.IsNullOrEmpty(dataRowSRC["CustomerID"].ToString()) ? null : dataRowSRC["CustomerID"].ToString().Trim();
                    cs.CustomerName = string.IsNullOrEmpty(dataRowSRC["CustomerName"].ToString()) ? null : dataRowSRC["CustomerName"].ToString().Trim();
                    couponsInfo.CustomerCondition.RelCustomers.CustomerIDList.Add(cs);
                    #endregion
                }
                //coupons.Add(couponsInfo);
            }
            return coupons;
        }
        /// <summary>
        /// 创建优惠卷获取记录
        /// </summary>
        /// <param name="couponCodeCustomerLog"></param>
        public void CreateCouponCodeCustomerLog(CouponCodeCustomerLog couponCodeCustomerLog)
        {
            var cmd = DataCommandManager.GetDataCommand("CreateCouponCodeCustomerLog");
            cmd.SetParameterValue(couponCodeCustomerLog);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 检查用户这个订单是否已经赠送过优惠券
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public bool CheckExistCouponCodeCustomerLog(int couponSysNo, int customerSysNo, int soSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("CheckExistCouponCodeCustomerLog");
            cmd.SetParameterValue("@CouponSysNo", couponSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            return cmd.ExecuteScalar<int>() > 0;
        }
        #endregion

        public static void AddBindRulesProductCondition(CouponsInfo info)
        {
            if (info != null
                && info.BindRule != null
                && info.BindRule.RelProducts != null
                && info.BindRule.RelProducts.ProductList != null
                && info.BindRule.RelProducts.ProductList.Count > 0)
            {
                foreach (RelProductAndQty product in info.BindRule.RelProducts.ProductList)
                {
                    AddCoupon_BindRulesProduct(info.SysNo.Value, info.InUser
                        , info.BindRule.RelProducts.IsIncludeRelation ==true  ? "Y" : "N"
                        , product);
                }
            }
        }

        private static void AddCoupon_BindRulesProduct(int couponsSysNo, string username, string relation, RelProductAndQty product)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCoupon_BindRulesProductCondition");
            cmd.SetParameterValue("@CouponSysNo", couponsSysNo);
            cmd.SetParameterValue("@RuleItemType", "I");
            cmd.SetParameterValue("@ItemDataSysNo", product.ProductSysNo);
            cmd.SetParameterValue("@RelationType", relation);
            cmd.SetParameterValue("@InUser", username);
            cmd.ExecuteNonQuery();
        }
    }
}
