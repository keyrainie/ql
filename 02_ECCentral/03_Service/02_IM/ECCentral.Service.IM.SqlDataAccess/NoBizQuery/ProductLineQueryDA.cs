using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility.DataAccess;
using System.Xml.Linq;

using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductLineQueryDA))]
    public class ProductLineQueryDA : IProductLineQueryDA
    {

        #region IProductLineQueryDA Members

        public List<ProductManagerInfo> GetUserNameList(string strList)
        {
            if(string.IsNullOrEmpty(strList))
            {
                strList = null;
            }
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetPMBySysNos");
            cmd.SetParameterValue("@PMSysNoList", strList);
            return cmd.ExecuteEntityList<ProductManagerInfo>();
        }

        public DataTable QueryProductLineList(ProductLineFilter filter, out int totalCount) 
        {
            if (filter.IsSearchEmptyCategory.HasValue && filter.IsSearchEmptyCategory.Value)
            {
                return QueryEmptyCategory2List(filter, out totalCount);
            }
            else 
            {
                return Query(filter,out totalCount);
            }
        }

        private DataTable Query(ProductLineFilter filter, out int totalCount)
        {
            string pmrangetype = string.Empty;
            if (filter.PMRangeType.HasValue)
            {
                pmrangetype=filter.PMRangeType.ToString();
            }

            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetList");
            cmd.SetParameterValue("@SortField",filter.PagingInfo.SortBy);
            cmd.SetParameterValue("@PageSize",filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent",filter.PagingInfo.PageIndex);
            cmd.SetParameterValue("@CompanyCode",filter.CompanyCode);
            
            cmd.SetParameterValue("@BrandSysNo",filter.BrandSysNo);
            cmd.SetParameterValue("@C1SysNo",filter.C1SysNo);
            cmd.SetParameterValue("@C2SysNo",filter.C2SysNo);
            cmd.SetParameterValue("@PMSysNo",filter.PMUserSysNo);
            cmd.SetParameterValue("@PMRangeType", pmrangetype);
            DataTable table = cmd.ExecuteDataTable();
            table.Columns.Add("BackupPMNameList", typeof(string));
            table.Columns.Add("MerchandiserName",typeof(string));
            foreach (DataRow row in table.Rows)
            {
                row["MerchandiserName"] = GetUserNameList(row["MerchandiserSysNo"].ToString())[0].UserInfo.UserDisplayName;

                string bakpmstring = string.Format("{0}", row["BackupPMSysNoList"]);
                if (string.IsNullOrEmpty(bakpmstring)) 
                {
                    continue;
                }
                string sysnolist = string.Format("{0}", row["BackupPMSysNoList"]).Replace(';', ',');
                List<string> bakpm = bakpmstring.Split(';').ToList<string>();

                int result = 0;
                bakpm = bakpm.Where(p =>(!string.IsNullOrEmpty(p)&&int.TryParse(p,out result))).ToList<string>();

                List<string> pms = new List<string>();
                GetUserNameList(bakpm.Join(",")).ForEach(pm =>
                {
                    pms.Add(pm.UserInfo.UserDisplayName);
                });
                row["BackupPMNameList"] = pms.Join(";");
                //row["MerchandiserSysNo"] = bakpm.Join(";");
            }
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return table;
        }

        private DataTable QueryEmptyCategory2List(ProductLineFilter filter, out int totalCount) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetEmptyCategory2List");
            cmd.SetParameterValue("@SortField", filter.PagingInfo.SortBy);
            cmd.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);
            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);

            cmd.SetParameterValue("@BrandSysNo", filter.BrandSysNo);
            cmd.SetParameterValue("@C1SysNo", filter.C1SysNo);
            cmd.SetParameterValue("@C2SysNo", filter.C2SysNo);
            cmd.SetParameterValue("@PMSysNo", filter.PMUserSysNo);

            DataTable table = cmd.ExecuteDataTable();

            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return table;
        }
        #endregion
    }
}
