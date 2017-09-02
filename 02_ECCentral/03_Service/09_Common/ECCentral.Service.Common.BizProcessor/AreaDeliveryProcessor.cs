using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(AreaDeliveryProcessor))]
    public class AreaDeliveryProcessor
    {
        public virtual AreaDeliveryInfo Create(AreaDeliveryInfo entity)
        {
            List<AreaDeliveryInfo> tmpList = ObjectFactory<IAreaDeliveryDA>.Instance.GetAreaDeliveryList();
            tmpList.Sort((comp1, comp2) =>
            {
                int tmpNum = Convert.ToInt32(comp1.Priority ?? 0) - Convert.ToInt32(comp2.Priority ?? 0);
                if (tmpNum > 0) return 1;
                else if (tmpNum < 0) return -1;
                return tmpNum;
            });

            string existedPrioriyList = string.Empty;
            bool existedPrioriy = false;
            foreach (AreaDeliveryInfo item in tmpList)
            {
                if (item.WHArea == entity.WHArea
                       && item.City.Trim().ToLower() == entity.City.Trim().ToLower())
                {
                    throw new BizException(string.Format("该区域已经存在名称为{0}的城市！", entity.City));
                }
                existedPrioriyList += item.Priority.ToString() + ",";
                if (item.Priority == entity.Priority)
                {
                    existedPrioriy = true;
                } 
            }

            if (existedPrioriy)
            {
                throw new BizException("已经存在此优先级！现有的优先级有:" + existedPrioriyList);
            }

            entity.InDate = DateTime.Now;
            entity.Status = "A";

            return ObjectFactory<IAreaDeliveryDA>.Instance.Create(entity);
        }

        public virtual AreaDeliveryInfo Update(AreaDeliveryInfo entity)
        {
            List<AreaDeliveryInfo> tmpList = ObjectFactory<IAreaDeliveryDA>.Instance.GetAreaDeliveryList();

            string existedPrioriyList = string.Empty;
            bool existedPrioriy = false;
            foreach (AreaDeliveryInfo item in tmpList)
            {
                if (item.WHArea == entity.WHArea
                         && item.City.ToLower() == entity.City.ToLower()
                         && item.SysNo != entity.SysNo)
                {
                    throw new BizException(string.Format("该区域已经存在编码为{0}的城市！", entity.WHArea));
                }
                existedPrioriyList += item.Priority.ToString() + ",";
                if (item.Priority == entity.Priority && item.SysNo != entity.SysNo)
                {
                    existedPrioriy = true;
                }
            }

            if (existedPrioriy)
            {
                throw new BizException("已经存在此优先级！现有的优先级有:" + existedPrioriyList);
            }

            entity.InDate = DateTime.Now;
            entity.Status = "A";

            return ObjectFactory<IAreaDeliveryDA>.Instance.Update(entity);
        }

        public virtual void Delete(int transactionNumber)
        {
            ObjectFactory<IAreaDeliveryDA>.Instance.Delete(transactionNumber);
        }

        public virtual AreaDeliveryInfo GetAreaDeliveryInfoByID(int transactionNumber)
        {
            return ObjectFactory<IAreaDeliveryDA>.Instance.GetAreaDeliveryInfoByID(transactionNumber);
        }

    }
}
