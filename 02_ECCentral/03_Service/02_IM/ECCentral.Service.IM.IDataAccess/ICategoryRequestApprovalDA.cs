using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
namespace ECCentral.Service.IM.IDataAccess
{
   public interface ICategoryRequestApprovalDA
    {
       /// <summary>
       /// 根据query获取需要审核的数据
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetCategoryRequestApprovalList(CategoryRequestApprovalQueryFilter query, out int TotalCount);

       /// <summary>
       /// 审核
       /// </summary>
       void ActiveCategoryRequest(CategoryRequestApprovalInfo info);

       ///// <summary>
       ///// 同步IPP3.dbo.Sys_Sync
       ///// </summary>
       //void UpdateSyncForCategoryApprove();

       /// <summary>
       /// 创建类别审核
       /// </summary>
       /// <param name="info"></param>
       void CreateCategoryRequest(CategoryRequestApprovalInfo info);

       /// <summary>
       /// 更新类别审核
       /// </summary>
       /// <param name="info"></param>
       void UpdateCategoryRequest(CategoryRequestApprovalInfo info);

       /// <summary>
       /// 是否存在CategoryRequest
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool IsExistsCategoryRequest(CategoryRequestApprovalInfo info);

       /// <summary>
       /// 检测审核人和创建人是否是同一个人
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool CheckCategoryUser(int? requetSysNo);
    }
}
