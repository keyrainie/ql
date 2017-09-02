using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IBrandRequestDA))]
    public class BrandRequestDA : IBrandRequestDA
    {
        /// <summary>
        ///根据query得到所有待审核的品牌信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetAllBrandRequest(BrandRequestQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllBrandRequest");
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            cmd.SetParameterValue("@Status", "O");
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;

        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditBrandRequest(BrandRequestInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AuditBrandRequest");
            cmd.SetParameterValue("@SysNo",info.SysNo);
            cmd.SetParameterValue("@Status", info.ReustStatus);
            cmd.SetParameterValue("@Auditor", info.User.UserName);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
         
            cmd.ExecuteNonQuery();

        }

       /// <summary>
       /// 提交审核 
       /// </summary>
       /// <param name="info"></param>
        public void InsertBrandRequest(BrandRequestInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertBrandRequest");
            cmd.SetParameterValue("@ManufacturerSysno",info.Manufacturer.SysNo);
            cmd.SetParameterValue("@BrandName_Ch", info.BrandNameLocal.Content);
            cmd.SetParameterValue("@BrandName_En", info.BrandNameGlobal);
            cmd.SetParameterValue("@Reason", info.Reason);
            cmd.SetParameterValue("@Status", info.ReustStatus);
            cmd.SetParameterValue("@InUser", info.User.UserName);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.SetParameterValue("@ProductLine", info.ProductLine);
            cmd.SetParameterValue("@BrandCode", info.BrandCode);
            cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// 是否存在该审核
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsBrandRequest(BrandRequestInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsBrandRequest");
            cmd.SetParameterValue("@ManufacturerSysno", info.Manufacturer.SysNo);
            cmd.SetParameterValue("@BrandName_Ch", info.BrandNameLocal.Content);
            cmd.SetParameterValue("@BrandName_En", info.BrandNameGlobal);
            cmd.SetParameterValue("@Reason", info.Reason);
            cmd.SetParameterValue("@ProductLine", info.ProductLine);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }
        /// <summary>
        /// 是否存在该审核
        /// 当中文名或者英文名存在重的名称即认为存在品牌重复
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsBrandRequest_New(BrandRequestInfo info)
        {
            string queryStr = "";
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsBrandRequest_New");
                if ((!string.IsNullOrWhiteSpace(info.BrandNameLocal.Content))&& string.IsNullOrWhiteSpace(info.BrandNameGlobal))
                {
                    queryStr = " AND BrandName_Ch=@BrandName_Ch";
                    
                }
                if ((!string.IsNullOrWhiteSpace(info.BrandNameGlobal)) && string.IsNullOrWhiteSpace(info.BrandNameLocal.Content))
                {
                    queryStr = " AND BrandName_En=@BrandName_En";
                }
                if ((!string.IsNullOrWhiteSpace(info.BrandNameGlobal)) && (!string.IsNullOrWhiteSpace(info.BrandNameLocal.Content)))
                {
                    queryStr = " AND (BrandName_Ch=@BrandName_Ch or BrandName_En=@BrandName_En)";
                }
                if ((!string.IsNullOrWhiteSpace(info.BrandNameGlobal)) && string.IsNullOrWhiteSpace(info.BrandNameLocal.Content))
                {
                    queryStr = " AND BrandName_Ch='' AND BrandName_En='' ";
                }
                cmd.SetParameterValue("@BrandName_Ch", info.BrandNameLocal.Content);
                cmd.SetParameterValue("@BrandName_En", info.BrandNameGlobal);

                cmd.ReplaceParameterValue("#strWhere#", queryStr);
                cmd.ExecuteNonQuery();
                return (int)cmd.GetParameterValue("@flag") < 0;

        }

        /// <summary>
        /// 检测创建人和审核人是不是同一个人，返回true:是同一个人 flase：不是同一个人
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public bool BrandCheckUser(BrandRequestInfo info)
        {
            return false;
        }
    }
}
