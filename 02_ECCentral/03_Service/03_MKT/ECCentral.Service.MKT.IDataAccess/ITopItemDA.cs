using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ITopItemDA
    {
        /// <summary>
        /// 添加商品置顶信息
        /// </summary>
        /// <param name="entity"></param>
        void CreateTopItem(TopItemInfo entity);

        /// <summary>
        /// 移除商品置顶信息
        /// </summary>
        /// <param name="entity"></param>
        void DeleteTopItem(TopItemInfo entity);
        /// <summary>
        /// 更新置顶商品优先级
        /// </summary>
        /// <param name="entity"></param>
        void UpdateTopItemPriority(TopItemInfo entity);

        /// <summary>
        /// 查询置顶商品列表
        /// </summary>
        /// <param name="PageType"></param>
        /// <param name="RefSysNo"></param>
        /// <returns></returns>
        List<TopItemInfo> QueryTopItem(int PageType, int RefSysNo);

        /// <summary>
        /// 添加商品置顶配置信息
        /// </summary>
        /// <param name="entity"></param>
        void CreateTopItemConfig(TopItemConfigInfo entity);

        /// <summary>
        /// 更新商品置顶配置信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateTopItemConfig(TopItemConfigInfo entity);
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="PageType"></param>
        /// <param name="RefSysNo"></param>
        /// <returns></returns>
        TopItemConfigInfo LoadItemConfig(int PageType, int RefSysNo);


    }
}
