using ECommerce.DataAccess.Store;
using ECommerce.Entity.Common;
using ECommerce.Entity.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.Entity.Store.Vendor;
using System.IO;
using System.Xml.Serialization;
using ECommerce.Service.Common;
using ECommerce.DataAccess.Product;

namespace ECommerce.Service.Store
{
    public class StoreService
    {
        #region Store Page
        public static QueryResult<StorePage> QueryStorePageList(StorePageListQueryFilter filter, int SellSysno)
        {
            return StoreDA.QueryStorePageList(filter, SellSysno);
        }

        public static QueryResult<StoreNavigation> QueryStoreNavigationList(StorePageListQueryFilter filter, int SellSysno)
        {
            return StoreDA.QueryStoreNavigationList(filter, SellSysno);
        }

        public static int SaveNavigationForm(StoreNavigation navigation)
        {
            return StoreDA.SaveNavigationForm(navigation);
        }

        public static int DeleteNavigation(int sysNo, int sellerSysNo)
        {
            return StoreDA.DeleteNavigation(sysNo, sellerSysNo);
        }

        public static List<StorePageType> QueryAllPageType()
        {
            return StoreDA.QueryAllPageType();
        }

        public static List<StorePageTemplate> QueryStorePageTemplate(string pageTypeKey)
        {
            return StoreDA.QueryStorePageTemplate(pageTypeKey);
        }

        public static StorePageTemplate QueryStorePageTemplateByTemplateKey(string TemplateKey)
        {
            return StoreDA.QueryStorePageTemplateByTemplateKey(TemplateKey);
        }

        public static StorePageType QueryStorePageType(string pageTypeKey)
        {
            var pageTypes = QueryAllPageType();
            var pageType = from t in pageTypes
                           where t.Key.Trim().Equals(pageTypeKey, StringComparison.OrdinalIgnoreCase)
                           select t;
            return pageType.FirstOrDefault();
        }

        public static StorePage QueryStorePageByPageName(string pageName, int sellerSysNo)
        {
            var page = StoreDA.QueryStorePageByPageName(pageName, sellerSysNo);
            if (page != null)
            {
                var value = page.DataValue;
                if (!string.IsNullOrEmpty(value))
                {
                    var result = SerializationUtility.JsonDeserialize2<StorePage>(value);
                    result.SysNo = page.SysNo;
                    return result;
                }

            }
            return null;
        }

        public static List<StorePageLayout> QueryAllPageLayout()
        {
            return StoreDA.QueryAllPageLayout();
        }

        /// <summary>
        /// publish store page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static int PublishStorePage(StorePage page)
        {
            using (var trans = TransactionManager.Create())
            {

                //step 1 save drafts
                var sysNo = SaveStorePage(page);
                page.SysNo = sysNo;
                //StoreDA.SaveStorePageDraft(page);
                //step 2 publish
                StoreDA.PublishStorePage(page);

                trans.Complete();
            }
            return 1;
        }
        /// <summary>
        /// save drafts
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static int SaveStorePage(StorePage page)
        {
            var result = StoreDA.QueryStorePageByPageName(page.PageName, page.SellerSysNo.Value);
            if (result != null)
            {
                page.SysNo = result.SysNo;
                return StoreDA.UpdateStorePage(page);
            }
            else
            {
                return StoreDA.SaveStorePage(page);
            }
        }

        #endregion

        #region store basic info

        public static StoreBasicInfo QueryStoreBasicInfoBySellerSysNo(int sellerSysNo)
        {
            return StoreDA.QueryStoreBasicInfoBySellerSysNo(sellerSysNo);
        }

        /// <summary>
        /// 保存商家的基本信息
        /// 1.信息保存到ECStore.dbo.StoreBasicInfo
        /// 2.信息同步到IPP3.dbo.Vendor
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int SaveStoreBasicInfo(StoreBasicInfo info)
        {
            using (var trans = TransactionManager.Create())
            {
                if (info.SysNo.HasValue)
                {
                    StoreDA.UpdateStoreBasicInfo(info);
                }
                else
                {
                    StoreDA.InsertStoreBasicInfo(info);
                }

                var basicInfo = StoreDA.QueryStoreBasicInfoBySellerSysNo(info.SellerSysNo.Value);

                StoreDA.SaveStoreBasicInfoToIPP3Vendor(ConvertStoreBasicToIPP3Vendor(basicInfo), info.InUserSysNo.Value);

                trans.Complete();
            }
            return 1;
        }

