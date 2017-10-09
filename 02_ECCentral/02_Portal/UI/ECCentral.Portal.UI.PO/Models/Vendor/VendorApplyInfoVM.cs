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

namespace ECCentral.Portal.UI.PO.Models.Vendor
{
    public class VendorApplyInfoVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 品牌编号
        /// </summary>
        private int? sellerSysNo;

        public int? SellerSysNo
        {
            get { return sellerSysNo; }
            set { base.SetValue("SellerSysNo", ref sellerSysNo, value); }
        }

        /// <summary>
        /// 品牌名称
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

        /// <summary>
        /// 品牌状态
        /// </summary>
        private string status;

        public string Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string applicationType;

        public string ApplicationType
        {
            get
            {
                if ("c".Equals(applicationType, StringComparison.OrdinalIgnoreCase))
                {
                    return "企业认证申请";
                }
                if ("P".Equals(applicationType, StringComparison.OrdinalIgnoreCase))
                {
                    return "企业代买申请";
                }
                return null;
            }
            set { base.SetValue("ApplicationType", ref applicationType, value); }
        }

        private DateTime inDate;

        public DateTime InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { base.SetValue("Url", ref url, value); }
        }

        private string lastEditUser;

        public string LastEditUser
        {
            get { return lastEditUser; }
            set { base.SetValue("LastEditUser", ref lastEditUser, value); }
        }


        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { base.SetValue("FileName", ref fileName, value); }
        }

    }
    public class Attachment : ModelBase
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { base.SetValue("Url", ref url, value); }
        }


        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { base.SetValue("FileName", ref fileName, value); }
        }
    }

}
