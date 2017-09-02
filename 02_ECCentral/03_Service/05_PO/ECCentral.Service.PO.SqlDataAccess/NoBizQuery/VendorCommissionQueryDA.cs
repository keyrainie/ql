using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICommissionQueryDA))]
    public class VendorCommissionQueryDA : ICommissionQueryDA
    {
        #region ICommissionQueryDA Members

        public System.Data.DataTable QueryCommission(QueryFilter.PO.CommissionQueryFilter queryFilter, out int totalCount, out decimal totalAmt)
        {
            DataTable dt = new DataTable();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("SearchCommission");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            string whereSql = "WHERE 1=1";

            if (queryFilter.SysNo.HasValue)
            {
                dataCommand.AddInputParameter("@SysNo", DbType.Int32, queryFilter.SysNo.Value);
                whereSql += " AND commMaster.SysNo = @SysNo";
            }

            if (queryFilter.VendorSysNo.HasValue)
            {
                dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, queryFilter.VendorSysNo.Value);
                whereSql += " AND commMaster.MerchantSysNo = @VendorSysNo";
            }

            //InDate
            if (queryFilter.InDateBegin.HasValue)
            {
                whereSql += " AND commMaster.InDate >= @InDateBegin";
                dataCommand.AddInputParameter("@InDateBegin", DbType.DateTime, queryFilter.InDateBegin.Value);
            }
            if (queryFilter.InDateEnd.HasValue)
            {
                whereSql += " AND commMaster.InDate <= @InDateEnd";
                dataCommand.AddInputParameter("@InDateEnd", DbType.DateTime, queryFilter.InDateEnd.Value.AddDays(1));
            }
            //OutListDate出单时间
            if (queryFilter.OutListDateBegin.HasValue)
            {
                whereSql += " AND commMaster.EndDate >= @OutListDateBegin";
                dataCommand.AddInputParameter("@OutListDateBegin", DbType.DateTime, queryFilter.OutListDateBegin.Value);
            }
            if (queryFilter.OutListDateEnd.HasValue)
            {
                whereSql += " AND commMaster.EndDate <= @OutListDateEnd";
                dataCommand.AddInputParameter("@OutListDateEnd", DbType.DateTime, queryFilter.OutListDateEnd.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(queryFilter.CompanyCode))
            {
                whereSql += " AND commMaster.CompanyCode = @CompanyCode ";
                dataCommand.AddInputParameter("@CompanyCode", DbType.String, queryFilter.CompanyCode);

            }
            if (queryFilter.PageInfo != null)
            {
                dataCommand.AddInputParameter("@StartNumber", DbType.Int32, queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                dataCommand.AddInputParameter("@EndNumber", DbType.Int32, queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                dataCommand.AddOutParameter("@TotalCount", DbType.Int32, 4);
                dataCommand.AddOutParameter("@TotalAmt", DbType.String, 10);
            }

            dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere#", whereSql);

            EnumColumnList list = new EnumColumnList();
            list.Add("Status", typeof(VendorCommissionMasterStatus));
            dt = dataCommand.ExecuteDataTable(list);

            totalAmt = Convert.ToDecimal(dataCommand.GetParameterValue("@TotalAmt"));
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            return dt;
        }

        public decimal QueryCommissionTotalAmt(QueryFilter.PO.CommissionQueryFilter queryFilter)
        {
            DataTable dt = new DataTable();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("SearchCommissionTotalAmt");

            string whereSql = "WHERE 1=1";

            if (queryFilter.SysNo.HasValue)
            {
                dataCommand.AddInputParameter("@SysNo", DbType.Int32, queryFilter.SysNo.Value);
                whereSql += " AND commMaster.SysNo = @SysNo";
            }

            if (queryFilter.VendorSysNo.HasValue)
            {
                dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, queryFilter.VendorSysNo.Value);
                whereSql += " AND commMaster.MerchantSysNo = @VendorSysNo";
            }

            //InDate
            if (queryFilter.InDateBegin.HasValue)
            {
                whereSql += " AND commMaster.InDate >= @InDateBegin";
                dataCommand.AddInputParameter("@InDateBegin", DbType.DateTime, queryFilter.InDateBegin.Value);
            }
            if (queryFilter.InDateEnd.HasValue)
            {
                whereSql += " AND commMaster.InDate <= @InDateEnd";
                dataCommand.AddInputParameter("@InDateEnd", DbType.DateTime, queryFilter.InDateEnd.Value.AddDays(1));
            }
            //OutListDate出单时间
            if (queryFilter.OutListDateBegin.HasValue)
            {
                whereSql += " AND commMaster.EndDate >= @OutListDateBegin";
                dataCommand.AddInputParameter("@OutListDateBegin", DbType.DateTime, queryFilter.OutListDateBegin.Value);
            }
            if (queryFilter.OutListDateEnd.HasValue)
            {
                whereSql += " AND commMaster.EndDate <= @OutListDateEnd";
                dataCommand.AddInputParameter("@OutListDateEnd", DbType.DateTime, queryFilter.OutListDateEnd.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(queryFilter.CompanyCode))
            {
                whereSql += " AND commMaster.CompanyCode = @CompanyCode ";
                dataCommand.AddInputParameter("@CompanyCode", DbType.String, queryFilter.CompanyCode);

            }
            if (queryFilter.PageInfo != null)
            {
                dataCommand.AddOutParameter("@TotalCount", DbType.Int32, 4);
                dataCommand.AddOutParameter("@TotalAmt", DbType.String, 10);
            }

            dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere#", whereSql);

            EnumColumnList list = new EnumColumnList();
            list.Add("Status", typeof(VendorCommissionMasterStatus));
            dataCommand.ExecuteScalar();

            decimal totalAmt = Convert.ToDecimal(dataCommand.GetParameterValue("@TotalAmt"));
            return totalAmt;
        }

        public DataTable QueryCommissionRules(QueryFilter.PO.CommissionQueryFilter queryFilter, out int totalCount)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
