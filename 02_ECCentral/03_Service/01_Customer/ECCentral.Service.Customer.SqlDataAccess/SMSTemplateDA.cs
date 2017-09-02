using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ISMSTemplateDA))]
    public class SMSTemplateDA : ISMSTemplateDA
    {
        #region ISMSTemplateDA Members

        public virtual BizEntity.Customer.SMSTemplate Create(BizEntity.Customer.SMSTemplate entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateSMSTemplate");
            cmd.SetParameterValue("@Template", entity.Template);
            cmd.SetParameterValueAsCurrentUserSysNo("@CreateUserSysno");
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));

            return entity;
        }

        public virtual void Update(BizEntity.Customer.SMSTemplate entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSMSTemplate");
            cmd.SetParameterValue("@Template", entity.Template);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.ExecuteNonQuery();
        }

        public virtual BizEntity.Customer.SMSTemplate Load(int sysNo)
        {
            SMSTemplate entity = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSMSTemplateBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                entity = DataMapper.GetEntity<SMSTemplate>(row);
            }
            return entity;
        }

        #endregion
    }
}
