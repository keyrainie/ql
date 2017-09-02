using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class BatchUpdatePMVM : ModelBase
    {
        public BatchUpdatePMVM() 
        {
            ProductLineList = new List<ProductLineVM>();
        }

        public int? PMUserSysNo { get; set; }

        public int? MerchandiserSysNo { get; set; }

        public string BackupPMSysNoList { get; set; }

        public bool IsEmptyC2Create { get; set; }

        public string BakPMUpdateType { get; set; }

        public List<ProductLineVM> ProductLineList { get; set; }

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

        public List<BackupPMUser> BackupAllPMList { get; set; }

        public string InUser { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }
    }
}
