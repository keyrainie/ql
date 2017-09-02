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
    public class ChannelProductPercentBP : ChannelInventoryBaseBP
    {
        private int ChannelSysNo;

        public ChannelProductPercentBP(JobContext context, int channelSysNo)
            : base(context)
        {
            this.ChannelSysNo = channelSysNo;
        }

        protected override int GetRecordsCount()
        {
            return ChannelInventoryDA.GetPercentListCountByChannelSysNo(ChannelSysNo);
        }

        protected override List<ChannelProductEntity> GetChannelProductList(int page, int pageSize)
        {
            List<ChannelProductPercentEntity> list = ChannelInventoryDA.GetAllPercentListByChannelSysNo(page, pageSize, ChannelSysNo);

            return Transfomer(list);
        }

        private List<ChannelProductEntity> Transfomer(List<ChannelProductPercentEntity> list)
        {
            List<ChannelProductEntity> result = new List<ChannelProductEntity>(list.Count);
            foreach (ChannelProductPercentEntity item in list)
            {
                result.Add(Transfomer(item));
            }
            return result;
        }

        private ChannelProductEntity Transfomer(ChannelProductPercentEntity percentEntity)
        {
            return percentEntity;
            //if (percentEntity == null)
            //{
            //    return null;
            //}
            //return new ChannelProductEntity
            //{
            //    ChannelQty = GetPercentInventoryQty(percentEntity),
            //    ChannelSysNo = percentEntity.ChannelSysNo,
            //    ProductSysNo = percentEntity.ProductSysNo,
            //    SynProductID = percentEntity.SynProductID
            //};
        }

        //private int GetPercentInventoryQty(ChannelProductPercentEntity percentEntity)
        //{
        //    if (percentEntity.IsClearInventory == "Y")
        //    {
        //        return 0;
        //    }
        //    int qty = Convert.ToInt32(Math.Floor(percentEntity.MaxOnlineQty * percentEntity.InventoryPercent)) - percentEntity.SafeInventoryCount;
        //    return qty > 0 ? qty : 0;
        //}

        protected override void On_Exception(BusinessException ex, ChannelProductEntity errorEntity)
        {
            WriteLog(ex.Message);
            WriteLog(ex.ErrorDescription);
        }

        protected override void On_LoadData()
        {
            WriteLog(string.Format("{0}:正在检索“非指定库存”相关数据……", ChannelSysNo));
        }

        protected override void On_SyncBegin(int records)
        {
            WriteLog(string.Format("{0}:本次共检索到“非指定库存”数据:{1}条", ChannelSysNo, records));
        }

        protected override void On_SyncRunning(ChannelProductEntity entity)
        {
            WriteLog(string.Format("{0}:正在同步“非指定库存”商品:{1}({2})，库存:{3}", entity.ChannelSysNo, entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty));
        }

        protected override void On_SyncRunned(ChannelProductEntity entity)
        {
            WriteLog(string.Format("{0}:“非指定库存”商品:{1}({2})，库存:{3} 同步成功", entity.ChannelSysNo, entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty));

            ChannelProductPercentEntity percentEntity = entity as ChannelProductPercentEntity;
            if (percentEntity != null && percentEntity.IsClearInventory == "Y")
            {
                WriteLog(string.Format("{0}:正在回写“非指定库存”商品:{1}({2})，库存:{3} 在Content的状态。", entity.ChannelSysNo, entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty));
                CommonDA.UpdateECommerceStatus(entity.ProductSysNo, entity.ChannelSysNo, "N");
                WriteLog(string.Format("{0}:回写“非指定库存”商品:{0}({1})，库存:{2} 在Content的状态成功。", entity.ChannelSysNo, entity.ProductSysNo, entity.SynProductID, entity.SynChannelQty));
            }
            if (percentEntity != null)
            {
                ChannelInventoryDA.ChangePercentSynQty(percentEntity, Config.UserLoginName);
            }
        }
    }
}
