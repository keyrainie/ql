using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ManufacturerRelationAppService))]
    public class ManufacturerRelationAppService
    {
        /// <summary>
        /// 根据SysNo获取厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public ManufacturerRelationInfo GetManufacturerRelationInfoByLocalManufacturerSysNo(ManufacturerRelationInfo entity)
        {
            var result = ObjectFactory<ManufacturerRelationProcessor>.Instance.GetManufacturerRelationInfoByLocalManufacturerSysNo(entity);
            return result;
        }


        /// <summary>
        /// 修改厂商同步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ManufacturerRelationInfo UpdateManufacturerRelation(ManufacturerRelationInfo entity)
        {
            var result = ObjectFactory<ManufacturerRelationProcessor>.Instance.UpdateManufacturerRelation(entity);
            return result;
        }
    }
}
