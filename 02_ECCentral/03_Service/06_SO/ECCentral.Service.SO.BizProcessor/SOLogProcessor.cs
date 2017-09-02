using System;
using System.Linq;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOLogProcessor))]
    public class SOLogProcessor
    {
        ISOLogDA LogDA = ObjectFactory<ISOLogDA>.Instance;
        public void WriteSOLog(SOLogInfo logInfo)
        {
            try
            {
                if (logInfo == null) return;
                logInfo.IP = ServiceContext.Current.ClientIP;
                logInfo.LogTime = DateTime.Now;
                logInfo.UserSysNo = ServiceContext.Current.UserSysNo;
                LogDA.InsertSOLog(logInfo);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        public void WriteSOLog(ECCentral.BizEntity.Common.BizLogType logType, int soSysNo, string note)
        {
            SOLogInfo logInfo = new SOLogInfo();
            logInfo.SOSysNo = soSysNo;
            logInfo.Note = note;
            logInfo.OperationType = logType;
            WriteSOLog(logInfo);
        }

        /// <summary>
        /// 根据订单编号和日志类型取得订单日志内容
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="logType">日志类型</param>
        /// <returns></returns>
        public List<SOLogInfo> GetSOLogBySOSysNoAndLogType(int soSysNo, ECCentral.BizEntity.Common.BizLogType logType)
        {
            return LogDA.GetSOLogBySOSysNoAndLogType(soSysNo, logType);
        }

        public void WriteSOLog(ECCentral.BizEntity.Common.BizLogType logType, string operationName, SOInfo soInfo)
        {
            try
            {
                SOLogInfo logInfo = new SOLogInfo
                {
                    OperationType = logType,
                    OperationName = operationName,
                    IP = ServiceContext.Current.ClientIP,
                    LogTime = DateTime.Now,
                    SOSysNo = soInfo.SysNo.Value,
                    UserSysNo = ServiceContext.Current.UserSysNo,
                    CompanyCode = soInfo.CompanyCode
                };
                SOLogNote notInfo = new SOLogNote
                {
                    ActionName = logInfo.OperationName,
                    SOSysNo = soInfo.SysNo,
                    PayType = soInfo.BaseInfo.PayTypeSysNo,
                    RecvSysNo = soInfo.ReceiverInfo.AreaSysNo,
                    RecvAddress = soInfo.ReceiverInfo.Address,
                    CustomerSysNo = soInfo.BaseInfo.CustomerSysNo,
                    ShipType = soInfo.ShippingInfo.ShipTypeSysNo,
                    SOItems = (from item in soInfo.Items
                               select new SOLogItemEntity
                               {
                                   ProductSysNo = 1,
                                   Qty = 1,
                                   Price = 0
                               }).ToList()
                };
                logInfo.Note = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(notInfo);

                WriteSOLog(logInfo);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// 根据订单编号查询是否存在修改日志，如果还存在则添加
        /// </summary>
        /// <param name="logInfo"></param>
        public void WriteSOChangeLog(SOChangeLog logInfo)
        {
            LogDA.InsertSOChangeLogIfNotExist(logInfo);
        }

        [Serializable]
        public class SOLogNote
        {
            public string ActionName { get; set; }

            public int? SOSysNo { get; set; }
            public int? CustomerSysNo { get; set; }
            public int? RecvSysNo { get; set; }
            public string RecvAddress { get; set; }
            public int? ShipType { get; set; }
            public int? PayType { get; set; }

            public List<SOLogItemEntity> SOItems { get; set; }
        }

        [Serializable]
        public class SOLogItemEntity
        {
            public int? ProductSysNo { get; set; }
            public int? Qty { get; set; }
            public Decimal? Price { get; set; }
        }
    }
}