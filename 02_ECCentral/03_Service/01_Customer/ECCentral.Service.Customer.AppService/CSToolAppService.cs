using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CSToolAppService))]
    public class CSToolAppService
    {
        #region OrderCheckMaster
        /// <summary>
        /// 批量更新OrderCheckMaster信息的状态
        /// </summary>
        public virtual void BatchUpdateOrderCheckMasterStatus(OrderCheckMaster msg, List<int> SysNoList)
        {
            ObjectFactory<OrderCheckMasterProcessor>.Instance.BatchUpdateOrderCheckMasterStatus(msg, SysNoList);
        }

        /// <summary>
        /// 创建OrderCheckMaster
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual OrderCheckMaster CreateOrderCheckMaster(OrderCheckMaster msg)
        {
            return ObjectFactory<OrderCheckMasterProcessor>.Instance.CreateOrderCheckMaster(msg);
        }


        #endregion

        #region OrderCheckItem
        /// <summary>
        /// 创建OrderCheckItem
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual OrderCheckItem CreateOrderCheckItem(OrderCheckItem msg)
        {
            return ObjectFactory<OrderCheckItemProcessor>.Instance.CreateOrderCheckItem(msg);
        }
        /// <summary>
        /// 批量创建OrderCheckItem
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual List<OrderCheckItem> BatchCreateOrderCheckItem(List<OrderCheckItem> msgs, string ReferenceType)
        {
            return ObjectFactory<OrderCheckItemProcessor>.Instance.BatchCreateOrderCheckItem(msgs, ReferenceType);
        }
        /// <summary>
        /// 更新OrderCheckItem状态
        /// </summary>
        /// <param name="msg"></param>
        public virtual void UpdateOrderCheckItem(OrderCheckItem msg)
        {
            ObjectFactory<OrderCheckItemProcessor>.Instance.UpdateOrderCheckItem(msg);

        }

        #endregion

        #region FPCheckMaster
        public virtual void UpdateFPCheckMaster(List<FPCheck> msg)
        {
            foreach (var item in msg)
            {
                ObjectFactory<FPCheckProcessor>.Instance.Update(item);
            }
        }

        #endregion
        #region FPCheckItem
        /// <summary>
        /// 添加串货订单详细项
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="status"></param>
        /// <param name="categorySysNo"></param>
        /// <param name="ProductID"></param>
        public virtual void CreateCH(string channelID, FPCheckItemStatus? status, int? categorySysNo, string ProductID)
        {
            ObjectFactory<FPCheckItemProcessor>.Instance.CreateCH(channelID, status, categorySysNo, ProductID);
        }
        public virtual void UpdateCHItemStatus(int id)
        {
            ObjectFactory<FPCheckItemProcessor>.Instance.UpdateCHItemStatus(id);
        }
        public virtual void UpdateETC(int sysNo, string param, bool? status)
        {
            ObjectFactory<FPCheckItemProcessor>.Instance.UpdateETC(sysNo, param, status);
        }
        #endregion


      
    }
}
