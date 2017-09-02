using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IOldChangeNewDA))]
    public class OldChangeNewDA : IOldChangeNewDA
    {
        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public OldChangeNewInfo Create(OldChangeNewInfo entity)
        {

            DataCommand command = DataCommandManager.GetDataCommand("Invoice_Create_OldChangeNew");
            command.SetParameterValue("@SoSysNo", entity.SoSysNo);
            command.SetParameterValue("@Licence", entity.Licence);
            command.SetParameterValue("@OrderAmt", entity.OrderAmt);
            command.SetParameterValue("@Rebate", entity.Rebate);
            command.SetParameterValue("@ReviseRebate", entity.ReviseRebate);
            command.SetParameterValue("@InUser", entity.InUser);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            object obj = command.ExecuteScalar();
            if (obj != DBNull.Value)
            {
                int sysNo = Convert.ToInt32(obj);
                if (sysNo > 0)
                {
                    entity.SysNo = sysNo;
                    return entity;
                }
                return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="into"></param>
        /// <returns></returns>
        public OldChangeNewInfo UpdateOldChangeNewRebate(OldChangeNewInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_Update_OldChangeNewRebate");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@ReviseRebate", info.ReviseRebate);
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            if (command.ExecuteNonQuery() > 0)
            {
                return info;
            }
            return null; 
        }

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public OldChangeNewInfo UpdateOldChangeNewStatus(OldChangeNewInfo info)
        {
            //如果是直接提交，先保存
            if (info.Status == 1 && info.SysNo == 0)
            {
                if (!IsExistsSO(info))
                {
                   info= Create(info);
                }
            }
            //生成TradeInId
            if (info.SysNo > 0 && info.Status > 0)
            {
                info.TradeInId = "Y000" + info.SysNo.ToString().PadLeft(6, '0'); 
            }
            else
            {
                info.TradeInId = string.Empty; 
            }
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_Update_OldChangeNewStatus");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@Status", info.Status);
            command.SetParameterValue("@TradeInId", info.TradeInId);
            command.SetParameterValue("@ConfirmUser", info.ConfirmUser);
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            if (command.ExecuteNonQuery() > 0)
            {
                return info;
            }
            return null; 
        }

        /// <summary>
        /// 检查订单信息是否有效
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool IsExistsSO(OldChangeNewInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_Check_IsExistsSO");
            command.SetParameterValue("@SoSysNo", info.SoSysNo);
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            return command.ExecuteScalar<int>() > 0;
        }

        #region 设置凭证号
        public OldChangeNewInfo MaintainReferenceID(OldChangeNewInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Update_MaintainReferenceID");
            if (!string.IsNullOrEmpty(info.SysNoList))
            {
                command.CommandText = command.CommandText.Replace("@SysNoList", info.SysNoList);
            }
            else
            {
                command.CommandText = command.CommandText.Replace("@SysNoList", info.SysNo.ToString());
            }
            command.AddInputParameter("@ReferenceID", DbType.String, info.ReferenceID);
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, info.CompanyCode);
            if (command.ExecuteNonQuery() > 0)
            {
                return info;
            }
            return null;
        }
        #endregion

        #region 添加财务备注
        public OldChangeNewInfo MaintainStatusWithNote(OldChangeNewInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Update_MaintainStatusWithNote");
            if (!string.IsNullOrEmpty(info.SysNoList))
            {
                command.CommandText = command.CommandText.Replace("@SysNoList", info.SysNoList);
            }
            else
            {
                command.CommandText = command.CommandText.Replace("@SysNoList", info.SysNo.ToString());
            }
            command.AddInputParameter("@Note", DbType.String, info.Note);
            command.AddInputParameter("@Status", DbType.Int32, info.Status);
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, info.CompanyCode);
            if (command.ExecuteNonQuery() > 0)
            {
                return info;
            }
            return null; 
        }
        #endregion
    }
}
