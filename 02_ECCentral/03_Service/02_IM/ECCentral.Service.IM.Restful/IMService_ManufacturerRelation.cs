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
        /// <summary>
        /// 根据SysNo获取生产商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ManufacturerRelation/GetManufacturerRelationInfoByLocalManufacturerSysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ManufacturerRelationInfo GetManufacturerRelationInfoByLocalManufacturerSysNo(ManufacturerRelationInfo request)
        {
            var entity = ObjectFactory<ManufacturerRelationAppService>.Instance.GetManufacturerRelationInfoByLocalManufacturerSysNo(request);
            return entity;
        }

        /// <summary>
        /// 修改厂商
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ManufacturerRelation/UpdateManufacturerRelation", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ManufacturerRelationInfo UpdateManufacturerRelation(ManufacturerRelationInfo request)
        {
            var entity = ObjectFactory<ManufacturerRelationAppService>.Instance.UpdateManufacturerRelation(request);
            return entity;
        }
    }
}
