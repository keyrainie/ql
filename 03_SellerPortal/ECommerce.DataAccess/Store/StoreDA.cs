using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Store;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Common;
using System.Data;
using ECommerce.Entity.Store.Vendor;
using System.IO;
using System.Xml.Serialization;

namespace ECommerce.DataAccess.Store
{
    public class StoreDA
    {

        /// <summary>
        /// 创建品牌报备
        /// </summary>
        public static int InsertStoreBrandFiling(StoreBrandFiling entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStoreBrandFiling");
            cmd.SetParameterValue<StoreBrandFiling>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新品牌报备
        /// </summary>
        public static void UpdateStoreBrandFiling(StoreBrandFiling entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStoreBrandFiling");
            cmd.SetParameterValue<StoreBrandFiling>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载品牌报备
        /// </summary>
        public static StoreBrandFiling LoadStoreBrandFiling(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadStoreBrandFiling");
            cmd.SetParameterValue("@SysNo", SysNo);
            StoreBrandFiling result = cmd.ExecuteEntity<StoreBrandFiling>();
            return result;
        }


        public static List<StorePageTheme> QueryAllStorePageTheme()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryAllStorePageTheme");
            var result = cmd.ExecuteEntityList<StorePageTheme>();
            return result;
        }

        /// <summary>
        /// 创建页面主题(字典表)
        /// </summary>
        public static int InsertStorePageTheme(StorePageTheme entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStorePageTheme");
            cmd.SetParameterValue<StorePageTheme>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 删除页面主题(字典表)
        /// </summary>
        public static void DeleteStorePageTheme(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteStorePageTheme");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新页面主题(字典表)
        /// </summary>
        public static void UpdateStorePageTheme(StorePageTheme entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStorePageTheme");
            cmd.SetParameterValue<StorePageTheme>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载页面主题(字典表)
        /// </summary>
        public static StorePageTheme LoadStorePageTheme(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadStorePageTheme");
            cmd.SetParameterValue("@SysNo", SysNo);
            StorePageTheme result = cmd.ExecuteEntity<StorePageTheme>();
            return result;
        }

        /// <summary>
        /// 创建佣金规则
        /// </summary>
        public static int InsertCommissionRule(CommissionRule entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCommissionRule");
            cmd.SetParameterValue<CommissionRule>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新佣金规则
        /// </summary>
        public static void UpdateCommissionRule(CommissionRule entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCommissionRule");
            cmd.SetParameterValue<CommissionRule>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载佣金规则
        /// </summary>
        public static CommissionRule LoadCommissionRule(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCommissionRule");
            cmd.SetParameterValue("@SysNo", SysNo);
            CommissionRule result = cmd.ExecuteEntity<CommissionRule>();
            return result;
        }

        //public List<VendorAgentInfo> LoadVendorAgentInfoList(VendorInfo vendorInfo)
        //{
        //    DataCommand dataCommand = DataCommandManager.GetDataCommand("GetManufacturerByVendorSysNo");
        //    dataCommand.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
        //    List<VendorAgentInfo> list = dataCommand.ExecuteEntityList<VendorAgentInfo>();
        //    //反序列化SaleRules的XML
        //    list.ForEach(delegate(VendorAgentInfo agentInfo)
        //    {
        //        agentInfo.IsNeedConfirmOrder = agentInfo.IsNeedConfirmOrder ?? false;
        //        agentInfo.VendorCommissionInfo.SaleRuleEntity = new VendorStagedSaleRuleEntity();
        //        if (!string.IsNullOrEmpty(agentInfo.VendorCommissionInfo.StagedSaleRuleItemsXml))
        //        {
        //            StringReader xmlReader = new StringReader(agentInfo.VendorCommissionInfo.StagedSaleRuleItemsXml);
        //            XmlSerializer xmlSerializer = new XmlSerializer(typeof(VendorStagedSaleRuleEntity));
        //            VendorStagedSaleRuleEntity saleRuleEntity = xmlSerializer.Deserialize(xmlReader) as VendorStagedSaleRuleEntity;
        //            if (null != saleRuleEntity)
        //            {
        //                agentInfo.VendorCommissionInfo.SaleRuleEntity = saleRuleEntity;
        //            }
        //        }
        //    });
        //    return list;
        //}
        //public VendorInfo LoadVendorInfo(int vendorSysNo)
        //{
        //    VendorInfo returnVendorInfo = new VendorInfo();
        //    CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorbySysNoRequestRank");
        //    using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "SysNo desc"))
        //    {
        //        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.SysNo",
        //        DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, vendorSysNo);
        //        dataCommand.CommandText = sqlBuilder.BuildQuerySql();
        //        returnVendorInfo = dataCommand.ExecuteEntity<VendorInfo>();

        //        return returnVendorInfo;
        //    }
        //}
        public static VendorAgentInfo CreateVendorCommissionInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo, string createUserName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorCommissionRule");

            var sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                if (vendorAgentInfo.VendorCommissionInfo != null)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(VendorStagedSaleRuleEntity));
                    xs.Serialize(sw, vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity);
                    command.SetParameterValue("@OrderCommissionFee", vendorAgentInfo.VendorCommissionInfo.OrderCommissionAmt);
                    command.SetParameterValue("@DeliveryFee", vendorAgentInfo.VendorCommissionInfo.DeliveryFee);
                    command.SetParameterValue("@RentFee", vendorAgentInfo.VendorCommissionInfo.RentFee);
                }
                else
                {
                    command.SetParameterValue("@OrderCommissionFee", DBNull.Value);
                    command.SetParameterValue("@DeliveryFee", DBNull.Value);
                    command.SetParameterValue("@RentFee", DBNull.Value);
                }
            }
            command.SetParameterValue("@VendorManufacturerSysNo", vendorAgentInfo.AgentSysNo);
            command.SetParameterValue("@InUser", createUserName);
            command.SetParameterValue("@EditUser", createUserName);
            command.SetParameterValue("@SalesRule", sb.ToString());
            command.SetParameterValue("@CompanyCode", vendorAgentInfo.CompanyCode);
            command.ExecuteNonQuery();
            return vendorAgentInfo;
        }
        public static VendorAgentInfo CreateVendorManufacturerInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorManufacturer");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.SetParameterValue("@C2SysNo", vendorAgentInfo.C2SysNo);
            command.SetParameterValue("@AgentLevel", vendorAgentInfo.AgentLevel);
            command.SetParameterValue("@Status", vendorAgentInfo.Status);
            command.SetParameterValue("@C3SysNo", vendorAgentInfo.C3SysNo.Value);
            if (vendorAgentInfo.BrandInfo != null && vendorAgentInfo.BrandInfo.SysNo.HasValue)
            {
                command.SetParameterValue("@BrandSysNo", vendorAgentInfo.BrandInfo.SysNo.Value);
            }
            else
            {
                command.SetParameterValue("@BrandSysNo", DBNull.Value);
            }
            command.SetParameterValue("@CompanyCode", vendorAgentInfo.CompanyCode);

            return command.ExecuteEntity<VendorAgentInfo>();
        }

        /// <summary>
        /// 加载页面类型，不允许随意修改，页面类型配合页面ID可以定位到所有的数据页面，所有店铺模板的页面类型都是一样的
        /// </summary>
        public static List<StorePageType> QueryAllPageType()
        {
            var cmd = DataCommandManager.GetDataCommand("QueryAllPageType");
            var result = cmd.ExecuteEntityList<StorePageType>();
            return result;
        }

        /// <summary>
        /// 加载页面模板，不允许随意修改，可供业务类型选用； 本表中保存各种页面类型中预设的几种模板，和几个通用型的自定义模板。
        /// </summary>
        public static List<StorePageTemplate> QueryStorePageTemplate(string pageTypeKey)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStorePageTemplate");
            cmd.SetParameterValue("@PageTypeKey", pageTypeKey);
            var result = cmd.ExecuteEntityList<StorePageTemplate>();
            return result;
        }

