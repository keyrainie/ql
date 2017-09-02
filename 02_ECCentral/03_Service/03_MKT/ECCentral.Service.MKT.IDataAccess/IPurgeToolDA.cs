using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
   public interface IPurgeToolDA
    {
       /// <summary>
       /// 根据query获取PurgeTool
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetPurgeToolByQuery(PurgeToolQueryFilter query,out int totalCount);

       /// <summary>
       /// 创建PurgeTool
       /// </summary>
       /// <param name="info"></param>
       void CreatePurgeTool(PurgeToolInfo info);
    }
}
