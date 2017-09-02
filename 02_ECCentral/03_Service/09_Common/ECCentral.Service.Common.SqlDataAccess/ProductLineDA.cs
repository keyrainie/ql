using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IProductLineDA))]
    public class ProductLineDA : IProductLineDA
    {
        public bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckOperateRightForCurrentUser");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@PMSysNo", pmSysNo);
             
            object obj = cmd.ExecuteScalar(); 
            if (obj != null) 
                if (obj.ToInteger() > 0)
                    return true;
            return false;
        }

        public List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductLineSysNoByProductList");
            DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd,"productsysno");
            builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "p.sysNo", System.Data.DbType.Int32, productSysNo.ToList<int>());
            
            cmd.CommandText = builder.BuildQuerySql();

            return cmd.ExecuteEntityList<ProductPMLine>();
        }
        
        public List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductLineInfoByPM");
            cmd.SetParameterValue("@PMSysNo", pmSysNo);

            return cmd.ExecuteEntityList<ProductPMLine>();
        }
    }
}
