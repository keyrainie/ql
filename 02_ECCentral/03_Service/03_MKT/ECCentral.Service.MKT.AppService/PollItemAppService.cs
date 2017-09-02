using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(PollItemAppService))]
    public class PollItemAppService
    {
        #region  投票

        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        public virtual int CreatePollMaster(PollMaster item)
        {
            return ObjectFactory<PollItemProcessor>.Instance.CreatePollMaster(item);
        }

        /// <summary>
        /// 编辑投票基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollMaster(PollMaster item)
        {
            ObjectFactory<PollItemProcessor>.Instance.UpdatePollMaster(item);
        }

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual PollMaster LoadPollMaster(int itemID)
        {
            return ObjectFactory<PollItemProcessor>.Instance.LoadPollMaster(itemID);
        }


        /// <summary>
        /// 加载投票所有信息，本行为中会调用聚合根下所有聚合模型的加载行为        ??
        /// </summary>
        /// <param name="item"></param>
        //public virtual PollItem LoadPollInfo(int itemID)
        //{
        //    return ObjectFactory<PollItemProcessor>.Instance.LoadPollInfo(itemID);
        //}

        #endregion

        #region 投票--问题组（PollItemGroup）
        /// <summary>
        /// 创建投票问题组，只创建问题组的基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreatePollItemGroup(PollItemGroup item)
        {
            ObjectFactory<PollItemProcessor>.Instance.CreatePollItemGroup(item);
        }


        /// <summary>
        /// 编辑投票问题组基本信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollItemGroup(PollItemGroup item)
        {
            ObjectFactory<PollItemProcessor>.Instance.UpdatePollItemGroup(item);
        }


        /// <summary>
        /// 删除投票问题组
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeletePollItemGroup(int itemID)
        {
            ObjectFactory<PollItemProcessor>.Instance.DeletePollItemGroup(itemID);
        }


        /// <summary>
        /// 加载投票问题组所有问题
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual List<PollItemGroup> GetPollItemGroupList(int sysNo)
        {
            return ObjectFactory<PollItemProcessor>.Instance.GetPollItemGroupList(sysNo);
        }



        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        //public virtual PollItemGroup LoadPollItemGroup(int itemID)
        //{
        //    return ObjectFactory<PollItemProcessor>.Instance.LoadPollItemGroup(itemID);
        //}

        #endregion

        #region  投票问题组--选项（PollItem）
        /// <summary>
        /// 创建投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreatePollItem(PollItem item)
        {
            ObjectFactory<PollItemProcessor>.Instance.CreatePollItem(item);
        }


        /// <summary>
        /// 编辑投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdatePollItem(PollItem item)
        {
            ObjectFactory<PollItemProcessor>.Instance.UpdatePollItem(item);
        }


        /// <summary>
        /// 删除投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeletePollItem(int itemID)
        {
            ObjectFactory<PollItemProcessor>.Instance.DeletePollItem(itemID);
        }


        /// <summary>
        /// 加载投票问题组选项
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public virtual List<PollItem> GetPollItemList(int itemID)
        {
            return ObjectFactory<PollItemProcessor>.Instance.GetPollItemList(itemID);
        }
        public virtual List<PollItemAnswer> GetPollAnswer(int itemID)
        {
            return ObjectFactory<PollItemProcessor>.Instance.GetPollAnswer(itemID);
        }
        #endregion
    }
}