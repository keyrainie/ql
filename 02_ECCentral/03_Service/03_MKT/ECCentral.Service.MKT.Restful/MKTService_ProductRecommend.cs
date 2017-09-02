using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebInvoke(UriTemplate = "/ProductRecommend/Query", Method = "POST")]
        public virtual QueryResult QueryProductRecommend(ProductRecommendQueryFilter filter)
        {
            int totalCount;
            var dt = ObjectFactory<IProductRecommendQueryDA>.Instance.Query(filter, out totalCount);
            QueryResult result = new QueryResult();
            result.Data = dt;
            result.TotalCount = totalCount;
            GetInventoryInfoByStock(dt);
            dt.Columns.Add("PageTypeName", typeof(string));
            dt.Columns.Add("PositionName", typeof(string));
            //获取页面类型
            var pageTypes = ObjectFactory<PageTypeAppService>.Instance.GetPageType(filter.CompanyCode, filter.ChannelID, ModuleType.ProductRecommend);
            Dictionary<string, List<CodeNamePair>> dictPositionCache = new Dictionary<string, List<CodeNamePair>>();
            foreach (DataRow dr in result.Data.Rows)
            {
                string pageTypeID = dr["PageType"].ToString();
                var foundPageType = pageTypes.FirstOrDefault(item => item.Code == pageTypeID);
                if (foundPageType != null)
                {
                    dr["PageTypeName"] = foundPageType.Name;
                }
                //根据PageType查询位置信息

                List<CodeNamePair> currentPagePosition;
                if (!dictPositionCache.TryGetValue(pageTypeID, out currentPagePosition))
                {
                    currentPagePosition = this.GetProductRecommendPosition(pageTypeID);
                    if(currentPagePosition!=null&&currentPagePosition.Count>0)
                        {
                            currentPagePosition.ForEach(v=>
                                                {
                                                    if(v.Name.Contains("专卖店--"))
                                                    {
                                                        v.Name = v.Name.Replace("专卖店--", "");
                                                    }
                                                });
                        }
                    dictPositionCache.Add(pageTypeID, currentPagePosition);
                }
                var foundPosition = currentPagePosition.FirstOrDefault(item => item.Code == dr["PositionID"].ToString());
                if (foundPosition != null)
                {
                    dr["PositionName"] = foundPosition.Name;
                }
            }

            return result;
        }

        #region 商品查询合并分仓库存信息
        /// <summary>
        /// 获取包含分仓库存的数据
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual void GetInventoryInfoByStock(DataTable product)
        {
            if (product == null || product.Rows.Count == 0) return;
            var stockList = new Dictionary<string, int> { 
                                                            { "Shanghai", 51 }, 
                                                            { "Begjin", 52 }, 
                                                            { "Guangzhou", 53 } 
                                                           };
            var displayField = new[] { "OnlineQty"};

            var columns = (from k in stockList.Keys
                           from c in displayField
                           select k + c).ToList();
            AddTableColumns(product, columns);

            var productSysNoList = (from e in product.AsEnumerable()
                                    select e.Field<int>("ProductSysNo")).ToList();
            var prarm = (from k in productSysNoList
                         from c in stockList.Keys
                         select new { ProductSysNo = k, Stock = c }).ToList();
            prarm.ForEach(v =>
            {
                var entity = _productRecommendAppService.GetProductInventoryInfoByStock(v.ProductSysNo, stockList[v.Stock]);
                var oneProduct = (from e in product.AsEnumerable()
                                  where e.Field<int>("ProductSysNo") == v.ProductSysNo
                                  select e).FirstOrDefault();

                displayField.ForEach(k =>
                {
                    int value = 0;
                    if (entity != null)
                    {
                        object tempvalue = null;
                        var pro = entity.GetType().GetProperty(k);
                        if (pro != null)
                        {
                            tempvalue = Invoker.PropertyGet(entity, k);
                        }
                       
                        value = Convert.ToInt32(tempvalue);
                    }
                    var columnName = v.Stock + k;
                    if (oneProduct != null)
                        oneProduct.SetField(columnName, value);
                });

            });

        }

        /// <summary>
        /// 合并仓库数据
        /// </summary>
        /// <param name="product"></param>
        /// <param name="columns"></param>
        private void AddTableColumns(DataTable product, List<string> columns)
        {
            if (columns == null || columns.Count == 0) return;
            foreach (var item in columns) product.Columns.Add(item).DefaultValue = 0;
        }

        #endregion

        private ProductRecommendAppService _productRecommendAppService = ObjectFactory<ProductRecommendAppService>.Instance;

        /// <summary>
        /// 创建
        /// </summary>
        [WebInvoke(UriTemplate = "/ProductRecommend/Create", Method = "POST")]
        public virtual void CreateProductRecommend(ProductRecommendInfo entity)
        {
            _productRecommendAppService.Create(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        [WebInvoke(UriTemplate = "/ProductRecommend/Update", Method = "PUT")]
        public virtual void UpdateProductRecommend(ProductRecommendInfo entity)
        {
            _productRecommendAppService.Update(entity);
        }

        /// <summary>
        /// 加载
        /// </summary>
        [WebGet(UriTemplate = "/ProductRecommend/Load/{sysNo}")]
        public virtual ProductRecommendInfo LoadProductRecommend(string sysNo)
        {
            int id = int.Parse(sysNo);
            return _productRecommendAppService.Load(id);
        }

        /// <summary>
        /// 将商品推荐置为无效
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        [WebInvoke(UriTemplate = "/ProductRecommend/Deactive/{sysNo}", Method = "PUT")]
        public virtual void DeactiveProductRecommend(string sysNo)
        {
            int id = int.Parse(sysNo);
            _productRecommendAppService.Deactive(id);
        }

        [WebGet(UriTemplate = "/ProductRecommend/GetPosition/{pageTypeID}")]
        public virtual List<CodeNamePair> GetProductRecommendPosition(string pageTypeID)
        {
            int pageType = int.Parse(pageTypeID);
            var presentationType = PageTypeUtil.ResolvePresentationType(ModuleType.ProductRecommend, pageTypeID);
            //if (presentationType == PageTypePresentationType.Brand)
            //{
            //    //当PageType为专卖店时返回空，专卖店有特殊的加载位置信息的逻辑
            //    return new List<CodeNamePair>();
            //}

            //商品推荐位置Key格式：ProductRecommend+[PageTypeID],比如ProductRecommend0代表首页的推荐位
            var positionConfigKey = ModuleType.ProductRecommend.ToString();
            //if (pageType > 100)
            //{
            //    //PageTypeID>100表示是首页domain馆
            //    positionConfigKey += "-DomainList";
            //}
            //else
            //{
                positionConfigKey += pageType.ToString();
            //}

            var result= CodeNamePairManager.GetList("MKT", positionConfigKey);
            return result;
        }

        [WebGet(UriTemplate = "/ProductRecommend/GetBrandPosition/{pageID}/{comapnyCode}/{channelID}")]
        public virtual List<CodeNamePair> GetProductRecommandBrandPosition(string pageID, string comapnyCode, string channelID)
        {
            var brandLocation = _productRecommendAppService.GetBrandPosition(int.Parse(pageID), comapnyCode, channelID);
            List<CodeNamePair> result = new List<CodeNamePair>();
            foreach (var loc in brandLocation)
            {
                result.Add(new CodeNamePair
                {
                    Code = loc.PositionID.ToString(),
                    Name = string.Format("专卖店--{0}", loc.Description)
                });
            }

            return result;
        }
    }
}
