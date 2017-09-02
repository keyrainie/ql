using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using System.Data;

namespace ECCentral.Service.PO.IDataAccess
{
    /// <summary>
    /// 虚库采购单
    /// </summary>
    public interface IVirtualPurchaseOrderDA
    {
        /// <summary>
        /// 加载虚库采购单 
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <returns></returns>
        VirtualStockPurchaseOrderInfo LoadVSPO(int vspoSysNo);

        /// <summary>
        /// 更新虚库采购单
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <returns></returns>
        VirtualStockPurchaseOrderInfo UpdateVSPO(VirtualStockPurchaseOrderInfo vspoInfo);

        /// <summary>
        /// 创建虚库采购单
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        VirtualStockPurchaseOrderInfo CreateVSPO(VirtualStockPurchaseOrderInfo vspoInfo);

        /// <summary>
        /// 作废虚库采购单
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        VirtualStockPurchaseOrderInfo AbandonVSPO(VirtualStockPurchaseOrderInfo vspoInfo);

        /// <summary>
        /// 验证对应的PO中是否有对应的商品
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        bool ValidateFromPO(int vspoSysNo, int poSysNo);

        /// <summary>
        /// 验证对应的Shift中是否有对应的商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="shiftsysno"></param>
        /// <returns></returns>
        bool ValidateFromShift(int vspoSysNo, int shiftsysno);

        /// <summary>
        /// 验证对应的Transfer中是否有对应的商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool ValidateFromTransfer(int vspoSysNo, int transferSysNo);

        /// <summary>
        /// 计算虚库申请单需要的采购数量
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <returns></returns>
        int CalcVSPOQuantity(int soItemSysNo);

        /// <summary>
        /// 判断销售单是否已经生成了虚库采购单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        int CheckExistsSOVirtualItemRequest(int soSysNo);

        /// <summary>
        /// 获取邮件发件人(更新虚库采购单 ）
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <returns></returns>
        DataTable GetEmailContentForUpdateVSPO(int sysNo);

        /// <summary>
        /// 获取邮件发件人(创建虚库采购单 ）
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <returns></returns>
        DataTable GetEmailContentForCreateVSPO(int soSysNo, int productSysNo);

        /// <summary>
        /// 获取对应的备份PM邮件地址
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        string GetBackUpPMEmailAddress(int userSysNo);

        /// <summary>
        /// 获取虚库采购单memo信息
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <returns></returns>
        VirtualStockPurchaseOrderInfo GetMemoInfoFromVirtualRequest(int itemSysNo);
    }
}
