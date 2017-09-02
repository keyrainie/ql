//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理Restful实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;
using ECCentral.Service.IM.IDataAccess;

namespace ECCentral.Service.IM.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class IMService
    {
        #region 查询

        /// <summary>
        /// 查询品牌多条件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/QueryBrand", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryBrand(BrandQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandCondtionIsNull"));
            }
            int totalCount;
            var dataTable = ObjectFactory<IBrandQueryDA>.Instance.QueryBrand(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询品牌简单条件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/QueryBrandInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryBrandInfo(BrandQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandCondtionIsNull"));
            }
            int totalCount;
            var dataTable = ObjectFactory<IBrandQueryDA>.Instance.QueryBrandInfo(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 根据SysNO获取品牌信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/GetBrandInfoBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public BrandInfo GetBrandInfoBySysNo(int brandSysNo)
        {
            var entity = ObjectFactory<BrandAppService>.Instance.GetBrandInfoBySysNo(brandSysNo);
            return entity;
        }
        #endregion

        #region 修改以及添加操作
        /// <summary>
        /// 自动生成品牌Code
        /// </summary>
        [WebInvoke(UriTemplate = "/Brand/GetBrandCode", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public string GetBrandCode()
        {
            return ObjectFactory<BrandAppService>.Instance.GetBrandCode();
        }
        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/CreateBrand", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public BrandInfo CreateBrand(BrandInfo request)
        {
            var entity = ObjectFactory<BrandAppService>.Instance.CreateBrand(request);
            return entity;
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/UpdateBrand", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public BrandInfo UpdateBrand(BrandInfo request)
        {
            var entity = ObjectFactory<BrandAppService>.Instance.UpdateBrand(request);
            return entity;
        }

        /// <summary>
        /// 批量设置品牌置顶
        /// </summary>
        /// <param name="SysNos">SysNo集合</param>

        [WebInvoke(UriTemplate = "/Brand/SetTopBrands", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void SetTopBrands(List<string> list)
        {
            ObjectFactory<BrandAppService>.Instance.SetTopBrands(list);
        }
        /// <summary>
        /// 跟据生产商Sys更新品牌
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Brand/UpdateBrandMasterByManufacturerSysNo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateBrandMasterByManufacturerSysNo(BrandInfo entity)
        {
            ObjectFactory<BrandAppService>.Instance.UpdateBrandMasterByManufacturerSysNo(entity);
        }
        #endregion

        #region "授权牌操作"

        /// <summary>
        /// 根据品牌SysNo得到该品牌的所有授权牌
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/GetBrandAuthorizedByBrandSysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetBrandAuthorizedByBrandSysNo(BrandAuthorizedFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IBrandDA>.Instance.GetBrandAuthorizedByBrandSysNo(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量删除授权牌
        /// </summary>
        /// <param name="SysNo"></param>
        [WebInvoke(UriTemplate = "/Brand/DeleteBrandAuthorized", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteBrandAuthorized(List<int> list)
        {
            ObjectFactory<BrandAppService>.Instance.DeleteBrandAuthorized(list);

        }

        /// <summary>
        /// 更新授权牌的状态
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Brand/UpdateBrandAuthorized", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateBrandAuthorized(List<BrandAuthorizedInfo> listInfo)
        {
            ObjectFactory<BrandAppService>.Instance.UpdateBrandAuthorized(listInfo);
        }

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/Brand/InsertBrandAuthorized", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void InsertBrandAuthorized(BrandAuthorizedInfo info)
        {
            ObjectFactory<BrandAppService>.Instance.InsertBrandAuthorized(info);
        }

        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/IsExistBrandAuthorized", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public bool IsExistBrandAuthorized(BrandAuthorizedInfo info)
        {
            return ObjectFactory<BrandAppService>.Instance.IsExistBrandAuthorized(info);
        }
        /// <summary>
        /// 检测图片地址是否正确
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Brand/UrlIsExist", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public bool UrlIsExist(string url)
        {
            return ObjectFactory<BrandAppService>.Instance.UrlIsExist(url);
        }
        #endregion

    }
}
