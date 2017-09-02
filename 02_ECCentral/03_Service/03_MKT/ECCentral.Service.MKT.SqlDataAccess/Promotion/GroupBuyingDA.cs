using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity;


namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IGroupBuyingDA))]
    public class GroupBuyingDA : IGroupBuyingDA
    {
        /// <summary>
        /// 创建团购
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Create(GroupBuyingInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateProductGroupBuying");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@GroupBuyingTitle", entity.GroupBuyingTitle.Content);
            command.SetParameterValue("@GroupBuyingDesc", entity.GroupBuyingDesc.Content);
            command.SetParameterValue("@GroupBuyingRules", entity.GroupBuyingRules.Content);
            command.SetParameterValue("@GroupBuyingDescLong", entity.GroupBuyingDescLong.Content);
            command.SetParameterValue("@GroupBuyingPicUrl", entity.GroupBuyingPicUrl.Content);
            command.SetParameterValue("@GroupBuyingMiddlePicUrl", entity.GroupBuyingMiddlePicUrl.Content);
            command.SetParameterValue("@GroupBuyingSmallPicUrl", entity.GroupBuyingSmallPicUrl.Content);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@IsByGroup", entity.IsByGroup.Value ? "Y" : "N");
            command.SetParameterValue("@MaxPerOrder", entity.MaxCountPerOrder ?? 0);
            command.SetParameterValue("@OriginalPrice", 0);
            command.SetParameterValue("@DealPrice", 0);
            command.SetParameterValue("@SettlementStatus", "N");
            command.SetParameterValue("@GroupBuyingTypeSysNo", entity.GroupBuyingTypeSysNo);
            command.SetParameterValue("@GroupBuyingAreaSysNo", entity.GroupBuyingAreaSysNo);
            command.SetParameterValue("@LimitOrderCount", entity.LimitOrderCount);
            command.SetParameterValue("@CurrencySysNo", 1);
            command.SetParameterValue("@Status", "O");
            command.SetParameterValue("@Reasons", "");
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@InUser", entity.InUser);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", entity.CompanyCode);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@CurrentSellCount", entity.CurrentSellCount);
            command.SetParameterValue("@VendorSysNo", entity.GroupBuyingVendorSysNo);
            command.SetParameterValue("@CouponValidDate", entity.CouponValidDate);
            command.SetParameterValue("@LotteryRule", entity.LotteryRule);
            command.SetParameterValue("@GroupBuyingCategorySysNo", entity.GroupBuyingCategorySysNo);
            command.SetParameterValue("@IsWithoutReservation", entity.IsWithoutReservation ? 1 : 0);
            command.SetParameterValue("@IsVouchers", entity.IsVouchers ? 1 : 0);

            command.ExecuteNonQuery();

            return (int)command.GetParameterValue("@SysNo");
        }
        /// <summary>
        /// 创建团购阶梯价格
        /// </summary>
        /// <param name="ProductGroupBuyingSysNo"></param>
        /// <param name="SellCount"></param>
        /// <param name="GroupBuyingPrice"></param>
        /// <returns></returns>
        public int CreateProductGroupBuyingPrice(int ProductGroupBuyingSysNo, int? SellCount, decimal? GroupBuyingPrice, int? GroupBuyingPoint,decimal? costAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateProductGroupBuyingPrice");
            command.SetParameterValue("@ProductGroupBuyingSysNo", ProductGroupBuyingSysNo);
            command.SetParameterValue("@SellCount", SellCount.Value);
            command.SetParameterValue("@GroupBuyingPrice", GroupBuyingPrice);
            command.SetParameterValue("@GroupBuyingPoint", GroupBuyingPoint);
            command.SetParameterValue("@CostAmt", costAmt);

            return command.ExecuteNonQuery();
        }

        public void CreateGroupBuyingActivityRel(int groupBuyingSysNo, int vendorStoreSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateGroupBuyingActivityRel");
            command.SetParameterValue("@GroupBuyingSysNo", groupBuyingSysNo);
            command.SetParameterValue("@VendorStoreSysNo", vendorStoreSysNo);
            command.SetParameterValueAsCurrentUserAcct("@CreateUser");

            command.ExecuteNonQuery();
        }

        public void DeleteGroupBuyingActivityRel(int groupBuyingSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteGroupBuyingActivityRel");
            command.SetParameterValue("@GroupBuyingSysNo", groupBuyingSysNo);                        

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新团购
        /// </summary>
        /// <param name="entity"></param>
        public void Update(GroupBuyingInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductGroupBuying");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@GroupBuyingTitle", entity.GroupBuyingTitle.Content);
            command.SetParameterValue("@GroupBuyingDesc", entity.GroupBuyingDesc.Content);
            command.SetParameterValue("@GroupBuyingRules", entity.GroupBuyingRules.Content);
            command.SetParameterValue("@GroupBuyingDescLong", entity.GroupBuyingDescLong.Content);
            command.SetParameterValue("@GroupBuyingPicUrl", entity.GroupBuyingPicUrl.Content);
            command.SetParameterValue("@GroupBuyingMiddlePicUrl", entity.GroupBuyingMiddlePicUrl.Content);
            command.SetParameterValue("@GroupBuyingSmallPicUrl", entity.GroupBuyingSmallPicUrl.Content);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@IsByGroup", entity.IsByGroup.Value ? "Y" : "N");
            command.SetParameterValue("@MaxPerOrder", entity.MaxCountPerOrder ?? 0);
            command.SetParameterValue("@GroupBuyingTypeSysNo", entity.GroupBuyingTypeSysNo);
            command.SetParameterValue("@GroupBuyingAreaSysNo", entity.GroupBuyingAreaSysNo);
            command.SetParameterValue("@LimitOrderCount", entity.LimitOrderCount);
            command.SetParameterValue("@Reasons", "");
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@EditUser", entity.EditUser);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@VendorSysNo", entity.GroupBuyingVendorSysNo);
            command.SetParameterValue("@CouponValidDate", entity.CouponValidDate);
            command.SetParameterValue("@LotteryRule", entity.LotteryRule);
            command.SetParameterValue("@GroupBuyingCategorySysNo", entity.GroupBuyingCategorySysNo);
            command.SetParameterValue("@IsWithoutReservation", entity.IsWithoutReservation ? 1 : 0);
            command.SetParameterValue("@IsVouchers", entity.IsVouchers ? 1 : 0);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载团购信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public GroupBuyingInfo Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingEntity");
            cmd.SetParameterValue("@SysNo", sysNo);

            var ds = cmd.ExecuteDataSet();
            GroupBuyingInfo result = null;
            if (ds.Tables[0].Rows.Count > 0)
            {
                result = new GroupBuyingInfo();
                //DataMapper.AutoMap<GroupBuyingInfo>(result, ds.Tables[0].Rows[0]);

                DataRow row = ds.Tables[0].Rows[0];
                result.CompanyCode = row["CompanyCode"].ToString();
                result.SysNo = Convert.ToInt32(row["SysNo"]);
                result.ProductSysNo = Convert.ToInt32(row["ProductSysNo"]);
                result.ProductID = row["ProductID"].ToString();
                result.GroupBuyingTypeSysNo = Convert.ToInt32(row["GroupBuyingTypeSysNo"]);
                result.GroupBuyingAreaSysNo = Convert.ToInt32(row["GroupBuyingAreaSysNo"]);
                result.GroupBuyingVendorSysNo = row["VendorSysNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["VendorSysNo"]);
                result.RequestSysNo = row["RequestSysNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["RequestSysNo"]);
                result.GroupBuyingVendorName = row["VendorName"] == null ? "" : row["VendorName"].ToString();
                result.GroupBuyingTitle = new LanguageContent(row["GroupBuyingTitle"].ToString());
                result.GroupBuyingRules = new LanguageContent(row["GroupBuyingRules"].ToString());
                result.GroupBuyingDesc = new LanguageContent(row["GroupBuyingDesc"].ToString());
                result.GroupBuyingDescLong = new LanguageContent(row["GroupBuyingDescLong"].ToString());
                result.GroupBuyingPicUrl = new LanguageContent(row["GroupBuyingPicUrl"].ToString());
                result.Reasons = row["Reasons"] == null ? string.Empty : row["Reasons"].ToString();
                result.InUser = row["InUser"] == null ? string.Empty : row["InUser"].ToString();
                result.GroupBuyingMiddlePicUrl = new LanguageContent(row["GroupBuyingMiddlePicUrl"].ToString());
                result.GroupBuyingSmallPicUrl = new LanguageContent(row["GroupBuyingSmallPicUrl"].ToString());
                result.BeginDate = Convert.ToDateTime(row["BeginDate"]);
                result.EndDate = Convert.ToDateTime(row["EndDate"]);
                result.IsByGroup = row["IsByGroup"].Equals(1);
                result.LimitOrderCount = Convert.ToInt32(row["LimitOrderCount"]);
                result.MaxCountPerOrder = Convert.ToInt32(row["MaxPerOrder"]);
                result.OriginalPrice = Convert.ToDecimal(row["OriginalPrice"]);
                result.GBPrice = Convert.ToDecimal(row["GBPrice"]);
                result.CurrentSellCount = Convert.ToInt32(row["CurrentSellCount"]);
                result.CouponValidDate = row["CouponValidDate"] == DBNull.Value ? default(DateTime?) : Convert.ToDateTime(row["CouponValidDate"]);
                result.LotteryRule = row["LotteryRule"] == null ? string.Empty : row["LotteryRule"].ToString();
                result.GroupBuyingCategorySysNo = (row["GroupBuyingCategorySysNo"] == null || row["GroupBuyingCategorySysNo"] == DBNull.Value)? default(int?) : Convert.ToInt32(row["GroupBuyingCategorySysNo"]);
                result.IsWithoutReservation = Convert.ToInt32(row["IsWithoutReservation"]) == 1;
                result.IsVouchers = Convert.ToInt32(row["IsVouchers"]) == 1;
                object categoryType;
                if (EnumCodeMapper.TryGetEnum(row["GroupBuyingCategoryType"], typeof(GroupBuyingCategoryType), out categoryType))
                {
                    result.CategoryType = (GroupBuyingCategoryType)categoryType;
                }

                object settlementStatus;
                if (EnumCodeMapper.TryGetEnum(row["SettlementStatus"], typeof(GroupBuyingStatus), out settlementStatus))
                {
                    result.SettlementStatus = (GroupBuyingSettlementStatus)settlementStatus;
                }
                object status;
                if (EnumCodeMapper.TryGetEnum(row["Status"], typeof(GroupBuyingStatus), out status))
                {
                    result.Status = (GroupBuyingStatus)status;
                }
                if (row["Priority"] != DBNull.Value)
                {
                    result.Priority = Convert.ToInt32(row["Priority"]);
                }
                if (row["SuccessDate"] != DBNull.Value)
                {
                    result.SuccessDate = Convert.ToDateTime(row["SuccessDate"]);
                }
                //result.InUser = row["InUser"].ToString(); 

                //result.PriceRankList = DataMapper.GetEntityList<PSPriceDiscountRule, List<PSPriceDiscountRule>>(ds.Tables[1].Rows);
                result.PriceRankList = new List<PSPriceDiscountRule>();
                foreach (DataRow item in ds.Tables[1].Rows)
                {
                    if (result.GroupBuyingTypeSysNo == 6) { result.GroupBuyingPoint = Convert.ToInt32(item["GroupBuyingPoint"]); }
                    result.PriceRankList.Add(new PSPriceDiscountRule()
                    {
                        MinQty = Convert.ToInt32(item["SellCount"]),
                        DiscountValue = Convert.ToDecimal(item["GroupBuyingPrice"]),
                        ProductSysNo = Convert.ToInt32(item["SysNo"]),
                    });
                    decimal? costAmt = item["CostAmt"] != DBNull.Value ? Convert.ToDecimal(item["CostAmt"]) : default(decimal?);
                    if (costAmt.HasValue)
                    {
                        result.CostAmt = costAmt;
                    }
                }
                //团购类型
                result.GroupBuyingTypeList = this.GetGroupBuyingTypes();
                //地区
                result.GroupBuyingAreaList = this.GetGroupBuyingAreas();
            }
            return result;
        }

        public List<int> GetGroupBuyingVendorStores(int groupBuyingSysNo)
        {
             List<int> list = new List<int>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetGroupBuyingVendorStores");
            cmd.SetParameterValue("@GroupBuyingSysNo", groupBuyingSysNo);
            var dt = cmd.ExecuteDataTable();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                   int vendorStoreSysNo = Convert.ToInt32(item["VendorStoreSysNo"]);
                    list.Add(vendorStoreSysNo);
                }
            }
            return list;
        }

        public List<ProductPromotionDiscountInfo> GetProductGroupBuyingPriceByProductSysNo(int productSysNo, GroupBuyingStatus gbStatus)
        {
            List<ProductPromotionDiscountInfo> listTmp = new List<ProductPromotionDiscountInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingPriceByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Status", gbStatus);
            var ds = cmd.ExecuteDataSet();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    listTmp.Add(new ProductPromotionDiscountInfo()
                    {
                        Discount = Convert.ToDecimal(item["GroupBuyingPrice"]),
                        PromotionType = PromotionType.GroupBuying,
                        ReferenceSysNo = Convert.ToInt32(item["ProductGroupBuyingSysNo"])
                    });
                }
            }
            return listTmp;
        }

        public Dictionary<int, string> GetGroupBuyingTypes()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingTypes");
            Dictionary<int, string> result = new Dictionary<int, string>();
            DataSet dsResut = cmd.ExecuteDataSet();
            if (dsResut.Tables.Count > 0 && dsResut.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsResut.Tables[0].Rows)
                {
                    result.Add((int)dr[0], dr[1].ToString());
                }
            }
            return result;
        }

        public Dictionary<int, string> GetGroupBuyingAreas()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingAreas");
            Dictionary<int, string> result = new Dictionary<int, string>();
            DataSet dsResut = cmd.ExecuteDataSet();
            if (dsResut.Tables.Count > 0 && dsResut.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsResut.Tables[0].Rows)
                {
                    result.Add((int)dr[0], dr[1].ToString());
                }
            }
            return result;
        }

        public Dictionary<int, string> GetGroupBuyingVendors()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetGroupBuyingVendorList");
            Dictionary<int, string> result = new Dictionary<int, string>();
            DataSet dsResut = cmd.ExecuteDataSet();
            if (dsResut.Tables.Count > 0 && dsResut.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsResut.Tables[0].Rows)
                {
                    result.Add((int)dr[0], dr[1].ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 更新团购状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="status"></param>
        /// <param name="userName"></param>
        public void UpdataSatus(int sysNo, string status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateGroupBuyStatus");
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);

            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新团购状态,用于审核流程
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateProductGroupBuyingStatus(GroupBuyingInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductGroupBuyingStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@AuditUser", entity.AuditUser);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新团购结束时间为当前时间，用于中止团购
        /// </summary>
        /// <param name="sysNo"></param>
        public void UpdateGroupBuyingEndDate(int sysNo, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateGroupBuyingEndDate");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 同步Seller Portal团购状态
        /// </summary>
        /// <param name="entity"></param>
        public void SyncGroupBuyingStatus(GroupBuyingInfo entity)
        {
            if (entity.RequestSysNo > 0)
            {
                DataCommand command = DataCommandManager.GetDataCommand("SyncGroupBuyingStatus");

                command.SetParameterValue("@RequestSysNo", entity.RequestSysNo);
                command.SetParameterValue("@Status", entity.Status);
                command.SetParameterValue("@Reasons", entity.Reasons);
                command.SetParameterValue("@ReplyType", "ChangeGroupBuyingStatus");

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 检查团购冲突
        /// </summary>
        /// <param name="excludeSysNo"></param>
        /// <param name="productSysNos"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool CheckConflict(int? excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            if (productSysNos == null || productSysNos.Count == 0)
                throw new ArgumentException("productSysNos");
            string inProductSysNos = productSysNos[0].ToString();
            for (int i = 1; i < productSysNos.Count; i++)
            {
                inProductSysNos += "," + productSysNos[i].ToString();
            }
            DataCommand command = DataCommandManager.GetDataCommand("CheckProductInGBByDateTime");
            command.ReplaceParameterValue("#ProductSysNos#", inProductSysNos);
            command.SetParameterValue("@ExcludeSysNo", excludeSysNo.HasValue ? excludeSysNo.Value : -999);
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);

            return command.ExecuteScalar<int>() > 0;
        }

        #region Job 相关

        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        public List<GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyNeedProcess");
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<GroupBuyingInfo>();
        }

        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        public void UpdateGroupBuySettlementStatus(int sysNo, GroupBuyingSettlementStatus settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangeGroupBuySettlement");
            command.SetParameterValue("@SysNo", sysNo);
            object status = "N";
            EnumCodeMapper.TryGetCode(settlementStatus, out status);
            command.SetParameterValue("@SettlementStatus", status);
            command.ExecuteNonQuery();
        }


        public List<GroupBuyingInfo> GetGroupBuyingList(int groupBuyingSysNo, int companyCode)
        {
            List<GroupBuyingInfo> result;
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyingList");
            command.SetParameterValue("@GroupBuyingSysNo", groupBuyingSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            result = command.ExecuteEntityList<GroupBuyingInfo>();

            return result;
        }

        #endregion

        public List<int> GetProductsOnGroupBuying(IEnumerable<int> products)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ProductsOnGroupBuying");
            command.ReplaceParameterValue("#ProductsStr#", string.Join(",", products));

            List<int> result = new List<int>();
            IDataReader dr = command.ExecuteDataReader();
            while (dr.Read())
            {
                result.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
            return result;
        }

        //public decimal GetProductOriginalPrice(int productSysNo, string isByGroup, string companyCode)
        //{
        //    DataCommand command = DataCommandManager.GetDataCommand("GetProductOriginalPrice");
        //    command.SetParameterValue("@ProductSysNo", productSysNo);
        //    command.SetParameterValue("@IsByGroup", isByGroup);
        //    command.SetParameterValue("@CompanyCode", companyCode);

        //    return Convert.ToDecimal(command.ExecuteScalar());
        //}

        /// <summary>
        /// 读取商品原价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="isByGroup">是否根据组读取</param>
        /// <returns></returns>
        public List<object> GetProductOriginalPrice(int productSysNo, string isByGroup, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductOriginalPrice");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@IsByGroup", isByGroup);
            command.SetParameterValue("@CompanyCode", companyCode);
            DataSet dsResut = command.ExecuteDataSet();
            List<object> result = new List<object>();
            if (dsResut.Tables.Count > 0 && dsResut.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dsResut.Tables[0].Rows[0];
                for (int i = 0; i < dsResut.Tables[0].Columns.Count; i++)
                {
                    object fieldVal = dr[i] == DBNull.Value ? "" : dr[i];
                    result.Add(fieldVal);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除团购阶梯价格
        /// </summary>
        /// <param name="ProductGroupBuyingSysNo"></param>
        public void DeleteProductGroupBuyingPrice(int ProductGroupBuyingSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteProductGroupBuyingPrice");
            command.SetParameterValue("@ProductGroupBuyingSysNo", ProductGroupBuyingSysNo);
            command.ExecuteNonQuery();
        }

        public GroupBuyingCategoryInfo CreateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateGroupBuyingCategory");
            command.SetParameterValue(entity);

            command.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            return GetGroupBuyingCategoryBySysNo(entity.SysNo.Value);
        }

        public GroupBuyingCategoryInfo UpdateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingCategory");
            command.SetParameterValue(entity);

            command.ExecuteNonQuery();

            return GetGroupBuyingCategoryBySysNo(entity.SysNo.Value);
        }

        public GroupBuyingCategoryInfo GetGroupBuyingCategoryBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyingCategoryBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<GroupBuyingCategoryInfo>();  
        }

        public List<GroupBuyingCategoryInfo> GetAllGroupBuyingCategory()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllGroupBuyingCategory");
            return command.ExecuteEntityList<GroupBuyingCategoryInfo>();            
        }

        public bool CheckGroupBuyingCategoryNameExists(string categoryName, int excludeSysNo, GroupBuyingCategoryType categoryType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckGroupBuyingCategoryNameExists");
            command.SetParameterValue("@CategoryName", categoryName);
            command.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            command.SetParameterValue("@CategoryType", categoryType);
            return command.ExecuteScalar<int>() > 0;  
        }

        public void ReadGroupbuyingFeedback(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ReadGroupbuyingFeedback");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.ExecuteNonQuery();
        }

        public void HandleGroupbuyingBusinessCooperation(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HandleGroupbuyingBusinessCooperation");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.ExecuteNonQuery();
        }

        public GroupBuyingSettlementInfo LoadGroupBuyingSettleBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadGroupBuyingSettlementBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<GroupBuyingSettlementInfo>();
        }

        public void UpdateGroupBuyingSettlementStatus(int sysNo, SettlementBillStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingSettlementStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }


        public GroupBuyingTicketInfo LoadGroupBuyingTicketBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadGroupBuyingTicketBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<GroupBuyingTicketInfo>();
        }


        public void UpdateGroupBuyingTicketStatus(int sysNo, GroupBuyingTicketStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingTicketStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValueAsCurrentUserSysNo("@EditUser");

            command.ExecuteNonQuery();
        }


        public void UpdateGroupBuyingTicketRefundInfo(GroupBuyingTicketInfo item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingTicketRefundInfo");
            command.SetParameterValue("@SysNo", item.SysNo);
            command.SetParameterValue("@Status", item.Status);
            command.SetParameterValue("@RefundDate", item.RefundDate);
            command.SetParameterValue("@RefundStatus", item.RefundStatus);
            command.SetParameterValue("@RefundMemo", item.RefundMemo);
            command.SetParameterValueAsCurrentUserSysNo("@EditUser");

            command.ExecuteNonQuery();
        }
    }
}
