using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(PollItemProcessor))]
    public class PollItemProcessor
    {
        private IPollItemDA pollItemDA = ObjectFactory<IPollItemDA>.Instance;

        #region  投票

        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        public virtual int CreatePollMaster(PollMaster item)
        {
            int returnValue = 0;

            if (item.Status.HasValue && item.Status.Value == ADStatus.Active)
            {
                if (pollItemDA.CheckPageTypeForCreatePollMaster(item) > 1 && item.PageType == 4)
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_JustTwoValidPollItemInCategory"));
                else if (pollItemDA.CheckForCreatePollMaster(item) > 0)
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_JustOneValidPollItemInCategory"));
                else
                    returnValue = pollItemDA.CreatePollMaster(item);
            }
            else
            {
                returnValue = pollItemDA.CreatePollMaster(item);
            }

            ExternalDomainBroker.CreateOperationLog("创建投票主题", BizLogType.MKT_Poll_Master_Create, returnValue, item.CompanyCode);

            return returnValue;
        }

        /// <summary>
        /// 修改投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollMaster(PollMaster item)
        {

            if (item.Status.HasValue && item.Status.Value == ADStatus.Active)
            {
                if (pollItemDA.CheckPageTypeForCreatePollMaster(item) > 1 && item.PageType == 4)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_JustTwoValidPollItemInCategory"));
                }
                if (pollItemDA.CheckForCreatePollMaster(item) > 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_JustOneValidPollItemInCategory"));
                }
            }

            pollItemDA.UpdatePollMaster(item);

            ExternalDomainBroker.CreateOperationLog("修改投票主题", BizLogType.MKT_Poll_Master_Update, item.SysNo.Value, item.CompanyCode);

        }

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual PollMaster LoadPollMaster(int itemID)
        {
            PollMaster item =pollItemDA.LoadPollMaster(itemID);
            item.PollItemGroupList = pollItemDA.GetPollItemGroupList(itemID);
            return item;
        }

        #endregion

        #region 投票--问题组（PollItemGroup）
        /// <summary>
        /// 创建投票问题组，只创建问题组的基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreatePollItemGroup(PollItemGroup item)
        {
            PollMaster master = LoadPollMaster(item.PollSysNo.Value);

            if (master.PageType != 4)
            {
                List<PollItemGroup> itemList = GetPollItemGroupList(item.PollSysNo.Value);
                if (itemList.Count > 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_PollPageTypeOnlyOneItem"));
                }
            }

            pollItemDA.CreatePollItemGroup(item);
        }


        /// <summary>
        /// 编辑投票问题组基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollItemGroup(PollItemGroup item)
        {
            pollItemDA.UpdatePollItemGroup(item);
        }

        /// <summary>
        /// 删除投票问题组
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeletePollItemGroup(int itemID)
        {
            pollItemDA.DeletePollItemGroup(itemID);
        }

        /// <summary>
        /// 加载投票问题组所有问题
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual List<PollItemGroup> GetPollItemGroupList(int sysNo)
        {
            return pollItemDA.GetPollItemGroupList(sysNo);
        }

        #endregion

        #region  投票问题组--选项（PollItem）
        /// <summary>
        /// 创建投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreatePollItem(PollItem item)
        {
            if (pollItemDA.CheckCreatePollItem(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_PollItemNameISTheSame"));
            else
                pollItemDA.CreatePollItem(item);
        }

        /// <summary>
        /// 编辑投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollItem(PollItem item)
        {
            if (pollItemDA.CheckCreatePollItem(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_PollItemNameISTheSame"));
            else
                pollItemDA.UpdatePollItem(item);
        }

        /// <summary>
        /// 删除投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeletePollItem(int itemID)
        {
            pollItemDA.DeletePollItem(itemID);
        }

        /// <summary>
        /// 加载投票问题组选项
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual List<PollItem> GetPollItemList(int sysNo)
        {
            return pollItemDA.GetPollItemList(sysNo);
        }

        public virtual List<PollItemAnswer> GetPollAnswer(int sysNo)
        {
            return pollItemDA.GetPollAnswer(sysNo);
        }
        #endregion
    }
}
