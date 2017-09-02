//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		        PM管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductManagerAppService))]
    public class ProductManagerAppService
    {
        /// <summary>
        /// 根据SysNO获取PM信息
        /// </summary>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        public ProductManagerInfo GetProductManagerInfoBySysNo(int productManagerInfoSysNo)
        {
            var result = ObjectFactory<ProductManagerProcessor>.Instance.GetProductManagerInfoBySysNo(productManagerInfoSysNo);
            return result;
        }

        /// <summary>
        /// 创建PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerInfo CreateProductManagerInfo(ProductManagerInfo entity)
        {
            var result = ObjectFactory<ProductManagerProcessor>.Instance.CreateProductManagerInfo(entity);
            return result;
        }

        /// <summary>
        /// 修改PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerInfo UpdateProductManagerInfo(ProductManagerInfo entity)
        {
            var result = ObjectFactory<ProductManagerProcessor>.Instance.UpdateProductManagerInfo(entity);
            return result;
        }
        /// <summary>
        /// 获取PM List (PM控件用)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="loginName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<ProductManagerInfo> GetPMList(PMQueryType queryType, string loginName, string companyCode)
        {
            return ObjectFactory<ProductManagerProcessor>.Instance.GetPMList(queryType, loginName, companyCode);
        }

        public List<ProductManagerInfo> GetPMLeaderList(string companyCode)
        {
            return ObjectFactory<ProductManagerProcessor>.Instance.GetPMLeaderList(companyCode);
        }
    }
}