        public static void SaveStoreAgentProduct(List<VendorAgentInfo> agentsInfoes, int sellerSysNo, string createUserName
            , bool isRequest)
        {
            var singleAgentInfo = new List<VendorAgentInfo>();

            #region check 是否存在重复的代理,品牌,C3
            if (isRequest == false)
            {
                var message = new StringBuilder();
                var originAgentInfo = StoreService.QueryStoreAgentInfos(sellerSysNo);
                agentsInfoes.ForEach(p =>
                {
                    var isExists = originAgentInfo.Any(q => q.C3Name == p.C3Name
                        && q.BrandInfo.SysNo == p.BrandInfo.SysNo);
                    if (isExists)
                    {
                        message.AppendFormat("{0},{1},{2}已代理<br/>", p.BrandInfo.BrandNameLocal, p.C3Name, p.AgentLevel);
                    }
                    else
                    {
                        singleAgentInfo.Add(p);
                    }
                });
                if (message.Length > 0)
                {
                    throw new BusinessException(message.ToString());
                }
            }
            #endregion

            var agentedProduct = StoreDA.QueryStoreAgentInfos(sellerSysNo).Where(p => p.RequestType == VendorModifyRequestStatus.VerifyPass);

            using (var trans = TransactionManager.Create())
            {
                agentsInfoes.ForEach(p =>
                {
                    if (p.AgentSysNo.HasValue && agentedProduct.Any(q => q.AgentSysNo == p.AgentSysNo.Value))
                    {
                        return;
                    }
                    p.RequestType = VendorModifyRequestStatus.Apply;
                    //CreateVendorManufacturerInfo

                    VendorAgentInfo createdVendorAgentInfo;
                    if (p.AgentSysNo.HasValue)
                    {
                        //p.Status = VendorAgentStatus.Normal;
                        //提交审核需要将Status置零
                        if (isRequest)
                        {
                            p.Status = VendorAgentStatus.Normal;
                        }
                        createdVendorAgentInfo = StoreDA.UpdateVendorManufacturerInfo(p, sellerSysNo);
                    }
                    else
                    {
                        createdVendorAgentInfo = StoreDA.CreateVendorManufacturerInfo(p, sellerSysNo);
                    }
                    p.AgentSysNo = createdVendorAgentInfo.AgentSysNo;
                    //CreateVendorCommissionInfo
                    //根据C3和Brand获取佣金规则,设置VendorCommissionInfo
                    //p.BrandInfo.SysNo
                    //p.C3SysNo
                    if (p.BrandInfo != null && p.BrandInfo.SysNo.HasValue)
                    {
                        CommissionRule rule = StoreDA.QueryCommissionRule(p.BrandInfo.SysNo.Value, p.C3SysNo.Value);
                        if (rule != null)
                        {
                            if (p.VendorCommissionInfo == null)
                            {
                                p.VendorCommissionInfo = new VendorCommissionInfo();
                            }
                            p.VendorCommissionInfo.DeliveryFee = rule.DeliveryFee;
                            //p.VendorCommissionInfo.GuaranteedAmt=
                            p.VendorCommissionInfo.RentFee = rule.RentFee;

                            if (string.IsNullOrWhiteSpace(rule.SalesRule))
                            {
                                p.VendorCommissionInfo.SaleRuleEntity = new VendorStagedSaleRuleEntity();
                            }
                            else
                            {
                                p.VendorCommissionInfo.SaleRuleEntity = SerializationUtility.XmlDeserialize<VendorStagedSaleRuleEntity>(rule.SalesRule);
                            }



                            p.VendorCommissionInfo.OrderCommissionAmt = rule.OrderCommissionFee;
                        }
                    }
                    StoreDA.CreateVendorCommissionInfo(p, sellerSysNo, createUserName);

                    #region 写入品牌商检信息
                    //seller portal不需要些商检信息,在ecc中品牌审核通过后写品牌的商检信息

                    //var brandFiling = new StoreBrandFiling();
                    //brandFiling.AgentLevel = p.AgentLevel;
                    //brandFiling.BrandSysNo = p.BrandInfo.SysNo.Value;
                    //brandFiling.CompanyCode = p.CompanyCode;
                    //brandFiling.EditDate = DateTime.Now;
                    //brandFiling.EditUserName = createUserName;
                    //brandFiling.EditUserSysNo = sellerSysNo;
                    //brandFiling.InDate = DateTime.Now;
                    //brandFiling.InUserName = createUserName;
                    //brandFiling.InUserSysNo = sellerSysNo;
                    //brandFiling.Staus = 1;
                    //brandFiling.SellerSysNo = sellerSysNo;
                    //brandFiling.InspectionNo = CommonService.GenerateInspectionNo();
                    //StoreDA.InsertStoreBrandFiling(brandFiling);
                    #endregion

                    #region 写申请表
                    if (isRequest)
                    {
                        VendorModifyRequestInfo requestVendorAgentInfo = new VendorModifyRequestInfo
                        {
                            ActionType = VendorModifyActionType.Add,
                            RequestType = VendorModifyRequestType.Manufacturer,
                            VendorSysNo = sellerSysNo,
                            //生产商的编号读取品牌对应的生产商
                            //ManufacturerSysNo = agentInfo.ManufacturerInfo.SysNo.Value,
                            VendorManufacturerSysNo = p.AgentSysNo.Value,
                            AgentLevel = p.AgentLevel,
                            C2SysNo = p.C2SysNo,
                            C3SysNo = p.C3SysNo,
                            Status = VendorModifyRequestStatus.Apply,
                            SettlePercentage = p.SettlePercentage,
                            SettleType = p.SettleType,
                            BuyWeekDay = p.BuyWeekDay,
                            SendPeriod = p.SendPeriod,
                            BrandSysNo = p.BrandInfo.SysNo,
                            CompanyCode = p.CompanyCode,
                            //CRL20146 By Kilin
                            //写代销结算类型
                            //SettlePeriodType = vendorInfo.VendorFinanceInfo.SettlePeriodType,
                            SettlePeriodType = null,
                            AutoAudit = false,
                            MaxNoPayTimeForOrder = p.MaxNoPayTimeForOrder,
                            IsNeedConfirmOrder = p.IsNeedConfirmOrder
                            //]]
                        };
                        //
                        StoreDA.CreateModifyRequest(requestVendorAgentInfo, sellerSysNo);
                    }
                    #endregion
                });
                trans.Complete();
            }
        }

