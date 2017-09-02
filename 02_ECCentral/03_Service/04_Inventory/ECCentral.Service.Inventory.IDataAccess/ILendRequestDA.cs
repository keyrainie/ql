using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface ILendRequestDA
    {
        #region 借货单维护

        /// <summary>
        /// 根据SysNo获取借货单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        LendRequestInfo GetLendRequestInfoBySysNo(int requestSysNo);

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        LendRequestInfo GetProductLineInfo(int sysNo);

        /// <summary>
        /// 创建借货单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        LendRequestInfo CreateLendRequest(LendRequestInfo entity);

        /// <summary>
        /// 更新借货单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        LendRequestInfo UpdateLendRequest(LendRequestInfo entity);

        /// <summary>
        /// 更新借货单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        LendRequestInfo UpdateLendRequestStatus(LendRequestInfo entity);

        /// <summary>
        /// 为待创建的借货单获取系统编号
        /// </summary>        
        /// <returns></returns>
        int GetLendRequestSequence();

        #endregion


        #region 借货商品维护

        /// <summary>
        /// 根据借货单SysNo获取借货商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<LendRequestItemInfo> GetLendItemListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建借货商品记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        LendRequestItemInfo CreateLendItem(LendRequestItemInfo lendItem, int requestSysNo);

        /// <summary>
        /// 更新借货商品记录
        /// </summary>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        LendRequestItemInfo UpdateLendItem(LendRequestItemInfo lendItem);

        /// <summary>
        /// 删除借货单中所有借货商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        /// 
        void DeleteLendItemByRequestSysNo(int requestSysNo);

        #endregion 借货商品维护

        #region 归还商品维护

        /// <summary>
        /// 根据借货单SysNo获取归还商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<LendRequestReturnItemInfo> GetReturnItemListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建借货商品归还记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        LendRequestReturnItemInfo CreateReturnItem(LendRequestReturnItemInfo returnItem, int requestSysNo);

        #endregion 归还商品维护
    }
}
