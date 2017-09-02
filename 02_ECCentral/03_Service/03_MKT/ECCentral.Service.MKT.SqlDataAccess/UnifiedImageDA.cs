using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IUnifiedImageDA))]
    public class UnifiedImageDA : IUnifiedImageDA
    {
        #region IUnifiedImageDA Members

        public void CreateUnifiedImage(UnifiedImage entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UnifiedImage_InsertImage");
            dc.SetParameterValue<UnifiedImage>(entity);
            dc.ExecuteNonQuery();
        }

        #endregion


    }
}
