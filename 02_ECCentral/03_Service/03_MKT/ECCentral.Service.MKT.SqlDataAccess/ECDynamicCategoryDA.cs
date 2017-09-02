using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IECDynamicCategoryDA))]
    public class ECDynamicCategoryDA : IECDynamicCategoryDA
    {
        public ECDynamicCategory Create(ECDynamicCategory info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_CreateDynamicCategory");
            cmd.SetParameterValue(info);

            cmd.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return info;
        }

        public void Update(ECDynamicCategory info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_UpdateDynamicCategory");
            cmd.SetParameterValue(info);
            
            cmd.ExecuteNonQuery();
        }

        public void Delete(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_DeleteDynamicCategory");
            cmd.SetParameterValue("@SysNo",sysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }

        public void DeleteCategoryMapping(int dynamicCategorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_DeleteCategoryMapping");
            cmd.SetParameterValue("@DynamicCategorySysNo", dynamicCategorySysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");
            cmd.ExecuteNonQuery();
        }

        //验证同级分类名称是否重复
        public bool CheckNameDuplicate(string name, int excludeSysNo, int level, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_CheckDynamicCategoryNameDuplicate");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@CategoryName", name);
            cmd.SetParameterValue("@Level", level);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加渠道参数

            return cmd.ExecuteScalar<int>() > 0;
        }

        public bool CheckSubCategoryExists(int dynamicCategorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_CheckSubCategoryExists");
            cmd.SetParameterValue("@DynamicCategorySysNo", dynamicCategorySysNo);                      

            return cmd.ExecuteScalar<int>() > 0;
        }

        public List<ECDynamicCategory> GetDynamicCategories(DynamicCategoryStatus? status, DynamicCategoryType? categoryType)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("MKT_GetDynamicCategories");
            using(DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd,"SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition( QueryConditionRelationType.AND, "Status", System.Data.DbType.Int32,"@Status", QueryConditionOperatorType.Equal,status);
                builder.ConditionConstructor.AddCondition( QueryConditionRelationType.AND, "CategoryType", System.Data.DbType.Int32,"@CategoryType", QueryConditionOperatorType.Equal,categoryType);                
                cmd.CommandText = builder.BuildQuerySql();
                return cmd.ExecuteEntityList<ECDynamicCategory>();
            }                       
        }

        public bool ExistCategoryProductMapping(int dynamicCategorySysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_ExistCategoryProductMapping");
            cmd.SetParameterValue("@DynamicCategorySysNo", dynamicCategorySysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<int>() > 0;
        }

        public void InsertCategoryProductMapping(int dynamicCategorySysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_InsertCategoryProductMapping");
            cmd.SetParameterValue("@DynamicCategorySysNo", dynamicCategorySysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@InUser");
            cmd.ExecuteNonQuery();
        }

        public void DeleteCategoryProductMapping(int dynamicCategorySysNo, int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("MKT_DeleteCategoryProductMapping");

            cmd.SetParameterValue("@DynamicCategorySysNo", dynamicCategorySysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }
    }
}
