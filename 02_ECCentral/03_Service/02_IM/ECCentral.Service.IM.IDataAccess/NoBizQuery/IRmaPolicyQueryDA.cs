using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
   
   public interface IRmaPolicyQueryDA
    {
       /// <summary>
       /// 查询退换货政策
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       DataTable QueryRmaPolicy(RmaPolicyQueryFilter query,out int totalCount);

       /// <summary>
       /// 根据SysNo得到RmaPolicyInfo
       /// </summary>
       /// <param name="SysNo"></param>
       /// <returns></returns>
       RmaPolicyInfo QueryRmaPolicyBySysNo(int SysNo);

       /// <summary>
       /// 查询标准退换货政策(唯一)
       /// </summary>
       /// <returns></returns>
       RmaPolicyInfo GetStandardRmaPolicy();

       /// <summary>
       /// 获取所有退换货信息
       /// </summary>
       /// <returns></returns>
       List<RmaPolicyInfo> GetAllRmaPolicy();
    }
}
