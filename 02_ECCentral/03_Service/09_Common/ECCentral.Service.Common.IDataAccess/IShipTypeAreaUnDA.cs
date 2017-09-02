using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
   public  interface IShipTypeAreaUnDA
    {
        /// <summary>
        /// 删除配送方式-地区(非)
        /// </summary>
        /// <param name="sysnoList"></param>
       void VoidShipTypeAreaUn(List<int> sysnoList);
       /// <summary>
       /// 新增配送方式-地区(非)
       /// </summary>
       /// <param name="sysnoList"></param>
       ErroDetail CreateShipTypeAreaUn(ShipTypeAreaUnInfo entity);

       /// <summary>
       /// IPP-Bob.H.Li获取指定配送方式指定地区的非表  Jack移植 2012-10-25
       /// </summary>
       /// <param name="shipTypeSysNo">配送方式</param>
       /// <param name="areaSysNo">地区</param>
       /// <returns></returns>
       List<ShipTypeAreaUnInfo> QueryShipAreaUnByAreaSysNo(IEnumerable<int> shipTypeSysNoS, int areaSysNo);
   }
}
