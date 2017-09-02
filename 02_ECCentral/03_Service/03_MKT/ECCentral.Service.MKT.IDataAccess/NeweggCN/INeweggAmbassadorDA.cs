using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface INeweggAmbassadorDA
    {
        #region Action

        void MaintainNeweggAmbassadorStatusActive(NeweggAmbassadorSatusInfo entity);

        void MaintainNeweggAmbassadorStatusCancel(NeweggAmbassadorSatusInfo entity);

        //记录取消，激活的Log
        void LogNeweggAmbassadorMaintainInfo(NeweggAmbassadorMaintainLogInfo entity);

        int CheckCustomerStatus(NeweggAmbassadorSatusInfo entity);

        NeweggAmbassadorEntity GetNeweggAmbassadorInfo(NeweggAmbassadorEntity entity);

        /// <summary>
        /// 取消申请
        /// </summary>
        /// <param name="entity">泰隆优选大使信息</param>
        /// <returns></returns>
        bool CancelRequestNeweggAmbassador(NeweggAmbassadorSatusInfo entity);

        #endregion


        
    }
}