        /// <summary>
        /// 加载页面模板
        /// </summary>
        public static StorePageTemplate QueryStorePageTemplateByTemplateKey(string TemplateKey)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStorePageTemplateByTemplateKey");
            cmd.SetParameterValue("@TemplateKey", TemplateKey);
            var result = cmd.ExecuteEntity<StorePageTemplate>();
            return result;
        }

        /// <summary>
        /// 查询店铺所有页面
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<StorePage> QueryStorePageList(StorePageListQueryFilter queryFilter, int SellSysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStorePageList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "s.SysNo ASC" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.[SellerSysNo]", DbType.Int32, "@SellerSysNo", QueryConditionOperatorType.Equal, SellSysno);

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<StorePage> resultList = command.ExecuteEntityList<StorePage>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<StorePage>()
                {
                    PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields },
                    ResultList = resultList
                };
            }
        }

        /// <summary>
        /// 查询店铺导航
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<StoreNavigation> QueryStoreNavigationList(StorePageListQueryFilter queryFilter, int SellSysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStoreNavigationList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "s.SysNo ASC" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.[SellerSysNo]", DbType.Int32, "@SellerSysNo", QueryConditionOperatorType.Equal, SellSysno);

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<StoreNavigation> resultList = command.ExecuteEntityList<StoreNavigation>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<StoreNavigation>()
                {
                    PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields },
                    ResultList = resultList
                };
            }
        }

        public static int SaveNavigationForm(StoreNavigation navigation)
        {
            var cmd = DataCommandManager.GetDataCommand("SaveNavigationForm");
            if (cmd != null)
            {
                cmd.SetParameterValue(navigation);
                cmd.ExecuteNonQuery();
                return (int)cmd.GetParameterValue("@SysNo");
            }
            throw new Exception("command [SaveStorePage] is not defined");
        }

        public static int DeleteNavigation(int sysNo, int sellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DeleteNavigation");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@SellerSysNo", sellerSysNo);
            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 禁用店铺页面（专题页）
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static int DisableStorePage(int sysno, int SellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DisableStorePage");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 启用店铺页面（专题页）
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static int EnableStorePage(int sysno, int SellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("EnableStorePage");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 删除店铺页面（专题页）
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static int DeleteStorePage(int sysno, int SellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DeleteStorePage");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            return command.ExecuteScalar<int>();
        }

        public static int DeleteStorePageFromPublish(int sysno, int SellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DeleteStorePageFromPublish");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            return command.ExecuteScalar<int>();
        }

        public static StorePage QueryStorePageByPageName(string pageName, int sellerSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStorePageByPageName");
            if (cmd != null)
            {
                cmd.SetParameterValue("@PageName", pageName);
                cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
                return cmd.ExecuteEntity<StorePage>();
            }
            throw new Exception("command [QueryStorePageByPageName] is not defined");
        }

        public static List<StorePageLayout> QueryAllPageLayout()
        {
            var cmd = DataCommandManager.GetDataCommand("QueryAllPageLayout");
            if (cmd != null)
            {
                return cmd.ExecuteEntityList<StorePageLayout>();
            }
            throw new Exception("command [QueryAllPageLayout] is not defined");
        }

        public static int SaveStorePage(StorePage page)
        {
            var cmd = DataCommandManager.GetDataCommand("SaveStorePage");
            if (cmd != null)
            {
                cmd.SetParameterValue(page);
                cmd.ExecuteNonQuery();
                return (int)cmd.GetParameterValue("@SysNo");
            }
            throw new Exception("command [SaveStorePage] is not defined");
        }

        public static int PublishStorePage(StorePage page)
        {
            var cmd = DataCommandManager.GetDataCommand("PublishStorePage");
            if (cmd != null)
            {
                cmd.SetParameterValue(page);
                cmd.SetParameterValue("@StorePageSysNo", page.SysNo.Value);
                return cmd.ExecuteNonQuery();
            }
            throw new Exception("command [PublishStorePage] is not defined");
        }

        /// <summary>
        /// 根据PageTypeKey获取页面元素
        /// </summary>
        /// <param name="pageTypeKey"></param>
        /// <returns></returns>
        public static List<StorePageElement> GetPageElementByPageTypeKey(string pageTypeKey)
        {
            var cmd = DataCommandManager.GetDataCommand("GetPageElementByPageTypeKey");
            if (cmd != null)
            {
                cmd.SetParameterValue("@PageTypeKey", pageTypeKey);
                return cmd.ExecuteEntityList<StorePageElement>();
            }
            throw new Exception("command [PublishStorePage] is not defined");
        }

        /// <summary>
        /// 创建企业(商家)基本信息
        /// </summary>
        public static int InsertStoreBasicInfo(StoreBasicInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStoreBasicInfo");
            cmd.SetParameterValue<StoreBasicInfo>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新企业(商家)基本信息
        /// </summary>
        public static void UpdateStoreBasicInfo(StoreBasicInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStoreBasicInfo");
            cmd.SetParameterValue<StoreBasicInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载企业(商家)基本信息
        /// </summary>
        public static StoreBasicInfo LoadStoreBasicInfo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadStoreBasicInfo");
            cmd.SetParameterValue("@SysNo", SysNo);
            StoreBasicInfo result = cmd.ExecuteEntity<StoreBasicInfo>();
            return result;
        }


        /// <summary>
        /// 创建企业(商家)附件
        /// </summary>
        public static int InsertStoreAttachment(StoreAttachment entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStoreAttachment");
            cmd.SetParameterValue<StoreAttachment>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新企业(商家)附件
        /// </summary>
        public static void UpdateStoreAttachment(StoreAttachment entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStoreAttachment");
            cmd.SetParameterValue<StoreAttachment>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载企业(商家)附件
        /// </summary>
        public static StoreAttachment LoadStoreAttachment(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadStoreAttachment");
            cmd.SetParameterValue("@SysNo", SysNo);
            StoreAttachment result = cmd.ExecuteEntity<StoreAttachment>();
            return result;
        }

        public static List<StoreAttachment> QueryStoreAttachment(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryStoreAttachment");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            var result = cmd.ExecuteEntityList<StoreAttachment>();
            return result;
        }

        public static StoreBasicInfo QueryStoreBasicInfoBySellerSysNo(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryStoreBasicInfoBySellerSysNo");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);

            var ds = cmd.ExecuteDataSet();

            List<string> BrandNames = new List<string>();
            if (ds.Tables.Count > 0)
            {
                var dt0 = ds.Tables[0];
                foreach (DataRow row in dt0.Rows)
                {
                    BrandNames.Add(row[0].ToString());
                }
            }

            List<string> CategoryNames = new List<string>();
            if (ds.Tables.Count > 1)
            {
                var dt1 = ds.Tables[1];
                foreach (DataRow row in dt1.Rows)
                {
                    CategoryNames.Add(row[0].ToString());
                }
            }

            StoreBasicInfo result = new StoreBasicInfo();
            if (ds.Tables.Count > 2)
            {
                var dt2 = ds.Tables[2];
                if (dt2.Rows.Count > 0)
                {
                    DataMapper.AutoMap<StoreBasicInfo>(result, dt2.Rows[0]);
                }
            }
            result.MainBrand = String.Join("/", BrandNames.ToArray());
            result.MainProductCategory = String.Join("/", CategoryNames.ToArray());
            return result;
        }

        public static int UpdateStorePage(StorePage page)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStorePage");
            cmd.SetParameterValue(page);
            cmd.ExecuteNonQuery();
            return page.SysNo.Value;
        }

        /// <summary>
        /// 创建店铺网页的Header信息。
        /// </summary>
        public static int InsertStorePageHeader(StorePageHeader entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStorePageHeader");
            cmd.SetParameterValue<StorePageHeader>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新店铺网页的Header信息。
        /// </summary>
        public static void UpdateStorePageHeader(StorePageHeader entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStorePageHeader");
            cmd.SetParameterValue<StorePageHeader>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载店铺网页的Header信息。
        /// </summary>
        public static StorePageHeader LoadStorePageHeader(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadStorePageHeader");
            cmd.SetParameterValue("@SysNo", SysNo);
            StorePageHeader result = cmd.ExecuteEntity<StorePageHeader>();
            return result;
        }


        public static StorePageHeader QueryPageHeaderBySellerSysNo(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryPageHeaderBySellerSysNo");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            StorePageHeader result = cmd.ExecuteEntity<StorePageHeader>();
            return result;
        }

        public static int DeleteAttachmentBySellerSysNo(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAttachmentBySellerSysNo");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static void SaveStoreBasicInfoToIPP3Vendor(Entity.Store.Vendor.VendorBasicInfo vendorInfo, int inUserSysNO)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SaveStoreBasicInfoToIPP3Vendor");
            //command.SetParameterValue("@VendorID", entity.VendorID);
            command.SetParameterValue("@VendorName", vendorInfo.VendorNameLocal);
            command.SetParameterValue("@EnglishName", vendorInfo.VendorNameGlobal);
            command.SetParameterValue("@BriefName", vendorInfo.VendorBriefName);
            command.SetParameterValue("@VendorType", (int)vendorInfo.VendorType);
            command.SetParameterValue("@District", vendorInfo.District);
            command.SetParameterValue("@Address", vendorInfo.Address);
            command.SetParameterValue("@Zip", vendorInfo.ZipCode);
            command.SetParameterValue("@Phone", vendorInfo.Phone);
            command.SetParameterValue("@Fax", vendorInfo.Fax);
            command.SetParameterValue("@Email", vendorInfo.EmailAddress);
            command.SetParameterValue("@Site", vendorInfo.Website);
            command.SetParameterValue("@Bank", "");
            command.SetParameterValue("@Account", "");
            command.SetParameterValue("@TaxNo", "");
            command.SetParameterValue("@Contact", vendorInfo.Contact);  //供应商联系人
            command.SetParameterValue("@Comment", vendorInfo.Comment);
            command.SetParameterValue("@Note", vendorInfo.Note);
            command.SetParameterValue("@Status", (int)vendorInfo.VendorStatus.Value);
            command.SetParameterValue("@RMAPolicy", "");
            command.SetParameterValue("@PayPeriod", -1);
            command.SetParameterValue("@RepairAddress", "");
            command.SetParameterValue("@RepairAreaSysNo", -1);
            command.SetParameterValue("@RepairContact", ""); //售后联系人
            command.SetParameterValue("@RepairContactPhone", "");
            command.SetParameterValue("@Cellphone", vendorInfo.CellPhone);
            command.SetParameterValue("@AcctContactName", ""); //财务联系人
            command.SetParameterValue("@AcctPhone", "");
            command.SetParameterValue("@RMAServiceArea", "");
            command.SetParameterValue("@IsConsign", (int)vendorInfo.ConsignFlag);
            command.SetParameterValue("@PayPeriodType", -1);
            command.SetParameterValue("@ValidDate", DateTime.Now);
            command.SetParameterValue("@ExpiredDate", DateTime.Now.AddYears(100));
            command.SetParameterValue("@ContractAmt", -1);
            command.SetParameterValue("@TotalPOMoney", -1);
            command.SetParameterValue("@CreateVendorUserSysNo", inUserSysNO);
            command.SetParameterValue("@VendorContractInfo", vendorInfo.Contact);
            command.SetParameterValue("@IsCooperate", (int)vendorInfo.VendorIsCooperate.Value);
            command.SetParameterValue("@AcctContactEmail", "");
            command.SetParameterValue("@SellerID", vendorInfo.SellerID);
            command.SetParameterValue("@RepairPostcode", "");
            command.SetParameterValue("@CompanyCode", "");
            //command.SetParameterValue("@UpdateTime", DateTime.Now);
            command.ExecuteNonQuery();
            //VendorInfo vendorEntity = command.ExecuteEntity<VendorInfo>();
            //vendorEntity.VendorBasicInfo.ExtendedInfo.IsShowStore = vendorInfo.ExtendedInfo.IsShowStore;
            //CreateOrEditIsShowStore(vendorEntity);
            //return vendorEntity;
        }

        public static VendorAgentInfo UpdateVendorManufacturerInfo(VendorAgentInfo p, int sellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorManufacturerInfo");
            command.SetParameterValue("@VendorSysNo", sellerSysNo);
            //command.SetParameterValue("@ManufacturerSysNo", DBNull.Value);
            command.SetParameterValue("@C2SysNo", p.C2SysNo);
            command.SetParameterValue("@AgentLevel", p.AgentLevel);
            command.SetParameterValue("@Status", p.Status);
            command.SetParameterValue("@C3SysNo", p.C3SysNo.Value);
            command.SetParameterValue("@SysNo", p.AgentSysNo.Value);
            command.SetParameterValue("@CompanyCode", p.CompanyCode);

            return command.ExecuteEntity<VendorAgentInfo>();
        }

        public static CommissionRule QueryCommissionRule(int brandSysNo, int c3SysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryCommissionRule");
            command.SetParameterValue("@BrandSysNo", brandSysNo);
            command.SetParameterValue("@C3SysNo", c3SysNo);

            return command.ExecuteEntity<CommissionRule>();
        }

        public static List<VendorAgentInfo> QueryStoreAgentInfos(int sellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryStoreAgentInfos");
            command.SetParameterValue("@VendorSysNo", sellerSysNo);

            var result = command.ExecuteEntityList<VendorAgentInfo>();

            var xmlSer = new XmlSerializer(typeof(VendorStagedSaleRuleEntity));
            result.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.VendorCommissionInfo.StagedSaleRuleItemsXml))
                {
                    //p.VendorCommissionInfo.SaleRuleEntity
                    using (var stream = new MemoryStream(Encoding.GetEncoding("UTF-16").GetBytes(p.VendorCommissionInfo.StagedSaleRuleItemsXml)))
                    {
                        p.VendorCommissionInfo.SaleRuleEntity = (VendorStagedSaleRuleEntity)xmlSer.Deserialize(stream);
                        p.VendorCommissionInfo.StagedSaleRuleItemsXml = "";
                    }
                }
            });

            return result;
        }

        public static void DelStoreAgentProduct(int agentSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DelStoreAgentProduct");
            command.SetParameterValue("@AgentSysNo", agentSysNo);

            command.ExecuteNonQuery();
        }

        public static void DelStoreAttachment(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelStoreAttachment");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
        }

        public static VendorModifyRequestInfo CreateModifyRequest(VendorModifyRequestInfo requestInfo, int sellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateModifyRequest");
            command.SetParameterValue("@VendorSysNo", requestInfo.VendorSysNo);
            command.SetParameterValue("@CompanyCode", requestInfo.CompanyCode);
            //command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@CreateUserSysNo", sellerSysNo);
            command.SetParameterValue("@Status", requestInfo.Status);
            command.SetParameterValue("@RequestType", (int)requestInfo.RequestType);
            command.SetParameterValue("@RANK", requestInfo.Rank);
            command.SetParameterValue("@ActionType", (int?)requestInfo.ActionType);
            command.SetParameterValue("@ManufacturerSysNo", requestInfo.ManufacturerSysNo);
            command.SetParameterValue("@VendorManufacturerSysNo", requestInfo.VendorManufacturerSysNo);
            command.SetParameterValue("@VendorManufacturerStatus", VendorAgentStatus.Requesting);
            command.SetParameterValue("@LEVEL", requestInfo.AgentLevel);
            command.SetParameterValue("@C2SysNo", requestInfo.C2SysNo);
            command.SetParameterValue("@C3SysNo", requestInfo.C3SysNo);
            command.SetParameterValue("@Content", requestInfo.Content);
            command.SetParameterValue("@Memo", requestInfo.Memo);
            if (requestInfo.SettleType.HasValue)
            {
                command.SetParameterValue("@SettleType", requestInfo.SettleType.Value.ToString());
            }
            else
            {
                command.SetParameterValue("@SettleType", "");
            }

            command.SetParameterValue("@SettlePercentage", requestInfo.SettlePercentage.HasValue ? requestInfo.SettlePercentage.Value : 0);
            command.SetParameterValue("@BuyWeekDay", requestInfo.BuyWeekDay);
            command.SetParameterValue("@SendPeriod", requestInfo.SendPeriod);
            command.SetParameterValue("@BrandSysNo", requestInfo.BrandSysNo);

            //CRL20146 By Kilin
            command.SetParameterValue("@SettlePeriodType", (int?)requestInfo.SettlePeriodType);
            command.SetParameterValue("@AutoAudit", requestInfo.AutoAudit.HasValue && requestInfo.AutoAudit == true ? "Y" : null);
            command.SetParameterValue("@CompanyCode", requestInfo.CompanyCode);

            //[[ added by poseidon.y.tong at [2012-08-21 20:52:22]
            //command.SetParameterValue("@MaxNoPayTimeForOrder", requestInfo.MaxNoPayTimeForOrder);
            //command.SetParameterValue("@IsNeedConfirmOrder", requestInfo.IsNeedConfirmOrder);
            //]]
            if (command.ExecuteNonQuery() > 0)
            {
                return requestInfo;
            }
            else
            {
                return null;
            }
        }

        public static int RestoreDefaultStorePage(int SysNo, int SellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("RestoreDefaultStorePage");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@SellerSysNo", SellerSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static VendorBasicInfo LoadVendorInfo(int vendorSysNo)
        {
            VendorBasicInfo vendorInfo = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadVendorInfo");
            cmd.SetParameterValue("@VendorSysNo", vendorSysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables != null)
            {
                if (ds.Tables.Count > 0)
                {
                    vendorInfo = DataMapper.GetEntity<VendorBasicInfo>(ds.Tables[0].Rows[0]);
                }
                if (vendorInfo != null && ds.Tables.Count > 1)
                {
                    vendorInfo.ExtendedInfo = DataMapper.GetEntity<VendorExtendInfo>(ds.Tables[1].Rows[0]);
                }
            }
            return vendorInfo;
        }

        public static void SaveStorePageDraft(StorePage page)
        {
            //DataCommand cmd = DataCommandManager.GetDataCommand("SaveStorePageDraft");
        }

        public static QueryResult<VendorAgentInfo> QueryStoreAgentInfosByPage(StorePageListQueryFilter queryFilter, int SellSysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStoreAgentInfosByPage");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "AgentSysNo ASC" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.[VendorSysNo]", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, SellSysno);

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "a.Status in (0,2)");

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<VendorAgentInfo> resultList = command.ExecuteEntityList<VendorAgentInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                var xmlSer = new XmlSerializer(typeof(VendorStagedSaleRuleEntity));
                resultList.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.VendorCommissionInfo.StagedSaleRuleItemsXml))
                    {
                        //p.VendorCommissionInfo.SaleRuleEntity
                        using (var stream = new MemoryStream(Encoding.GetEncoding("UTF-16").GetBytes(p.VendorCommissionInfo.StagedSaleRuleItemsXml)))
                        {
                            p.VendorCommissionInfo.SaleRuleEntity = (VendorStagedSaleRuleEntity)xmlSer.Deserialize(stream);
                            p.VendorCommissionInfo.StagedSaleRuleItemsXml = "";
                        }
                    }
                });

                return new QueryResult<VendorAgentInfo>()
                {
                    PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields },
                    ResultList = resultList
                };
            }
        }




    }
}
