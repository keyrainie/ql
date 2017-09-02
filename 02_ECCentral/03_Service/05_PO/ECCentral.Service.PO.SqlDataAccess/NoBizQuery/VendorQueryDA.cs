using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IVendorQueryDA))]
    public class VendorQueryDA : IVendorQueryDA
    {
        #region IVendorQueryDA Members

        public System.Data.DataTable QueryVendorList(QueryFilter.PO.VendorQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = -1;
                return null;
            }
            CustomDataCommand dataCommand = null;
            if (queryFilter.RANKStatus == VendorRankStatus.AuditWaiting)
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorListByLeverRequest");
            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorList");
            }
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "V.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(queryFilter.InvoiceType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ex.InvoiceType",
                           DbType.AnsiStringFixedLength, "@InvoiceType", QueryConditionOperatorType.Equal, queryFilter.InvoiceType);
                }
                if (!string.IsNullOrEmpty(queryFilter.StockType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ex.StockType",
                    DbType.AnsiStringFixedLength, "@StockType", QueryConditionOperatorType.Equal, queryFilter.StockType);
                }

                if (!string.IsNullOrEmpty(queryFilter.ShippingType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ex.ShippingType",
                    DbType.AnsiStringFixedLength, "@ShippingType", QueryConditionOperatorType.Equal, queryFilter.ShippingType);
                }

                if (queryFilter.IsConsign.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.IsConsign",
                    DbType.Int32, "@IsConsign", QueryConditionOperatorType.Equal, queryFilter.IsConsign);
                }

                if (!string.IsNullOrEmpty(queryFilter.Account))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.Account",
                    DbType.String, "@Account", QueryConditionOperatorType.Equal, queryFilter.Account);
                }

                if (queryFilter.VendorSysNo.HasValue)
                {
                    //临时修改，支持在供应商编号输入VendorID查询， Ocean
                    string SysNoList = queryFilter.VendorSysNo.Value.ToString();
                    if (queryFilter.VendorSysNo.Value < 1000000)
                    {
                        int newSysNo = queryFilter.VendorSysNo.Value + 1000000;
                        SysNoList = SysNoList + ',' + newSysNo.ToString();
                    }

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "v.SysNo", QueryConditionOperatorType.In, SysNoList);

                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.SysNo",
                    //DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }
                if (!string.IsNullOrEmpty(queryFilter.VendorName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.VendorName",
                    DbType.String, "@VendorName", QueryConditionOperatorType.Like, queryFilter.VendorName);
                }

                if (!string.IsNullOrEmpty(queryFilter.Address))
                {
                    queryFilter.Address.Replace("'", "’");
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.Address",
                    DbType.String, "@Address", QueryConditionOperatorType.LeftLike, queryFilter.Address.Trim());
                }

                if (!string.IsNullOrEmpty(queryFilter.Contact))
                {
                    queryFilter.Contact.Replace("'", "’");
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.Contact",
                    DbType.String, "@Contact", QueryConditionOperatorType.LeftLike, queryFilter.Contact.Trim());
                }
                if (!string.IsNullOrEmpty(queryFilter.Phone))
                {
                    queryFilter.Phone.Replace("'", "’");
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.Phone",
                    DbType.String, "@Phone", QueryConditionOperatorType.LeftLike, queryFilter.Phone.Trim());
                }
                if (queryFilter.Status != null && queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status);
                }
                if (!string.IsNullOrEmpty(queryFilter.RANK))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.RANK",
                        DbType.String, "@RANK", QueryConditionOperatorType.Equal, queryFilter.RANK.Trim());
                }
                //合作起止日期
                if (queryFilter.ExpiredDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.ExpiredDate",
                    DbType.DateTime, "@ExpiredDateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.ExpiredDateFrom.Value);
                }
                if (queryFilter.ExpiredDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.ExpiredDate",
                        DbType.DateTime, "@ExpiredDateTo", QueryConditionOperatorType.LessThanOrEqual, queryFilter.ExpiredDateTo.Value);
                }
                if (queryFilter.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, @"( SELECT top 1 f.Status
					 FROM IPP3.dbo.Vendor_ModifyRequest f  WITH(NOLOCK)
					 WHERE f.VendorSysNo =V.sysno	AND   f.RequestType = 0		ORDER  BY  sysno DESC)",
                        DbType.Int32, "@RequestStatus", QueryConditionOperatorType.Equal, queryFilter.RequestStatus.Value);
                }

                if (queryFilter.VendorType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.VendorType", System.Data.DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, queryFilter.VendorType);
                }
                if (queryFilter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.PaySettleCompany", System.Data.DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, queryFilter.PaySettleCompany.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                if(queryFilter.EPortSysNo.HasValue&& queryFilter.EPortSysNo>0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.[ePortSysNo]", System.Data.DbType.Int32, "@EPortSysNo", QueryConditionOperatorType.Equal, queryFilter.EPortSysNo.Value);
                }

                #region [ //代理品牌 二级类 三级类 代理级别 ]

                if ((queryFilter.ManufacturerSysNo != null && queryFilter.ManufacturerSysNo.Trim() != "")
                    || (queryFilter.AgentLevel != null && queryFilter.AgentLevel.Trim() != "")
                    || (queryFilter.C1SysNo != null && queryFilter.C1SysNo.Trim() != ""))
                {
                    string subCond = @"SELECT distinct vendorsysno
                                   FROM IPP3.dbo.Vendor_Manufacturer  WITH(NOLOCK) ";

                    string strWhere = string.Empty;

                    //代理品牌 二级类 三级类 代理级别 
                    if (!string.IsNullOrEmpty(queryFilter.ManufacturerSysNo))
                    {
                        strWhere += "ManufacturerSysNo=" + queryFilter.ManufacturerSysNo.Trim();
                    }
                    if (!string.IsNullOrEmpty(queryFilter.AgentLevel))
                    {
                        if (strWhere != string.Empty)
                        {
                            strWhere += " AND ";
                        }
                        strWhere += "AgentLevel='" + queryFilter.AgentLevel.Trim() + "'";
                    }
                    if (!string.IsNullOrEmpty(queryFilter.C2SysNo))
                    {
                        if (strWhere != string.Empty)
                        {
                            strWhere += " AND ";
                        }
                        strWhere += "C2SysNo=" + queryFilter.C2SysNo.Trim();
                    }
                    if (!string.IsNullOrEmpty(queryFilter.C3SysNo))
                    {
                        if (strWhere != string.Empty)
                        {
                            strWhere += " AND ";
                        }
                        strWhere += "C3SysNo=" + queryFilter.C3SysNo.Trim();
                    }
                    if (!string.IsNullOrEmpty(strWhere))
                    {
                        subCond += "WHERE " + strWhere;
                    }
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "V.SysNo",
                            QueryConditionOperatorType.In, subCond);
                }
                #endregion
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                dataCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                dataCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                EnumColumnList columnEnums = new EnumColumnList();
                columnEnums.Add(19, typeof(VendorStatus));
                columnEnums.Add(30, typeof(VendorConsignFlag));
                columnEnums.Add("AuditStatus", typeof(VendorFinanceRequestStatus));
                columnEnums.Add("PaySettleCompany", typeof(PaySettleCompany));
                DataTable dt = dataCommand.ExecuteDataTable(columnEnums);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }   


        }

        public DataTable QueryVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorPayBalanceByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            return command.ExecuteDataTable();
        }

        public DataTable QueryStorePageType()
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryStorePageType");
            return command.ExecuteDataTable();
        }

        public DataTable QueryStorePageInfo(StorePageQueryFilter filter,out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStorePageInfo");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PageInfo.SortBy,
                StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "sp.SysNo desc"))
            {
                if (filter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sp.SellerSysNo",
                    DbType.Int32, "@SellerSysNo", QueryConditionOperatorType.Equal, filter.MerchantSysNo.Value);
                }

                if (!string.IsNullOrEmpty(filter.PageType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sp.PageTypeKey",
                        DbType.String, "@PageTypeKey", QueryConditionOperatorType.Equal, filter.PageType);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public void DeleteStorePageInfo(int SysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("BatchDeleteStorePageInfo");
            dataCommand.SetParameterValue("@SysNo", SysNo);
            dataCommand.ExecuteNonQuery();
        }

        public void CheckStorePageInfo(int SysNo, int Status)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("CheckStorePageInfo");
            dataCommand.SetParameterValue("@SysNo", SysNo);
            dataCommand.SetParameterValue("@Status", Status);
            dataCommand.ExecuteNonQuery();
        }

        #endregion

        #region IVendorQueryDA Members


        public DataTable QueryCanLockPMListByVendorSysNo(VendorQueryFilter queryFilter)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetCanLockPMListByVendorSysNo");
            dataCommand.SetParameterValue("@VendorSysno", queryFilter.VendorSysNo.Value);
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            return dataCommand.ExecuteDataTable();
        }

        #endregion

        #region IVendorQueryDA Members


        public DataTable QueryVendorPMHoldInfoByVendorSysNo(VendorQueryFilter queryFilter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorPMHoldInfoByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", queryFilter.VendorSysNo.Value);
            command.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            return command.ExecuteDataTable();
        }

        #endregion
    }
}
