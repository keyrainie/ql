using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IAutoInnerOnlineList))]
    public class AutoInnerOnlineListDA : IAutoInnerOnlineList
    {

        public void ClearTableOnLinelist(string day)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ClearTableOnLinelist");
            command.ReplaceParameterValue("@Day", day);
            command.ExecuteNonQuery();
        }
    }
}
