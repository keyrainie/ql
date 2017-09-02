using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(PayTypeProcessor))]
    public class PayTypeProcessor
    {
        public virtual PayType Create(PayType entity)
        {
            if(ObjectFactory<IPayTypeDA>.Instance.IsExistPayTypeID(entity.PayTypeID))
                throw new BizException(string.Format("支付方式编码为{0}的数据已存在！", entity.PayTypeID));

            if (ObjectFactory<IPayTypeDA>.Instance.IsExistPayTypeName(entity.PayTypeName))
                throw new BizException(string.Format("支付方式名称为{0}的数据已存在！", entity.PayTypeName));
            
            PayType tmpObj;

            using (TransactionScope scope = new TransactionScope())
            {
                tmpObj = ObjectFactory<IPayTypeDA>.Instance.Create(entity);

                string log = string.Format(@"用户""{0}""增加了系统编号为{1}的支付方式。", ServiceContext.Current.UserSysNo, entity.SysNo);
                ObjectFactory<LogProcessor>.Instance.CreateOperationLog(log,BizLogType.Basic_PayType_Add,(int)BizLogType.Basic_PayType_Add,"8601",ServiceContext.Current.ClientIP);

                scope.Complete();
            }
            return tmpObj;
        }

        public virtual PayType Update(PayType entity)
        {
            PayType tmpObj;

            using (TransactionScope scope = new TransactionScope())
            {
                tmpObj = ObjectFactory<IPayTypeDA>.Instance.Update(entity);
                
                string log = string.Format(@"用户""{0}""编辑了系统编号为{1}的支付方式。", ServiceContext.Current.UserSysNo, entity.SysNo);
                ObjectFactory<LogProcessor>.Instance.CreateOperationLog(log,BizLogType.Basic_PayType_Update,(int)BizLogType.Basic_PayType_Update,"8601",ServiceContext.Current.ClientIP);

                scope.Complete();
            }
            return tmpObj;
        }

        public virtual PayType LoadPayType(int sysNo)
        {
            return ObjectFactory<IPayTypeDA>.Instance.LoadPayType(sysNo);
        }
    }
}
