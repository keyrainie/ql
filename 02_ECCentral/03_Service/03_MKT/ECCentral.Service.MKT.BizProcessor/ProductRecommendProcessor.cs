using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductRecommendProcessor))]
    public class ProductRecommendProcessor
    {
        private IProductRecommendDA _recommendDA = ObjectFactory<IProductRecommendDA>.Instance;

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create(ProductRecommendInfo entity)
        {
            Validate(entity);
            //处理三级分类扩展生效
            //处理扩展生效前先将pageid保存起来
            int? cachedPageID = entity.Location.PageID;
            ProcessECCategory3Extend(entity);
            //将保存的pageid还原到实体上
            entity.Location.PageID = cachedPageID;
            CreateProductRecommend(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update(ProductRecommendInfo entity)
        {
            Validate(entity);
            //处理三级分类扩展生效
            //处理扩展生效前先将pageid保存起来
            int? cachedPageID = entity.Location.PageID;
            ProcessECCategory3Extend(entity);
            //将保存的pageid还原到实体上
            entity.Location.PageID = cachedPageID;
            InsertOrUpdateLocation(entity);
            _recommendDA.Update(entity);
        }

        /// <summary>
        /// 加载
        /// </summary>
        public virtual ProductRecommendInfo Load(int sysNo)
        {
            var result = _recommendDA.Load(sysNo);
            var productInfo = ExternalDomainBroker.GetProductInfo(result.ProductID);
            result.ProductSysNo = productInfo.SysNo;
            return result;
        }

        /// <summary>
        /// 将商品推荐置为无效
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        public virtual void Deactive(int sysNo)
        {
            var current = _recommendDA.Load(sysNo);
            if (current.Status != ADStatus.Active)
            {
                //throw new BizException("当前状态不能执行作废操作。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_CannotDeactive"));
            }
            _recommendDA.Deactive(sysNo);
        }

        /// <summary>
        /// 获取品牌页面位置信息
        /// </summary>
        /// <param name="pageID">品牌页面ID</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        public List<ProductRecommendLocation> GetBrandPosition(int pageID, string companyCode, string channelID)
        {
            //品牌页面类型编号
            int brandPageTypeID = PageTypeUtil.ProductRecommendBrandPageTypeID;
            var result = _recommendDA.GetProductRecommendLocationList(4, pageID, companyCode, channelID);
            if (result == null || result.Count == 0)
            {
                InitBrandPosition(brandPageTypeID, pageID, companyCode, channelID);
                //可能会有新插入的数据，因此要重新加载
                result = _recommendDA.GetProductRecommendLocationList(4, pageID, companyCode, channelID);
            }
            else if (result.Count >= 1 && result.Count <= 23)
            {
                CheckBrandPosition(brandPageTypeID, pageID, companyCode, channelID);
                //可能会有新插入的数据，因此要重新加载
                result = _recommendDA.GetProductRecommendLocationList(4, pageID, companyCode, channelID);
            }
            return result;
        }

        #region 私有方法
        /// <summary>
        /// 为品牌初始化位置信息
        /// </summary>
        /// <param name="pageID">品牌页面ID</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        private void InitBrandPosition(int brandPageTypeID, int pageID, string companyCode, string channelID)
        {
            int i = 1;
            int positionID = 70;
            while (i < 21)
            {
                CreateLocation(brandPageTypeID, pageID, positionID, "自定义" + i, companyCode, channelID);
                positionID++;
                i++;
            }
            CreateLocation(brandPageTypeID, pageID, 31, "新品上架", companyCode, channelID);
            CreateLocation(brandPageTypeID, pageID, 32, "让利促销", companyCode, channelID);
            CreateLocation(brandPageTypeID, pageID, 33, "当季热销", companyCode, channelID);
        }

        /// <summary>
        /// 检查品牌位置信息
        /// </summary>
        /// <param name="pageID">品牌页面ID</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        private void CheckBrandPosition(int brandPageTypeID, int pageID, string companyCode, string channelID)
        {
            int i = 1;
            int positionID = 70;
            while (i < 21)
            {
                if (!_recommendDA.ExitsOnlineListPosition(brandPageTypeID, pageID, positionID, companyCode, channelID))
                {
                    CreateLocation(brandPageTypeID, pageID, positionID, "自定义" + i, companyCode, channelID);
                }
                positionID++;
                i++;
            }
            if (!_recommendDA.ExitsOnlineListPosition(brandPageTypeID, pageID, 31, companyCode, channelID))
            {
                CreateLocation(brandPageTypeID, pageID, 31, "新品上架", companyCode, channelID);
            }
            if (!_recommendDA.ExitsOnlineListPosition(brandPageTypeID, pageID, 32, companyCode, channelID))
            {
                CreateLocation(brandPageTypeID, pageID, 32, "让利促销", companyCode, channelID);
            }
            if (!_recommendDA.ExitsOnlineListPosition(brandPageTypeID, pageID, 33, companyCode, channelID))
            {
                CreateLocation(brandPageTypeID, pageID, 33, "当季热销", companyCode, channelID);
            }
        }

        private void CreateProductRecommend(ProductRecommendInfo entity)
        {
            InsertOrUpdateLocation(entity);
            _recommendDA.Create(entity);
        }

        private void InsertOrUpdateLocation(ProductRecommendInfo entity)
        {
            ProductRecommendLocation loc = _recommendDA.LoadLocation(entity.Location);
            if (entity.Location.PageType == 14)
            {
                entity.Location.PageID = -1;
            }
            if (loc == null)
            {
                //如果位置信息不存在就创建
                _recommendDA.CreateLocation(entity.Location);
            }
            else
            {
                entity.Location.SysNo = loc.SysNo;
                //如果存在就更新描述,更新描述前先验证同一位置是否存在相同的位置描述
                if (!string.IsNullOrWhiteSpace(entity.Location.Description))
                {
                    if (_recommendDA.ExitsSameDescription(entity.Location) > 0)
                    {
                        //throw new BizException("已存在相同的模块名称。");
                        throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ExistsSameModuleName"));
                    }
                    _recommendDA.UpdateLocationDesc(loc.SysNo.Value, entity.Location.Description);
                }
            }
        }

        //通过前台3级类别找到对应的后台3级类别，
        //然后把与后台3级类别对用的所有前台3级类别找出来，所有类别都插入记录
        private void ProcessECCategory3Extend(ProductRecommendInfo entity)
        {
            var presentationType = PageTypeUtil.ResolvePresentationType(ModuleType.ProductRecommend, entity.Location.PageType.Value.ToString());
            if (entity.IsExtendValid && entity.Status == ADStatus.Active
                && presentationType == PageTypePresentationType.Category3
                && entity.Location.PageID != -1)
            {
                var relatedECCategory3List = ObjectFactory<IECCategoryDA>.Instance.GetRelatedECCategory3SysNo(entity.Location.PageID.Value);
                foreach (var c3 in relatedECCategory3List)
                {
                    entity.Location.PageID = c3.SysNo;

                    if (!CheckExists(entity))
                    {
                        CreateProductRecommend(entity);
                    }
                }
            }
        }

        private void Validate(ProductRecommendInfo entity)
        {
            //大于等于100的PageType是首页Domain馆,对应的PageID应置为0
            if (entity.Location.PageType >= 100)
            {
                entity.Location.PageID = 0;
            }
            //如果PageID为空，则置为0
            if (!entity.Location.PageID.HasValue)
            {
                entity.Location.PageID = 0;
            }
            if (entity.BeginDate == DateTime.MinValue)
            {
                entity.BeginDate = null;
            }
            if (entity.EndDate == DateTime.MinValue)
            {
                entity.EndDate = null;
            }
            //验证商品状态必须为show
            ProductInfo product = ExternalDomainBroker.GetProductInfo(entity.ProductID);
            if (product == null)
            {
                //throw new BizException("商品不存在。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductNotExists"));
            }
            if (product.ProductStatus != ProductStatus.Active)
            {
                //throw new BizException("商品状态必须为上架状态。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductStatusNotValid"));
            }
            ////验证商品必须有库存OnlineQty>0
            //var inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(entity.ProductSysNo);
            //if (inventory.OnlineQty <= 0)
            //{
            //    throw new BizException("商品库存不足。");
            //}


            //如果是首页新品推荐，验证商品FirstOnlineTime不能大于1天
            bool isNewRecommend = PageTypeUtil.IsProductRecommendHomePageNewPosition(entity.Location.PageType, entity.Location.PageID, entity.Location.PositionID);
            if (isNewRecommend)
            {
                if (!product.FirstOnSaleDate.HasValue || product.FirstOnSaleDate.Value < DateTime.Parse(DateTime.Now.AddDays(-1).ToLongDateString() + " 00:00:01"))
                {
                    //throw new BizException("请选择上架时间小于1天的商品进行维护。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductOnlineTimeInvalid"));
                }
            }

            //8.PageType=4,当类型是专卖店时判断description是否有重复的
            var presentationType = PageTypeUtil.ResolvePresentationType(ModuleType.ProductRecommend, entity.Location.PageType.ToString());
            //如果是专卖店,判断description是否有重复的
            if (presentationType == PageTypePresentationType.Brand)
            {
                if (product.ProductBasicInfo.ProductBrandInfo.SysNo != entity.Location.PageID)
                {
                    //throw new BizException("该产品不属于所选专卖店。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductBrandInvalid"));
                }
                if (_recommendDA.ExitsSameDescription(entity.Location) > 0)
                {
                   // throw new BizException("已存在相同的模块名称。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ExistsSameModuleName"));
                }
            }
            //如果是品牌专属或类别专属
            else if (presentationType == PageTypePresentationType.BrandExclusive)
            {
                if (product.ProductBasicInfo.ProductBrandInfo.SysNo != entity.Location.PageID)
                {
                    //throw new BizException("该产品不属于所选专卖店。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductBrandInvalid"));
                }
            }
            else if (presentationType == PageTypePresentationType.Merchant)
            {
                if (product.Merchant.MerchantID != entity.Location.PageID)
                {
                    //throw new BizException("该产品不属于所选商家。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductMerchantInvalid"));
                }
            }
            else if (presentationType == PageTypePresentationType.Category3)
            {
                //-1表示默认类别，即不指定任何分类
                if (entity.Location.PageID.Value != -1)
                {
                    var ecC3 = ObjectFactory<ECCategoryProcessor>.Instance.Load(entity.Location.PageID.Value);
                    if (ecC3 == null || !ecC3.C3SysNo.HasValue)
                    {
                        //throw new BizException("请选择产品三级分类！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_Category3NotNull"));
                    }
                    if (product.ProductBasicInfo.ProductCategoryInfo.SysNo != ecC3.C3SysNo)
                    {
                        //throw new BizException("该产品不属于所选分类。");
                        throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ProductCategoryInvalid"));
                    }
                }
            }

            if (CheckExists(entity))
            {
                //throw new BizException("已经存在同一位置同一时期的商品，请更换后重试。");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductRecommend", "ProductRecommend_ExistsSameProductRecommend"));
            }
        }

        private bool CheckExists(ProductRecommendInfo entity)
        {
            var list = _recommendDA.GetProductRecommendByPosition(entity.Location.PageType.Value
                , entity.Location.PageID.Value, entity.Location.PositionID, entity.ProductID, entity.CompanyCode, entity.WebChannel.ChannelID);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            var conflicatedProductRecommend = list.FirstOrDefault(
                p => ((entity.BeginDate >= p.BeginDate && entity.BeginDate <= p.EndDate)
                        || (entity.EndDate >= p.BeginDate && entity.EndDate <= p.EndDate)
                        || (entity.BeginDate <= p.BeginDate && entity.EndDate >= p.EndDate))
                      && p.SysNo != entity.SysNo);
            if (conflicatedProductRecommend == null)
            {
                return false;
            }
            return true;
        }

        private void CreateLocation(int pageType, int pageID, int positionID, string locDesc, string companyCode, string channelID)
        {
            ProductRecommendLocation l = new ProductRecommendLocation();
            l.PageType = pageType;
            l.PageID = pageID;
            l.PositionID = positionID;
            l.Description = locDesc;
            l.CompanyCode = companyCode;
            l.WebChannel = new BizEntity.Common.WebChannel();
            l.WebChannel.ChannelID = channelID;
            _recommendDA.CreateLocation(l);
        }

        #endregion
    }
}