        public static List<VendorAgentInfo> QueryStoreAgentInfos(int sellerSysNo)
        {
            return StoreDA.QueryStoreAgentInfos(sellerSysNo);
        }

        public static VendorBasicInfo ConvertStoreBasicToIPP3Vendor(StoreBasicInfo info)
        {
            var vendorBasicInfo = new VendorBasicInfo();
            //vendorBasicInfo.VendorID = info.SellerSysNo.Value.ToString();
            vendorBasicInfo.SellerID = info.SellerSysNo.Value.ToString();
            vendorBasicInfo.VendorNameLocal = info.Name;
            vendorBasicInfo.VendorStatus = VendorStatus.Available;
            vendorBasicInfo.VendorType = VendorType.VendorPortal;
            vendorBasicInfo.ConsignFlag = VendorConsignFlag.Sell;
            vendorBasicInfo.VendorIsCooperate = VendorIsCooperate.No;
            return vendorBasicInfo;
        }

        public static int SaveStoreAttachment(List<StoreAttachment> attas)
        {
            if (attas.Count <= 0) return 0;
            using (var trans = TransactionManager.Create())
            {
                //StoreDA.DeleteAttachmentBySellerSysNo(attas[0].SellerSysNo.Value);
                foreach (var item in attas)
                {
                    if (item.SysNo.HasValue)
                    {
                        //TODO update
                        StoreDA.UpdateStoreAttachment(item);
                    }
                    else
                    {
                        StoreDA.InsertStoreAttachment(item);
                    }

                }
                trans.Complete();
            }
            return 1;
        }
        #endregion

