using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IOrderCheckMasterDA))]
    public class OrderCheckMasterDA : IOrderCheckMasterDA
    {
        public virtual OrderCheckMaster Creat(OrderCheckMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertOrderCheckMaster");
            cmd.SetParameterValue<OrderCheckMaster>(entity);
            cmd.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }
        public virtual void UpdateOrderCheckMasterAllDisable(OrderCheckMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateOrderCheckMasterAllDisable");
            cmd.SetParameterValue("@Status", 1);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.ExecuteNonQuery();
        }

        public virtual List<OrderCheckMaster> GetCurrentOrderCheckMasterList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOrderCheckMaster");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<OrderCheckMaster>();
        }

        public virtual void UpdateOrderCheckMaster(OrderCheckMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateOrderCheckMaster");
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.ExecuteNonQuery();
        }
    }


    [VersionExport(typeof(IOrderCheckItemDA))]
    public class OrderCheckItemDA : IOrderCheckItemDA
    {

        #region IOrderCheckItemDA Members

        public virtual OrderCheckItem Creat(OrderCheckItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertOrderCheckItem");
            cmd.SetParameterValue<OrderCheckItem>(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }
        public virtual void UpdateOrderCheckItem(OrderCheckItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateOrderCheckItem");
            cmd.SetParameterValue<OrderCheckItem>(entity);
            cmd.ExecuteNonQuery();
        }
        public virtual int DeleteOrderCheckItem(OrderCheckItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteOrderCheckItemByType");
            cmd.SetParameterValue("@ReferenceType", entity.ReferenceType);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteNonQuery();
        }
        public virtual int GetSACount(OrderCheckItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSACount");
            cmd.SetParameterValue("@BeginDate", DateTime.Parse(entity.ReferenceContent));
            cmd.SetParameterValue("@EndDate", DateTime.Parse(entity.Description));
            object o = cmd.ExecuteScalar();
            if (o != null && o != DBNull.Value)
            {
                return Convert.ToInt32(o);
            }

            return 1;
        }
        public virtual List<OrderCheckItem> GetOrderCheckItemByQuery(OrderCheckItem queryEntity)
        {
            if (queryEntity == null) return null;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetOrderCheckItem");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                cmd.CommandText, cmd,
                new PagingInfoEntity
                {
                    StartRowIndex = 0,
                    MaximumRows = int.MaxValue
                },
                "a.SysNo DESC"))
            {
                if (!string.IsNullOrEmpty(queryEntity.ReferenceTypeIn) &&
                    string.Compare(queryEntity.ReferenceTypeIn, "'DT11','DT12'") == 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.sysno",
                    DbType.Int32, "@sysno",
                    QueryConditionOperatorType.Equal,
                    queryEntity.SysNo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ReferenceType",
                    DbType.String, "@ReferenceType",
                    QueryConditionOperatorType.Equal,
                    queryEntity.ReferenceType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ReferenceContent",
                    DbType.String, "@ReferenceContent",
                    QueryConditionOperatorType.Equal,
                    queryEntity.ReferenceContent);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.Equal,
                    queryEntity.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Description",
                    DbType.String, "@Description",
                    QueryConditionOperatorType.Equal,
                    queryEntity.Description);

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "a.ReferenceType",
                    QueryConditionOperatorType.In,
                    queryEntity.ReferenceTypeIn);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                return cmd.ExecuteEntityList<OrderCheckItem>();
            }
        }
        #endregion

        #region IOrderCheckItemDA Members


        public List<OrderCheckItem> GetOrderCheckItem(string checkType, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOrderCheckItems");
            cmd.SetParameterValue("@CheckType", checkType);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<OrderCheckItem>();
        }

        #endregion
    }
}
