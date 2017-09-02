using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 手机验证码对象
    /// </summary>
    public class CellPhoneConfirm : EntityBase
    {
        public int SysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public string CellPhone { get; set; }
        public string ConfirmKey { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
