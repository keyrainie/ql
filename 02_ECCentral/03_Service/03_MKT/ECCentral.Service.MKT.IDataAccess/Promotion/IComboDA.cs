using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IComboDA
    {
        ComboInfo Load(int sysNo);

        int CreateMaster(ComboInfo info);

        void UpdateMaster(ComboInfo info);

        void UpdateStatus(int? sysNo, ComboStatus targetStatus);

        int AddComboItem(ComboItem item);

        void DeleteComboAllItem(int comboSysNo);

        List<ComboInfo> GetComboListForCurrentSO(List<int> productSysNoList);

        List<ComboInfo> GetComboListByProductSysNo(int productSysNo);

        int CheckComboExits(string comboName, int productSysNo);
        /// <summary>
        /// 同步状态
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="status"></param>
        void SyncSaleRuleStatus(int requestSysNo, ComboStatus? status);
    }
}
