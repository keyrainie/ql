using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Xml;
using Nesoft.Utility.DataAccess.Database.Config;
using Nesoft.Utility.DataAccess.Database;
using System.Data;

namespace Nesoft.Utility.DataAccess.RealTime
{
    internal static class ConfigHelper
    {                
        private static XmlNode[] GetChildrenNodes(XmlNode node, string nodeName)
        {
            return GetChildrenNodes(node, delegate(XmlNode child)
            {
                return child.Name == nodeName;
            });
        }

        private static XmlNode[] GetChildrenNodes(XmlNode node, Predicate<XmlNode> match)
        {
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count <= 0)
            {
                return new XmlNode[0];
            }
            List<XmlNode> nodeList = new List<XmlNode>(node.ChildNodes.Count);
            foreach (XmlNode child in node.ChildNodes)
            {
                if (match(child))
                {
                    nodeList.Add(child);
                }
            }
            return nodeList.ToArray();
        }

        private static string GetNodeAttribute(XmlNode node, string attributeName)
        {
            if (node.Attributes == null
                        || node.Attributes[attributeName] == null
                        || node.Attributes[attributeName].Value == null
                        || node.Attributes[attributeName].Value.Trim() == string.Empty)
            {
                return string.Empty;
            }
            return node.Attributes[attributeName].Value.Trim();
        }        

        private static string GetConfigPath()
        {
            string path = ConfigurationManager.AppSettings["RealTimeConfigFilePath"];
            if (path == null || path.Trim().Length <= 0)
            {
                return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Configuration/RealTime.config");
            }
            string p = Path.GetPathRoot(path);
            if (p == null || p.Trim().Length <= 0) // 说明是相对路径
            {
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
            }
            return path;
        }

        //internal static List<RealTimeExtensionConfig> GetExtensionConfig()
        //{
        //    List<RealTimeExtensionConfig> list = new List<RealTimeExtensionConfig>();
        //    string path = GetConfigPath();
        //    if (!File.Exists(path))
        //    {
        //        return list;
        //    }
        //    XmlDocument x = new XmlDocument();
        //    x.Load(path);
        //    XmlNode node = x.SelectSingleNode(@"//extensions");
        //    if (node == null || node.ChildNodes == null || node.ChildNodes.Count <= 0)
        //    {
        //        return list;
        //    }
        //    XmlNode[] eventList = GetChildrenNodes(node, "data");
        //    foreach (XmlNode ev in eventList)
        //    {
        //        string name = GetNodeAttribute(ev, "name");
        //        string dataType = GetNodeAttribute(ev, "type");
        //        string extensionType = GetNodeAttribute(ev, "extensionType");
        //        RealTimeExtensionConfig config = new RealTimeExtensionConfig();
        //        config.Name = name;
        //        config.DataType = dataType;
        //        config.ExtensionType = extensionType;
        //        list.Add(config);
        //    }
        //    return list;
        //}

        //internal static RealTimeExtensionConfig GetExtensionConfig(string dataName)
        //{
        //    var list = GetExtensionConfig();
        //    if (list != null)
        //    {
        //        return list.FirstOrDefault(p => p.Name.Trim().ToUpper() == dataName.Trim().ToUpper());
        //    }
        //    return null;
        //}

        internal static List<RealTimeMethod> GetRealTimeConfig()
        {
            List<RealTimeMethod> list = new List<RealTimeMethod>();
            string path = GetConfigPath();
            if (!File.Exists(path))
            {
                return list;
            }
            XmlDocument x = new XmlDocument();
            x.Load(path);
            XmlNode node = x.SelectSingleNode(@"//realTime");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count <= 0)
            {
                return list;
            }
            XmlNode[] eventList = GetChildrenNodes(node, "query");
           
