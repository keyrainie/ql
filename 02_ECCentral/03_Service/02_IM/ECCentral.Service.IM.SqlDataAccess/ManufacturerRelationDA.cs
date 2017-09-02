//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商同步管理业务接口实现
// 作成者				Roman.Z.Li
// 改版日				2016.9.22
// 改版内容				新建
//************************************************************************
using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;


namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IManufacturerRelationDA))]
    public class ManufacturerRelationDA : IManufacturerRelationDA
    {
        /// <summary>
        /// 根据localManufacturerSysNo查询厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public virtual ManufacturerRelationInfo GetManufacturerRelationInfoByLocalManufacturerSysNo(ManufacturerRelationInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetManufacturerRelationByLocalManufacturerSysNo");
            cmd.SetParameterValue("@localManufacturerSysNo", entity.LocalManufacturerSysNo);
            cmd.SetParameterValue("@InUser", entity.User.UserName);

            var sourceEntity = cmd.ExecuteEntity<ManufacturerRelationInfo>();
            return sourceEntity;
        }


        /// <summary>
        /// 修改厂商同步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerRelationInfo UpdateManufacturerRelation(ManufacturerRelationInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateManufacturerRelation");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@NeweggManufacturer", entity.NeweggManufacturer);
            cmd.SetParameterValue("@AmazonManufacturer", entity.AmazonManufacturer);
            cmd.SetParameterValue("@EBayManufacturer", entity.EBayManufacturer);
            cmd.SetParameterValue("@OtherManufacturerSysNo", entity.OtherManufacturerSysNo);
            cmd.SetParameterValue("@EditUser", entity.User.UserName);
            cmd.ExecuteNonQuery();
            return entity;
        }
    }
}
