using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    partial class ProductQueryDA
    {
        private static readonly Dictionary<string, string> Dict = new Dictionary<string, string>();
        private const string MinTime = "1900-01-01 00:00:00.000";
        /// <summary>
        /// int空值默认为
        /// </summary>
        const int EmptyValue = -999;
        const string EmptyValueStr = "-999";
        static ProductQueryDA()
        {
            Dict.Add("ProductSysNo", "SysNo");
            Dict.Add("ProductStatus", "Status");
            Dict.Add("VirtualPicStatus", "virtualPicStatus");
        }

        #region 中蛋网查询商品
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductEx(NeweggProductQueryFilter filter, out int totalCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryProductEx");
            filter.CompanyCode = "8601";
            filter.CurrentUserSysNo = ServiceContext.Current.UserSysNo;
            CreateQueryProductCommand(filter, command);
            var sort = filter.PagingInfo.SortBy;
            var sortField = "";
            var sortType = "";
            if (!String.IsNullOrEmpty(sort))
            {
                var input = Regex.Replace(filter.PagingInfo.SortBy.Trim(), "\\s+", " ");
                var pageArry = input.Split(' ');
                if (pageArry.Length == 2)
                {
                    sortField = pageArry[0];
                    sortType = pageArry[1];
                }
            }
            command.SetParameterValue("@SortField", sortField);
            command.SetParameterValue("@SortType", sortType);
            command.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            command.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);
            var columnConfig = new EnumColumnList
                                   {{"Status", typeof (ProductStatus)}};
            var table = command.ExecuteDataTable(columnConfig);
            totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return table;
        }

        #region 私有方法
        /// <summary>
        /// 根据实体创建查询条件
        /// </summary>
        /// <param name="query"></param>
        /// <param name="command"></param>
        private void CreateQueryProductCommand(object query, DataCommand command)
        {
            if (query == null || command == null)
            {
                return;
            }
            var targetProArray = query.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (targetProArray.Length <= 0) return;
            foreach (var pro in targetProArray)
            {
                var tempType = pro.PropertyType;
                var value = pro.GetValue(query, null);
                if (!tempType.IsGenericType && tempType.IsClass
                    && !tempType.Name.ToUpper().Contains("STRING")
                     && !tempType.Name.ToUpper().Contains("PAGINGINFO")
                    )
                {
                    if (value != null)
                    {
                        CreateQueryProductCommand(value, command);
                    }
                }
                else if (tempType.IsGenericType && tempType.Name.ToLower().Contains("comparison")
                     && tempType.GetGenericArguments().Length == 2)
                {
                    var targe = GetGenericType(null,tempType.GetGenericArguments()[0]);

                    var otherTarge = tempType.GetGenericArguments()[1];
                    if (targe.Name.ToLower().Contains("decimal"))
                    {
                        var tempValue = (QueryFilter.IM.Comparison<decimal?, OperatorType>)value;
                        CreateComparisonCommand<decimal?, OperatorType>(tempValue, pro.Name, command);
                    }
                    else if (targe.Name.ToLower().Contains("int") 
                        && otherTarge.Name.ToLower().Contains("operatortype"))
                    {
                        var tempValue = (QueryFilter.IM.Comparison<int?, OperatorType>)value;
                        CreateComparisonCommand<int?, OperatorType>(tempValue, pro.Name, command);
                    }
                    else if (targe.Name.ToLower().Contains("int")
                        && otherTarge.Name.ToLower().Contains("comparison"))
                    {
                        var tempValue = (QueryFilter.IM.Comparison<int?, Comparison>)value;
                        CreateComparisonCommand<int?, Comparison>(tempValue, pro.Name, command);
                    }
                }
                else if (tempType.IsGenericType
                    && tempType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && tempType.GetGenericArguments().Length == 1)
                {
                    var baseType = GetGenericType(value, tempType);
                    if (baseType.IsEnum)
                    {
                       if(value==null)
                       {
                           if(EnumCodeMapper.IsExistMap(baseType))
                           {
                               CreateCommand(EmptyValueStr, pro.Name, command);
                           }
                           else
                           {
                               CreateCommand(EmptyValue, pro.Name, command);
                           }
                       }
                       else
                       {
                           if (EnumCodeMapper.IsExistMap(baseType))
                           {
                               var code = Convert.ToInt32(value);
                               var codeValue = (char) code;
                               CreateCommand(Convert.ToString(codeValue), pro.Name, command);
                           }
                           else
                           {
                               CreateCommand(Convert.ToInt32(value), pro.Name, command);
                           }
                       }
                    }
                    else if (baseType.Name.ToLower().Contains("datetime"))
                    {
                        var tempValue = value == null ? DateTime.Parse(MinTime) : DateTime.Parse(value.ToString());
                        CreateCommand(tempValue, pro.Name, command);
                    }
                    else if (baseType.IsValueType)
                    {
                        var tempValue = value == null ? EmptyValue : Convert.ToInt32(value);
                        CreateCommand(tempValue, pro.Name, command);
                    }

                }
                else if (tempType.IsValueType || tempType.Name.ToLower().Contains("string"))
                {
                    if (tempType.Name.ToLower().Contains("string")&&value==null)
                    {
                        CreateCommand("", pro.Name, command);
                    }
                    else
                    {
                        CreateCommand(value, pro.Name, command);
                    }
                  
                }
            }
        }

        /// <summary>
        /// 获取类基累积型泛
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private Type GetGenericType(object source, Type sourceType)
        {
            Type targe = sourceType;
            while (targe.IsGenericType
                && targe.GetGenericTypeDefinition() == typeof(Nullable<>)
                && targe.GetGenericArguments().Length == 1)
            {
                targe = targe.GetGenericArguments()[0];
            }
            return targe;
        }

        /// <summary>
        /// 获得比较的Sql命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="query"></param>
        /// <param name="name"></param>
        /// <param name="command"></param>
        private void CreateComparisonCommand<T,TV>(object query, string name ,DataCommand command)
        {
            if (command == null || String.IsNullOrEmpty(name) || query == null) return;
            if(!typeof(TV).IsEnum) return;
            dynamic tempValue = (Comparison<T, TV>)query;
            if( tempValue.ComparisonValue==null)
            {
                tempValue.ComparisonValue = -999;
            }
            if (Dict.ContainsKey(name))
            {
                name = Dict[name];
            }
            var prarm = String.Format("@{0}", name);
            var prarm1 = String.Format("@{0}OP", name);
            
            command.SetParameterValue(prarm, tempValue.ComparisonValue);
            var desc = EnumHelper.GetDescription(tempValue.QueryConditionOperator);
            command.SetParameterValue(prarm1, desc);
        }

        /// <summary>
        /// 创建命令
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        /// <param name="command"></param>
        private void CreateCommand(object query, string name, DataCommand command)
        {
            if (command == null || String.IsNullOrEmpty(name)) return;
            if (Dict.ContainsKey(name))
            {
                name = Dict[name];
            }
            var prarm = String.Format("@{0}", name);
            if (query==null)
            {
                command.SetParameterValue(prarm, "");
            }
            if (query.GetType().Name.ToLower().Contains("string"))
            {
                var value = Convert.ToString(query).Trim();
                command.SetParameterValue(prarm, value);
            }
            else
            {
                command.SetParameterValue(prarm, query);
            }


        }
        #endregion


        #endregion

        /// <summary>
        /// 查询导出商品
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public DataTable QueryExporterEntryFile(List<int> productSysNoList)
        {
            System.Text.StringBuilder sb=new System.Text.StringBuilder();
            foreach(int productSysno in productSysNoList)
            {
                sb.AppendLine(string.Format(" INSERT @ProductTable(ProductSysNo) values({0}) ",productSysno));
            }
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("IM_Product_ExporterEntryFile_Query");
            command.CommandText = command.CommandText.Replace("#InstertProductSysNoToTempTable#", sb.ToString());  
            var table = command.ExecuteDataTable();
            return table;
        }

    }
}
