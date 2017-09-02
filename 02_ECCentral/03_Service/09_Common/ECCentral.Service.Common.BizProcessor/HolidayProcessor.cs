using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using System.Data;
using System.Transactions;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(HolidayProcessor))]
    public class HolidayProcessor
    {

        public virtual List<DateTime> GetHolidayList(string blockedService, string CompanyCode)
        {
            return ObjectFactory<IHolidayDA>.Instance.GetHolidayList(blockedService, CompanyCode);
        }

        public virtual List<Holiday> GetHolidaysAfterToday(string companyCode)
        {
            return ObjectFactory<IHolidayDA>.Instance.GetHolidaysAfterToday(companyCode);
        }

        public virtual void Create(Holiday entity)
        {
            //获取相同日期，相同配送类型的数据
            List<Holiday> tSameHolidayList = ObjectFactory<IHolidayDA>.Instance.GetHolidaysByEntity(entity.ShipTypeSysNo, entity.HolidayDate);
            if (tSameHolidayList != null)
            {
                //检测是否存在 服务类型一样的记录
                tSameHolidayList.ForEach(item =>
                {
                    if (entity.BlockedService == item.BlockedService)
                        throw new BizException(string.Format("物流日期配置已存在相关记录，请重新添加"));
                    else if(item.BlockedService!=BlockedServiceType.NoCSWorkTime&&entity.BlockedService!=BlockedServiceType.NoCSWorkTime)
                        throw new BizException(string.Format("物流日期配置已存在相关记录，请重新添加"));
                });
            }
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<IHolidayDA>.Instance.Create(entity);
                //如果服务类型为 正常节假日，则增加一条该日期的“不配送”记录
                if (entity.BlockedService == BlockedServiceType.NormalHoliday)
                {
                    entity.BlockedService = BlockedServiceType.NoneDelivery;
                    ObjectFactory<IHolidayDA>.Instance.Create(entity);
                }
                scope.Complete();
            }
        }

        public virtual void DeleteBatch(List<int> sysNos)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (int tNum in sysNos)
                    ObjectFactory<IHolidayDA>.Instance.Delete(tNum);

                scope.Complete();
            }
        }
    }
}
