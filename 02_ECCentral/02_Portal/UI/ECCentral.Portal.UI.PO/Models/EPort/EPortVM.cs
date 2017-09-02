using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.PO.Models.EPort
{
    public class EPortVM : ModelBase
    {
        public List<KeyValuePair<EPortStatusENUM?, string>> ListStatus { get; set; }
        public List<KeyValuePair<EPortShippingTypeENUM?, string>> ListShippingType { get; set; }

        public EPortVM()
        {
            this.ListStatus = EnumConverter.GetKeyValuePairs<EPortStatusENUM>();
            this.ListShippingType = EnumConverter.GetKeyValuePairs<EPortShippingTypeENUM>();
        }        
        /// <summary>
        /// 系统编号
        /// </summary>
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        private string eportName;
        public string ePortName
        {
            get { return eportName; }
            set { base.SetValue("ePortName", ref eportName, value); }
        }
        /// <summary>
        /// 免税限额
        /// </summary>
        private int taxfreelimit;
        public int TaxFreeLimit
        {
            get { return taxfreelimit; }
            set { base.SetValue("TaxFreeLimit", ref taxfreelimit, value); }
        }
        /// <summary>
        /// 发货方式
        /// </summary>
        private EPortShippingTypeENUM shippingtype;
        public EPortShippingTypeENUM ShippingType
        {
            get { return shippingtype; }
            set { base.SetValue("ShippingType",ref shippingtype,value); }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        private string payType;
        public string PayType
        {
            get { return payType; }
            set { base.SetValue("PayType", ref payType, value); }
        }
        /// <summary>
        /// 状态
        /// </summary>
        private EPortStatusENUM? status;
        public EPortStatusENUM? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime indate;
        public DateTime Indate
        {
            get { return indate; }
            set { base.SetValue("Indate", ref indate, value); }
        }
        /// <summary>
        /// 创建人SysNo
        /// </summary>
        private int? inUser;
        public int? InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        private string createUser;
        public string CreateUser
        {
            get { return createUser; }
            set { base.SetValue("CreateUser", ref createUser, value); }
        }
        /// <summary>
        /// 最后的编辑时间
        /// </summary>
        private DateTime lastEditdate;
        public DateTime LastEditdate
        {
            get { return lastEditdate; }
            set { base.SetValue("LastEditdate", ref lastEditdate, value); }
        }

        /// <summary>
        /// 最后的编辑人
        /// </summary>
        private int? lastEditUser;
        public int? LastEditUser
        {
            get { return lastEditUser; }
            set { base.SetValue("LastEditUser", ref lastEditUser, value); }
        }


        /// <summary>
        /// 备注
        /// </summary>
        private string memo;
        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

    }
}
