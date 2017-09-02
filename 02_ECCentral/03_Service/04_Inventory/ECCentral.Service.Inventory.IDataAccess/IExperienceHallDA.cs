using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IExperienceHallDA
    {
        #region 调拨单维护

        /// <summary>
        /// 根据SysNo获取调拨单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        ExperienceInfo GetExperienceInfoBySysNo(int requestSysNo);
        
        /// <summary>
        /// 创建调拨单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ExperienceInfo CreateExperience(ExperienceInfo entity);

        /// <summary>
        /// 更新调拨单信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ExperienceInfo UpdateExperience(ExperienceInfo entity);

        /// <summary>
        /// 更新调拨单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void UpdateExperienceStatus(ExperienceInfo entity, int status);

        /// <summary>
        /// 调入添加库存、调出减少库存
        /// </summary>
        /// <param name="entity"></param>
        void AuditExperienceInOrOut(ExperienceItemInfo item, int sysno);

        
        /// <summary>
        /// 取消审核时：调入减少库存、调出添加库存（要还原）
        /// </summary>
        /// <param name="entity"></param>
        void CancelAuditExperienceInOrOut(ExperienceItemInfo item, int sysno);

        #endregion

        #region 借货商品维护

        /// <summary>
        /// 根据调拨单SysNo获取借货商品信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<ExperienceItemInfo> GetExperienceItemListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建借货商品记录
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="lendItem"></param>
        /// <returns></returns>
        ExperienceItemInfo CreateExperienceItem(ExperienceItemInfo lendItem, int requestSysNo);

        /// <summary>
        /// 删除调拨单中所有借货商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        /// 
        void DeleteExperienceItemByRequestSysNo(int requestSysNo);

        int GetValidStockQty(int productSysno);

        #endregion 借货商品维护
    }
}
