using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.IDataAccess
{
  public interface IBrandRequestDA
    {
      /// <summary>
      /// 根据query得到所有待审核的品牌信息
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      DataTable GetAllBrandRequest(BrandRequestQueryFilter query,out int totalCount);

      /// <summary>
      /// 审核
      /// </summary>
      /// <param name="info"></param>
      void AuditBrandRequest(BrandRequestInfo info);

      /// <summary>
      /// 提交品牌审核
      /// </summary>
      /// <param name="ingo"></param>
      void InsertBrandRequest(BrandRequestInfo info);

      /// <summary>
      /// 是否存在品牌审核
      /// </summary>
      /// <param name="info"></param>
      /// <returns></returns>
      bool IsExistsBrandRequest(BrandRequestInfo info);

      /// <summary>
      /// 是否存在品牌审核
      /// </summary>
      /// <param name="info"></param>
      /// <returns></returns>
      bool IsExistsBrandRequest_New(BrandRequestInfo info);

      /// <summary>
      /// 审核时检测创建人和审核人是不是同一个人
      /// </summary>
      /// <param name="userName"></param>
      /// <returns></returns>
      bool BrandCheckUser(BrandRequestInfo info);
    }
}
