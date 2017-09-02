using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.BizEntity;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.IM;
using ValidStatus = ECCentral.BizEntity.IM.ValidStatus;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(PageTypeProcessor))]
    public class PageTypeProcessor
    {
        /// <summary>
        /// PageType在CodeNamePair配置中Key的格式
        /// {0}代表companyCode
        /// {1}代表channelID
        /// {2}代表moduleType
        /// </summary>
        private const string PageType_CodeNamePair_KeyFormat = "PageType-{0}-{1}-{2}";

        /// <summary>
        /// Page在CodeNamePair、配置中Key的格式
        /// {0}代表companyCode
        /// {1}代表channelID
        /// {2}代表PageTypePresentationType
        /// </summary>
        private const string Page_CodeNamePair_KeyFormat = "Page-{0}-{1}-{2}";

        /// <summary>
        /// 获取页面类型
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <param name="moduleType">模块类型</param>
        /// <returns>页面类型CodeNamePair列表</returns>
        public virtual List<CodeNamePair> GetPageType(string companyCode, string channelID, ModuleType moduleType)
        {
            List<CodeNamePair> result = null;
            switch (moduleType)
            {
                case ModuleType.NewsAndBulletin://新闻公告
                    result = ObjectFactory<INewsDA>.Instance.GetAllNewTypes(companyCode, channelID);
                    break;
                case ModuleType.Banner:
                case ModuleType.SEO:
                    result = ObjectFactory<IPageTypeDA>.Instance.GetPageTypes(companyCode, channelID);
                    break;
                case ModuleType.ProductRecommend:
                case ModuleType.HotKeywords:
                    result = GetPageTypeFromConfig(companyCode, channelID, moduleType.ToString());
                    //var sections = ObjectFactory<IHomePageSectionQueryDA>.Instance.GetDomainCodeNames(companyCode, channelID);
                    //result.AddRange(sections);
                    break;
                case ModuleType.DefaultKeywords:
                case ModuleType.HotSale:
                case ModuleType.Poll:
                default:
                    result = GetPageTypeFromConfig(companyCode, channelID, moduleType.ToString());
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据页面类型（PageTypeID），获取Page List
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <param name="moduleType"></param>
        /// <param name="pageTypeID"></param>
        /// <returns></returns>
        public virtual PageResult GetPage(string companyCode, string channelID, ModuleType moduleType, string pageTypeID)
        {
            PageResult result = new PageResult();
            result.PresentationType = PageTypeUtil.ResolvePresentationType(moduleType, pageTypeID);

            switch (result.PresentationType)
            {
                case PageTypePresentationType.NoneSubPages:
                    result.PageList = GetNoneSubPages(companyCode, channelID, moduleType, pageTypeID);
                    break;
                case PageTypePresentationType.OtherSales://其他促销页面
                    result.PageList = ObjectFactory<ISaleAdvTemplateQueryDA>.Instance.GetActiveCodeNames(companyCode, channelID);
                    break;
                case PageTypePresentationType.Merchant://商家页面
                    //读商家列表
                    var vendorList = ObjectFactory<IPOBizInteract>.Instance.GetVendorNameListByVendorType(VendorType.VendorPortal, companyCode);
                    if (vendorList != null)
                    {
                        result.PageList = new List<WebPage>();
                        result.PageList.Add(new WebPage { ID = 1, PageName = "泰隆优选" });
                        foreach (var kv in vendorList)
                        {
                            result.PageList.Add(new WebPage { ID = kv.Key, PageName = kv.Value });
                        }
                    }
                    break;
                case PageTypePresentationType.AppleZone://Apple专区
                    result.PageList = GetApplePage(companyCode, channelID, PageTypePresentationType.AppleZone);
                    break;
                case PageTypePresentationType.Brand://品牌
                    //TODO:读品牌列表
                    result.PageList = GetWebPageFromBrandList((b) => b.Status == ValidStatus.Active && b.BrandStoreType > 0);
                    break;
                case PageTypePresentationType.BrandExclusive://品牌专卖
                    //TODO:读品牌列表
                    result.PageList = GetWebPageFromBrandList((b) => b.Status == ValidStatus.Active);
                    break;
                case PageTypePresentationType.Flagship://品牌旗舰店
                    //TODO:读品牌旗舰店列表
                    //result.PageList = GetWebPageFromBrandList((b) => b.BrandStoreType == BrandStoreType.FlagshipStore && b.Status == ValidStatus.Active);
                    result.PageList = GetStoresList((b) => b.BrandStoreType == BrandStoreType.FlagshipStore, companyCode);
                    break;
                case PageTypePresentationType.Stores://专卖店
                    result.PageList = GetStoresList((b) => b.Status == BizEntity.IM.ManufacturerStatus.Active, companyCode);
                    break;
            }

            return result;
        }

        private List<WebPage> GetNoneSubPages(string companyCode, string channelID, ModuleType moduleType, string pageTypeID)
        {
            var pageTypes = ObjectFactory<PageTypeProcessor>.Instance.GetPageType(companyCode, channelID, moduleType);
            if (pageTypes != null)
            {
                return pageTypes.Where(p => p.Code == pageTypeID).ToList().ConvertAll(x => new WebPage()
                {
                    ID = 0,
                    PageName = x.Name
                });
            }
            return new List<WebPage>(0);
        }

        private List<WebPage> GetWebPageFromBrandList(Predicate<BrandInfo> match)
        {
            List<WebPage> result = new List<WebPage>();
            var list = ExternalDomainBroker.GetBrandList();
            if (list != null)
            {
                var found = list.FindAll(match);
                if (found.Count > 0)
                    found = found.OrderByDescending(p => p.SysNo).ToList();
                foreach (var b in found)
                {
                    string pageName = "";
                    if (b.BrandNameLocal != null)
                    {
                        pageName += b.BrandNameLocal.Content;
                    }
                    pageName += b.BrandNameGlobal;
                    result.Add(new WebPage { ID = b.SysNo, PageName = pageName });
                }
            }
            return result;
        }


        /// <summary>
        /// 获取专卖店列表
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private List<WebPage> GetStoresList(Predicate<ManufacturerInfo> match, string compayCode)
        {
            List<WebPage> result = new List<WebPage>();
            var list = ExternalDomainBroker.GetManufacturerList(compayCode);
            if (list != null)
            {
                var found = list.FindAll(match);
                if (found.Count > 0)
                    found = found.OrderBy(p => p.ManufacturerNameGlobal).ToList();
                foreach (var b in found)
                {
                    string pageName = "";
                    if (b.ManufacturerNameLocal != null)
                    {
                        pageName += b.ManufacturerNameLocal.Content;
                    }
                    pageName += b.ManufacturerNameGlobal;
                    result.Add(new WebPage { ID = b.SysNo, PageName = pageName });
                }
            }
            return result;
        }

        //从CodeNamePair配置中读取页面类型
        private List<CodeNamePair> GetPageTypeFromConfig(string companyCode, string channelID, string moduleType)
        {
            string key = string.Format(PageType_CodeNamePair_KeyFormat, companyCode, channelID, moduleType);
            var kvList = CodeNamePairManager.GetList("MKT", key);
            return kvList;
        }
        private List<WebPage> GetApplePage(string companyCode, string channelID, PageTypePresentationType presentationType)
        {
            string key = string.Format(Page_CodeNamePair_KeyFormat, companyCode, channelID, presentationType.ToString());
            var kvList = CodeNamePairManager.GetList("MKT", key);
            List<WebPage> result = new List<WebPage>(kvList.Count);
            kvList.ForEach((kv) => result.Add(new WebPage { ID = int.Parse(kv.Code), PageName = kv.Name }));
            return result;
        }
        private IPageTypeDA _pageTypeDA = ObjectFactory<IPageTypeDA>.Instance;

        //插入页面类型
        public virtual void Create(PageType entity)
        {
            Validate(entity);
            //生成PageTypeID
            int maxPageID = _pageTypeDA.GetMaxPageTypeID(entity.CompanyCode, entity.WebChannel.ChannelID);
            entity.PageTypeID = maxPageID + 1;
            _pageTypeDA.Create(entity);
        }

        //更新页面类型
        public virtual void Update(PageType entity)
        {
            Validate(entity);
            _pageTypeDA.Update(entity);
        }

        //加载页面类型
        public virtual PageType Load(int sysNo)
        {
            return _pageTypeDA.Load(sysNo);
        }

        //验证PageType实体
        private void Validate(PageType entity)
        {
            //验证PageTypeName不能重复
            int pageTypenameDuplicateCount = _pageTypeDA.CountPageTypeName(entity.SysNo ?? 0, entity.PageTypeName, entity.CompanyCode, entity.WebChannel.ChannelID);
            if (pageTypenameDuplicateCount > 0)
                throw new BizException(ResouceManager.GetMessageString("MKT.PageType", "PageType_ExistsSameName"));
        }
    }
}
