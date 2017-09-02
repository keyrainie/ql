using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IFPCheckDA))]
    public class FPCheckDA : IFPCheckDA
    {


        public virtual void Update(BizEntity.Customer.FPCheck entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFPCheckMaster");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", entity.FPCheckStatus);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 添加串货订单项
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="status"></param>
        /// <param name="categorySysNo"></param>
        /// <param name="ProductID"></param>
        public virtual void CreateCH(string channelID, FPCheckItemStatus? status, int? categorySysNo, string ProductID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCH");
            if (!string.IsNullOrEmpty(ProductID))
                cmd.SetParameterValue("@Type", "PID");
            else
                cmd.SetParameterValue("@Type", "PC3");
            cmd.SetParameterValue("@ProductID", ProductID);
            cmd.SetParameterValue("@Cateogry3No", categorySysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新FPItem的状态
        /// </summary>
        /// <param name="id"></param>
        public virtual void UpdateCHItemStatus(int id)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFPItemStatus");
            cmd.SetParameterValue("@SysNo", id);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新炒货订单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="param"></param>
        /// <param name="status"></param>
        public virtual void UpdateETC(int sysNo, string param, bool? status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateETC");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Param", param);
            cmd.SetParameterValue("@Status", status.Value ? 1 : 0);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.ExecuteNonQuery();
        }

        public List<FPCheck> GetFPCheckMasterList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFPCheckMaster");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<FPCheck>();
        }

        public List<FPCheckItem> LoadItemsByFPCheckCode(string fPCheckCode, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadItemsByFPCheckCode");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@FPCheckCode", fPCheckCode);
            return cmd.ExecuteEntityList<FPCheckItem>();
        }

    }
}
