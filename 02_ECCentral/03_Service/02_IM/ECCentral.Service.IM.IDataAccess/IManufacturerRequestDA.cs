using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public  interface IManufacturerRequestDA
    {
       /// <summary>
       /// 得到所有带审核的生产商
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetAllManufacturerRequest(ManufacturerRequestQueryFilter query, out int totalCount);

       /// <summary>
       /// 审核生产商
       /// </summary>
       /// <param name="info"></param>
       void AuditManufacturerRequest(ManufacturerRequestInfo info);

       /// <summary>
       ///生产商的提交审核
       /// </summary>
       /// <param name="info"></param>
       void InsertManufacturerRequest(ManufacturerRequestInfo info);

       /// <summary>
       /// 是否存在该生产商的审核
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool IsExistsManufacturerRequest(ManufacturerRequestInfo info);

       /// <summary>
       /// 检测审核人和创建人是否是同一个人
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool CheckManufacturerUser(int SysNo);

       /// <summary>
       /// 检查是否存在生产商
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool CheckIsExistsManufacturer(string localName, string BirName);
    }
}
