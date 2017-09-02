using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
  [VersionExport(typeof(ManufacturerRequestAppService))]
   public  class ManufacturerRequestAppService
    {
       /// <summary>
        /// 生产商审核
        /// </summary>
        /// <param name="info"></param>
      public void AuditManufacturerRequest(ManufacturerRequestInfo info)
      {
          ObjectFactory<ManufacturerRequestProcessor>.Instance.AuditManufacturerRequest(info);
      }
      /// <summary>
      /// 生产商提交审核
      /// </summary>
      /// <param name="info"></param>
      public void InsertManufacturerRequest(ManufacturerRequestInfo info)
      {
          ObjectFactory<ManufacturerRequestProcessor>.Instance.InsertManufacturerRequest(info);
      }
    }
}
