using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using System;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductLineVM : ModelBase
    {
        public ProductLineVM()
        {
            this.BackupAllPMList = new List<BackupPMUser>();
            this.backupPMList = new List<int?>();
            this.CategoryEnabled = true;
            this.BrandEnabled = true;
        }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private int? c1SysNo;
        public int? C1SysNo
        {
            get
            {
                return c1SysNo;
            }
            set
            {
                SetValue("C1SysNo", ref c1SysNo, value);
            }
        }

        private string c1Name;
        public string C1Name
        {
            get
            {
                return c1Name;
            }
            set
            {
                SetValue("C1Name", ref c1Name, value);
            }
        }

        private int? c2SysNo;
        [Validate(ValidateType.Required)]
        public int? C2SysNo
        {
            get
            {
                return c2SysNo;
            }
            set
            {
                SetValue("C2SysNo", ref c2SysNo, value);
            }
        }

        private string c2Name;
        public string C2Name
        {
            get
            {
                return c2Name;
            }
            set
            {
                SetValue("C2Name", ref c2Name, value);
            }
        }

        private int? brandSysNo;
        public int? BrandSysNo
        {
            get
            {
                return brandSysNo;
            }
            set
            {
                SetValue("BrandSysNo", ref brandSysNo, value);
            }
        }

        private string brandName;
        public string BrandName
        {
            get
            {
                return brandName;
            }
            set
            {
                SetValue("BrandName", ref brandName, value);
            }
        }

        private int pmUserSysNo;
        [Validate(ValidateType.Required)]
        public int PMUserSysNo
        {
            get
            {
                return pmUserSysNo;
            }
            set
            {
                SetValue("PMUserSysNo", ref pmUserSysNo, value);
            }
        }

        private string pmUserName;
        public string PMUserName
        {
            get
            {
                return pmUserName;
            }
            set
            {
                SetValue("PMUserName", ref pmUserName, value);
            }
        }

        private int merchandiserSysNo;
        [Validate(ValidateType.Required)]
        public int MerchandiserSysNo
        {
            get 
            {
                return merchandiserSysNo;
            }
            set 
            {
                SetValue("MerchandiserSysNo", ref merchandiserSysNo, value);
            }
        }

        private string merchandiserName;
        [Validate(ValidateType.Required)]
        public string MerchandiserName
        {
            get
            {
                return merchandiserName;
            }
            set
            {
                SetValue("MerchandiserName", ref merchandiserName, value);
            }
        }

        private string backupPMSysNoList=string.Empty;
        public string BackupPMSysNoList 
        { 
            get 
            {
                return backupPMSysNoList;
            }
            set 
            {
                backupPMSysNoList = value;
                if (backupPMList.Count == 0)
                {
                    foreach (string item in backupPMSysNoList.Split(';'))
                    {
                        if (string.IsNullOrEmpty(item)) 
                        {
                            continue;
                        }
                        Int32 pmsysno;
                        if (Int32.TryParse(item, out pmsysno))
                        {
                            backupPMList.Add(pmsysno);
                        }
                        else
                        {
                            if (sysNo.HasValue)
                            {
                                PropertyFault = string.Format(ResProductMaintain.BackupPMSysNoListMessage1, sysNo);
                            }
                            else 
                            {
                                PropertyFault = ResProductMaintain.BackupPMSysNoListMessage2;
                            }
                            
                        }
                    }
                }
            }
        }

        public string PropertyFault
        {
            get;
            set;
        }

        public string BackupPMNameList
        {
            get;
            set;
        }

        public string InUser { get; set; }

        public DateTime InDate { get; set; }

        public string EditUser { get; set; }
        public DateTime EditDate { get; set; }

        private List<int?> backupPMList;
        public List<int?> BackupPMList
        {
            get
            {
                return backupPMList;
            }
            set
            {
                backupPMList = value;
                if (backupPMList.Count > 0)
                {
                    BackupPMSysNoList = string.Join(";", backupPMList);
                }
            }
        }

        public List<BackupPMUser> BackupAllPMList  { get; set; }

        public bool CategoryEnabled { get; set; }
        public bool BrandEnabled { get; set; }
    }

    public class BackupPMUser 
    {
        public string UserName { get; set; }
        public bool IsChecked { get; set; }
        public int? UserSysNo { get; set; }
    }
}
