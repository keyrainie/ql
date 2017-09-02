using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using System.Data;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategoryRequestApprovalDA))]
    public class CategoryRequestApprovalDA : ICategoryRequestApprovalDA
    {
        /// <summary>
        /// 获取所有需要审核的类别信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public DataTable GetCategoryRequestApprovalList(CategoryRequestApprovalQueryFilter query, out int TotalCount)
        {
            DataCommand dc = null;
            switch (query.Category)
            {
                case CategoryType.CategoryType1:
                    dc = DataCommandManager.GetDataCommand("GetCategoryRequest1List");
                    break;
                case CategoryType.CategoryType2:
                    dc = DataCommandManager.GetDataCommand("GetCategoryRequest2List");
                    break;
                case CategoryType.CategoryType3:
                    dc = DataCommandManager.GetDataCommand("GetCategoryRequest3List");
                    break;
                default:
                    dc = DataCommandManager.GetDataCommand("GetCategoryRequest1List");
                    break;
            }
            dc.SetParameterValue("@PageCurrent",query.PagingInfo.PageIndex);
            dc.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            dc.SetParameterValue("@CreateUserSysNo", 0);
            dc.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable();
            TotalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;


        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void ActiveCategoryRequest(CategoryRequestApprovalInfo info)
        {
             DataCommand dc =DataCommandManager.GetDataCommand("ActiveCategoryRequest");
             dc.SetParameterValue("@SysNo", info.SysNo);
             dc.SetParameterValue("@Status", info.Status);
             dc.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
             dc.SetParameterValue("@CategorySysNo", info.CategorySysNo);
             dc.ExecuteNonQuery();
        }


        //public void UpdateSyncForCategoryApprove()
        //{
        //    DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSyncForCategoryApprove");
        //    cmd.SetParameterValue("@SyncType", 3);
        //    cmd.ExecuteNonQuery();
        //}


        public void CreateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCategoryRequest");
            cmd.SetParameterValue("@CategoryType", info.CategoryType);
            cmd.SetParameterValue("@CategorySysNo", info.CategorySysNo);
            cmd.SetParameterValue("@ParentCategorySysNo", info.ParentSysNumber);
            cmd.SetParameterValue("@OperationType", info.OperationType);
            cmd.SetParameterValue("@CategoryName", info.CategoryName);
            cmd.SetParameterValue("@CategoryStatus", info.CategoryStatus);
            cmd.SetParameterValue("@Reasons", info.Reasons);
            cmd.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.SetParameterValue("@C3Code", info.C3Code);
             cmd.ExecuteNonQuery();
        }

        public void UpdateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryRequest");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@CategoryType", info.CategoryType);
            cmd.SetParameterValue("@CategorySysNo", info.CategorySysNo);
            cmd.SetParameterValue("@ParentCategorySysNo", info.ParentSysNumber);
            cmd.SetParameterValue("@OperationType", info.OperationType);
            cmd.SetParameterValue("@CategoryName", info.CategoryName);
            cmd.SetParameterValue("@CategoryStatus", info.CategoryStatus);
            cmd.SetParameterValue("@Reasons", info.Reasons);
            cmd.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@C3Code", info.C3Code);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 判重
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsCategoryRequest(CategoryRequestApprovalInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategoryRequest");
            cmd.SetParameterValue("@ParentCategorySysNo", info.ParentSysNumber);
            cmd.SetParameterValue("@CategoryName",info.CategoryName);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") > 0;
        }

        /// <summary>
        /// 检测审核人和创建人是否是同一个人 返回true 是同一个人
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CheckCategoryUser(int? requetSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckCategoryUser");
            cmd.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@SysNo", requetSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }
    }
}
