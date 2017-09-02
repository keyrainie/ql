using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.WPMessage.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.WPMessage.BizEntity;
using System.Data;
using ECCentral.WPMessage.QueryFilter;

namespace ECCentral.Service.WPMessage.SqlDataAccess
{
    [VersionExport(typeof(IWPMessageDA))]
    public class WPMessageDA : IWPMessageDA
    {
        #region 待办事项相关处理
        public ECCentral.WPMessage.BizEntity.WPMessage AddWPMessage(ECCentral.WPMessage.BizEntity.WPMessage msg)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_InsertMessage");
            command.SetParameterValue(msg);
            msg.SysNo = command.ExecuteScalar<int>();
            return msg;
        }

        public List<ECCentral.WPMessage.BizEntity.WPMessage> GetWPMessage(int categorySysNo, string bizSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_GetMessageByBizSysNo");
            command.SetParameterValue("@CategorySysNo", categorySysNo);
            command.SetParameterValue("@BizSysNo", bizSysNo);
            return command.ExecuteEntityList<ECCentral.WPMessage.BizEntity.WPMessage>();

        }
        public void UpdateWPMessageToProcessing(int msgSysNo, int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_UpdateMessageToProcessingBySysNo");
            command.SetParameterValue("@SysNo", msgSysNo);
            command.SetParameterValue("@ProcessUserSysNo", userSysNo);
            command.ExecuteNonQuery();
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo, string memo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_CompleteMessageByCategorySysNoAndBizSysNo");
            command.SetParameterValue("@CategorySysNo", categorySysNo);
            command.SetParameterValue("@BizSysNo", bizSysNo);
            if (memo == null)
            {
                command.SetParameterValue("@Memo", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@Memo", memo);
            }
            command.SetParameterValue("@CompletedUserSysNo", userSysNo);
            command.ExecuteNonQuery();
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo)
        {
            CompleteWPMessage(categorySysNo, bizSysNo, userSysNo);
        }
        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (tsort[0].ToUpper())
                {
                    case "SYSNO":
                        tsort[0] = "m.SysNo";
                        break;
                    case "CATEGORYSYSNO":
                        tsort[0] = "m.CategorySysNo";
                        break;
                    case "CREATETIME":
                        tsort[0] = "m.CreateTime";
                        break;
                    case "STATUS":
                        tsort[0] = "m.[Status]";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            else
            {
                sortField = "m.CategorySysNo DESC,m.SysNo";
            }
            return sortField;
        }
        private PagingInfoEntity ToPagingInfo(int pageIndex, int pageSize, string sortField)
        {
            return new PagingInfoEntity()
            {
                SortField = SortFieldMapping(sortField),
                StartRowIndex = pageIndex * pageSize,
                MaximumRows = pageSize
            };
        }
        public DataTable QueryWPMessageByUserSysNo(WPMessageQueryFilter filter, out int dataCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("WPMessage_QueryMessage");
            using (DynamicQuerySqlBuilder queryBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, ToPagingInfo(filter.PageIndex, filter.PageSize, filter.SortField), "m.CategorySysNo DESC,m.SysNo"))
            {
                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "m.[CreateTime]", DbType.DateTime, "@BeginCreateTime", QueryConditionOperatorType.MoreThanOrEqual, filter.BeginCreateTime);
                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "m.[CreateTime]", DbType.DateTime, "@EndCreateTime", QueryConditionOperatorType.LessThan, filter.EndCreateTime);
                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "m.[CategorySysNo]", DbType.Int32, "@CategorySysNo", QueryConditionOperatorType.Equal, filter.CategorySysNo);
                if (filter.WPMessageStatus.HasValue)
                {
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "m.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.WPMessageStatus);
                }
                else
                {
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "m.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.NotEqual, WPMessageStatus.Completed);
                }
                queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " wc.[Status] = 1");

                command.CommandText = queryBuilder.BuildQuerySql();
            }
            command.SetParameterValue("@UserSysNo", filter.UserSysNo);
            DataTable dt = command.ExecuteDataTable();

            object o = command.GetParameterValue("@TotalCount");
            dataCount = o == null ? 0 : (int)o;
            return dt;
        }

        public bool HasWPMessageByUserSysNo(int userSysNo)
        {
            int dataCount;
            QueryWPMessageByUserSysNo(new WPMessageQueryFilter
           {
               UserSysNo = userSysNo,
               PageIndex = 0,
               PageSize = 1
           }, out dataCount);
            return dataCount > 0;
        }
        #endregion

        #region 待办事项类型相关
        public List<WPMessageCategory> GetAllCategory()
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_GetAllCategory");
            return command.ExecuteEntityList<WPMessageCategory>();
        }
        public List<WPMessageCategory> GetCategoryByUserSysNo(int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_GetCategoryByUserSysNo");
            command.SetParameterValue("@UserSysNo", userSysNo);
            return command.ExecuteEntityList<WPMessageCategory>();
        }

        public void UpdateCategoryRoleByCategorySysNo(int categorySysNo, List<int> roleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_UpdateCategoryRoleByCategorySysNo");
            command.SetParameterValue("@CategorySysNo", categorySysNo);

            StringBuilder xmlBuilder = new StringBuilder("<r>");
            if (roleSysNo != null && roleSysNo.Count > 0)
            {
                foreach (int n in roleSysNo)
                {
                    xmlBuilder.AppendFormat("<no>{0}</no>", n);
                }
            }
            xmlBuilder.Append("</r>");
            command.SetParameterValue("@Role", xmlBuilder.ToString());
            command.ExecuteNonQuery();
        }

        public List<int> GetRoleSysNoByCategorySysNo(int categorySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_GetRoleSysNoByCategorySysNo");
            command.SetParameterValue("@CategorySysNo", categorySysNo);
            DataTable dt = command.ExecuteDataTable();
            List<int> roleSysNo = new List<int>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    roleSysNo.Add((int)dr[0]);
                }
            }
            return roleSysNo;
        }

        public List<int> GetCategorySysNoByRoleSysNo(int roleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_GetCategorySysNoByRoleSysNo");
            command.SetParameterValue("@RoleSysNo", roleSysNo);
            DataTable dt = command.ExecuteDataTable();
            List<int> list = new List<int>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add((int)dr[0]);
                }
            }
            return list;
        }

        public void UpdateCategoryRoleByRoleSysNo(int roleSysNo, List<int> categorySysNoList)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WPMessage_UpdateCategoryRoleByRoleSysNo");
            command.SetParameterValue("@RoleSysNo", roleSysNo);

            StringBuilder xmlBuilder = new StringBuilder("<r>");
            if (categorySysNoList != null && categorySysNoList.Count > 0)
            {
                foreach (int n in categorySysNoList)
                {
                    xmlBuilder.AppendFormat("<no>{0}</no>", n);
                }
            }
            xmlBuilder.Append("</r>");
            command.SetParameterValue("@Category", xmlBuilder.ToString());
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
