using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IFPCheckDA
    {
        void Update(FPCheck entity);
        void CreateCH(string channelID, FPCheckItemStatus? status, int? categorySysNo, string ProductID);
        void UpdateCHItemStatus(int id);
        void UpdateETC(int sysNo, string param, bool? status);
        List<FPCheck> GetFPCheckMasterList(string companyCode);
        List<FPCheckItem> LoadItemsByFPCheckCode(string fPCheckCode, string companyCode);
    }
}
