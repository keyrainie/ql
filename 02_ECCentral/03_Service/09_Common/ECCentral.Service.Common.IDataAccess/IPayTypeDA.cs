using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IPayTypeDA
    {
        PayType Create(PayType entity);

        PayType Update(PayType entity);

        PayType LoadPayType(int sysNo);

        bool IsExistPayTypeID(string payTypeID);

        bool IsExistPayTypeName(string payTypeName);
    }
}
