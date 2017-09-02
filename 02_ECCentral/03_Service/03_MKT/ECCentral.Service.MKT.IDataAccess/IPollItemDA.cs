using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IPollItemDA
    {
        #region  投票
        
        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        int CreatePollMaster(PollMaster item);

        /// <summary>
        /// 编辑投票基本信息
        /// </summary>
        /// <param name="item"></param>
        void UpdatePollMaster(PollMaster item);

        /// <summary>
        /// 检查 PageType=4 AND Status='A'的情况，不能超过三次
        /// </summary>
        /// <returns></returns>
        int CheckPageTypeForCreatePollMaster(PollMaster item);

        /// <summary>
        /// 查检其它PageType类型是否已经创建两次。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int CheckForCreatePollMaster(PollMaster item);

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="item"></param>
        PollMaster LoadPollMaster(int itemID);

        /// <summary>
        /// 加载投票所有信息，本行为中会调用聚合根下所有聚合模型的加载行为        ??
        /// </summary>
        /// <param name="item"></param>
        //PollItem LoadPollInfo(int itemID);
        #endregion

        #region 投票--问题组（PollItemGroup）
        /// <summary>
        /// 创建投票问题组，只创建问题组的基本信息
        /// </summary>
        /// <param name="item"></param>
        void CreatePollItemGroup(PollItemGroup item);

        /// <summary>
        /// 编辑投票问题组基本信息
        /// </summary>
        /// <param name="item"></param>
        void UpdatePollItemGroup(PollItemGroup item);

        /// <summary>
        /// 删除投票问题组
        /// </summary>
        /// <param name="item"></param>
        void DeletePollItemGroup(int itemID);
        

        /// <summary>
        /// 加载投票问题组所有问题
        /// </summary>
        /// <param name="sysNo"></param>
        List<PollItemGroup> GetPollItemGroupList(int sysNo);

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        //PollItemGroup LoadPollItemGroup(int itemID);
        #endregion

        #region  投票问题组--选项（PollItem）
        /// <summary>
        /// 检查是否可以创建该投票子项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckCreatePollItem(PollItem item);

        /// <summary>
        /// 创建投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        void CreatePollItem(PollItem item);

        /// <summary>
        /// 编辑投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        void UpdatePollItem(PollItem item);

        /// <summary>
        /// 删除投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        void DeletePollItem(int itemID);

        /// <summary>
        /// 加载投票问题组选项
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        List<PollItem> GetPollItemList(int itemID);
        List<PollItemAnswer> GetPollAnswer(int itemID);
        #endregion
    }
}
