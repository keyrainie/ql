using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(TariffProcessor))]
    public class TariffProcessor
    {
        private readonly ITariffDA _tariffDa = ObjectFactory<ITariffDA>.Instance;
        public List<TariffInfo> QueryTariffCategory(string tariffcode)
        {
            return _tariffDa.QueryTariffCategory(tariffcode);
        }


        public TariffInfo CreateTariffInfo(TariffInfo entity)
        {
            //PreCheck
            CheckTariffInfo(entity);
            return _tariffDa.CreateTariffInfo(entity);
        }

        public void CheckTariffInfo(TariffInfo entity)
        {
            //归类名称不能重复
            TariffInfo entity1 = _tariffDa.GetTariffInfoByName(entity.ItemCategoryName);
            if (entity1 != null)
            {
                throw new BizException("归类名称重复，请重新输入!");
            }

            //TariffCode不能重复
            TariffInfo entity2 = _tariffDa.GetTariffInfoByTariffCode(entity.Tariffcode);
            if (entity2 != null)
            {
                throw new BizException("税则号重复，请重新输入!");
            }
        }

        public TariffInfo GetTariffInfo(int sysNo)
        {
            return _tariffDa.GetTariffInfo(sysNo);
        }

        public bool UpdateTariffInfo(TariffInfo entity)
        {
            return _tariffDa.UpdateTariffInfo(entity);
        }
    }

}
