using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class VendorUserVM : ModelBase
    {
        private string m_VendorName;
        public string VendorName
        {
            get { return m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private string m_Rank;
        public string Rank
        {
            get { return m_Rank; }
            set { this.SetValue("Rank", ref m_Rank, value); }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private Int32? m_UserNum;
        public Int32? UserNum
        {
            get { return this.m_UserNum; }
            set { this.SetValue("UserNum", ref m_UserNum, value); }
        }

        private String m_UserID;
        [Validate(ValidateType.Required)]
        public String UserID
        {
            get { return this.m_UserID; }
            set { this.SetValue("UserID", ref m_UserID, value); }
        }

        private String m_UserName;
        [Validate(ValidateType.Required)]
        public String UserName
        {
            get { return this.m_UserName; }
            set { this.SetValue("UserName", ref m_UserName, value); }
        }

        private String m_Pwd;
        public String Pwd
        {
            get { return this.m_Pwd; }
            set { this.SetValue("Pwd", ref m_Pwd, value); }
        }

        private String m_Email;
        [Validate(ValidateType.Regex, @"^([\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7})+(;+([\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7})){0,}$", ErrorMessageResourceType = typeof(ResVendorInfo), ErrorMessageResourceName = "Msg_Email_Check")]
        public String Email
        {
            get { return this.m_Email; }
            set { this.SetValue("Email", ref m_Email, value); }
        }

        private String m_Phone;
        public String Phone
        {
            get { return this.m_Phone; }
            set { this.SetValue("Phone", ref m_Phone, value); }
        }

        private String m_Department;
        public String Department
        {
            get { return this.m_Department; }
            set { this.SetValue("Department", ref m_Department, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private ECCentral.BizEntity.ExternalSYS.ValidStatus? m_Status;
        public ECCentral.BizEntity.ExternalSYS.ValidStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private VendorStatus? m_VendorStatus;

        public VendorStatus? VendorStatus
        {
            get { return this.m_VendorStatus; }
            set { this.SetValue("VendorStatus", ref m_VendorStatus, value); }
        }

        private String m_InUser;
        public String InUser
        {
            get { return this.m_InUser; }
            set { this.SetValue("InUser", ref m_InUser, value); }
        }

        private DateTime? m_InDate;
        public DateTime? InDate
        {
            get { return this.m_InDate; }
            set { this.SetValue("InDate", ref m_InDate, value); }
        }

        private String m_EditUser;
        public String EditUser
        {
            get { return this.m_EditUser; }
            set { this.SetValue("EditUser", ref m_EditUser, value); }
        }

        private List<Int32> m_ManufacturerSysNoList;
        public List<Int32> ManufacturerSysNoList
        {
            get { return this.m_ManufacturerSysNoList; }
            set { this.SetValue("ManufacturerSysNoList", ref m_ManufacturerSysNoList, value); }
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private String m_APIKey;
        public String APIKey
        {
            get { return this.m_APIKey; }
            set { this.SetValue("APIKey", ref m_APIKey, value); }
        }

        private ECCentral.BizEntity.ExternalSYS.ValidStatus? m_APIStatus;
        public ECCentral.BizEntity.ExternalSYS.ValidStatus? APIStatus
        {
            get { return this.m_APIStatus; }
            set { this.SetValue("APIStatus", ref m_APIStatus, value); }
        }


        private Boolean m_IsSelectAPI;
        public Boolean IsSelectAPI
        {
            get { return this.m_IsSelectAPI; }
            set { this.SetValue("IsSelectAPI", ref m_IsSelectAPI, value); }
        }

        public string SerialNum
        {
            get
            {
                if (VendorSysNo.HasValue)
                {
                    return string.Format("{0}-{1:0000}", VendorSysNo, UserNum);
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsEdit
        {
            get;
            set;
        }
    }
    public class VendorUserMappingVM : ModelBase
    {
        public int? UserSysNo { get; set; }
        public int? VendorExSysNo { get; set; }
        public int? RoleSysNo { get; set; }
        public int? ManufacturerSysNo { get; set; }
        public int? IsAuto { get; set; }
        public int? ProductSysNo { get; set; }
        public string RoleName { get; set; }
    }

    public class VendorUserRoleListVM : ModelBase
    {
        public int? UserSysNo { get; set; }

        public int? ManufacturerSysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public List<int> RoleSysNoList { get; set; }
    }

    public class VendorProductListVM : ModelBase
    {
        public int? UserSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public int? ManufacturerSysNo { get; set; }
        public int? ProductSysNo { get; set; }
        public int? IsAuto { get; set; }
        public bool? SetAndCancelAll { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }
        public List<int> SetProductSysNoList { get; set; }
        public List<int> CancelSetProductSysNoList { get; set; }
        public int? VendorManufacturerSysNo { get; set; }
    }
}
