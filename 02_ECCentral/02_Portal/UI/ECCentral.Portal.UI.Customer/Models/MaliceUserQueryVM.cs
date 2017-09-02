using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Models
{

    public class MaliceUserQueryView : ModelBase
    {
        private List<MaliceUserQueryVM> maliceUser;
        public List<MaliceUserQueryVM> MaliceUser
        {
            get { return maliceUser; }
            set { SetValue<List<MaliceUserQueryVM>>("MaliceUser", ref maliceUser, value); }
        }
    }

    public class MaliceUserQueryVM : ModelBase
    {
        /// <summary>
        /// 顾客编号
        /// </summary>
        //public string CustomerCode { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public string CustomerSysNo { get; set; }

        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 顾客类型
        /// </summary>
        public CustomerType CustomersType { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string CreateUserName { get; set; }
        
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 处理动作
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
