using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IDeductDA))]
    public class DeductDA : IDeductDA
    {
        /// <summary>
        /// 查询单条扣款项信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public Deduct GetSingleDeductBySysNo(string sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSingleDeductBySysNo");
            command.SetParameterValue("@SysNo",Convert.ToInt32(sysNo));
            return command.ExecuteEntity<Deduct>();
        }

        /// <summary>
        /// 作废单个扣款项维护信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public int DeleteDeductBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteDudectBySysNo");
            command.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑扣款项
        /// </summary>
        /// <param name="deduct"></param>
        public Deduct UpdateDeduct(Deduct deduct)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EditDudect");
            command.SetParameterValue("@Name", deduct.Name);
            command.SetParameterValue("@DeductType", (int)deduct.DeductType);
            command.SetParameterValue("@AccountType", (int)deduct.AccountType);
            command.SetParameterValue("@DeductMethod", (int)deduct.DeductMethod);
            command.SetParameterValue("@EditUser",ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@SysNo", deduct.SysNo);
            return command.ExecuteEntity<Deduct>();
        }

        /// <summary>
        /// 创建扣款项
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        public Deduct CreateDeduct(Deduct deduct)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateDeduct");
            command.SetParameterValue("@Name", deduct.Name);
            command.SetParameterValue("@DeductType", deduct.DeductType.Value);
            command.SetParameterValue("@AccountType", deduct.AccountType.Value);
            command.SetParameterValue("@DeductMethod", deduct.DeductMethod.Value);
            command.SetParameterValue("@InUser",ServiceContext.Current.UserSysNo);
            return command.ExecuteEntity<Deduct>();
        }

        /// <summary>
        /// 根据扣款项名称查询扣款项信息
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        public Deduct SelectDeductInfoByName(Deduct deduct)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSingleDeductByName");
            cmd.SetParameterValue("@SysNo", deduct.SysNo);
            cmd.SetParameterValue("@Name", deduct.Name);
            return cmd.ExecuteEntity<Deduct>();
        }
    }
}
