using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IEIMSEventMemoQueryFilterDA))]
    public class EIMSEventMemoQueryFilterDA : IEIMSEventMemoQueryFilterDA
    {
        #region EIMS结算类型变更单据查询

        public DataTable EIMSEventMemoQuery(EIMSEventMemoQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EIMSEventMemoQuery");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "EventDate DESC"))
            {
                AddEIMSEventMemoQueryParameters(filter, sb, cmd);
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddEIMSEventMemoQueryParameters(EIMSEventMemoQueryFilter filter, DynamicQuerySqlBuilder sb, CustomDataCommand cmd)
        {
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.EventType", DbType.Int32, "@EventType", QueryConditionOperatorType.Equal, 195);
            string tempMemo = string.Empty;
            if (!string.IsNullOrEmpty(filter.Memo))
            {
                tempMemo = filter.Memo.Replace(" ", "%");
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.EventMemo", DbType.String, "@EventMemo", QueryConditionOperatorType.Like, "%" + tempMemo + "%");
            }
            if (filter.BeginDate.HasValue)
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.EventDate", DbType.String, "@BDate", QueryConditionOperatorType.MoreThanOrEqual, filter.BeginDate);
            }
            if (filter.EndDate.HasValue)
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.EventDate", DbType.String, "@EDate", QueryConditionOperatorType.LessThanOrEqual, filter.EndDate.Value.ToShortDateString() + " 23:59:59");
            }
            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region EIMS发票信息查询
        public DataTable InvoiceInfoListQuery(EIMSInvoiceEntryQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EIMS_Query_InvoiceInfoList");
            MapSortField(filter);
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "ei.CreateDate DESC"))
            {
                AddInvoiceInfoListParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(InvoiceStatus));
                enumColList.Add("EIMSType", typeof(EIMSType));
                enumColList.Add("ReceiveType", typeof(ReceiveType));
                cmd.ConvertEnumColumn(dt, enumColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public List<EIMSInvoiceEntryInfo> InvoiceInfoListQuery(EIMSInvoiceEntryQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EIMS_Query_InvoiceInfoList");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "ei.CreateDate DESC"))
            {
                AddInvoiceInfoListParameters(filter, cmd, sb);

                return cmd.ExecuteEntityList<EIMSInvoiceEntryInfo>();
            }
        }

        private void AddInvoiceInfoListParameters(EIMSInvoiceEntryQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            List<Object> receiveTypeList = new List<Object>();
            receiveTypeList.Add(1);
            receiveTypeList.Add(2);
            List<Object> status = new List<Object>();
            status.Add("O");
            status.Add("F");
            sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ei.ReceiveType",
                    DbType.Int32,
                    "@MyReceiveType",
                    QueryConditionOperatorType.In,
                    receiveTypeList);
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "ei.[STATUS]",
                DbType.String,
                "@MySTATUS",
                QueryConditionOperatorType.In,
                status);
            if (!string.IsNullOrEmpty(filter.AssignedCode))
            {
                filter.AssignedCode = filter.AssignedCode.Replace(" ", string.Empty);
                string[] spiltResults = filter.AssignedCode.Split('.');
                string invoiceNumbers = string.Empty;
                string assignedCodes = string.Empty;
                int invoiceNumber = 0;
                if (spiltResults.Length > 1)
                {
                    for (int i = 0; i < spiltResults.Length; i++)
                    {
                        if (int.TryParse(spiltResults[i], out invoiceNumber))
                        {
                            if (invoiceNumbers.Length > 0)
                            {
                                invoiceNumbers += ",";
                            }
                            invoiceNumbers += spiltResults[i];
                        }
                        else
                        {
                            if (assignedCodes.Length > 0)
                            {
                                assignedCodes += ",";
                            }
                            assignedCodes += "'" + spiltResults[i] + "'";
                        }
                    }
                    if (invoiceNumbers.Length > 0 && assignedCodes.Length == 0)
                    {
                        sb.ConditionConstructor.AddSubQueryCondition(
                        QueryConditionRelationType.AND,
                        "ei.InvoiceNumber",
                        QueryConditionOperatorType.In,
                        invoiceNumbers);
                    }
                    if (assignedCodes.Length > 0 && invoiceNumbers.Length == 0)
                    {
                        sb.ConditionConstructor.AddSubQueryCondition(
                        QueryConditionRelationType.AND,
                        "ei.AssignedCode",
                        QueryConditionOperatorType.In,
                        assignedCodes);
                    }
                    if (invoiceNumbers.Length > 0 && assignedCodes.Length > 0)
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddSubQueryCondition(
                        QueryConditionRelationType.AND,
                        "ei.InvoiceNumber",
                        QueryConditionOperatorType.In,
                        invoiceNumbers);
                        sb.ConditionConstructor.AddSubQueryCondition(
                        QueryConditionRelationType.OR,
                        "ei.AssignedCode",
                        QueryConditionOperatorType.In,
                        assignedCodes);
                        sb.ConditionConstructor.EndGroupCondition();
                    }
                }
                else
                {
                    if (int.TryParse(filter.AssignedCode, out invoiceNumber))
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "ei.AssignedCode",
                            DbType.String,
                            "@AssignedCode",
                            QueryConditionOperatorType.Equal,
                            filter.AssignedCode);
                        sb.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.OR,
                            "ei.InvoiceNumber",
                            DbType.String,
                            "@InvoiceNumber",
                            QueryConditionOperatorType.Equal,
                            invoiceNumber);
                        sb.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        sb.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "ei.AssignedCode",
                            DbType.String,
                            "@AssignedCode",
                            QueryConditionOperatorType.Equal,
                            filter.AssignedCode);
                    }
                }
            }
            if (filter.EIMSType.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ei.EIMSType",
                        DbType.String,
                        "@EIMSType",
                        QueryConditionOperatorType.Equal,
                        filter.EIMSType);
            }
            if (!string.IsNullOrEmpty(filter.InvoiceInputStatus))
            {
                string subSql = "SELECT COUNT(1) " +
                                "FROM  eims.dbo.EIMSInvoiceInput iei WITH(NOLOCK) " +
                            "INNER JOIN eims.dbo.EIMSInvoiceInput_Ex iex WITH(NOLOCK) " +
                            "ON iei.SysNo = iex.InvoiceInputSysNO AND iei.[status]=0 AND iex.[status]=0 " +
                            "WHERE iex.InvoiceNumber = ei.InvoiceNumber ";
                if (filter.InvoiceInputStatus == "已录入")
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "(" + subSql + ")",
                        DbType.Int32,
                        "@InvoiceInputStatusSql",
                        QueryConditionOperatorType.MoreThan,
                        0);
                }
                else if (filter.InvoiceInputStatus == "未录入")
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "(" + subSql + ")",
                        DbType.Int32,
                        "@InvoiceInputStatusSql",
                        QueryConditionOperatorType.Equal,
                        0);
                }
            }
            if (filter.ReceiveType.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ei.ReceiveType",
                        DbType.String,
                        "@ReceiveType",
                        QueryConditionOperatorType.Equal,
                        filter.ReceiveType);
            }
            if (!string.IsNullOrEmpty(filter.RuleAssignedCode))
            {
                sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "er.AssignedCode",
                        DbType.String,
                        "@RuleAssignedCode",
                        QueryConditionOperatorType.Equal,
                        filter.RuleAssignedCode);
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ei.[Status]",
                        DbType.String,
                        "@Status",
                        QueryConditionOperatorType.Equal,
                        filter.Status);
            }
            if (filter.VendorNumber.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ei.VendorNumber",
                        DbType.String,
                        "@VendorNumber",
                        QueryConditionOperatorType.Equal,
                        filter.VendorNumber);
            }
            if (!string.IsNullOrEmpty(filter.IsSAPImported))
            {
                if (filter.IsSAPImported == "Y")
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "fp.IsSAPImported",
                        DbType.String,
                        "@IsSAPImported",
                        QueryConditionOperatorType.Equal,
                        filter.IsSAPImported);
                }
                else if (filter.IsSAPImported == "N")
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "fp.IsSAPImported",
                        DbType.String,
                        "@IsSAPImported",
                        QueryConditionOperatorType.Equal,
                        filter.IsSAPImported);
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.OR,
                        "fp.IsSAPImported",
                        DbType.String,
                        "@InvoiceNumber",
                        QueryConditionOperatorType.IsNull,
                        null);
                }
            }

            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                string[] arr = filter.InvoiceNumber.Split('.');
                List<Object> invoiceNumberList = new List<Object>();
                string invoiceNumber = string.Empty;
                for (int i = 0; i < arr.Length; i++)
                {
                    invoiceNumberList.Add(arr[i]);
                }
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ei.InvoiceNumber",
                    DbType.String,
                    "@InvoiceNumber",
                    QueryConditionOperatorType.In,
                    invoiceNumberList);
            }

            if (!string.IsNullOrEmpty(filter.InvoiceInputNo))
            {
                string subSql = "EXISTS(SELECT 1 " +
                                "FROM EIMS.dbo.EIMSInvoiceInput_Ex eiiex WITH(NOLOCK) " +
                                "INNER JOIN eims.dbo.EIMSInvoiceInput eii WITH(NOLOCK) " +
                                "	ON eiiex.InvoiceInputSysNO = eii.SysNO  " +
                                "WHERE eii.Status=0  " +
                                "   AND eiiex.Status=0 " +
                                "	AND eii.InvoiceInputNo=" + filter.InvoiceInputNo + "  " +
                                "   AND eiiex.InvoiceNumber=ei.InvoiceNumber)";
                cmd.CommandText += subSql;
                sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, subSql);

            }

            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 根据单据号查询发票信息
        public EIMSInvoiceInfo QueryInvoiceList(string invoiceNumber)
        {
            EIMSInvoiceInfo entity = new EIMSInvoiceInfo();

            entity.EIMSList = EIMSListQuery(invoiceNumber);

            string invoiceNumberStr = string.Empty;

            for (int i = 0; i < entity.EIMSList.Count; i++)
            {
                var extend = entity.EIMSList[i].EIMSInvoiceInputExtendList;
                if (extend == null || extend.Count == 0)
                {
                    continue;
                }
                for (int j = 0; j < extend.Count; j++)
                {
                    if (!string.IsNullOrEmpty(invoiceNumberStr))
                    {
                        invoiceNumberStr += ".";
                    }
                    invoiceNumberStr += extend[j].InvoiceNumber;
                }
            }

            EIMSInvoiceEntryQueryFilter filter = new EIMSInvoiceEntryQueryFilter()
            {
                InvoiceNumber = invoiceNumberStr,
                PagingInfo = new PagingInfo { PageIndex = 0, PageSize = int.MaxValue }
            };

            entity.InvoiceInfoList = InvoiceInfoListQuery(filter);

            entity.OldEIMSList = entity.EIMSList;

            return entity;
        }

        /// <summary>
        /// 根据单据号查询发票信息
        /// </summary>
        /// <param name="invoiceNo">单据号</param>
        /// <returns></returns>
        public List<EIMSInvoiceInfoEntity> EIMSListQuery(string invoiceNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EIMS_Query_EIMSList");

            command.SetParameterValue("@InvoiceNumber", invoiceNo);

            List<EIMSInvoiceInfoEntity> list = command.ExecuteEntityList<EIMSInvoiceInfoEntity>();
            for (int i = 0; i < list.Count; i++)
            {
                int sysno = list[i].SysNo;
                list[i].EIMSInvoiceInputExtendList = QueryEIMSInvoiceInputExtend(sysno);
            }
            return list;
        }

        public List<EIMSInvoiceInputExtendInfo> QueryEIMSInvoiceInputExtend(int invoiceInputSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EIMS_Query_EIMSInvoiceInputExtend");

            command.SetParameterValue("@InvoiceInputSysNo", invoiceInputSysNo);

            return command.ExecuteEntityList<EIMSInvoiceInputExtendInfo>();
        }
        #endregion

        #region 根据发票号查询相关信息
        /// <summary>
        /// 根据发票号查询发票信息
        /// </summary>
        /// <param name="invoiceInputSysNos"></param>
        /// <returns></returns>
        public List<EIMSInvoiceInfoEntity> QueryEIMSInvoiceInputByInvoiceInputSysNo(List<string> invoiceInputSysNos)
        {
            if (invoiceInputSysNos == null
                || invoiceInputSysNos.Count == 0)
            {
                return new List<EIMSInvoiceInfoEntity>();
            }
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("EIMS_Query_InvoiceListByInvoiceInputSysNo");
            string InvoiceInputNo = string.Empty;
            for (int i = 0; i < invoiceInputSysNos.Count; i++)
            {
                if (i > 0)
                {
                    InvoiceInputNo += ",";
                }
                InvoiceInputNo += string.Format("'{0}'", invoiceInputSysNos[0]);
            }
            command.CommandText = command.CommandText.Replace("#InvoiceInputNo#", InvoiceInputNo);
            List<EIMSInvoiceInfoEntity> list = new List<EIMSInvoiceInfoEntity>();
            using (IDataReader reader = command.ExecuteDataReader())
            {
                EIMSInvoiceInfoEntity model = null;
                while (reader.Read())
                {
                    model = new EIMSInvoiceInfoEntity();
                    model.SysNo = Convert.ToInt32(reader["SysNo"]);
                    model.InvoiceInputNo = reader["InvoiceInputNo"].ToString();
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据发票号查询单据信息
        /// </summary>
        /// <param name="invoiceInputSysNos"></param>
        /// <returns></returns>
        public List<EIMSInvoiceInputExtendInfo> QueryEIMSInvoiceInputExtendByInvoiceInputSysNo(params int[] invoiceInputSysNos)
        {
            if (invoiceInputSysNos == null
               || invoiceInputSysNos.Length == 0)
            {
                return new List<EIMSInvoiceInputExtendInfo>();
            }
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("EIMS_Query_ExtendListByInvoiceInputSysNo");
            string InvoiceInputSysNo = string.Empty;
            for (int i = 0; i < invoiceInputSysNos.Length; i++)
            {
                if (i > 0)
                {
                    InvoiceInputSysNo += ",";
                }
                InvoiceInputSysNo += string.Format("{0}", invoiceInputSysNos[i]);
            }
            command.CommandText = command.CommandText.Replace("#InvoiceInputSysNo#", InvoiceInputSysNo);
            List<EIMSInvoiceInputExtendInfo> list = new List<EIMSInvoiceInputExtendInfo>();
            using (IDataReader reader = command.ExecuteDataReader())
            {
                EIMSInvoiceInputExtendInfo model = null;
                while (reader.Read())
                {
                    model = new EIMSInvoiceInputExtendInfo();
                    model.SysNo = Convert.ToInt32(reader["SysNo"]);
                    model.InvoiceNumber = reader["InvoiceNumber"].ToString();
                    model.InvoiceInputSysNo = Convert.ToInt32(reader["InvoiceInputSysNo"]);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region 录入发票信息
        public void CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EIMS_Create_InvoiceInput");
            string sql = BuildSql(entities, cmd);
            cmd.CommandText = sql;
            cmd.ExecuteScalar();

            //写入日志表
            List<EIMSInvoiceInfoEntity> list = entities;
            List<string> invoiceNumberList = new List<string>();
            list.ForEach(entity =>
            {
                entity.EIMSInvoiceInputExtendList.ForEach(ex =>
                {
                    if (!invoiceNumberList.Contains(ex.InvoiceNumber))
                    {
                        invoiceNumberList.Add(ex.InvoiceNumber);
                        int invoiceNumber = 0;
                        int.TryParse(ex.InvoiceNumber, out invoiceNumber);
                        this.InsertEIMSEventLog(invoiceNumber, 196, entity.CurrentUser, string.Format("给单据{0}录入发票信息", invoiceNumber));
                    }
                });
            });
        }

        private string BuildSql(List<EIMSInvoiceInfoEntity> entities, CustomDataCommand cmd)
        {
            DateTime now = DateTime.Now;
            string InvoiceInputSql = " INSERT INTO EIMS.dbo.EIMSInvoiceInput " +
                                "(" +
                                "	VendorNumber," +
                                "	InvoiceInputNo," +
                                "	InvoiceDate," +
                                "	InvoiceInputAmount," +
                                "	TaxRate," +
                                "	InvoiceInputDateTime," +
                                "	InvoiceInputUser," +
                                "	InvoiceEditDateTime," +
                                "	InvoiceEditUser," +
                                "	Memo," +
                                "	Status" +
                                " ) " +
                                " VALUES" +
                                "(" +
                                "	@VendorNumber{0}," +
                                "	@InvoiceInputNo{0}," +
                                "	@InvoiceDate{0}," +
                                "	@InvoiceInputAmount{0}," +
                                "	@TaxRate{0}," +
                                "	@InvoiceInputDateTime{0}," +
                                "	@InvoiceInputUser{0}," +
                                "	@InvoiceEditDateTime{0}," +
                                "	@InvoiceEditUser{0}," +
                                "	@Memo{0}," +
                                "	@Status{0}" +
                                "); ";
            string IdentitySql = " SET @InvoiceInputSysNo = @@IDENTITY; ";
            string InvoiceInputExSql = " INSERT INTO EIMS.dbo.EIMSInvoiceInput_Ex" +
                                "(" +
                                "	InvoiceNumber," +
                                "	InvoiceInputSysNO," +
                                "	Status" +
                                ") " +
                                " VALUES" +
                                "(" +
                                "	@InvoiceNumber{0}," +
                                "	@InvoiceInputSysNo," +
                                "	@Ex_Status{0}" +
                                "); ";
            StringBuilder sql = new StringBuilder();
            sql.Append(" DECLARE @InvoiceInputSysNo int " +
                        " BEGIN TRANSACTION ");
            int i = 0;
            int j = 0;
            entities.ForEach(entity =>
            {
                if (entity.EIMSInvoiceInputExtendList == null
                    || entity.EIMSInvoiceInputExtendList.Count == 0)
                {
                    throw new Exception(string.Format("EIMSInvoiceInput_Ex数据不能为空,发票号:{0}", entity.InvoiceInputNo));
                }
                sql.AppendFormat(InvoiceInputSql, i);
                cmd.AddInputParameter(string.Format("@VendorNumber{0}", i), DbType.Int32, entity.VendorNumber);
                cmd.AddInputParameter(string.Format("@InvoiceInputNo{0}", i), DbType.String, entity.InvoiceInputNo);
                cmd.AddInputParameter(string.Format("@InvoiceDate{0}", i), DbType.DateTime, entity.InvoiceDate);
                cmd.AddInputParameter(string.Format("@InvoiceInputAmount{0}", i), DbType.Decimal, entity.InvoiceInputAmount);
                cmd.AddInputParameter(string.Format("@TaxRate{0}", i), DbType.Decimal, entity.TaxRate);
                cmd.AddInputParameter(string.Format("@InvoiceInputDateTime{0}", i), DbType.DateTime, entity.InvoiceInputDateTime ?? now);
                cmd.AddInputParameter(string.Format("@InvoiceInputUser{0}", i), DbType.String, entity.InvoiceInputUser);
                cmd.AddInputParameter(string.Format("@InvoiceEditDateTime{0}", i), DbType.DateTime, entity.InvoiceEditDateTime);
                cmd.AddInputParameter(string.Format("@InvoiceEditUser{0}", i), DbType.String, entity.InvoiceEditUser);
                cmd.AddInputParameter(string.Format("@Memo{0}", i), DbType.String, entity.Memo);
                cmd.AddInputParameter(string.Format("@Status{0}", i), DbType.Int32, entity.Status);

                sql.Append(IdentitySql);

                entity.EIMSInvoiceInputExtendList.ForEach(extend =>
                {
                    sql.AppendFormat(InvoiceInputExSql, j);
                    cmd.AddInputParameter(string.Format("@InvoiceNumber{0}", j), DbType.String, extend.InvoiceNumber);
                    cmd.AddInputParameter(string.Format("@Ex_Status{0}", j), DbType.Int32, extend.Status);
                    j++;
                });

                i++;
            });
            sql.Append("COMMIT TRANSACTION ");
            return sql.ToString();
        }
        #endregion

        #region 修改发票信息
        public void UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                entity.InvoiceEditDateTime = DateTime.Now;
                entity.InvoiceEditUser = entities[i].InvoiceInputUser;
                UpdateInvoiceInput(entity);
                if (entity.EIMSInvoiceInputExtendList != null
                    && entity.EIMSInvoiceInputExtendList.Count > 0)
                {
                    for (int j = 0; j < entity.EIMSInvoiceInputExtendList.Count; j++)
                    {
                        UpdateInvoiceInputExtend(entity.EIMSInvoiceInputExtendList[j]);
                    }
                }
            }
            List<EIMSInvoiceInfoEntity> list = entities;
            List<string> invoiceNumberList = new List<string>();
            list.ForEach(entity =>
            {
                entity.EIMSInvoiceInputExtendList.ForEach(ex =>
                {
                    if (!invoiceNumberList.Contains(ex.InvoiceNumber))
                    {
                        invoiceNumberList.Add(ex.InvoiceNumber);
                        int invoiceNumber = 0;
                        int.TryParse(ex.InvoiceNumber, out invoiceNumber);
                        this.InsertEIMSEventLog(invoiceNumber, 197, entity.CurrentUser, string.Format("修改单据{0}发票信息", invoiceNumber));
                    }
                });
            });
        }

        public void UpdateInvoiceInput(EIMSInvoiceInfoEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EIMS_Update_InvoiceInput");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@InvoiceDate", entity.InvoiceDate);
            command.SetParameterValue("@InvoiceEditDateTime", entity.InvoiceEditDateTime);
            command.SetParameterValue("@InvoiceEditUser", entity.InvoiceEditUser);
            command.SetParameterValue("@InvoiceInputAmount", entity.InvoiceInputAmount);
            command.SetParameterValue("@InvoiceInputNo", entity.InvoiceInputNo);
            command.SetParameterValue("@Memo", entity.Memo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@TaxRate", entity.TaxRate);
            command.SetParameterValue("@VendorNumber", entity.VendorNumber);

            command.ExecuteNonQuery();
        }

        public void UpdateInvoiceInputExtend(EIMSInvoiceInputExtendInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EIMS_Update_InvoiceInputExtend");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@InvoiceInputSysNo", entity.InvoiceInputSysNo);
            command.SetParameterValue("@InvoiceNumber", entity.InvoiceNumber);
            command.SetParameterValue("@Status", entity.Status);
            command.ExecuteNonQuery();
        }

        #endregion

        #region 记录日志
        public int InsertEIMSEventLog(int OrderNumber, int EventType, string UserID, string EventMemo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EIMS_Insert_EIMSEventLog");
            command.SetParameterValue("OrderNumber", OrderNumber);
            command.SetParameterValue("EventType", EventType);
            command.SetParameterValue("UserID", UserID);
            command.SetParameterValue("EventMemo", EventMemo);

            int result = 0;
            result = Convert.ToInt32(command.ExecuteScalar());

            return result;
        }
        #endregion

        #region 映射排序字段
        private static void MapSortField(EIMSInvoiceEntryQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortField = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "AssignedCode":
                        filter.PagingInfo.SortBy = sortField.Replace("AssignedCode", "ei.AssignedCode");
                        break;
                    case "InvoiceNumber":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceNumber", "ei.InvoiceNumber");
                        break;
                    case "InvoiceName":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceName", "ei.InvoiceName");
                        break;
                    case "RuleAssignedCode":
                        filter.PagingInfo.SortBy = sortField.Replace("RuleAssignedCode", "er.AssignedCode");
                        break;
                    case "VendorNumber":
                        filter.PagingInfo.SortBy = sortField.Replace("VendorNumber", "ei.VendorNumber");
                        break;
                    case "VendorName":
                        filter.PagingInfo.SortBy = sortField.Replace("VendorName", "vendor.VendorName");
                        break;
                    case "ReceiveType":
                        filter.PagingInfo.SortBy = sortField.Replace("ReceiveType", "ei.ReceiveType");
                        break;
                    case "EIMSType":
                        filter.PagingInfo.SortBy = sortField.Replace("EIMSType", "ei.EIMSType");
                        break;
                    case "InvoiceAmount":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceAmount", "ei.InvoiceAmount");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortField.Replace("Status", "ei.[Status]");
                        break;
                    case "CreateDate":
                        filter.PagingInfo.SortBy = sortField.Replace("CreateDate", "ei.CreateDate");
                        break;
                    case "ApproveDate":
                        filter.PagingInfo.SortBy = sortField.Replace("ApproveDate", "ei.ApproveDate");
                        break;
                    case "PM":
                        filter.PagingInfo.SortBy = sortField.Replace("PM", "PM.UserName");
                        break;
                }
            }
        }
        #endregion
    }
}
