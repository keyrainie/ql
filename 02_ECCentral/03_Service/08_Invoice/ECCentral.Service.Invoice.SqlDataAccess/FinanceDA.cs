using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IFinanceDA))]
    public class FinanceDA:IFinanceDA
    {
        #region NoBizQuery
        #region 应付款汇总查询
        public DataTable FinanceQuery(FinanceQueryFilter filter, out int totalCount, out double totalPayAmt)
        {
            string sqlName = string.Empty;
            string orderStr = "SysNo DESC";
            if (filter.IsGroupByVendor == true)
            {
                sqlName = "Invoice_Query_FinanceGroupByVendor";
            }
            else
            {
                sqlName = "Invoice_Query_Finance";
                if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
                {
                    orderStr = filter.PagingInfo.SortBy;
                }
            }
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig(sqlName);
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), orderStr))
            {
                cmd.CommandText = sb.BuildQuerySql();
                if (filter.IsGroupByVendor == true)
                {
                    AddFinanceParametersGroupByVendor(cmd, filter);
                }
                else
                {
                    AddFinanceParameters(filter, cmd);
                }

                //合计已到应付总额
                cmd.AddOutParameter("@TotalPayableAmt", DbType.Double, 12);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                if (filter.IsGroupByVendor == false
                    || filter.IsGroupByVendor == null)
                {
                    enumColList.Add("AuditStatus", typeof(PayableAuditStatus));
                    //codeNameColList.Add("OrderType", "Invoice", "OrderType");
                    enumColList.Add("OrderType", typeof(PayableOrderType));
                    //codeNameColList.Add("OrderStatus", "Invoice", "OrderStatus");
                    codeNameColList.Add("InvoiceStatus", "Invoice", "InvoiceStatus");
                    codeNameColList.Add("IsConsign", "Invoice", "VendorType");

                }
                else
                {
                    codeNameColList.Add("IsConsign", "Invoice", "VendorType");
                }
                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                var totalPayableAmtParam = cmd.GetParameterValue("@TotalPayableAmt");
                if (totalPayableAmtParam != DBNull.Value)
                {
                    totalPayAmt = Convert.ToDouble(totalPayableAmtParam);
                }
                else
                {
                    totalPayAmt = 0.0;
                }

                return dt;
            }
        }

        /// <summary>
        /// 非按供应商汇总
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cmd"></param>
        /// <param name="sb"></param>
        private void AddFinanceParameters(FinanceQueryFilter filter, CustomDataCommand cmd)
        {
            #region 生成条件字典
            Dictionary<string, StringBuilder> whereDic
                = new Dictionary<string, StringBuilder>();
            string[] keys1 = @"0;1;10;11;12".Split(';');//正数应付款
            string[] keys2 = @"0_1;5_1;1_1;8_1;9_1;10_1;11_1;12_1".Split(';');//负数应付款
            string[] keys3 = @"7".Split(';');//票扣

            string[] keys4 = "0;0_1;1;1_1;5_1;12;12_1".Split(';');//使用PM3 “OrderType IN (0,1,5)”
            string[] keys5 = "7;8_1".Split(';');//使用PM1 “OrderType IN (7,8)”
            string[] keys6 = "9_1".Split(';');//使用PM2 “OrderType IN (9)”
            string[] keys7 = "0;0_1;1;1_1".Split(';');   //使用产品线

            string[] keys = new string[keys1.Length + keys2.Length + keys3.Length];
            Array.Copy(keys1, 0, keys, 0, keys1.Length);
            Array.Copy(keys2, 0, keys, keys1.Length, keys2.Length);
            Array.Copy(keys3, 0, keys, keys1.Length + keys2.Length, keys3.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                whereDic[keys[i]] = CreateDefaultStringBuilder();
            }
            #endregion

            #region 公共条件

            if (!filter.IsMangerPM && filter.OperationUserSysNo!=null)
            {
                string result = string.Empty;
                List<ProductLineEntity> ProductLineList = GetProductLineByPMUserSysNo(filter.OperationUserSysNo);
                if (ProductLineList != null && ProductLineList.Count > 0)
                {
                    result = string.Join(",", (from x in ProductLineList                                                
                                               select x.ProductLineSysNo.ToString()).ToArray());

                    string str = "";
                    str += " AND EXISTS ( SELECT TOP 1 1 FROM   ( SELECT  F1 AS ProductLineSysNo";
                    str += " FROM IPP3.dbo.f_splitstr(@productLineList, ',') ) PLP";
                    str += " INNER JOIN (";
                    str += " SELECT F1 AS ProductLineSysNo FROM   IPP3.dbo.f_splitstr(biz.ProductLineSysNoList, ',')";
                    str += " ) PLT ON PLP.ProductLineSysNo = PLT.ProductLineSysNo )";
                    InputParameter(whereDic, string.Format(str, result), keys5);
                    InputParameter(whereDic, string.Format(str.Replace("biz.ProductLineSysNoList", "biz.ProductLineSysno"), result), keys7);
                    cmd.AddInputParameter("@productLineList", DbType.String, result);
                }
                else
                {
                    InputParameter(whereDic, string.Format("AND 1=2 ", result), keys5);
                    InputParameter(whereDic, string.Format("AND 1=2 ", result), keys7);
                }

            }



            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);
            if (!string.IsNullOrEmpty(filter.PMUserSysNo))
            {
                InputParameter(whereDic, string.Format("AND biz.PMSysNo IN ({0}) ", filter.PMUserSysNo), keys4);
                InputParameter(whereDic, string.Format("AND biz.PM IN ({0}) ", filter.PMUserSysNo), keys5);
                InputParameter(whereDic, string.Format("AND biz.PMUserSysNo IN ({0}) ", filter.PMUserSysNo), keys6);
            }
            if (filter.VendorNo.HasValue)
            {
                InputParameter(whereDic, @"AND fp.VendorSysNo = @VendorSysNo ");
                cmd.AddInputParameter("@VendorSysNo", DbType.Int32, filter.VendorNo.Value);
            }
            if (!string.IsNullOrEmpty(filter.AuditStatus))
            {
                InputParameter(whereDic, @"AND fp.AuditStatus = @AuditStatus ");
                cmd.AddInputParameter("@AuditStatus", DbType.StringFixedLength, filter.AuditStatus);
            }
            //付款结算公司
            if (filter.PaySettleCompany.HasValue)
            {
                InputParameter(whereDic, @"AND v.PaySettleCompany = @PaySettleCompany ");
                cmd.AddInputParameter("@PaySettleCompany", DbType.Int32, filter.PaySettleCompany.Value);
            }
            if (!string.IsNullOrEmpty(filter.VendorName))
            {
                InputParameter(whereDic, @"AND v.VendorName LIKE @VendorName ");
                cmd.AddInputParameter("@VendorName", DbType.String, '%' + filter.VendorName + '%');
            }
            if (filter.VendorPayPeriod.HasValue)
            {
                InputParameter(whereDic, @"AND v.PayPeriodType = @PayTypeSysNo ");
                cmd.AddInputParameter("@PayTypeSysNo", DbType.Int32, filter.VendorPayPeriod.Value);
            }
            if (!string.IsNullOrEmpty(filter.InvoiceID))
            {
                string orderIdString = string.Empty;
                string[] orderIDArray = filter.InvoiceID.Trim(new char[] { '.' }).Split('.');
                for (var i = 0; i < orderIDArray.Length; i++)
                {
                    orderIdString += orderIDArray[i] + ",";
                }
                if (orderIdString.Length > 0)
                {
                    orderIdString = orderIdString.Substring(0, orderIdString.Length - 1);
                }

                string orderIDCondition = string.Format("AND fp.OrderSysNo in ({0}) ", orderIdString);
                InputParameter(whereDic, orderIDCondition);
            }
            #endregion

            #region 特殊逻辑处理
            //排除被锁定的
            InputParameter(whereDic, @"AND NOT EXISTS (SELECT top 1 1 FROM ipp3.dbo.Finance_Pay_Item fpi WITH(NOLOCK) " +
                        @" where fp.SysNo = fpi.PaySysNo and fpi.[Status]=2) ");


            //对于正数的应付款（正数表明是泰隆优选需要支付给商家），需要发票完整且在查询时间范围内已经达到预计付款时间的，才能查询出来            
            if (filter.ApplyDateFrom.HasValue)
            {
                InputParameter(whereDic, @"AND fp.ETP > @DateFrom ", keys1);
                cmd.AddInputParameter("@DateFrom", DbType.DateTime, filter.ApplyDateFrom.Value);
            }
            //结束时间默认今天23:59:59
            if (!filter.ApplyDateTo.HasValue)
            {
                filter.ApplyDateTo = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            InputParameter(whereDic, @"AND fp.ETP <= @DateTo ", keys1);
            cmd.AddInputParameter("@DateTo", DbType.DateTime, filter.ApplyDateTo.Value);

            InputParameter(whereDic, "AND fp.InvoiceStatus = 2 ", keys1);//增加发票完整性约束

            //票扣，需要发票状态完整，但无论是否达到预计付款时间，都查询出来
            InputParameter(whereDic, "AND fp.InvoiceStatus = 2 ", keys3);//增加发票完整性约束

            //对于负数的应付款（负数表明是泰隆优选需要从商家哪里扣除的款项），除票扣以外，无论是否达到了预计付款时间，无论发票是否完整，都查询出来
            //不加特殊筛选条件
            #endregion

            cmd.CommandText = BuidSql(cmd.CommandText, whereDic);
        }


         #region CRL21776 by Lily
        private List<ProductLineEntity> GetProductLineByPMUserSysNo(int OperationUserSysNo)
        {          
            if (OperationUserSysNo != 0)
            {
                List<ProductLineEntity> result = new List<ProductLineEntity>();
                DataCommand dataCommand = DataCommandManager.GetDataCommand("GetProductLineByPMUserSysNo");
                dataCommand.SetParameterValue("@PMUserSysNo", OperationUserSysNo);
                result = dataCommand.ExecuteEntityList<ProductLineEntity>().ToList();
                return result;
            }
            else
            {
                return null;
            }
        }
       
        #endregion

        /// <summary>
        /// 按供应商汇总
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="filter"></param>
        private void AddFinanceParametersGroupByVendor(CustomDataCommand cmd, FinanceQueryFilter filter)
        {
            //所有条件字典
            Dictionary<string, StringBuilder> whereDic = new Dictionary<string, StringBuilder>();

            #region 数据初始化

            #region #item 临时表条件
            string[] keys2 = "0;1;10;11;12".Split(';');//正数
            string[] keys3 = "0_1;5_1;1_1;8_1;9_1;10_1;11_1;12_1".Split(';');//负数
            string[] keys4 = "7".Split(';');// 票扣
            string[] keys5 = "10;10_1;11;11_1".Split(';');//NO PM
            string[] keys1 = new string[keys2.Length + keys3.Length + keys4.Length];//DATA视图的筛选条件
            #endregion

            #region #it 临时表条件
            string keys12 = "101";//正数
            string keys13 = "101_1";//负数
            string keys14 = "101->7";//票扣
            string[] keys11 = new string[] { keys12, keys13, keys14 };
            #endregion

            #region 查询结果中的条件
            //--已到应付
            string keys22 = "102";//正数
            string keys22_1 = "102_1";//负数
            string keys22_7 = "102->7";//票扣
            //--未到应付
            string keys23 = "103";//正数
            string keys23_1 = "103_1";//负数
            string keys23_7 = "103->7";//票扣
            //--余额
            string keys24 = "104";//正数
            string keys24_1 = "104_1";//负数
            string keys24_7 = "104->7";//票扣
            //--锁定应付
            string keys25 = "105";//正数
            string keys25_1 = "105_1";//负数
            string keys25_7 = "105->7";//票扣

            //string keys26 = "106";//正数
            //string keys26_1 = "106_1";//负数
            //string keys26_7 = "106->7";//票扣
            string[] keys21 = new string[] { keys22, keys22_1, keys22_7, keys23, keys23_1, keys23_7,
                                        keys24, keys24_1, keys24_7, keys25, keys25_1, keys25_7, 
                                        //keys26, keys26_1, keys26_7
            };
            #endregion

            Array.Copy(keys2, 0, keys1, 0, keys2.Length);
            Array.Copy(keys3, 0, keys1, keys2.Length, keys3.Length);
            Array.Copy(keys4, 0, keys1, keys2.Length + keys3.Length, keys4.Length);

            //所有条件key集合
            string[] keys = new string[keys1.Length + keys11.Length + keys21.Length];
            Array.Copy(keys1, 0, keys, 0, keys1.Length);
            Array.Copy(keys11, 0, keys, keys1.Length, keys11.Length);
            Array.Copy(keys21, 0, keys, keys1.Length + keys11.Length, keys21.Length);

            foreach (string key in keys)
            {
                if (whereDic.ContainsKey(key))
                {
                    continue;
                }
                whereDic[key] = new StringBuilder();
            }
            #endregion

            #region 拼装条件

            //ETP结束时间默认今天23:59:59
            if (!filter.ApplyDateTo.HasValue)
            {
                filter.ApplyDateTo = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            #region #item 临时表的条件拼装
            foreach (string key in keys1)
            {
                string defaultWhereString = CreateDefaultStringBuilder().ToString();
                whereDic[key].Append(defaultWhereString);
            }
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);
            //审核状态
            if (!string.IsNullOrEmpty(filter.AuditStatus))
            {
                InputParameter(whereDic, "AND fp.AuditStatus = @AuditStatus ", keys1);
                cmd.AddInputParameter("@AuditStatus", DbType.StringFixedLength, filter.AuditStatus);
            }
            //付款结算公司
            if (filter.PaySettleCompany.HasValue)
            {
                InputParameter(whereDic, @"AND fp.PaySettleCompany = @PaySettleCompany ");
                cmd.AddInputParameter("@PaySettleCompany", DbType.Int32, filter.PaySettleCompany.Value);
            }
            //供应商
            if (filter.VendorNo.HasValue)
            {
                InputParameter(whereDic, "AND fp.VendorSysNo = @VendorSysNo ", keys1);
                cmd.AddInputParameter("@VendorSysNo", DbType.Int32, filter.VendorNo.Value);
            }
            //供应商帐期
            if (filter.VendorPayPeriod.HasValue)
            {
                InputParameter(whereDic, "AND fp.PayPeriodType = @PayTypeSysNo ", keys1);
                cmd.AddInputParameter("@PayTypeSysNo", DbType.Int32, filter.VendorPayPeriod.Value);
            }
            if (!string.IsNullOrEmpty(filter.VendorName))
            {
                InputParameter(whereDic, "AND fp.VendorName like @VendorName", keys1);
                cmd.AddInputParameter("@VendorName", DbType.String, '%' + filter.VendorName + '%');
            }
            //PM
            if (!string.IsNullOrEmpty(filter.PMUserSysNo))
            {
                foreach (string key in keys1)
                {
                    //排除没有PM的数据
                    if (keys5.Contains(key))
                    {
                        continue;
                    }
                    InputParameter(whereDic, string.Format("AND fp.PMSysNo IN ({0})",filter.PMUserSysNo), key);
                }
            }

            #endregion

            #region #it 临时表条件拼装
            //ETP开始时间
            if (filter.ApplyDateFrom.HasValue)
            {
                InputParameter(whereDic, "AND (fp.ETP > @DateFrom OR fp.ETP IS NULL)", keys12);
                cmd.AddInputParameter("@DateFrom", DbType.DateTime, filter.ApplyDateFrom.Value);
            }

            if (filter.ApplyDateTo.HasValue)
            {
                InputParameter(whereDic, "AND (fp.ETP <= @DateTo  OR fp.ETP IS NULL)", keys12);
                cmd.AddInputParameter("@DateTo", DbType.DateTime, filter.ApplyDateTo.Value);
            }
            ////正数、票扣需要发票状态完整
            //InputParameter(whereDic, "AND fp.InvoiceStatus = 2 ", keys12, keys14);

            //if (filter.ApplyDateFrom.HasValue)
            //{
            //    InputParameter(whereDic, " AND ((fp.ETP > @DateFrom AND fp.ETP <= @DateTo) OR fp.ETP is null)", keys12, keys26);
            //    cmd.AddInputParameter("@DateFrom", DbType.DateTime, filter.ApplyDateFrom.Value);
            //    cmd.AddInputParameter("@DateTo", DbType.DateTime, filter.ApplyDateTo.Value);
            //}
            //else
            //{
            //    InputParameter(whereDic, " AND (fp.ETP <= @DateTo OR fp.ETP is null)", keys12, keys26);
            //    cmd.AddInputParameter("@DateTo", DbType.DateTime, filter.ApplyDateTo.Value);
            //}


            //InputParameter(whereDic, "AND fp.InvoiceStatus = 2 ", keys14, keys26_7);
            //InputParameter(whereDic, @"AND NOT EXISTS (SELECT top 1 1 FROM ipp3.dbo.Finance_Pay_Item fpi WITH(NOLOCK) " +
            //@" where fp.SysNo = fpi.PaySysNo and fpi.[Status]=2) ", keys26, keys26_1, keys26_7);


            #endregion

            #region 查询结果中的条件拼装

            #region //--已到应付
            //ETP开始时间
            if (filter.ApplyDateFrom.HasValue)
            {
                InputParameter(whereDic, "AND fp.ETP > @DateFrom1 ", keys22);
                cmd.AddInputParameter("@DateFrom1", DbType.DateTime, filter.ApplyDateFrom.Value);
            }
            if (filter.ApplyDateTo.HasValue)
            {
                InputParameter(whereDic, "AND fp.ETP <= @DateTo1 ", keys22);
                cmd.AddInputParameter("@DateTo1", DbType.DateTime, filter.ApplyDateTo.Value);
            }
            //正数、票扣需要发票状态完整
            InputParameter(whereDic, "AND fp.InvoiceStatus = 2 ", keys22, keys22_7);

            //排除被锁定的
            InputParameter(whereDic, @"AND NOT EXISTS (SELECT top 1 1 FROM ipp3.dbo.Finance_Pay_Item fpi WITH(NOLOCK) " +
                        @" where fp.SysNo = fpi.PaySysNo and fpi.[Status]=2) ", keys22, keys22_1, keys22_7);
            #endregion

            #region //--未到应付
            //ETP开始时间
            bool hasETP = false;
            if (filter.ApplyDateFrom.HasValue)
            {
                InputParameter(whereDic, "AND ( ( fp.ETP < @DateFrom2 ", keys23);
                cmd.AddInputParameter("@DateFrom2", DbType.DateTime, filter.ApplyDateFrom.Value);
                hasETP = true;
            }
            if (filter.ApplyDateTo.HasValue)
            {
                string dateTo = string.Format("{0} {1}fp.ETP > @DateTo2 OR fp.ETP IS NULL) ",
                    filter.ApplyDateFrom.HasValue ? "OR" : "AND",
                    filter.ApplyDateFrom.HasValue ? string.Empty : "( (");

                InputParameter(whereDic, dateTo, keys23);
                cmd.AddInputParameter("@DateTo2", DbType.DateTime, filter.ApplyDateTo.Value);
                hasETP = true;
            }

            //正数、票扣发票状态不完整的计入未到应付
            InputParameter(whereDic,
                string.Format(" {0} fp.InvoiceStatus <> 2 {1} ",
                            hasETP ? "OR" : "AND",
                            hasETP ? ")" : string.Empty),
                    keys23);

            //负数的默认都为已到应付
            InputParameter(whereDic, " AND 1 <> 1 ", keys23_1);

            //票扣中状态不完整的统计为未到应付
            InputParameter(whereDic,
                string.Format(" AND fp.InvoiceStatus <> 2 "), keys23_7);

            //排除被锁定的
            InputParameter(whereDic, @"AND NOT EXISTS (SELECT top 1 1 FROM ipp3.dbo.Finance_Pay_Item fpi WITH(NOLOCK) " +
                        @" where fp.SysNo = fpi.PaySysNo and fpi.[Status]=2) ", keys23, keys23_1, keys23_7);
            #endregion

            //--余额
            //--锁定应付
            //无条件
            #endregion
            #endregion

            //替换条件
            cmd.CommandText = BuidSql(cmd.CommandText, whereDic);
        }

        private StringBuilder CreateDefaultStringBuilder()
        {
            string testVendorSysNosStr = GetTestVendorSysNoString();
            string defaultWhere = string.Format(@" AND fp.CompanyCode = @CompanyCode " +
                                    @"AND fp.PayStatus in (0,1) " +
                                    @"AND fp.VendorSysNo NOT IN ({0}) ", testVendorSysNosStr);
            StringBuilder sb = new StringBuilder(defaultWhere);

            return sb;
        }

        private string GetTestVendorSysNoString()
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_GetTestVendorSysNos");

            string result = cmd.CommandText;
            if (string.IsNullOrEmpty(result))
            {
                result = "0";
            }
            return result.Trim();
        }

        private void InputParameter(Dictionary<string, StringBuilder> dic, string whereStr)
        {
            InputParameter(dic, whereStr, dic.Keys.ToArray());
        }

        private void InputParameter(Dictionary<string, StringBuilder> dic, string whereStr, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key].AppendLine(whereStr);
                }
            }
        }

        private string BuidSql(string oldSql, Dictionary<string, StringBuilder> whereDic)
        {
            foreach (string key in whereDic.Keys)
            {
                oldSql = oldSql.Replace(string.Format("/*#WHERESTR{0}#*/", key),
                    whereDic[key].ToString());
            }
            return oldSql;
        }

        /// <summary>
        /// 根据SysNo查询Memo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public DataTable GetMemoBySysNo(int? sysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_QueryBySysNo");
            cmd.AddInputParameter("@SysNo", DbType.Int32, sysNo);
            DataTable dt = cmd.ExecuteDataTable();

            EnumColumnList enumColList = new EnumColumnList();
            enumColList.Add("AuditStatus", typeof(PayableAuditStatus));
            cmd.ConvertColumn(dt, enumColList,null);

            return dt;
        }

        #endregion

        public DataTable FinanceExport(FinanceQueryFilter filter, out int totalCount, out double totalPayAmt)
        {
            string sqlName = string.Empty;
            string orderStr = "SysNo DESC";
            if (filter.IsGroupByVendor == true)
            {
                sqlName = "Invoice_Query_FinanceGroupByVendor";
            }
            else
            {
                sqlName = "Invoice_Query_Finance";
                if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
                {
                    orderStr = filter.PagingInfo.SortBy;
                }
            }
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig(sqlName);
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), orderStr))
            {
                cmd.CommandText = sb.BuildQuerySql();
                if (filter.IsGroupByVendor == true)
                {
                    AddFinanceParametersGroupByVendor(cmd, filter);
                }
                else
                {
                    AddFinanceParameters(filter, cmd);
                }

                //合计已到应付总额
                cmd.AddOutParameter("@TotalPayableAmt", DbType.Double, 12);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                var totalPayableAmtParam = cmd.GetParameterValue("@TotalPayableAmt");
                if (totalPayableAmtParam != DBNull.Value)
                {
                    totalPayAmt = Convert.ToDouble(totalPayableAmtParam);
                }
                else
                {
                    totalPayAmt = 0.0;
                }

                return dt;
            }
        }
        /// <summary>
        /// 获取PMGroup
        /// </summary>
        /// <returns></returns>
        public DataTable GetPMGroupList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Finance_Get_PMGroupList");
            return cmd.ExecuteDataTable();
        }
        #endregion

        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="sysNo">单据编号</param>
        /// <param name="memo">备注内容</param>
        public void AddMemo(int? sysNo,string memo,string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Invoice_Insert_Memo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Memo", memo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询历史备注
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>结果集合</returns>
        public List<PayableInfo> PayableQuery(PayableCriteriaInfo filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPayable");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(
                cmd.CommandText, cmd, null, "SysNo desc"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo",
                  DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, filter.OrderSysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType",
                 DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PayStatus",
                 DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, filter.PayStatus);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                 DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BatchNumber",
                 DbType.Int32, "@BatchNumber", QueryConditionOperatorType.Equal, filter.BatchNumber);
                cmd.CommandText = sb.BuildQuerySql();
                var result = cmd.ExecuteEntityList<PayableInfo>();
                return result;
            }
        }

        public List<PayItemInfo> QueryPayItems(PayableItemCriteriaInfo query)
        {
            int? orderType = null;
            if (query.OrderType.HasValue)
            {
                orderType = (int)query.OrderType;
            }
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPayItems");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PayStyle",
                  DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, query.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PaySysNo",
                 DbType.Int32, "@PaySysNo", QueryConditionOperatorType.Equal, query.PaySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status",
                 DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                 DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, orderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo",
               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var result = dataCommand.ExecuteEntityList<PayItemInfo>();
                return result;
            }
        }

        #region 映射排序字段
        private static void MapSortFiled(FinanceQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0,filter.PagingInfo.SortBy.Length - index);
                var sortFiled = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "PayableAmt":
                        filter.PagingInfo.SortBy = sortFiled.Replace("PayableAmt", "[OrderAmt] - [AlreadyPayAmt]");
                        break;
                    case "VdndorName":
                        filter.PagingInfo.SortBy = sortFiled.Replace("VendorName", "v.[VendorName]");
                        break;
                }
            }
        }
        #endregion
    }
}
