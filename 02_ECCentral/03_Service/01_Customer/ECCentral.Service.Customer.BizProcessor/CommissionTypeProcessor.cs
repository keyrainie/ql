using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.Customer.Society;

namespace ECCentral.Service.Customer.BizProcessor
{
     [VersionExport(typeof(CommissionTypeProcessor))]
    public class CommissionTypeProcessor
    {
        public virtual CommissionType Create(CommissionType entity)
        {
            if (ObjectFactory<ICommissionTypeDA>.Instance.IsExistCommissionTypeID(entity.CommissionTypeID))
                throw new BizException(string.Format("返佣金方式编码为{0}的数据已存在！", entity.CommissionTypeID));

            if (ObjectFactory<ICommissionTypeDA>.Instance.IsExistCommissionTypeName(entity.CommissionTypeName))
                throw new BizException(string.Format("返佣金方式名称为{0}的数据已存在！", entity.CommissionTypeName));

            CommissionType tmpObj;

            using (TransactionScope scope = new TransactionScope())
            {
                tmpObj = ObjectFactory<ICommissionTypeDA>.Instance.Create(entity);

                string log = string.Format(@"用户""{0}""增加了系统编号为{1}的返佣金方式。", ServiceContext.Current.UserSysNo, entity.SysNo);
                //ObjectFactory<LogProcessor>.Instance.CreateOperationLog(log, BizLogType.Basic_PayType_Add, (int)BizLogType.Basic_PayType_Add, "8601", ServiceContext.Current.ClientIP);

                scope.Complete();
            }
            return tmpObj;
        }

        public virtual CommissionType Update(CommissionType entity)
        {
            CommissionType tmpObj;

            using (TransactionScope scope = new TransactionScope())
            {
                tmpObj = ObjectFactory<ICommissionTypeDA>.Instance.Update(entity);

                string log = string.Format(@"用户""{0}""编辑了系统编号为{1}的返佣金方式。", ServiceContext.Current.UserSysNo, entity.SysNo);
                //ObjectFactory<LogProcessor>.Instance.CreateOperationLog(log, BizLogType.Basic_PayType_Update, (int)BizLogType.Basic_PayType_Update, "8601", ServiceContext.Current.ClientIP);

                scope.Complete();
            }
            return tmpObj;
        }

        public virtual CommissionType QueryCommissionType(int sysNo)
        {
            return ObjectFactory<ICommissionTypeDA>.Instance.QueryCommissionType(sysNo);
        }

        public virtual ECCentral.Service.Utility.WCF.QueryResult QueryCommissionType(CommissionTypeQueryFilter request, out int totalCount)
        {
            var dataTable = ObjectFactory<ICommissionTypeQueryDA>.Instance.QueryCommissionType(request, out totalCount);
            return new ECCentral.Service.Utility.WCF.QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #region 扩展
        public virtual ECCentral.Service.Utility.WCF.QueryResult SocietyCommissionQuery(CommissionTypeQueryFilter request, out int totalCount)
        {
            var dataTable = ObjectFactory<ICommissionTypeQueryDA>.Instance.SocietyCommissionQuery(request, out totalCount);
            return new ECCentral.Service.Utility.WCF.QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}