        public static int DisableStorePage(int sysno, int SellerSysNo)
        {
            return StoreDA.DisableStorePage(sysno, SellerSysNo);
        }

        public static int EnableStorePage(int sysno, int SellerSysNo)
        {
            return StoreDA.EnableStorePage(sysno, SellerSysNo);
        }

        public static int DeleteStorePage(int sysno, int SellerSysNo)
        {
            return StoreDA.DeleteStorePage(sysno, SellerSysNo);
        }
        public static int DeleteStorePageFromPublish(int sysno, int SellerSysNo)
        {
            return StoreDA.DeleteStorePageFromPublish(sysno, SellerSysNo);
        }

        /// <summary>
        /// 根据PageTypeKey获取页面元素
        /// </summary>
        /// <param name="pageTypeKey"></param>
        public static Dictionary<string, List<StorePageElement>> GetPageElementByPageTypeKey(string pageTypeKey)
        {
            List<StorePageElement> list = StoreDA.GetPageElementByPageTypeKey(pageTypeKey);
            Dictionary<string, List<StorePageElement>> dic = new Dictionary<string, List<StorePageElement>>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.ElementGroupName))
                    {
                        if (dic.ContainsKey(item.ElementGroupName))
                        {
                            dic[item.ElementGroupName].Add(item);
                        }
                        else
                        {
                            List<StorePageElement> elements = new List<StorePageElement>();
                            elements.Add(item);
                            dic.Add(item.ElementGroupName, elements);
                        }
                    }
                }
            }

            return dic;
        }

        public static void SavePageHeader(StorePageHeader header)
        {
            if (header.SysNo.HasValue)
            {
                StoreDA.UpdateStorePageHeader(header);
            }
            else
            {
                StoreDA.InsertStorePageHeader(header);
            }
        }

        public static StorePageHeader QueryPageHeaderBySellerSysNo(int sellerSysNo)
        {
            return StoreDA.QueryPageHeaderBySellerSysNo(sellerSysNo);
        }
        public static List<StoreAttachment> QueryStoreAttachment(int sellerSysNo)
        {
            return StoreDA.QueryStoreAttachment(sellerSysNo);
        }

        public static void DelStoreAgentProduct(int agentSysNo)
        {
            StoreDA.DelStoreAgentProduct(agentSysNo);
        }

        public static void DelStoreAttachment(int sysNo)
        {
            StoreDA.DelStoreAttachment(sysNo);
        }

        public static List<StorePageTheme> QueryAllStorePageTheme()
        {
            return StoreDA.QueryAllStorePageTheme();
        }

        public static int RestoreDefaultStorePage(int sysno, int SellerSysNo)
        {
            return StoreDA.RestoreDefaultStorePage(sysno, SellerSysNo);
        }

        public static VendorBasicInfo LoadVendorInfo(int SellerSysNo)
        {
            return StoreDA.LoadVendorInfo(SellerSysNo);
        }

        public static List<ElementMappingDialog> GetElementMappingDialog(string config)
        {
            var xmlSer = new XmlSerializer(typeof(List<ElementMappingDialog>));
            using (var stream = new StreamReader(config))
            {
                return (List<ElementMappingDialog>)xmlSer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 分页查询商家代理
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="SellSysno"></param>
        /// <returns></returns>
        public static QueryResult<VendorAgentInfo> QueryStoreAgentInfosByPage(StorePageListQueryFilter filter, int SellSysno)
        {
            return StoreDA.QueryStoreAgentInfosByPage(filter, SellSysno);
        }

        public static int SaveCertification(CertificationInfo certificaiton)
        {
            if (certificaiton == null) return 0;
            using (var trans = TransactionManager.Create())
            {
                StoreDA.InsertApplication(certificaiton);
                StoreDA.InsertCertificationAttachment(certificaiton);
                trans.Complete();
            }
            return 1;
        }
    }
}
