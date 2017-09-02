using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
  public  interface IRmaPolicyDA
    {
      /// <summary>
      /// 创建退换货信息
      /// </summary>
      /// <param name="info"></param>
      void CreateRmaPolicy(RmaPolicyInfo info);
      /// <summary>
      /// 更新退换货信息
      /// </summary>
      /// <param name="info"></param>
      void UpdateRmaPolicy(RmaPolicyInfo info);
      
      /// <summary>
      ///作废
      /// </summary>
      /// <param name="sysNo"></param>
      void DeActiveRmaPolicy(int sysNo,UserInfo user);

      /// <summary>
      /// 激活
      /// </summary>
      /// <param name="sysNo"></param>
      void ActiveRmaPolicy(int sysNo, UserInfo user);

      /// <summary>
      /// 检查是否存在（标准类型'P'才检查）
      /// </summary>
      /// <param name="info"></param>
      /// <returns></returns>
      bool IsExistsRmaPolicy(int sysNo=0);


    }
}
