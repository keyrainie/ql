using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICSDA))]
    public class CSDA : ICSDA
    {


        #region ICSDA Members

        public virtual CSInfo GetCSByIPPUserSysNo(int ippUserSysNo, BizEntity.Customer.CSInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCSListByIPPUserSysNo");
            cmd.SetParameterValue("@IPPUserSysNo", ippUserSysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            return cmd.ExecuteEntity<CSInfo>();
        }

        public virtual BizEntity.Customer.CSInfo InsertCS(BizEntity.Customer.CSInfo entity, bool hasCheck)
        {
            string commandName = (hasCheck) ? "InsertCSListIsExists" : "InsertCSList";

            DataCommand cmd = DataCommandManager.GetDataCommand(commandName);
            cmd.SetParameterValue<CSInfo>(entity);
            cmd.ExecuteNonQuery();
            if (!hasCheck)
            {
                entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            }

            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CSInfo GetCSBySysNo(int sysNo)
        {
            
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCSListBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntity<CSInfo>();
        }

        public virtual List<CSInfo> GetCSByLeaderSysNo(int LeaderSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCSListByLeaderSysNo");
            cmd.SetParameterValue("@LeaderSysNo", LeaderSysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntityList<CSInfo>();
        }

        public virtual int UpdateCS(BizEntity.Customer.CSInfo entity)
        {
            int result = 0;
            DataCommand cmd = DataCommandManager.GetDataCommand("updateCSList");
            cmd.SetParameterValue<CSInfo>(entity);
            result = cmd.ExecuteNonQuery();
            return result;
        }

        public virtual int UpdateCSByIPPUserSysNo(BizEntity.Customer.CSInfo entity)
        {
            int result = 0;
            DataCommand cmd = DataCommandManager.GetDataCommand("updateCSListByIPPUserSysNo");
            cmd.SetParameterValue<CSInfo>(entity);
            result = cmd.ExecuteNonQuery();

            return result;
        }

        #endregion

        #region ICSDA Members


        public virtual List<CSInfo> GetCSWithDepartmentId(int depid)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCSWithDepartmentId");
            cmd.SetParameterValue("@DepartmentCode", depid);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntityList<CSInfo>();
        }

        public virtual List<CSInfo> GetAllCS(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCS");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<CSInfo>();
        }

        #endregion
    }
}
