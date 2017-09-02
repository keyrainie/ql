using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRMATrackingDA
    {
        bool Dispatch(int sysNo,int handlerSysNo);

        bool CancelDispatch(int sysNo);

        void Close(InternalMemoInfo entity);

        InternalMemoInfo Create(InternalMemoInfo entity);

        bool IsExistRegisterSysNo(int RegisterSysNo);

        InternalMemoInfo GetRMATrackingBySysNo(int sysNo);
    }
}
