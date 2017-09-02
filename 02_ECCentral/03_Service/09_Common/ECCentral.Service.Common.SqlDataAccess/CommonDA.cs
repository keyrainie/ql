using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(ICommonDA))]
    public class CommonDA : ICommonDA
    {
        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = pagingInfo.SortBy,
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        #region ICommonDA Members

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<WebChannel> GetWebChannelList(string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("GetChannel");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<WebChannel>();
        }

        public virtual WebChannel GetWebChannelByChannelID(string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("GetChannelByChannelID");
            cmd.SetParameterValue("@ChannelID", channelID);
            return cmd.ExecuteEntity<WebChannel>();
        }

        [Caching("local", new string[] { "companyCode", "userSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<WebChannel> GetWebChannelListByUser(string companyCode, int userSysNo)
        {
            List<WebChannel> result = new List<WebChannel>();
            result.Add(new WebChannel
            {
                SysNo = 1,
                ChannelID = "1",
                ChannelName = "食佳时代",
                ChannelType = WebChannelType.InternalChennel
            });
            return result;
        }

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<Company> GetCompanyList()
        {
            //ToDO: 暂时全部取出来，不考虑userSysNo
            List<Company> list = new List<Company>();
            list.Add(new Company
            {
                CompanyCode = "8601",
                CompanyName = "食佳时代"
            });

            return list;
        }

        [Caching("local", new string[] { "userSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<Company> GetCompanyListByUser(int userSysNo)
        {
            //ToDO: 暂时全部取出来，不考虑userSysNo
            List<Company> list = new List<Company>();
            //list.Add(new Company
            //{
            //    CompanyCode = "8601",
            //    CompanyName = "泰隆银行"
            //});
            //list.Add(new Company
            //{
            //    CompanyCode = "8602",
            //    CompanyName = "食佳时代"
            //});
            list.Add(new Company
            {
                CompanyCode = "8601",
                CompanyName = "食佳时代"
            });
            return list;
        }

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<ShippingType> GetShippingTypeList(string companyCode)
        {
            List<ShippingType> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShippingTypeList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<ShippingType, List<ShippingType>>(reader);
                return list;
            }
        }

        [Caching("local", new string[] { "shipTypeSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual ShippingType GetShippingTypeBySysNo(int shipTypeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShippingTypeBySysNo");
            cmd.SetParameterValue("@SysNo", shipTypeSysNo);
            return cmd.ExecuteEntity<ShippingType>();
        }

        [Caching("local",new string[]{"companyCode"}, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode)
        {
            List<ShipTypeAreaUnInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShipTypeAreaUnList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<ShipTypeAreaUnInfo, List<ShipTypeAreaUnInfo>>(reader);
                return list;
            }
        }

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<PayType> GetPayTypeList(string companyCode)
        {
            List<PayType> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPayTypeList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            list = cmd.ExecuteEntityList<PayType>((r, e) =>
            {
                e.IsNet = (int)r["IsNet"] > 0;
                e.IsPayWhenRecv = (int)r["IsPayWhenRecv"] > 0;
            });
            return list;
        }
        [Caching("local", new string[] { "payTypeSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual PayType GetPayTypeBySysNo(int payTypeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPayTypeBySysNo");
            cmd.SetParameterValue("@SysNo", payTypeSysNo);
            PayType result = cmd.ExecuteEntity<PayType>((r, e) =>
            {
                e.IsNet = (int)r["IsNet"] > 0;
                e.IsPayWhenRecv = (int)r["IsPayWhenRecv"] > 0;
            });
            return result;
        }

        /// <summary>
        /// 根据支付方式编号获取支付方式信息
        /// </summary>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        [Caching("local", new string[] { "payTypeID" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual PayType GetPayTypeByID(string payTypeID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPayTypeByID");
            cmd.SetParameterValue("@PayTypeID", payTypeID);
            PayType result = cmd.ExecuteEntity<PayType>((r, e) =>
            {
                e.IsNet = (int)r["IsNet"] > 0;
                e.IsPayWhenRecv = (int)r["IsPayWhenRecv"] > 0;
            });
            return result;
        }

        [Caching("local", new string[] { "userAccount" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual int? GetUserSysNo(string userAccount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetUserSysNoByLoginName");
            cmd.SetParameterValue("@LoginName", userAccount);
            int sysNo = cmd.ExecuteScalar<int>();
            return sysNo;
        }

        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<StockInfo> GetStockList(string companyCode)
        {
            List<StockInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllStock");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<StockInfo, List<StockInfo>>(reader);
                return list;
            }
        }

        [Caching("local", new string[] { "key", "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual string GetSysConfigurationValue(string key, string companyCode)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryConfigurationByKey");
            dataCommand.ReplaceParameterValue("@Key", key);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToString(dataCommand.ExecuteScalar());
        }

        [Caching("local", new string[] { "userID", "isSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual string GetUserFullName(string userID, bool isSysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetUserFullName");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, new PagingInfoEntity(), " ORDER BY UserSysNo"))
            {
                if (isSysNo)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "UserSysNo", DbType.Int32, "@UserSysNo", QueryConditionOperatorType.Equal, int.Parse(userID));
                }
                else
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "LoginName", DbType.Int32, "@LoginName", QueryConditionOperatorType.Equal, userID);
                }

                cmd.CommandText = sb.BuildQuerySql();
                string re = cmd.ExecuteScalar<string>();

                return re;
            }
        }

        /// <summary>
        /// 返回系统用户列表
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns></returns>
        //[Caching("local", new string[] { "filter" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual List<UserInfo> GetUserInfoList(UserInfoQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetUserInfoList");

            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "[DisplayName] ASC"))
            {
                //商品ID
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "DepartmentCode",
                 DbType.Int32,
                 "@DepartmentCode",
                 QueryConditionOperatorType.Equal,
                 filter.DepartmentId
                 );

                //公司编码
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "CompanyCode",
                 DbType.String,
                 "@CompanyCode",
                 QueryConditionOperatorType.Equal,
                 filter.CompanyCode
                 );

                if (filter.Status.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "Status",
                     DbType.Int32,
                     "@Status",
                     QueryConditionOperatorType.Equal,
                     filter.Status
                     );
                }
                cmd.CommandText = sb.BuildQuerySql();
                return cmd.ExecuteEntityList<UserInfo>();
            }
        }

        [Caching("local", new string[] { "companyCode", "languageCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<DepartmentInfo> GetDepartmentList(string companyCode, string languageCode)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllDepartment");
            cmd.AddInputParameter("@LanguageCode", DbType.AnsiStringFixedLength, languageCode);
            return cmd.ExecuteEntityList<DepartmentInfo>();
        }

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<CurrencyInfo> GetCurrencyList()
        {
            List<CurrencyInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCurrencyList");
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<CurrencyInfo, List<CurrencyInfo>>(reader);
                return list;
            }
        }

        #endregion ICommonDA Members

        /// <summary>
        /// 获取无参数查询的对象集合数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlKey">Sql配置对应键值</param>
        /// <returns>对象集合数据</returns>
        private List<T> GetNoParaList<T>(string sqlKey) where T : class, new()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand(sqlKey);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<T, List<T>>(reader);
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="content"></param>
        /// <param name="priority"></param>
        public virtual void SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertSMS");
            dc.SetParameterValue("@CellNumber", phoneNumber);
            dc.SetParameterValue("@SMSContent", content);
            dc.SetParameterValue("@Priority", priority);
            dc.SetParameterValueAsCurrentUserSysNo("@CreateUserSysno");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 取得业务操作用户列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "filter" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<UserInfo> GetBizOperationUser(QueryFilter.Common.BizOperationUserQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetBizOperationUser");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "SU.[UserName]"))
            {
                //sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OU.[BizTableName]", DbType.String, "@BizTableName", QueryConditionOperatorType.Equal, filter.BizTableName);
                //sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OU.[Type]", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.UserType);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SU.[Status]", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.UserStatus);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SU.[CompanyCode]", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sb.BuildQuerySql();
                return cmd.ExecuteEntityList<UserInfo>();
            }
        }

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSpecialRulesByShipTypeSysno");
            command.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<ItemShipRuleInfo>();
        }

        /// <summary>
        /// 获取商品配送规则
        /// </summary>
        /// <param name="c3SysNoStr">商品3级分类编号序列（如：254,256）</param>
        /// <param name="productSysNoStr">商品编号序列（如：157856,157896）</param>
        /// <param name="provinceSysNo">省编号</param>
        /// <param name="citySysNo">市编号</param>
        /// <param name="areaSysno">区编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public List<ItemShipRuleInfo> GetItemShipRuleList(string c3SysNoStr, string productSysNoStr, int? provinceSysNo, int? citySysNo, int? areaSysno, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetItemShipRuleList");
            command.ReplaceParameterValue("#C3SysNoStr#", c3SysNoStr);
            command.ReplaceParameterValue("#ProductSysNoStr#", productSysNoStr);
            command.SetParameterValue("@ProvinceSysNo", provinceSysNo);
            command.SetParameterValue("@CitySysNo", citySysNo);
            command.SetParameterValue("@AreaSysNo", areaSysno);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<ItemShipRuleInfo>();
        }

        public decimal GetExchangeRateByCurrencySysNo(int sysNo, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCurrencyBySysNo");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal, sysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String,
                   "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);

                command.CommandText = builder.BuildQuerySql();
                object getResult = command.ExecuteDataSet().Tables[0].Rows[0]["ExchangeRate"].ToString();
                if (null != getResult)
                {
                    return Convert.ToDecimal(getResult.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 获取所有验证通过的系统用户
        /// </summary>
        /// <returns>系统用户列表</returns>
        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<UserInfo> GetAllAuthSystemUser(string companyCode)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllAuthSystemUser");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<UserInfo>();
        }

        /// <summary>
        /// 取得投递员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<UserInfo> GetFreightManList(string companyCode)
        {
            DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetFreightManList");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<UserInfo>();
        }

        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<UserInfo> GetCustomerServiceList(string companyCode)
        {
            DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCustomerServiceList");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<UserInfo>();
        }

        public int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode)
        {
            DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetUserSysNo");
            command.SetParameterValue("@ACSourceDirectoryKey", sourceDirectoryKey);
            command.SetParameterValue("@ACPhysicalUserName", loginName);
            command.SetParameterValue("@CompanyCode", companyCode);

            object resultData = command.ExecuteScalar();
            if (resultData == null)
            {
                return 0;
            }
            return Convert.ToInt32(resultData);
        }

        public UserInfo GetUserInfoBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetUserInfoBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<UserInfo>();
        }

        /// <summary>
        /// 根据key添加或者更新IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">Value</param>
        /// <param name="updateUserSysNo">操作用户编号</param>
        /// <param name="companyCode">公司编号</param>
        public bool UpdateSystemConfigurationValueByKey(string key, string value, int updateUserSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSystemConfigurationValueByKey");
            command.SetParameterValue("@Key", key);
            command.SetParameterValue("@value", value);
            command.SetParameterValue("@updateUserSysNo", updateUserSysNo);
            command.SetParameterValue("@companyCode", companyCode);
            return command.ExecuteNonQuery() > 0 ? true : false;
        }
        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CreateApplicationEventLog(ApplicationEventLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_CreateApplicationEventLog");
            cmd.SetParameterValue<ApplicationEventLog>(entity);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }
    }
}