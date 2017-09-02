using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO.PurchaseOrder;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IDeductDA
    {
        /// <summary>
        /// 查询单条扣款项信息
        /// </summary>
        /// <param name="sysNo">sysNo</param>
        /// <returns>int</returns>     
        Deduct GetSingleDeductBySysNo(string sysNo);
        /// <summary>
        /// 作废单个扣款项维护信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        int DeleteDeductBySysNo(int sysNo);
        /// <summary>
        /// 编辑扣款项
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        Deduct UpdateDeduct(Deduct deduct);
        /// <summary>
        /// 创建扣款项
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        Deduct CreateDeduct(Deduct deduct);
        /// <summary>
        /// 根据扣款名称查扣款信息
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        Deduct SelectDeductInfoByName(Deduct deduct);
    }
}
