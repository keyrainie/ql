using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IAreaRelationDA))]
    public class AreaRelationDA : IAreaRelationDA
    {
        #region IAreaRelationDA Members

        public void Create(int AreaSysNo, int RefSysNo, BizEntity.MKT.AreaRelationType Type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AreaRelation_Create");
            dc.SetParameterValue("@AreaSysNo", AreaSysNo);
            dc.SetParameterValue("@RefSysNo", RefSysNo);
            dc.SetParameterValue("@Type", Type);
            dc.SetParameterValueAsCurrentUserAcct("@InUser");
            dc.ExecuteNonQuery();
        }

        public void Delete(int AreaSysNo, int RefSysNo, BizEntity.MKT.AreaRelationType Type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("AreaRelation_Update");
            dc.SetParameterValue("@AreaSysNo", AreaSysNo);
            dc.SetParameterValue("@RefSysNo", RefSysNo);
            dc.SetParameterValue("@Type", Type);
            dc.SetParameterValueAsCurrentUserAcct("@EditUser");

            dc.ExecuteNonQuery();
        }


        public List<int> GetSelectedArea(int RefSysNo, BizEntity.MKT.AreaRelationType Type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetSelectedArea");
            dc.SetParameterValue("@RefSysNo", RefSysNo);
            dc.SetParameterValue("@Type", Type);
            return dc.ExecuteFirstColumn<int>();
        }

        #endregion
    }
}
