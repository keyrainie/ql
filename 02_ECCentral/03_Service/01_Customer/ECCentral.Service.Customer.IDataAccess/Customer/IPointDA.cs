using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.IDataAccess
{

    public interface IPointDA
    {
        object Adjust(AdjustPointRequest adujstInfo);
        /// <summary>
        /// 调整客户积分有效期
        /// </summary>
        object UpateExpiringDate(int obtainSysNO, DateTime exprireDate);

        DataTable QueryRequestItemsBySysNo(CustomerPointsAddRequest info);

        int? QueryRequestStatusBySysNo(CustomerPointsAddRequest info);

        void ConfirmRequest(CustomerPointsAddRequest info);

        int CreateRequest(CustomerPointsAddRequest requestInfo);

        void CreateRequestItem(CustomerPointsAddRequestItem info);

        object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList);

        object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList);
        /// <summary>
        /// 获得价保积分
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="productSysNoList"></param>
        int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList);
    }

}
