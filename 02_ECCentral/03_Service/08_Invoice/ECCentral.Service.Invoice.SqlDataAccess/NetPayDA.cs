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

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(INetPayDA))]
    public class NetPayDA : INetPayDA
    {
        #region INetPayDA Members

        /// <summary>
        /// 创建netpay
        /// </summary>
        /// <param name="entity">netpay实体信息</param>
        /// <returns>创建好的netpay</returns>
        public NetPayInfo Create(NetPayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertNetPay");
            command.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return LoadBySysNo(entity.SysNo.Value);
        }

        /// <summary>
        /// 审核netpay
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        public void UpdateApproveInfo(int netpaySysNo, NetPayStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayApproveInfo");
            command.SetParameterValue("@SysNo", netpaySysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValueAsCurrentUserSysNo("@ApproveUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查netpay
        /// </summary>
        /// <param name="soSysNo"></param>
        public void UpdateReviewInfo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayReviewInfo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValueAsCurrentUserSysNo("@ApproveUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改netpay状态
        /// </summary>
        /// <param name="netpaySysNo">NetPay系统编号</param>
        public void UpdateStatus(int netpaySysNo, NetPayStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayStatus");
            command.SetParameterValue("@SysNo", netpaySysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValueAsCurrentUserSysNo("@ApproveUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据订单系统编号作废NetPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public void AbandonBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AbandonNetPayBySOSysNo");
            command.SetParameterValue("@Status", NetPayStatus.Abandon);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@WhereStatus", NetPayStatus.Approved);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据netpay系统编号加载netpay信息
        /// </summary>
        /// <param name="netpaySysNo">NetPay系统编号</param>
        /// <returns></returns>
        public NetPayInfo LoadBySysNo(int netpaySysNo)
        {
            var result = GetListByCriteria(new NetPayInfo()
            {
                SysNo = netpaySysNo
            });
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// 根据查询条件取得netpay列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public List<NetPayInfo> GetListByCriteria(NetPayInfo query)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetNetPayList");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOSysNo", DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, query.SOSysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                cmd.CommandText = sb.BuildQuerySql();
                return cmd.ExecuteEntityList<NetPayInfo>();
            }
        }

        /// <summary>
        /// 根据关联订单系统编号取得netpay列表
        /// </summary>
        /// <param name="relatedSoSysNo">关联订单系统编号</param>
        /// <returns></returns>
        public List<NetPayInfo> GetListByRelatedSoSysNo(int relatedSoSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNetPayByRelatedSoSysNo");
            command.SetParameterValue("@RelatedSoSysNo", relatedSoSysNo);
            return command.ExecuteEntityList<NetPayInfo>();
        }

        /// <summary>
        /// 获取外部引用key
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public string GetExternalKeyBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExternalKey");
            command.SetParameterValue("@SOSysNo", soSysNo);
            var r = command.ExecuteScalar();
            return (r is DBNull || r == null) ? string.Empty : r.ToString();
        }

        /// <summary>
        /// 更新netpay金额
        /// </summary>
        /// <param name="masterSOBaseInfo"></param>
        public void UpdateMasterSOAmt(SOBaseInfo masterSOBaseInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayAmt");
            command.SetParameterValue("@SOSysNo", masterSOBaseInfo.SysNo);
            command.SetParameterValue("@PayAmount", masterSOBaseInfo.ReceivableAmount);
            command.SetParameterValue("@OrderAmt", masterSOBaseInfo.SOTotalAmount);
            command.SetParameterValue("@PointPayAmt", masterSOBaseInfo.PointPay);
            command.SetParameterValue("@PrePayAmt", masterSOBaseInfo.PrepayAmount);
            command.SetParameterValue("@GiftCardPayAmt", masterSOBaseInfo.GiftCardPay);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据订单系统编号取得订单有效的netpay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public NetPayInfo GetValidBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidNetPayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<NetPayInfo>();
        }

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public bool IsExistOriginalBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsExistOriginalNetPayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return (command.ExecuteScalar() != null) ? true : false;
        }

        /// <summary>
        /// 根据关联订单编号取得最后一笔作废的netpay记录
        /// </summary>
        /// <param name="relatedSoSysNo">关联订单系统编号</param>
        /// <returns></returns>
        public NetPayInfo GetLastAboundedByRelatedSoSysNo(int relatedSoSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetLastAboundedNetPayByRelatedSoSysNo");
            command.SetParameterValue("@RelatedSoSysNo", relatedSoSysNo);
            return command.ExecuteEntity<NetPayInfo>();
        }

        #endregion INetPayDA Members

        #region [For SO Domain]

        public void UpdateStatusSplitForSO(NetPayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayStatusSplitForSO");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);

            command.ExecuteNonQuery();
        }

        public void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList, string externalKey)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AbandonSplitNetPayForSO");
            cmd.CommandText = cmd.CommandText.Replace("@SoSysNoList@", subList.ToListString("SysNo"));
            cmd.AddInputParameter("@MasterSoSysNo", DbType.AnsiStringFixedLength, master.SysNo);
            cmd.AddInputParameter("@ExternalKey", DbType.AnsiStringFixedLength, externalKey);
            cmd.ExecuteNonQuery();
        }

        #endregion [For SO Domain]
    }
}