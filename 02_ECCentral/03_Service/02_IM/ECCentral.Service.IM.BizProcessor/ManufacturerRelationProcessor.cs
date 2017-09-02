using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Transactions;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ManufacturerRelationProcessor))]
    public class ManufacturerRelationProcessor
    {
        #region 厂商业务逻辑
        private readonly IManufacturerRelationDA _manufacturerRelationDA = ObjectFactory<IManufacturerRelationDA>.Instance;

        /// <summary>
        /// 根据SysNo获取生产商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public virtual ManufacturerRelationInfo GetManufacturerRelationInfoByLocalManufacturerSysNo(ManufacturerRelationInfo entity)
        {
            return _manufacturerRelationDA.GetManufacturerRelationInfoByLocalManufacturerSysNo(entity);
        }

        /// <summary>
        /// 修改厂商同步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerRelationInfo UpdateManufacturerRelation(ManufacturerRelationInfo entity)
        {
            
            using (TransactionScope scope = new TransactionScope())
            {
                entity = _manufacturerRelationDA.UpdateManufacturerRelation(entity);
                scope.Complete();
            }
            return entity;
        }

        #endregion
    }
}
