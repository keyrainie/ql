using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NeweggCN.NoBizQuery
{
    [VersionExport(typeof(IComputerConfigQueryDA))]
    public class ComputerConfigQueryDA : IComputerConfigQueryDA
    {
        #region IComputerConfigQueryDA Members

        public System.Data.DataTable QueryMaster(ComputerConfigQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetComputerConfigMasterList");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "M.SysNo DESC");

            sqlBuilder.ConditionConstructor.AddCondition(
          QueryConditionRelationType.AND,
          "M.SysNo",
          DbType.Int32,
          "@SysNo",
          QueryConditionOperatorType.Equal,
        filter.SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
        QueryConditionRelationType.AND,
        "M.ComputerConfigName",
        DbType.String,
        "@ComputerConfigName",
        QueryConditionOperatorType.Like,
        filter.ComputerConfigName);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "M.ComputerConfigTypeSysNo",
           DbType.Int32,
           "@ComputerConfigTypeSysNo",
           QueryConditionOperatorType.Equal,
         filter.ComputerConfigType);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "M.Status",
           DbType.AnsiStringFixedLength,
           "@Status",
           QueryConditionOperatorType.Equal,
         filter.Status);

            #region 过滤前台删除状态C 的数据，后台不对此状态数据做处理
            sqlBuilder.ConditionConstructor.AddCondition(
            QueryConditionRelationType.AND,
            "M.Status",
            DbType.AnsiStringFixedLength,
            "@ForeStatus",
            QueryConditionOperatorType.NotEqual,
            "C");
            #endregion

            if (filter.Owner.HasValue)
            {
                //根据Owner确定查询操作符
                QueryConditionOperatorType ownerConditionOperator = QueryConditionOperatorType.LessThanOrEqual;

                //客户
                if (filter.Owner == ComputerConfigOwner.Customer)
                {
                    ownerConditionOperator = QueryConditionOperatorType.MoreThan;
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                      QueryConditionRelationType.AND,
                      "M.CustomerSysNo",
                      DbType.Int32,
                      "@CustomerSysNo",
                      ownerConditionOperator,
                    0);
            }

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "VCC.BaseTotalPrice",
           DbType.Decimal,
           "@MinPriceRange",
           QueryConditionOperatorType.MoreThanOrEqual,
         filter.MinPriceRange);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "VCC.BaseTotalPrice",
           DbType.Decimal,
           "@MaxPriceRange",
           QueryConditionOperatorType.LessThanOrEqual,
         filter.MaxPriceRange);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "M.Priority",
           DbType.Int32,
           "@Priority",
           QueryConditionOperatorType.Equal,
         filter.Priority);

            sqlBuilder.ConditionConstructor.AddCondition(
        QueryConditionRelationType.AND,
        "M.EditUser",
        DbType.AnsiString,
        "@EditUser",
        QueryConditionOperatorType.Like,
        filter.EditUser);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "M.CompanyCode",
           DbType.AnsiStringFixedLength,
           "@CompanyCode",
           QueryConditionOperatorType.Equal,
         filter.CompanyCode);
            //TODO:添加ChannelID参数

            cmd.CommandText = sqlBuilder.BuildQuerySql();

            //构造商品系统编号，商品编号查询参数
            string strWhere = "";
            if (filter.ProductSysNo.HasValue)
            {
                strWhere += " and Product.SysNo=@ProductSysNo ";
                cmd.AddInputParameter("@ProductSysNo", DbType.Int32, filter.ProductSysNo);
            }
            if (!string.IsNullOrWhiteSpace(filter.ProductID))
            {
                strWhere += " and Product.ProductID=@ProductID";
                cmd.AddInputParameter("@ProductID", DbType.AnsiString, filter.ProductID);
            }
            cmd.ReplaceParameterValue("#StrWhere_Product#", strWhere);

            EnumColumnList enumConfig = new EnumColumnList();
            enumConfig.Add("Status", typeof(ComputerConfigStatus));
            var dt = cmd.ExecuteDataTable(enumConfig);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        #endregion


        public List<ComputerConfigMaster> GetComputerConfigMasterList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigList");
            var computerConfigList = cmd.ExecuteEntityList<ComputerConfigMaster>();
            foreach (var computerConfig in computerConfigList)
            {
                cmd = DataCommandManager.GetDataCommand("GetComputerConfigItemList");
                cmd.SetParameterValue("@ComputerConfigMasterSysNo", computerConfig.SysNo.Value);
                computerConfig.ConfigItemList = cmd.ExecuteEntityList<ComputerConfigItem>();
            }
            return computerConfigList;
        }
    }
}
