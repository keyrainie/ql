using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using IPP.InventoryMgmt.Taobao.JobV31.Common;

namespace IPP.InventoryMgmt.Taobao.JobV31.BIZ
{
    public abstract class SynInventoryQtyBase
    {
        public SynInventoryQtyBase()
        {
            OnRunningBefor = new SynInventoryQtyRunning(On_RunningBefor);
            OnRunningAfter = new SynInventoryQtyRunning(On_RunningAfter);
        }

        #region EVENT
        protected virtual void On_RunningBefor(object sender, InventoryQtyArgs args) { }

        protected virtual void On_RunningAfter(object sender, InventoryQtyArgs args) { }


        /// <summary>
        /// 同步库存之前执行
        /// </summary>
        public event SynInventoryQtyRunning OnRunningBefor;

        /// <summary>
        /// 同步库存之后执行
        /// </summary>
        public event SynInventoryQtyRunning OnRunningAfter;

        #endregion

        /// <summary>
        /// 同步库存主方法，
        /// 调用服务
        /// </summary>
        public void SynInventoryQty(List<ProductEntity> productEntityList, List<InventoryQtyEntity> inventoryQtyList)
        {
            InventoryQtyArgs args = new InventoryQtyArgs();

            args.InventoryQtyEntityList = inventoryQtyList;
            args.ProductEntityList = productEntityList;

            OnRunningBefor(this, args);

            OnRunningAfter(this, args);
        }
    }
}
