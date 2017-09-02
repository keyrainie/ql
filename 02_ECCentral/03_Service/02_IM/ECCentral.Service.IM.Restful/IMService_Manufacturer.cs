//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理Restful实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 查询


        /// <summary>
        /// 根据SysNo获取生产商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/GetManufacturerInfoBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ManufacturerInfo GetManufacturerInfoBySysNo(int manufacturerSysNo)
        {
            var entity = ObjectFactory<ManufacturerAppService>.Instance.GetManufacturerInfoBySysNo(manufacturerSysNo);
            return entity;
        }

        /// <summary>
        /// 查询生产商
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/QueryManufacturer", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryManufacturer(ManufacturerQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryCondtionInvalid"));
            }
            int totalCount;
            var dataTable = ObjectFactory<IManufacturerQueryDA>.Instance.QueryManufacturer(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询品牌旗舰店首页分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/QueryManufacturerCategory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryManufacturerCategory(ManufacturerQueryFilter queryCriteria)
        {
            int totalCount;
            var entity = ObjectFactory<IManufacturerQueryDA>.Instance.GetManufacturerCategoryBySysNo(queryCriteria, out totalCount);
            return new QueryResult() { Data = entity, TotalCount = totalCount };
        }
        #endregion

        #region 添加以及修改操作
        /// <summary>
        /// 创建厂商
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/CreateManufacturer", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ManufacturerInfo CreateManufacturer(ManufacturerInfo request)
        {
            var entity = ObjectFactory<ManufacturerAppService>.Instance.CreateManufacturer(request);
            return entity;
        }

        /// <summary>
        /// 修改厂商
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/UpdateManufacturer", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ManufacturerInfo UpdateManufacturer(ManufacturerInfo request)
        {
            var entity = ObjectFactory<ManufacturerAppService>.Instance.UpdateManufacturer(request);
            return entity;
        }
        #endregion

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/DeleteBrandShipCategory", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteBrandShipCategory(int sysNo)
        {
            ObjectFactory<ManufacturerAppService>.Instance.DeleteBrandShipCategory(sysNo);
        }

        /// <summary>
        /// 创建旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Manufacturer/CreateBrandShipCategory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateBrandShipCategory(BrandShipCategory brandShipCategory)
        {
             ObjectFactory<ManufacturerAppService>.Instance.CreateBrandShipCategory(brandShipCategory);
        }
    }
}
