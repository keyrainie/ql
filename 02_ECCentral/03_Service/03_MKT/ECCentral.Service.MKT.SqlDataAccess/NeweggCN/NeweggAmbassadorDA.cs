using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NeweggCN
{
    [VersionExport(typeof(INeweggAmbassadorDA))]
    public class NeweggAmbassadorDA: INeweggAmbassadorDA
    {

        #region Action

        public void MaintainNeweggAmbassadorStatusActive(NeweggAmbassadorSatusInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("MaintainNeweggAmbassadorStatusActive");

            dc.SetParameterValue("@CustomerMark", entity.OrignCustomerMark);
            dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();
        }

        public void MaintainNeweggAmbassadorStatusCancel(NeweggAmbassadorSatusInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("MaintainNeweggAmbassadorStatusCancel");

            dc.SetParameterValue("@CustomerMark", entity.OrignCustomerMark);
            dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();
        }

        //记录取消，激活的Log
        public void LogNeweggAmbassadorMaintainInfo(NeweggAmbassadorMaintainLogInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LogNeweggAmbassadorMaintainInfo");
            
            dc.SetParameterValue("@CustomerMark", entity.Status);
            dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@Note", entity.Note);
            dc.ExecuteNonQuery();
        }


        public int CheckCustomerStatus(NeweggAmbassadorSatusInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CheckCustomerStatus");

            dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);

            return dc.ExecuteScalar<int>();
        }

        public NeweggAmbassadorEntity GetNeweggAmbassadorInfo(NeweggAmbassadorEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetNeweggAmbassadorInfo");
            dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);
            NeweggAmbassadorEntity result = dc.ExecuteEntity<NeweggAmbassadorEntity>();
            return result;
        }

        /// <summary>
        /// 取消申请
        /// </summary>
        /// <param name="entity">泰隆优选大使信息</param>
        /// <returns></returns>
        public bool CancelRequestNeweggAmbassador(NeweggAmbassadorSatusInfo entity)
        {
            if (entity!=null&&entity.AmbassadorSysNo != null)
            {
                DataCommand dc = DataCommandManager.GetDataCommand("CancelRequestNeweggAmbassador");
                dc.SetParameterValue("@CustomerSysno", entity.AmbassadorSysNo);
                dc.SetParameterValue("@CompanyCode", entity.CompanyCode);
                var count = dc.ExecuteNonQuery();
                if (count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
   
    }
}
