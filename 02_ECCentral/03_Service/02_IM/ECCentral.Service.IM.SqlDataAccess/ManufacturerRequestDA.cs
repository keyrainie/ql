using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IManufacturerRequestDA))]
    public class ManufacturerRequestDA : IManufacturerRequestDA
    {
        /// <summary>
        /// 获取待审核的所有生产商
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetAllManufacturerRequest(ManufacturerRequestQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllManufacturerRequest");
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "ManufacturerStatus", typeof(ManufacturerStatus) },
                };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount =(int) cmd.GetParameterValue("@TotalCount");
            return dt;

        }


        /// <summary>
        /// 生产商审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditManufacturerRequest(ManufacturerRequestInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AuditManufacturerRequest");
            cmd.SetParameterValue("@SysNo",info.SysNo);
            cmd.SetParameterValue("@ManufacturerSysNo", info.ManufacturerSysNo);
            cmd.SetParameterValue("@ManufacturerName", info.ManufacturerName);
            cmd.SetParameterValue("@ManufacturerStatus", info.ManufacturerStatus);
            cmd.SetParameterValue("@ProductLine", info.ProductLine);
            cmd.SetParameterValue("@Reasons", info.Reasons);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@ManufacturerBriefName", info.ManufacturerBriefName);
            cmd.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 提交审核 
        /// </summary>
        /// <param name="info"></param>
        public void InsertManufacturerRequest(ManufacturerRequestInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertManufacturerRequest");
            dc.SetParameterValue("@OperationType",info.OperationType);
            dc.SetParameterValue("@ManufacturerName", info.ManufacturerName);
            dc.SetParameterValue("@ManufacturerStatus", info.ManufacturerStatus);
            dc.SetParameterValue("@ManufacturerSysNo", info.ManufacturerSysNo);
            dc.SetParameterValue("@Reasons", info.Reasons);
            dc.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@AuditUserSysNo", 0);
            dc.SetParameterValue("@AuditTime", null);
            dc.SetParameterValue("@Status", 0);
            dc.SetParameterValue("@ManufacturerBriefName", info.ManufacturerBriefName);
            dc.SetParameterValue("@CompanyCode", info.CompanyCode);
            dc.SetParameterValue("@LanguageCode", info.LanguageCode);
            dc.SetParameterValue("@ProductLine", info.ProductLine);
            dc.ExecuteNonQuery();
            
        }


        public bool IsExistsManufacturerRequest(ManufacturerRequestInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IsExistsManufacturerRequest");
            dc.SetParameterValue("@ManufacturerName", info.ManufacturerName);
            dc.SetParameterValue("@ManufacturerStatus", info.ManufacturerStatus);
            dc.SetParameterValue("@ProductLine", info.ProductLine);
            dc.SetParameterValue("@Reasons", info.Reasons);
            dc.SetParameterValue("@ManufacturerBriefName", info.ManufacturerBriefName);
            dc.ExecuteNonQuery();
            return (int)dc.GetParameterValue("@flag") < 0;
        }

        /// <summary>
        /// 检测审核人和创建人是否是同一个人 返回true 是同一个人
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public bool CheckManufacturerUser(int SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CheckManufacturerUser");
            dc.SetParameterValue("@SysNo", SysNo);
            dc.SetParameterValue("@AuditUserSysNo", ServiceContext.Current.UserSysNo);
            dc.ExecuteNonQuery();
            return (int)dc.GetParameterValue("@flag") < 0;
        }

        /// <summary>
        /// 检查是否存在生产商
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CheckIsExistsManufacturer(string localName, string BirName)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CheckIsExistsManufacturer");
            dc.SetParameterValue("@ManufacturerName", localName);
            dc.SetParameterValue("@ManufacturerBriefName", BirName);
            dc.ExecuteNonQuery();
            return (int)dc.GetParameterValue("@flag") < 0;

        }
    }
}
