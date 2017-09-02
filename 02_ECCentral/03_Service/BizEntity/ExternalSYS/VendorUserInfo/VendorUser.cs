using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class VendorUser : ICompany, IIdentity
    {
        public string VendorName { get; set; }
        public string Rank { get; set; }
        public int? VendorSysNo { get; set; }
        public int? UserNum { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Note { get; set; }
        public ValidStatus? Status { get; set; }
        public string InUser { get; set; }
        public DateTime? InDate { get; set; }
        public string EditUser { get; set; }

        //public List<int> SysNoList { get; set; }
        public List<int> ManufacturerSysNoList { get; set; }

        #region IIdentity Members

        public int? SysNo
        {
            get;
            set;
        }

        #endregion

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion

        public string APIKey { get; set; }

        public ValidStatus? APIStatus { get; set; }

        public VendorStatus? VendorStatus { get; set; }
    }

    public class VendorUserMapping
    {
        public int? UserSysNo { get; set; }
        public int? VendorExSysNo { get; set; }
        public int? RoleSysNo { get; set; }
        public int? ManufacturerSysNo { get; set; }
        public int? IsAuto { get; set; }
        public int? ProductSysNo { get; set; }
        public string RoleName { get; set; }
        public int VendorSysNo { get; set; }
    }

    public class VendorUserRole
    {
        public int? UserSysNo { get; set; }
        public int? RoleSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public int? ManufacturerSysNo { get; set; }
    }

    public class VendorUserRoleList
    {
        public int? UserSysNo { get; set; }

        public int? ManufacturerSysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public List<int> RoleSysNoList { get; set; }
    }

    public class VendorProductList
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
