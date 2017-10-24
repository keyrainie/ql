using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Common;
namespace ECCentral.Service.Common.IDataAccess.NoBizQuery
{
    public interface IControlPanelSocietyQueryDA
    {
        DataTable QuerySociety(ControlPanelSocietyQueryFilter request, out int totalCount);
    }
}
