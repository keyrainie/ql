using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities;
using IPP.Oversea.CN.InventoryMgmt.JobV31.DataAccess;
using Newegg.Oversea.Framework.ExceptionBase;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Common;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Biz
{
    public class ChannelProductAppointBP : ChannelInventoryBaseBP
    {
        public ChannelProductAppointBP(JobContext context) : base(context) { }

        protected override int GetRecordsCount()
        {
            return ChannelInventoryDA.GetAppointListCount();
        }
        protected override List<ChannelProductEntity> GetChannelProductList(int page, int pageSize)
        {
            List<ChannelProductAppointEntity> list = ChannelInventoryDA.GetAllAppointList(page, pageSize);

            return Transfomer(list);
        }

        private List<ChannelProductEntity> Transfomer(List<ChannelProductAppointEntity> list)
        {
            List<ChannelProductEntity> result = new List<ChannelProductEntity>(list.Count);

            foreach (ChannelProductAppointEntity item in list)
            {
                result.Add(Transfomer(item));
            }

            return result;
        }

        private ChannelProductEntity Transfomer(ChannelProductAppointEntity appointEntity)
        {
            return appointEntity;
            //if (appointEntity == null)
            //{
            //    return null;
            //}
            //return new ChannelProductEntity
            //{
            //    ChannelQty = appointEntity.IsClearInventory == "Y" ? 0 : appointEntity.ChannelQty,
            //    ChannelSysNo = appointEntity.ChannelSysNo,
            //    ProductSysNo = appointEntity.ProductSysNo,
            //    SynProductID = appointEntity.SynProductID
            //};
        }

        protected override void On_Exception(BusinessException ex, ChannelProductEntity errorEntity)
        {
            WriteLog(ex.Message);
            WriteLog(ex.ErrorDescription);
        }

        protected override void On_LoadData()
        {
            WriteLog("正在检索“指定库存”相关数据……");
        }

        protected override void On_SyncBegin(int records)
        {
            WriteLog(string.Format("本次共检索到“指定库存”数据:{0}条", records));
        }

        protected override void On_SyncRunning(ChannelProductEntity entity)
        {
            WriteLog(string.Format("正在同步“指定库存”商品:{0}({1})，库存:{2}，渠道:{3}", entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty, entity.ChannelSysNo));
        }

        protected override void On_SyncRunned(ChannelProductEntity entity)
        {
            WriteLog(string.Format("“指定库存”商品:{0}({1})，库存:{2}，渠道:{3} 同步成功", entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty, entity.ChannelSysNo));
            ChannelInventoryDA.ChangeAppointSynStatus(entity.ProductSysNo, entity.ChannelSysNo, "N", Config.UserLoginName);

            ChannelProductAppointEntity appointEntity = entity as ChannelProductAppointEntity;
            if (appointEntity != null && appointEntity.IsClearInventory == "Y")
            {
                WriteLog(string.Format("正在回写“指定库存”商品:{0}({1})，库存:{2}，渠道:{3} 在Content的状态。", entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty, entity.ChannelSysNo));
                CommonDA.UpdateECommerceStatus(entity.ProductSysNo, entity.ChannelSysNo, "N");
                WriteLog(string.Format("回写“指定库存”商品:{0}({1})，库存:{2}，渠道:{3} 在Content的状态成功。", entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty, entity.ChannelSysNo));
            }
        }


    }
}
