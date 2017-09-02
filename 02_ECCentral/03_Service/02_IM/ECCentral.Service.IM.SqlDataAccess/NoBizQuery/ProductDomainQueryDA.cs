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
    [VersionExport(typeof(IProductDomainQueryDA))]
    public class ProductDomainQueryDA : IProductDomainQueryDA
    {
        private List<ProductManagerInfo> allPMList;

        public DataTable QueryProductDomainList(ProductDomainFilter filter, out int totalCount)
        {
            if (filter.IsSearchEmptyCategory ?? false)
            {
                return GetEmptyCategoryList(filter, out totalCount);
            }
            if (filter.AsAggregateStyle ?? false)
            {
                return GetAggregateStyleProductDomainList(filter, out totalCount);
            }

            return GetProductDomainList(filter, out totalCount);
        }

        private DataTable GetEmptyCategoryList(ProductDomainFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("ProductDomain_GetEmptyCategoryList");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "C1.[SysNo] DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C2.CompanyCode", DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal,
                    filter.CompanyCode);

                command.CommandText = builder.BuildQuerySql();
                var result = command.ExecuteDataTable();
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return result;
            }          
        }

        private DataTable GetProductDomainList(ProductDomainFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("ProductDomain_GetProductDomainList");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "D.[SysNo] DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.CompanyCode", DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal,
                    filter.CompanyCode);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.SysNo", DbType.Int32,
                    "@ProductDomainSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductDomainSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "D.ProductDomainName", DbType.String,
                   "@ProductDomainName", QueryConditionOperatorType.Like,
                   filter.ProductDomainName);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.BrandSysNo", DbType.Int32,
                    "@BrandSysNo", QueryConditionOperatorType.Equal,
                    filter.BrandSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.C1SysNo", DbType.Int32,
                    "@C1SysNo", QueryConditionOperatorType.Equal,
                    filter.Category1SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.C2SysNo", DbType.Int32,
                    "@C2SysNo", QueryConditionOperatorType.Equal,
                    filter.Category2SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.PMSysNo", DbType.Int32,
                    "@PMSysNo", QueryConditionOperatorType.Equal,
                    filter.PMSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.ProductDomainLeaderUserSysNo", DbType.Int32,
                    "@ProductDomainLeaderUserSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductDomainLeaderUserSysNo);

                command.CommandText = builder.BuildQuerySql();
                DataTable result = command.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(ValidStatus)}                    
                });

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                var list = from e in result.Rows.Cast<DataRow>()
                           select new
                           {
                               SysNo = (int)e["SysNo"]                               
                           };                

                var relatedProductDomains = list.Select(e => e.SysNo);

                var cmd = DataCommandManager.GetDataCommand("ProductDepartment_MerchandiserList");
                cmd.SetParameterValue("@ProductDomainSysNoList", relatedProductDomains.Join(","));

                DataTable dt = cmd.ExecuteDataTable();
                var merchandiserList = from e in dt.Rows.Cast<DataRow>()
                                       select new
                                       {
                                           SysNo = (int)e["SysNo"],
                                           ProductDomainSysNo = (int)e["ProductDomainSysNo"],
                                           MerchandiserSysNo = (int)e["MerchandiserSysNo"]
                                       };

                var realtedMerchandiserList = (from e in merchandiserList
                                               group e by e.ProductDomainSysNo).ToDictionary(e => e.Key, e => e.ToList());

                result.Columns.Add("DepartmentMerchandiserNameList", typeof(string));
                result.Columns.Add("DepartmentMerchandiserSysNoListStr", typeof(string));
                foreach (DataRow row in result.Rows)
                {
                    int sysNo = int.Parse(row["SysNo"].ToString());

                    if (realtedMerchandiserList.ContainsKey(sysNo))
                    {
                        row["DepartmentMerchandiserSysNoListStr"] = realtedMerchandiserList[sysNo].Select(p => p.MerchandiserSysNo).ToList().Join(";");
                        row["DepartmentMerchandiserNameList"] = GetUserNameList(realtedMerchandiserList[sysNo].Select(p => p.MerchandiserSysNo).ToList().Join(";"), filter.CompanyCode);
                    }                   
                    row["BackupUserList"] = GetUserNameList(row["BackupUserList"].ToString(), filter.CompanyCode);
                }                

                return result;
            }
        }

        private DataTable GetAggregateStyleProductDomainList(ProductDomainFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("ProductDomain_GetAggregateStyleProductDomainList");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "[CategorySysNo] DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.CompanyCode", DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal,
                    filter.CompanyCode);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.SysNo", DbType.Int32,
                    "@ProductDomainSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductDomainSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "D.ProductDomainName", DbType.String,
                   "@ProductDomainName", QueryConditionOperatorType.Like,
                   filter.ProductDomainName);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.BrandSysNo", DbType.Int32,
                    "@BrandSysNo", QueryConditionOperatorType.Equal,
                    filter.BrandSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.C1SysNo", DbType.Int32,
                    "@C1SysNo", QueryConditionOperatorType.Equal,
                    filter.Category1SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.C2SysNo", DbType.Int32,
                    "@C2SysNo", QueryConditionOperatorType.Equal,
                    filter.Category2SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.PMSysNo", DbType.Int32,
                    "@PMSysNo", QueryConditionOperatorType.Equal,
                    filter.PMSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "D.ProductDomainLeaderUserSysNo", DbType.Int32,
                    "@ProductDomainLeaderUserSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductDomainLeaderUserSysNo);

                command.CommandText = builder.BuildQuerySql();
                DataTable result = command.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(ValidStatus)}                    
                });
                totalCount = (int)command.GetParameterValue("@TotalCount");

                result.Columns.Add("BrandName", typeof(string));
                //得到当前分页结果中相关的pm c2分组结果， 用该内容找到相应的品牌            

                var list = from e in result.Rows.Cast<DataRow>()
                           select new
                           {
                               DataType = (int)e["DataType"],
                               C2SysNo = (int)e["C2SysNo"],
                               PMSysNo = (int)e["PMSysNo"]
                           };

                var xmlQuery = new XElement("Groups",
                                    from e in list
                                    where e.DataType == 2
                                    select new XElement("Group",
                                        new XElement("PMSysNo", e.PMSysNo),
                                        new XElement("C2SysNo", e.C2SysNo)));

                var cmdQuery = DataCommandManager.GetDataCommand("ProductDomain_GetBrandsByPMAndC2SysNo");
                cmdQuery.SetParameterValue("@xmlQuery", xmlQuery.ToString());

                var ds = cmdQuery.ExecuteDataSet();

                var brands = from e in ds.Tables[0].Rows.Cast<DataRow>()
                             select new
                             {
                                 PMSysNo = (int)e["PMSysNo"],
                                 C2SysNo = (int)e["C2SysNo"],
                                 BrandSysNo = (int)e["BrandSysNo"],
                                 BrandName = e["BrandName"].ToString()
                             };

                foreach (DataRow r in result.Rows)
                {
                    r["BackupUserList"] = GetUserNameList(r["BackupUserList"].ToString(), filter.CompanyCode);

                    if ((int)r["DataType"] == 2)
                    {
                        r["BrandName"] = (from e in brands
                                          where e.PMSysNo == (int)r["PMSysNo"] && e.C2SysNo == (int)r["C2SysNo"]
                                          select e.BrandName).Join(", ");
                    }
                }

                return result;
            }                                                          
        }

        public string GetUserNameList(string strList, string companyCode)
        {
            if (allPMList == null)
            {
                //获取所有的PMList
                allPMList = ObjectFactory<IProductManagerDA>.Instance.GetPMListByType(PMQueryType.All, null, companyCode);
            }

            if (string.IsNullOrEmpty(strList))
            {
                return string.Empty;
            }
            var result = new List<string>();

            foreach (var s in strList.Split(';'))
            {
                var userSysNo = 0;
                
                if (int.TryParse(s, out userSysNo))
                {
                    var pm = allPMList.FirstOrDefault(p => p.SysNo == userSysNo);
                    if (pm != null)
                    {
                        result.Add(pm.UserInfo.UserDisplayName);
                    }
                }
            }
            return String.Join("; ", result.ToArray());
        }
    }
}
