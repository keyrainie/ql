using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBigAreaQueryDA))]
    public class BigAreaQueryDA : IBigAreaQueryDA
    {
        #region IBigAreaQueryDA Members

        public DataSet GetAllBigAreas(BigAreaQueryFilter filter)
        {

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllBigAreas");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd, "SysNo DESC"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                return ds;
            }
        }

        #endregion
    }
}
