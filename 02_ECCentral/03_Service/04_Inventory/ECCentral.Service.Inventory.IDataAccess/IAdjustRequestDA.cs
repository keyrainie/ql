using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IAdjustRequestDA
    {
        #region 损益单主信息维护

        /// <summary>
        /// 根据SysNO获取损益单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        AdjustRequestInfo GetAdjustRequestInfoBySysNo(int sysNo);

        /// <summary>
        /// 创建损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        AdjustRequestInfo CreateAdjustRequest(AdjustRequestInfo entity);

        /// <summary>
        /// 更新损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        AdjustRequestInfo UpdateAdjustRequest(AdjustRequestInfo entity);

        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        AdjustRequestInfo UpdateAdjustRequestStatus(AdjustRequestInfo entity);

        /// <summary>
        /// 生成自增序列
        /// </summary>
        /// <returns></returns>
        int GetAdjustRequestSequence();

        /// <summary>
        /// 如果已存在有效的隔月补充增益单，不可以重新生成
        /// </summary>
        /// <param name="adjustSysNo"></param>
        /// <returns></returns>
        int CheckPositiveAdjustRequestSysNo(int adjustSysNo);

        #endregion 损益单主信息维护

        #region 损益单商品维护

        /// <summary>
        /// 根据SysNO获取损益单商品列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<AdjustRequestItemInfo> GetAdjustItemListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        AdjustRequestInfo GetProductLineInfo(int sysNo);
        
        /// <summary>
        /// 损益单商品
        /// </summary>
        /// <param name="adjustItem"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        AdjustRequestItemInfo CreateAdjustItem(AdjustRequestItemInfo adjustItem, int requestSysNo);

        /// <summary>
        /// 更新损益成本
        /// </summary>
        /// <param name="adjustItem"></param>      
        /// <returns></returns>
        void UpdateAdjustItemCost(AdjustRequestItemInfo adjustItem);


        /// <summary>
        /// 删除损益单商品
        /// </summary>
        /// <param name="requestSysNo"></param>
        void DeleteAdjustItemByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 获取损益单库存成本
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal GetItemCost(int productSysNo);

        #endregion 损益单商品维护

        #region 损益单发票维护

        /// <summary>
        /// 根据SysNO获取损益单发票信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        AdjustRequestInvoiceInfo GetInvoiceInfoByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        AdjustRequestInvoiceInfo CreateAdjustRequestInvoice(AdjustRequestInvoiceInfo invoiceInfo, int requestSysNo);

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        AdjustRequestInvoiceInfo UpdateAdjustRequestInvoice(AdjustRequestInvoiceInfo invoiceInfo);

        #endregion 损益单发票维护

        #region 其他, SSB        

        #endregion 其他, SSB        

        bool CheckISBatchNumberProduct(int productSysNo);

        void SendSSBToWMS(string paramXml);
    }

}
