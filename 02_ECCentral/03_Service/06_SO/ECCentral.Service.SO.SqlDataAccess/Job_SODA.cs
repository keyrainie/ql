using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.SO.SqlDataAccess
{
    public partial class SODA : ISODA
    {
        #region 团购订单相关
        /// <summary>
        /// 根据团购编号取得所有订单中的团购商品列表
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <returns></returns>
        public List<SOItemInfo> GetGroupBuySOItemByGroupBuySysNo(int groupBuySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_GroupBuySOItemsByGroupGuySysNo");
            command.SetParameterValue("@ReferenceSysNo", groupBuySysNo);
            return command.ExecuteEntityList<SOItemInfo>();
        }

        /// <summary>
        /// 根据团购编号修改所有团购订单的商品处理状态
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        public void UpdateGroupBuySOItemSettlementStatusByGroupBuySysNo(int groupBuySysNo, SettlementStatus settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySOItemSettlementStatusByGroupBuySysNo");
            command.SetParameterValue("@ReferenceSysNo", groupBuySysNo);
            object status = null;
            EnumCodeMapper.TryGetCode(settlementStatus, out status);
            command.SetParameterValue("@SettlementStatus", status);
            command.ExecuteNonQuery();
        }
        //
        /// <summary>
        /// 根据团购编号修改所有团购订单的处理状态
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        public void UpdateGroupBuySOSettlementStatusByGroupBuySysNo(int groupBuySysNo, SettlementStatus settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySettlementStatusByGroupBuySysNo");
            command.SetParameterValue("@ReferenceSysNo", groupBuySysNo);
            object status = null;
            EnumCodeMapper.TryGetCode(settlementStatus, out status);
            command.SetParameterValue("@SettlementStatus", status);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改团购订单的价格信息
        /// </summary>
        /// <param name="groupBuySysNo">团购编号</param>
        /// <param name="settlementStatus">处理状态</param>
        public void UpdateGroupBuySOAmount(SOBaseInfo baseInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySOMaster");
            command.SetParameterValue(baseInfo);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 修改团购商品价格
        /// </summary>
        public void UpdateGroupBuyProduct(SOItemInfo product)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySOItemPrice");
            command.SetParameterValue(product);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 根据订单编号和团购商品编号修改团购订单和其团购商品处理状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="productSysNo">订单中的团购商品编号</param>
        /// <param name="settlementStatus">处理状态</param>
        public void UpdateGroupBuySOAndItemSettlementStatus(int soSysNo, int productSysNo, SettlementStatus settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySOAndItemSettlementStatus");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@SettlementStatus", settlementStatus);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 根据订单编号修改团购订单处理状态
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="settlementStatus"></param>
        public void UpdateGroupBuySOSettlementStatusBySOSysNo(int soSysNo, SettlementStatus settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_GroupBuySOSettlementStatus");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@SettlementStatus", settlementStatus);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 取得无效的团购订单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetInvalidGroupBuySOSysNoList(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_InvalidGroupBuySOSysNoList");
            command.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = command.ExecuteDataTable();
            List<int> soSysNoList = new List<int>();
            if (dt != null)
            {
                using (dt)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0] != DBNull.Value)
                        {
                            soSysNoList.Add((int)dr[0]);
                        }
                    }
                }
            }
            return soSysNoList;
        }
        /// <summary>
        /// 取得48小时内没有支付的团购订单
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<int> GetNotPayGroupBuySOSysNoList(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_NotPayGroupBuySO");
            command.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = command.ExecuteDataTable();
            List<int> soSysNoList = new List<int>();
            if (dt != null)
            {
                using (dt)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0] != DBNull.Value)
                        {
                            soSysNoList.Add((int)dr[0]);
                        }
                    }
                }
            }
            return soSysNoList;
        }
        #endregion

        #region FPCheck

        /// <summary>
        /// 获取待检验的FP列表
        /// </summary>
        /// <param name="totalCount">最大获取数据行数</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>FPCheck订单列表</returns>
        public List<SOInfo> GetFPCheckSOList(int totalCount, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_FPCheckList");
            command.CommandText = command.CommandText.Replace("#TOPCOUNT#", totalCount.ToString());
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<SOInfo>();
        }

        /// <summary>
        /// 获取在一定时间段物流拒收的订单
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>订单列表</returns>
        public List<SOInfo> GetAutoRMASOInfoListInTime(string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_AutoRMACustomerSysNos");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<SOInfo>();
        }

        /// <summary>
        /// 是否是恶意用户
        /// </summary>
        /// <param name="CustomerSysNo">用户唯一编号</param>
        /// <returns>是返回真，否则返回假</returns>
        public bool IsSpiteCustomer(int customSysNo, string companyCode)
        {
            object o = GetDubiousUserSingle(customSysNo, null, null, null, companyCode, 2);
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public bool IsNewCustom(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsNewCustomer");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            object result = command.ExecuteScalar();
            if (result == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否是拒收用户
        /// </summary>
        /// <param name="customSysNo"></param>
        /// <param name="addr"></param>
        /// <param name="cellPhone"></param>
        /// <param name="phone"></param>
        /// <param name="companyCode"></param>
        /// <param name="duType"></param>
        /// <returns></returns>
        public bool IsRejectionCustomer(int? customSysNo, string addr, string cellPhone, string phone, string companyCode)
        {
            object o = GetDubiousUserSingle(customSysNo, addr, cellPhone, phone, companyCode, 1);
            if (o == null)
            {
                return false;
            }
            return true;
        }

        public bool IsOccupyStockCustomer(int? customSysNo, string addr, string cellPhone, string phone, string companyCode)
        {
            object o = GetDubiousUserSingle(customSysNo, addr, cellPhone, phone, companyCode, 0);
            if (o == null)
            {
                return false;
            }
            return true;
        }

        object GetDubiousUserSingle(int? customSysNo, string addr, string cellPhone, string phone, string companyCode, int duType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_DubiousUserSingle");

            command.SetParameterValue("@Address", addr);
            command.SetParameterValue("@CellPhone", cellPhone);
            command.SetParameterValue("@Phone", phone);
            command.SetParameterValue("@CustomerSysNo", customSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@DuType", duType);
            return command.ExecuteScalar();
        }

        /// <summary>
        /// 是否是新用户拒收或者是用户拒收订单的比例超过限度
        /// </summary>
        /// <param name="CustomerSysNo">用户编号</param>
        /// <returns>有数据返回假，无返回真</returns>
        public bool IsNewRejectionCustomerB(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsNewRejectionCustomerB");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 统计一天内统一用户不同订单地址
        /// </summary>
        /// <param name="customerSysNo">用户编号</param>
        /// <returns>不同订单地址</returns>
        public int GetSOCount4OneDay(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOCount4OneDay");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            return command.ExecuteScalar<int>();
        }

        public bool IsNewOccupyStockCustomerA(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsNewOccupyStockCustomerA");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        public bool IsNewOccupyStockCustomerB(int CustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsNewOccupyStockCustomerB");

            command.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                return false;
            }
            return true;
        }

        public List<SOInfo> GetChuanHuoSOListByProduct(int productSysNo, string customerIPAddress, DateTime createTime, string companyCode)
        {
            List<SOInfo> result = new List<SOInfo>();

            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_ChuanHuoSOSysNoListByProduct");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CustomerIPAddress", customerIPAddress);
            command.SetParameterValue("@CreateTime", createTime);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<SOInfo>();
        }

        public List<SOInfo> GetChuanHuoSOListByC3(int c3No, string customerIPAddress, DateTime createTime, string companyCode)
        {
            List<SOInfo> result = new List<SOInfo>();

            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_ChuanHuoSOSysNoListByC3");
            command.SetParameterValue("@C3No", c3No);
            command.SetParameterValue("@CustomerIPAddress", customerIPAddress);
            command.SetParameterValue("@CreateTime", createTime);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<SOInfo>();
        }

        public List<SOInfo> GetDuplicatSOList(int productSysNo, int customerSysNo, DateTime createTime, string companyCode)
        {
            List<SOInfo> result = new List<SOInfo>();

            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_DuplicatSOList");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@CreateTime", createTime);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<SOInfo>();
        }

        public void UpdateMarkException(string duplicateSOSysNo, int productSysNo, string soSysNos)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Update_MarkException");
            command.CommandText = command.CommandText.Replace("#SOSysNos#", soSysNos);

            command.SetParameterValue("@DuplicateSOSysNo", duplicateSOSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.ExecuteNonQuery();
        }

        public List<SOInfo> GetChaoHuoSOList(string receiveCellPhone, string receivePhone, int hours, DateTime orderDatetime, int? pointPromotionFlag, int? shipPriceFlag, int? isVATFlag, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_ChaoHuoSOList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    command.CommandText
                   , command
                   , null
                   , "SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.[CompanyCode]", System.Data.DbType.StringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.[Status] IN (0,1,4)");
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceivePhone", System.Data.DbType.AnsiString, "@ReceivePhone1", QueryConditionOperatorType.Equal, receivePhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceivePhone", System.Data.DbType.AnsiString, "@ReceiveCellPhone1", QueryConditionOperatorType.Equal, receiveCellPhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceiveCellPhone", System.Data.DbType.AnsiString, "@ReceivePhone2", QueryConditionOperatorType.Equal, receivePhone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "SM.ReceiveCellPhone", System.Data.DbType.AnsiString, "@ReceiveCellPhone2", QueryConditionOperatorType.Equal, receiveCellPhone);
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                if (pointPromotionFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SM.PointPay > 0 OR (PC.PromotionValue IS NOT NULL AND PC.PromotionValue > 0))");
                }

                if (shipPriceFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.ShipPrice = 0");
                }

                if (isVATFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SM.IsVAT = 1");
                }

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SM.OrderDate<= DATEADD(hh, {1}, '{0}' )", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), hours));
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SM.OrderDate>= DATEADD(hh, {1}, '{0}' )", orderDatetime.ToString("yyyy-MM-dd hh:mm:ss"), (-1 * hours)));

                command.CommandText = sqlBuilder.BuildQuerySql();

                return command.ExecuteEntityList<SOInfo>();
            }
        }

        public void UpdateMarkFPStatus(string soSysNos, int isFPSO, string fpReason, bool isMarkRed)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Update_MarkFPStatus");
            command.CommandText = command.CommandText.Replace("#SOSysNos#", soSysNos);
            command.SetParameterValue("@IsFPSO", isFPSO);
            command.SetParameterValue("@FPReason", fpReason);
            if (isMarkRed)
            {
                command.SetParameterValue("@FPExtend", "RED");
            }
            else
            {
                command.SetParameterValue("@FPExtend", DBNull.Value);
            }

            command.ExecuteNonQuery();
        }

        public int CountNotLocalWHSOItem(int soSysNo, string localWH)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_COUNT_NotLocalWHSOItem");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@LocalWH", localWH);

            return command.ExecuteScalar<int>();
        }

        public void UpdateLocalWHMark(int soSysNo, string localWH, int targetStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_LocalWHMark");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@LocalWH", localWH);
            command.SetParameterValue("@TargetStatus", targetStatus);

            command.ExecuteNonQuery();
        }

        #endregion

        #region 审核订单通过发送邮件和短信以及更新数据库
        public void AuditSendMailAndUpdateSO(int soSysNo)
        {
            UpdateSOPassAutoAuditSendMessage(soSysNo);
        }

        /// <summary>
        /// 电子卡订单出库发送shipping消息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void CreateEGiftCardOrderInvoice(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateEGiftCardOrderInvoice");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

       /// <summary>
       /// 更新操作
       /// </summary>
       /// <param name="soSysNo">订单编号</param>
       /// <returns>影响的记录数</returns>
        public int UpdateSOPassAutoAuditSendMessage(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_PassAutoAuditSendMessage");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteNonQuery();
        }
        #endregion 审核订单通过发送邮件和短信以及更新数据库

        #region SendCPS

        public List<SOInfo> GetCPSList(string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_CPSSOList");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<SOInfo>();
        }

        public void InsertCPSLog(int soNumber, string targetUrl, string returnMsg,int returnReceived)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_CPSLog");

            command.SetParameterValue("@SOSysNo", soNumber);
            command.SetParameterValue("@SendUrl", targetUrl);
            command.SetParameterValue("@ReturnMsg", returnMsg);
            command.SetParameterValue("@Status", returnReceived);

            command.ExecuteNonQuery();
        }

        #endregion

        #region InternalMemoReport

        public List<SOInternalMemoInfo> GetInternalMemoReportList(DateTime startDate, DateTime endDate,string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_InternalMemoReportList");
            command.SetParameterValue("@StartTime", startDate);
            command.SetParameterValue("@EndTime", endDate);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<SOInternalMemoInfo>();
        }

        #endregion

        #region AutoAuditSO

        public List<SOInfo> GetSOList4Audit(int topCount, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOInfo");
            StringBuilder sbCondition = new StringBuilder();
            //Jin：增加了必须是有成功的支付记录的SO，才能通过自动审核，避免了未支付订单反复拿出来审核
            sbCondition.Append(" INNER JOIN ipp3.dbo.Finance_NetPay netpay WITH(NOLOCK) ON m.SysNo=netpay.SOSysNo ");

            sbCondition.Append("WHERE m.Status = 0 "); 
            
            //Jin Test数据
            //sbCondition.Append(" AND m.SysNo in (130006599) ");
            //这里获取的时候直接获取订单减少数据提取额，增加速度
            sbCondition.Append(" AND m.SOAmt > 0 ");
            sbCondition.Append(" AND m.ReceiveAreaSysNo > 0 ");
            sbCondition.Append(" AND m.PayTypeSysNo > 0 ");
            sbCondition.Append(" AND m.ShipTypeSysNo > 0 ");

            sbCondition.Append(" AND (m.HoldMark IS NULL OR m.HoldMark <> 1) ");
            sbCondition.Append(" AND c.IsFPCheck IS NOT NULL");
            sbCondition.Append(" AND c.[IsCombine] IS NOT NULL");
            sbCondition.Append(" AND AuditType IS NULL");
            sbCondition.Append(" AND (c.IsBackOrder IS NULL OR  c.IsBackOrder <> 1)");
            sbCondition.Append(" AND (c.SOType IS NULL OR  c.SOType NOT IN (1,4,5,10))");
            sbCondition.Append(" AND (c.stockstatus IS NULL OR  c.stockstatus <> 1)");
            sbCondition.Append(" AND (c.HoldStatus IS NULL OR  c.HoldStatus = 0)");
            sbCondition.Append(" AND (c.SOType <> 7 OR (c.SOType=7 AND c.SettlementStatus='S'))");
            sbCondition.AppendFormat(" AND m.CompanyCode='{0}'", companyCode);

            //数据量过大不需要提取Item数据，单独循环提取
            command.CommandText = command.CommandText.Replace("#SO_ConditionString#", sbCondition.ToString())
                                .Replace("#SOItem_ConditionString#", " WHERE  1=2 ")
                                .Replace("#Top#", " TOP " + topCount.ToString());

            DataSet ds = command.ExecuteDataSet();
            return DataSetToSOList(ds, false);
        }

        public void UpdateCheckShippingAuditTypeBySysNo(int soSysNo, AuditType auditType, string autoAuditMemo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_CheckShippingAuditType");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@AuditType", (int)auditType);
            command.SetParameterValue("@AutoAuditMemo", autoAuditMemo);
            command.ExecuteNonQuery();
        }

        public void UpdateSO4AuditUserInfo(int soSysNo, int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SO4AuditUserInfo");

            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);

            command.ExecuteNonQuery();
        }

        public bool UpdateSO4PassAutoAudit(int soSysNo, int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SO4PassAutoAudit");

            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);

            return command.ExecuteNonQuery() > 0;
        }

        #endregion

        #region 订单申报相关

        /// <summary>
        /// 获取待申报的订单
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareSO> GetWaitDeclareSO()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_GetWaitDeclareSO");
            return command.ExecuteEntityList<WaitDeclareSO>();
        }

        /// <summary>
        /// 创建订单申报记录编号
        /// </summary>
        /// <returns></returns>
        public int CreateSODeclareRecordsSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_CreateSODeclareRecordsSysNo");
            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 创建订单申报记录
        /// </summary>
        /// <param name="entity">申报记录</param>
        /// <returns></returns>
        public void CreateSODeclareRecords(SODeclareRecords entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_CreateSODeclareRecords");
            command.SetParameterValue<SODeclareRecords>(entity);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 申报时获取订单信息
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        public DeclareOrderInfo DeclareGetOrderInfoBySOSysNo(int SOSysNo)
        {
            DeclareOrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("SO_DeclareGetOrderInfoBySOSysNo");
            command.SetParameterValue("@SOID", SOSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<DeclareOrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<DeclareSOItemInfo, List<DeclareSOItemInfo>>(itemTable.Rows);
                    }
                }
                if (result.Tables != null && result.Tables.Count > 2)
                {
                    DataTable payTable = result.Tables[2];
                    orderInfo.PayInfo = DataMapper.GetEntity<DeclareSOPayInfo>(payTable.Rows[0]);
                }
            }

            return orderInfo;
        }

        /// <summary>
        /// 申报时获取订单支付信息
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        public SOPaymentDeclare DeclareGetPaymentInfoBySOSysNo(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_DeclareGetPaymentInfoBySOSysNo");
            command.SetParameterValue("@SOID", SOSysNo);
            SOPaymentDeclare paymentInfo = command.ExecuteEntity<SOPaymentDeclare>();
            return paymentInfo;
        }
        /// <summary>
        /// 更新订单支付申报状态
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <param name="declareTrackingNumber">申报编号</param>
        /// <param name="declareStatus">申报状态</param>
        public void DeclareUpdatePaymentDeclareInfo(int SOSysNo, string declareTrackingNumber, int? declareStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_DeclareUpdatePaymentDeclareInfo");
            command.SetParameterValue("@SOID", SOSysNo);
            command.SetParameterValue("@DeclareTrackingNumber", declareTrackingNumber);
            command.SetParameterValue("@DeclareStatus", declareStatus);
            command.ExecuteNonQuery();
        }
        #endregion
    }
}
