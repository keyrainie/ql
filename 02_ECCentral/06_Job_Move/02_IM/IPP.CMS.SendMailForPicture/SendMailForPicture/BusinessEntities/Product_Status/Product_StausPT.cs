using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.Product_Status.Common;

namespace IPP.ContentMgmt.Product_Status.BusinessEntities
{
    public class Product_StausPT
    {
        public Product_StausPT()
        {
            Init();
        }

        public Product_StausPT(string address, string CC, string BC, string subject, string body)
        {
            Init();

            MailAddress = address;
            MailSubject = subject;
            MailBody = body;
            CCMailAddress = CC;
            BCMailAddress = BC;
        }

        public int SysNo;
        public string MailAddress;
        public string MailSubject;
        public string MailBody;
        public int Status;
        public int Priority;
        public string Department;
        public string CCMailAddress;
        public string BCMailAddress;

        public void Init()
        {
            SysNo = AppConst.IntNull;
            MailAddress = AppConst.StringNull;
            MailSubject = AppConst.StringNull;
            MailBody = AppConst.StringNull;
            Status = 0;
            Priority = 1; // Normal
            Department = AppConst.StringNull;
            CCMailAddress = AppConst.StringNull;
            BCMailAddress = AppConst.StringNull;
        }
    }
}
