using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IAreaRelationDA
    {
        void Create(int AreaSysNo, int RefSysNo, AreaRelationType Type);
        void Delete(int AreaSysNo, int RefSysNo, AreaRelationType Type);
        List<int> GetSelectedArea(int RefSysNo, AreaRelationType Type);
 
    }
}
