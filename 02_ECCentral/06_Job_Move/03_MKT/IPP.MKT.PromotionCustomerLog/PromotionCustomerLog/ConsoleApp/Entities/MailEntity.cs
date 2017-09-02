using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class MailEntity
    {
        public string MailAddress { get; set; }
        public string CCMailAddress { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public bool IsInternal { get; set; }

        public static MailEntity GetInternal()
        {
            MailEntity entity = new MailEntity();
            entity.CCMailAddress = ConstValues.CcMailAddress;
            entity.IsInternal = true;
            entity.MailAddress = ConstValues.ToMailAddress;
            entity.MailSubject = ConstValues.MailTitlePrefix;
            return entity;
        }
    }
}
