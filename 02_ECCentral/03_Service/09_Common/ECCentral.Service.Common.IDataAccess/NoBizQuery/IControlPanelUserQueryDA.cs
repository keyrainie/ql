using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess.NoBizQuery
{
    public interface IControlPanelUserQueryDA
    {
        DataTable QueryUser(ControlPanelUserQueryFilter request, out int totalCount);
    }
}
