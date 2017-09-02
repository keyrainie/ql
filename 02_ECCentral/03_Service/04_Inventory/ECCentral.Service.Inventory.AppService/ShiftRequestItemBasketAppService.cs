using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(ShiftRequestItemBasketAppService))]
    public class ShiftRequestItemBasketAppService
    {
        public virtual void BatchCreateShiftBasket(ShiftRequestItemBasket Basket)
        {
            ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.BatchCreateShiftBasket(Basket);
        }

        public virtual int CreateShiftBasket(ShiftRequestItemInfo item)
        {
            return ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.CreateShiftBasket(item);
        }

        public virtual void BatchDeleteShiftBasket(List<ShiftRequestItemInfo> itemList)
        {
            ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.BatchDeleteShiftBasket(itemList);
        }

        public virtual int DeleteShiftBasket(ShiftRequestItemInfo item)
        {
            return ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.DeleteShiftBasket(item);
        }

        public virtual void BatchUpdateShiftBasket(List<ShiftRequestItemInfo> itemList)
        {
            ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.BatchUpdateShiftBasket(itemList);
        }

        public virtual int UpdateShiftBasket(ShiftRequestItemInfo item)
        {
            return ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.UpdateShiftBasket(item);
        }

        public virtual string BatchCreateShiftRequest(List<ShiftRequestInfo> shiftInfo)
        {
            string resultInfo = string.Empty;
            int faults = 0;
            shiftInfo.ForEach(x =>
            {
                ShiftRequestInfo requestInfo= ObjectFactory<ShiftRequestProcessor>.Instance.BatchCreateRequest(x, out faults);
                if (faults==0)
                {
                    resultInfo += "," + requestInfo.RequestID;
                }
                //创建完成之后，删除相关的商品:
                ObjectFactory<ShiftRequestItemBasketProcessor>.Instance.BatchDeleteShiftBasket(x.ShiftItemInfoList);
            });
            //创建移仓单:
            if (resultInfo.Contains(','))
            {
                resultInfo = resultInfo.Remove(0, 1);
            }
            return resultInfo;
        }
    }
}
