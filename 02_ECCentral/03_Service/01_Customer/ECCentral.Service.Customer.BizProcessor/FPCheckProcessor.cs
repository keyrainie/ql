using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(FPCheckProcessor))]
    public class FPCheckProcessor
    {
        private IFPCheckDA da = ObjectFactory<IFPCheckDA>.Instance;
        public virtual FPCheck Create(FPCheck entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(FPCheck entity)
        {
            da.Update(entity);
        }

        public List<FPCheck> GetFPCheckList(string companyCode)
        {
            List<FPCheck> list = da.GetFPCheckMasterList(companyCode);
            list.ForEach(item =>
            {
                item.FPCheckItemList = ObjectFactory<FPCheckItemProcessor>.Instance.LoadItems(item.FPCheckCode, companyCode);
            });
            return list;
        }
    }

    [VersionExport(typeof(FPCheckItemProcessor))]
    public class FPCheckItemProcessor
    {
        private IFPCheckDA da = ObjectFactory<IFPCheckDA>.Instance;
      
        public virtual List<FPCheckItem> LoadItems(string fPCheckCode, string companyCode)
        {
            List<FPCheckItem> list = new List<FPCheckItem>();
            list = da.LoadItemsByFPCheckCode(fPCheckCode, companyCode);
            return list;
        }

        public virtual void CreateCH(string channelID, FPCheckItemStatus? status, int? categorySysNo, string ProductID)
        {
            da.CreateCH(channelID, status, categorySysNo, ProductID);
        }

        public virtual void UpdateCHItemStatus(int id)
        {
            da.UpdateCHItemStatus(id);
        }

        public virtual void UpdateETC(int sysNo, string param, bool? status)
        {
            da.UpdateETC(sysNo, param, status);
        }
    }
}
