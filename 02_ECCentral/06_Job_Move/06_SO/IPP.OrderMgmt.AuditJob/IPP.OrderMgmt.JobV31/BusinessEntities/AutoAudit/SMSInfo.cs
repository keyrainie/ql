
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class SMSInfo
    {
        public SMSInfo()
        {
            Init();
        }

        public SMSInfo(string cellNumber, string smsContent, int priority)
        {
            Init();
            CellNumber = cellNumber;
            SMSContent = smsContent;
            RetryCount = 0;
            Priority = priority;
            CreateTime = DateTime.Now;
            Status = 0; //orgion
        }
        public int SysNo;
        public string CellNumber;
        public string SMSContent;
        public int Priority;
        public int RetryCount;
        public DateTime CreateTime;
        public DateTime HandleTime;
        public int Status;
        public int CreateUserSysNo;

        public void Init()
        {
            SysNo = AppConst.IntNull;
            CellNumber = AppConst.StringNull;
            SMSContent = AppConst.StringNull;
            Priority = AppConst.IntNull;
            RetryCount = AppConst.IntNull;
            CreateTime = AppConst.DateTimeNull;
            HandleTime = AppConst.DateTimeNull;
            Status = AppConst.IntNull;
            CreateUserSysNo = AppConst.IntNull;
        }

    }
}
