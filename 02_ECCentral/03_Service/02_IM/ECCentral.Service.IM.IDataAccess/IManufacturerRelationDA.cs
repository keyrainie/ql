//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商同步管理N业务接口
// 作成者				Roman.Z.Li
// 改版日				2016.9.22
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IManufacturerRelationDA
    {
        /// <summary>
        /// 根据localManufacturerSysNo获取厂商同步信息
        /// </summary>
        /// <param name="localManufacturerSysNo"></param>
        /// <returns></returns>
        ManufacturerRelationInfo GetManufacturerRelationInfoByLocalManufacturerSysNo(ManufacturerRelationInfo entity);


        /// <summary>
        /// 修改厂商同步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ManufacturerRelationInfo UpdateManufacturerRelation(ManufacturerRelationInfo entity);
    }
}
