using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess
{
    public static class KeystoneDA
    {
        /// <summary>
        /// 根据角色名称获取授权用户
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="applicationId">应用程序编号</param>
        /// <returns></returns>
        public static List<AuthUser> GetAuthUserByRoleName(string roleName, string applicationId)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetAuthUserByRoleName");

            dataCommand.SetParameterValue("@ApplicationId", applicationId);
            dataCommand.SetParameterValue("@RoleName", roleName);

            return dataCommand.ExecuteEntityList<AuthUser>();
        }

        /// <summary>
        /// 根据FunctionName获取相应的授权用户
        /// </summary>
        /// <param name="functionList">FunctionName列表</param>
        /// <param name="applicationId">应用程序编号</param>
        /// <returns></returns>
        public static List<AuthUser> GetAuthUserByFunctionName(List<string> functionList, string applicationId)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAuthUserByFunctionName");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "URS.unique_identifier ASC"))
            {
                string sqlTemplate = @"(EXISTS
                                        ( 
                                        SELECT UR2.user_id FROM role_functional_ability AS RF2 WITH (NOLOCK) 
                                        INNER JOIN dbo.user_role AS UR2 WITH(NOLOCK) ON RF2.role_id = UR2.role_id 
                                        INNER JOIN dbo.functional_ability AS F2 WITH(NOLOCK) ON RF2.functional_ability_id = F2.id 
                                        WHERE F2.functional_ability_name = {0} AND F2.system_id = {1} AND UR2.user_id=U.id
                                        ) OR EXISTS
                                        ( 
                                        SELECT GU.user_id FROM role_functional_ability AS RF2 WITH (NOLOCK) 
                                        INNER JOIN dbo.functional_ability AS F2 WITH(NOLOCK) ON RF2.functional_ability_id = F2.id 
                                        INNER JOIN dbo.group_role AS GR WITH(NOLOCK) ON RF2.role_id = GR.role_id 
                                        INNER JOIN dbo.group_user AS GU WITH(NOLOCK) ON GU.group_id=GR.group_id 
                                        WHERE F2.functional_ability_name = {0} AND F2.system_id = {1} AND GU.user_id=U.id
                                        ))";
                for (var i = 0; i < functionList.Count; i++)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(sqlTemplate, "@FunctionalAbility" + i.ToString(), "@ApplicationId"));
                    dataCommand.AddInputParameter("@FunctionalAbility" + i, DbType.String, functionList[i]);
                }

                if (!StringUtility.IsNullOrEmpty(CPConfig.Keystone.TrustedUserName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "URS.unique_identifier",
                        DbType.String, "@unique_identifier", QueryConditionOperatorType.NotEqual, CPConfig.Keystone.TrustedUserName);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "U.first_name",
                        DbType.String, "@first_name", QueryConditionOperatorType.NotEqual, CPConfig.Keystone.TrustedUserName);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "U.last_name",
                        DbType.String, "@last_name", QueryConditionOperatorType.NotEqual, CPConfig.Keystone.TrustedUserName);
                }

                dataCommand.AddInputParameter("@ApplicationId", DbType.String, applicationId);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }

            return dataCommand.ExecuteEntityList<AuthUser>();
        }


        /// <summary>
        /// 获取当前登录用户的所有functional ability
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationIds"></param>
        /// <returns></returns>
        public static List<FunctionalAbilityEntity> GetAuthFunctionalAbilities(string userName, string directory, List<string> applicationIds)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAuthFunctionalAbilities");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "F.system_id ASC"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "F.system_id", DbType.String, applicationIds);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "UAS.unique_identifier", DbType.String, "@UserName", QueryConditionOperatorType.Equal, userName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Prefix", DbType.String, "@Directory", QueryConditionOperatorType.Equal, directory);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }

            return dataCommand.ExecuteEntityList<FunctionalAbilityEntity>();
        }

        /// <summary>
        /// 获取当前登录用户的所有role attribute
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationIds"></param>
        /// <returns></returns>
        public static List<RoleAttributeEntity> GetAuthRoleAttributes(string userName, string directory, List<string> applicationIds)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAuthRoleAttributes");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "R.system_id ASC"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "R.system_id", DbType.String, applicationIds);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "UAS.unique_identifier", DbType.String, "@UserName", QueryConditionOperatorType.Equal, userName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Prefix", DbType.String, "@Directory", QueryConditionOperatorType.Equal, directory);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }

            return dataCommand.ExecuteEntityList<RoleAttributeEntity>();
        }

        /// <summary>
        /// 获取当前用户所拥有的Role
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationIds"></param>
        /// <returns></returns>
        public static List<RoleEntity> GetAuthRolesByUser(string userName, string directory, List<string> applicationIds)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAuthRolesByUser");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "R.system_id ASC"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "R.system_id", DbType.String, applicationIds);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "UAS.unique_identifier", DbType.String, "@UserName", QueryConditionOperatorType.Equal, userName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Prefix", DbType.String, "@Directory", QueryConditionOperatorType.Equal, directory);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }

            return dataCommand.ExecuteEntityList<RoleEntity>();
        }
    }
}
