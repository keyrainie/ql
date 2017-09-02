using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ThirdPart.Interface
{
    public interface ISyncERPBizRecord
    {
        /// <summary>
        /// 创建ERP送货单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        ERPSHDInfo CreateSHD(ERPSHDInfo shdInfo);

        /// <summary>
        /// 创建ERP送货单_同城往返
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        ERPSHDInfo CreateSHD_TCWF(ERPSHDInfo shdInfo);

        /// <summary>
        /// 创建ERP送货单_返仓单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        ERPSHDInfo CreateSHD_FC(ERPSHDInfo shdInfo);

        /// <summary>
        /// 根据关联单据获取送货单状态
        /// </summary>
        /// <param name="refOrderList"></param>
        /// <returns></returns>
        List<ERPShippingInfo> GetERPShippingInfoByRefOrder(List<ERPShippingInfo> refOrderList);
    }
}
