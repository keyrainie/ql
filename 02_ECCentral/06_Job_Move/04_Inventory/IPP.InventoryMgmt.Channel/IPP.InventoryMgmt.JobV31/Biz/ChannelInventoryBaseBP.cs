using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Service;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ExceptionHandler;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Common;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Biz
{
    public abstract class ChannelInventoryBaseBP
    {
        public ChannelInventoryBaseBP(JobContext context)
        {
            this.Context = context;
        }

        protected JobContext Context { get; private set; }

        private int Records = 0;
        private const int PageSize = 100;
        private int page = 1;

        private List<ChannelProductEntity> ChannelProductList = null;

        public void Run()
        {
            page = 1;
            On_LoadData();
            Records = GetRecordsCount();

            On_SyncBegin(Records);

            if (Records > 0)
            {
                int pageCount = Records <= PageSize ? 1 : (Records % PageSize == 0 ? Records / PageSize : Records / PageSize + 1);

                for (; page <= pageCount; page++)
                {
                    ChannelProductList = GetChannelProductList(page, PageSize);
                    foreach (ChannelProductEntity entity in ChannelProductList)
                    {
                        On_SyncRunning(entity);
                        try
                        {
                            ThirdPartService.Sync(entity);
                            On_SyncRunned(entity);
                        }
                        catch (BusinessException ex)
                        {
                            On_Exception(ex as BusinessException, entity);
                            ExceptionHelper.HandleException(ex);
                        }
                    }
                }
            }

            On_SyncEnd();
        }

        protected abstract int GetRecordsCount();

        protected abstract List<ChannelProductEntity> GetChannelProductList(int page, int pageSize);

        protected virtual void On_LoadData()
        {

        }

        protected virtual void On_SyncBegin(int records)
        {

        }

        protected virtual void On_SyncRunning(ChannelProductEntity entity)
        {

        }

        protected virtual void On_SyncRunned(ChannelProductEntity entity)
        {

        }

        protected virtual void On_SyncEnd()
        {

        }

        protected virtual void On_Exception(BusinessException ex, ChannelProductEntity errorEntity)
        {

        }

        protected void WriteLog(string log)
        {
            Util.WriteLog(log, Context, Config.Debug);
        }
    }
}
