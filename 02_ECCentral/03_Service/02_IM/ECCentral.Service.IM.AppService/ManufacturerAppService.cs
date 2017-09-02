//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理业务逻辑
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ManufacturerAppService))]
    public class ManufacturerAppService
    {
        /// <summary>
        /// 根据SysNo获取厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public ManufacturerInfo GetManufacturerInfoBySysNo(int manufacturerSysNo)
        {
            var result = ObjectFactory<ManufacturerProcessor>.Instance.GetManufacturerInfoBySysNo(manufacturerSysNo);
            return result;
        }

        /// <summary>
        /// 创建厂商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ManufacturerInfo CreateManufacturer(ManufacturerInfo entity)
        {
            var result = ObjectFactory<ManufacturerProcessor>.Instance.CreateManufacturer(entity);
            return result;
        }


        /// <summary>
        /// 修改厂商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ManufacturerInfo UpdateManufacturer(ManufacturerInfo entity)
        {
            var result = ObjectFactory<ManufacturerProcessor>.Instance.UpdateManufacturer(entity);
            return result;
        }

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public void DeleteBrandShipCategory(int sysNo)
        {
            ObjectFactory<ManufacturerProcessor>.Instance.DeleteBrandShipCategory(sysNo);
        }

        /// <summary>
        /// 创建旗舰店首页分类
        /// </summary>
        /// <param name="brandShipCategory"></param>
        /// <returns></returns>
        public void CreateBrandShipCategory(BrandShipCategory brandShipCategory)
        {
            ObjectFactory<ManufacturerProcessor>.Instance.CreateBrandShipCategory(brandShipCategory);
        }
    }
}
