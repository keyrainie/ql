using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.BizProcessor
{
    [VersionExport(typeof(DeductProcessor))]
    public class DeductProcessor
    {
        public Deduct CreateDeductInfo(Deduct deduct)
        {
            var info = ObjectFactory<IDeductDA>.Instance.SelectDeductInfoByName(deduct);

            if (null != info)
            {
                throw new BizException("不能添加重复扣款项");
            }

            return ObjectFactory<IDeductDA>.Instance.CreateDeduct(deduct);
        }

        public Deduct MaintainDeductInfo(Deduct deduct)
        {
            var info = ObjectFactory<IDeductDA>.Instance.SelectDeductInfoByName(deduct);

            if (null != info)
            {
                throw new BizException("已有该扣款项");
            }

           return ObjectFactory<IDeductDA>.Instance.UpdateDeduct(deduct);
        }

        public  Deduct GetSingleDeductBySysNo(string sysNo)
        {
            return ObjectFactory<IDeductDA>.Instance.GetSingleDeductBySysNo(sysNo);
        }
            


    }
}