            foreach (XmlNode ev in eventList)
            {
                string dataType = GetNodeAttribute(ev, "dataType");
                string queryName = GetNodeAttribute(ev, "name");
                //string tableName = GetNodeAttribute(ev, "tableName");
                //string primaryField = GetNodeAttribute(ev, "primaryField");
                RealTimeMethod query = new RealTimeMethod
                {
                    Name = queryName,
                    DataType = dataType,
                    //TableName = tableName,
                    //PrimaryField = primaryField,
                    FilterFields = new List<FilterField>(),
                    ReturnFields = new List<ReturnField>()
                };
                list.Add(query);

                XmlNode filterFieldsNode = ev.SelectSingleNode("filterFields");
                XmlNode[] filterFields = GetChildrenNodes(filterFieldsNode,"field");

                foreach (XmlNode no in filterFields)
                {
                    string name = GetNodeAttribute(no, "name");
                    string valuePath = GetNodeAttribute(no, "valuePath");
                    string relationType = GetNodeAttribute(no, "relationType");
                    string operatorType = GetNodeAttribute(no, "operatorType");
                    string dbType = GetNodeAttribute(no, "dbType");
                    FilterField filed = new FilterField
                    {
                        Name = name,
                        ValuePath = valuePath,
                        OperatorType = operatorType,
                        RelationType = relationType,
                        DBType = dbType
                    };

                    query.FilterFields.Add(filed);
                }

                XmlNode returnFieldsNode = ev.SelectSingleNode("returnFields");
                XmlNode[] returnFields = GetChildrenNodes(returnFieldsNode, "field");

                foreach (XmlNode no in returnFields)
                {
                    string name = GetNodeAttribute(no, "name");
                    string valuePath = GetNodeAttribute(no, "valuePath");
                    string dbType = GetNodeAttribute(no, "dbType");
                    ReturnField filed = new ReturnField
                    {
                        Name = name,
                        ValuePath = valuePath,
                        DBType = dbType
                    };

                    query.ReturnFields.Add(filed);
                }
            }
            return list;
        }

        internal static RealTimeMethod GetRealTimeConfig(string name)
        {
            var list = GetRealTimeConfig();
            if (list != null)
            {
                return list.FirstOrDefault(p => p.Name.Trim().ToUpper() == name.Trim().ToUpper());
            }
            return null;
        }

        internal static IRealTimePersister GetDefaultPersiter()
        {
            return PersisteFactory.GetInstance();
        }        
    }

    public static class RealTimeHelper
    {
        private static string loadDataSql = @"
SELECT
    #XmlFields#
FROM EcommerceRealtime.dbo.RealTimeData r WITH(NOLOCK)
#StrWhere# 
UNION ALL
#InputSql#";

        private static string loadPagingDataSql = @"
SELECT @TotalCount = COUNT(1) 
FROM (
    SELECT
    #XmlFields#
    FROM EcommerceRealtime.dbo.RealTimeData r WITH(NOLOCK)
    #StrWhere# 
    UNION ALL
    #InputSql#
) result
      
SELECT
    #Columns#
FROM(
    SELECT TOP (@EndNumber)
        ROW_NUMBER() OVER(ORDER BY #SortColumnName#) AS RowNumber,
        #Columns#
    FROM 
    (
        SELECT
            #XmlFields#
        FROM EcommerceRealtime.dbo.RealTimeData r WITH(NOLOCK)
        #StrWhere# 
        UNION ALL
        #InputSql#
    ) unionResult ) result
WHERE RowNumber > @StartNumber
";

        // private static string excludeDataSql = @"NOT EXISTS(
        //    SELECT TOP 1 1 
        //    FROM #TableName# t WITH(NOLOCK) 
        //    WHERE r.Key = t.#PrimaryKey#)";


        /// <summary>
        /// 获取属性数据类型
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="property"></param>
        /// <param name="propertyNameIgnoreCase"></param>
        /// <param name="skipNotExistProperty"></param>
        /// <returns></returns>
        //private static Type GetPropertyType(object pro, string property, bool propertyNameIgnoreCase, bool skipNotExistProperty)
        //{
        //    Type type = null;
        //    string[] pNames = property.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        //    //object val = DataMapper.ConvertIfEnum(reader[i], typeof(T), pNames, propertyNameIgnoreCase, skipNotExistProperty);            
        //    int index = 0;
        //    foreach (string propertyName in pNames)
        //    {
        //        if (!Invoker.ExistPropertySet(pro.GetType(), propertyName, propertyNameIgnoreCase))
        //        {
        //            if (!skipNotExistProperty)
        //            {
        //                throw new ApplicationException("There is no public instance property that can be set '" + propertyName + "' in type '" + pro.GetType().FullName + "'");
        //            }
        //            break;
        //        }
        //        // 根据property的值（不区分大小写）找到在pro对象的类型中的属性的名称
        //        string realName = Invoker.GetPropertyNameIgnoreCase(pro.GetType(), propertyName);
        //        if (realName == null || (realName != propertyName && !propertyNameIgnoreCase))
        //        // realName == null 说明pro对象的类型中不存在名为property变量值的属性
        //        // realName != propertyName 说明存在属性，但属性名与输入的值的大小写不一致
        //        {
        //            if (!skipNotExistProperty)
        //            {
        //                throw new ApplicationException("There is no public instance property that can be set '" + propertyName + "' in type '" + pro.GetType().FullName + "'");
        //            }
        //            break;
        //        }
        //        if (index == pNames.Length - 1)
        //        {
        //            type = Invoker.GetPropertyType(pro.GetType(), realName);
        //        }
        //        else
        //        {
        //            object tmp = null;
        //            if (Invoker.ExistPropertyGet(pro.GetType(), realName))
        //            {
        //                tmp = Invoker.PropertyGet(pro, realName);
        //            }
        //            if (tmp == null)
        //            {
        //                type = Invoker.GetPropertyType(pro.GetType(), realName, false, false);
        //            }
        //            pro = tmp;
        //        }
        //        index++;
        //    }
        //    return type;
        //}

        //private static string GetDBTypeStr(Type type)
        //{
        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        type = type.GetGenericArguments()[0];
        //    }
        //    TypeCode code = Type.GetTypeCode(type);
        //    switch (code)
        //    {
        //        case TypeCode.Boolean:
        //            return "bit";
        //        case TypeCode.Int16:
        //            return "smallint";
        //        case TypeCode.Int32:
        //            return "int";
        //        case TypeCode.Int64:
        //            return "bigint";
        //        case TypeCode.String:
        //        case TypeCode.Char:
        //            return "nvarchar(max)";
        //        case TypeCode.Decimal:
        //            return "decimal(19,6)";
        //        case TypeCode.Double:
        //            return "double(19,6)";
        //        case TypeCode.DateTime:
        //            return "datetime";
        //        default:
        //            return "nvarchar(max)";
        //    }
        //}

        //private static System.Data.DbType GetDbType(Type type)
        //{
        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        type = type.GetGenericArguments()[0];
        //    }
        //    TypeCode code = Type.GetTypeCode(type);
        //    switch (code)
        //    {
        //        case TypeCode.Boolean:
        //            return DbType.Boolean;
        //        case TypeCode.Int16:
        //            return DbType.Int16;
        //        case TypeCode.Int32:
        //            return DbType.Int32;
        //        case TypeCode.Int64:
        //            return DbType.Int64;
        //        case TypeCode.String:
        //            return DbType.String;
        //        case TypeCode.Char:
        //            return DbType.AnsiStringFixedLength;
        //        case TypeCode.Decimal:
        //            return DbType.Decimal;
        //        case TypeCode.Double:
        //            return DbType.Double;
        //        case TypeCode.DateTime:
        //            return DbType.DateTime;
        //        default:
        //            return DbType.String;
        //    }
        //}

        /// <summary>
        /// 根据配置文件中的Sql数据类型获取DbType
        /// </summary>
        /// <param name="sqlTypeString"></param>
        /// <returns></returns>
        private static DbType GetDbType(string sqlTypeString)
        {
            sqlTypeString = sqlTypeString.Trim();
            if (sqlTypeString.Contains("("))
            {
                sqlTypeString = sqlTypeString.Substring(0, sqlTypeString.IndexOf("("));
            }
            switch (sqlTypeString)
            {
                case "bigint":
                    return DbType.Int64;
                case "binary":
                    return DbType.Binary;
                case "bit":
                    return DbType.Boolean;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "date":
                    return DbType.Date;
                case "datetime":
                    return DbType.DateTime;
                case "datetime2":
                    return DbType.DateTime2;
                case "datetimeoffset":
                    return DbType.DateTimeOffset;
                case "decimal":
                    return DbType.Decimal;
                case "filestream":
                    return DbType.Binary;
                case "float":
                    return DbType.Double;
                case "image":
                    return DbType.Binary;
                case "int":
                    return DbType.Int32;
                case "money":
                    return DbType.Decimal;
                case "nchar":
                    return DbType.StringFixedLength;
                case "ntext":
                    return DbType.String;
                case "numeric":
                    return DbType.Decimal;
                case "nvarchar":
                    return DbType.String;
                case "real":
                    return DbType.Single;
                case "rowversion":
                    return DbType.Binary;
                case "smalldatetime":
                    return DbType.DateTime;
                case "smallint":
                    return DbType.Int16;
                case "smallmoney":
                    return DbType.Decimal;
                case "sql_variant":
                    return DbType.Object;
                case "text":
                    return DbType.String;
                case "time":
                    return DbType.Time;
                case "timestamp":
                    return DbType.Binary;
                case "tinyint":
                    return DbType.Byte;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "varbinary":
                    return DbType.Binary;
                case "varchar":
                    return DbType.AnsiString;
                case "xml":
                    return DbType.Xml;
                default:
                    throw new ArgumentException("Invalid sql dbtype.");
            }
        }       

        private static T GetEnum<T>(string name) where T : struct
        {
            T result = default(T);
            bool flag = Enum.TryParse<T>(name, out result);
            if (flag)
            {
                return result;
            }
            throw new ArgumentException("Invalid value of enum {0}", typeof(T).Name);
        }

        private static void BuilCondition<Q>(Q filter, string dataType, List<FilterField> filterFields, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.BusinessDataType", DbType.String, "@BusinessDataType", QueryConditionOperatorType.Equal, dataType);
            List<string> changeTypes = new List<string>() { "A", "U" };
            sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "r.ChangeType", DbType.String, changeTypes);

            int index = 0;
            filterFields.ForEach(p =>
            {
                object parameterValue = Invoker.PropertyGet(filter, p.Name);
                string[] pNames = p.ValuePath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string path = string.Format("(//{0}/text())[1]", pNames.Join("/"));
                DbType dbType = GetDbType(p.DBType);
                string field = string.Format("r.BusinessData.value('{0}','{1}')", path, p.DBType);
                QueryConditionOperatorType operatorType = GetEnum<QueryConditionOperatorType>(p.OperatorType);
                QueryConditionRelationType relationType = GetEnum<QueryConditionRelationType>(p.RelationType);

                sqlBuilder.ConditionConstructor.AddCondition(relationType,
                   field, dbType, "@Parameter" + index.ToString(), operatorType, parameterValue);
                index++;
            });
        }

        private static void BuildColumns(List<ReturnField> returnFields, out string xmlFields, out List<string> columns)
        {                       
            StringBuilder fields = new StringBuilder();
            var cols = new List<string>();

            returnFields.ForEach(p =>
            {
                cols.Add(string.Format("[{0}]", p.Name));               
                string[] pNames = p.ValuePath.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string path = string.Format("(//{0}/text())[1]", pNames.Join("/"));

                fields.AppendFormat("r.BusinessData.value('{0}','{1}') AS [{2}],", path, p.DBType, p.Name);
            });
            fields.Remove(fields.Length - 1, 1);

            xmlFields = fields.ToString();
            columns = cols;
        }

        public static void Persiste<T>(RealTimeData<T> data) where T : class
        {
            ConfigHelper.GetDefaultPersiter().Persiste<T>(data);
        }

        public static object LoadData(int key)
        {
            return null;
        }

        public static T LoadData<T>(int key) where T : class, new()
        {
            //查询RealTime表中的数据
            return default(T);
        }

        /// <summary>
        /// 查询数据不分页
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> QueryData<Q, T>(CustomDataCommand command, Q filter, string configName) where T : class, new()
        {            
            var config = ConfigHelper.GetRealTimeConfig(configName);

            string xmlFields;
            List<string> columns;
            BuildColumns(config.ReturnFields, out xmlFields, out columns);

            var cmd = DataCommandManager.CreateCustomDataCommandFromSql(loadDataSql, command.DatabaseAliasName);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd, "SysNo desc"))
            {
                BuilCondition<Q>(filter, config.DataType, config.FilterFields, sqlBuilder);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.CommandText = cmd.CommandText.Replace("#XmlFields#", xmlFields.ToString());
                cmd.CommandText = cmd.CommandText.Replace("#InputSql#", command.CommandText);

                var list = cmd.ExecuteEntityList<T>();
                return list;
            }
        }
        
        /// <summary>
        /// 查询数据并分页
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="filter"></param>
        /// <param name="needRealTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<T> QueryData<Q, T>(CustomDataCommand command, Q filter, string configName,
            PagingInfoEntity pagingInfo, out int totalCount) where T : class, new()
        {
            pagingInfo.SortField = "[SOMaster.SOSysNo]";
            if (string.IsNullOrEmpty(pagingInfo.SortField))
            {
                throw new ApplicationException("You must specified one sort field at least.");
            }
            var config = ConfigHelper.GetRealTimeConfig(configName);

            string xmlFields;
            List<string> columns;
            BuildColumns(config.ReturnFields, out xmlFields, out columns);
                        
            var cmd = DataCommandManager.CreateCustomDataCommandFromSql(loadPagingDataSql, command.DatabaseAliasName);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd, pagingInfo, pagingInfo.SortField))
            {
                BuilCondition<Q>(filter, config.DataType, config.FilterFields, sqlBuilder);
                               
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                //把传入的参数添加到组合后的DataCommand中
                //command.DbParameterList.ForEach(p =>
                //{
                //    var param = cmd.DbParameterList.FirstOrDefault(k => k.ParameterName.Trim().ToUpper() == p.ParameterName.Trim().ToUpper());
                //    if (param == null)
                //    {
                //        cmd.AddInputParameter(p.ParameterName, p.DbType, p.Value);
                //    }
                //});

                cmd.CommandText = cmd.CommandText.Replace("#Columns#", columns.Join(","));
                cmd.CommandText = cmd.CommandText.Replace("#XmlFields#", xmlFields.ToString());
                cmd.CommandText = cmd.CommandText.Replace("#InputSql#", command.CommandText);

                var list = cmd.ExecuteEntityList<T>();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return list;
            }
        }
    }
}