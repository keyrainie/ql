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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductDepartmentCategoryVM : ModelBase
    {
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

        public int? ProductDomainSysNo { get; set; }

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
        [Validate(ValidateType.Required)]
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

        private int? pmSysNo;
        [Validate(ValidateType.Required)]
        public int? PMSysNo
        {
            get
            {
                return pmSysNo;
            }
            set
            {
                SetValue("PMSysNo", ref pmSysNo, value);                
            }
        }

        private string pmName;
		public string PMName 
		{ 
			get
			{
				return pmName;
			}			
			set
			{
				SetValue("PMName", ref pmName, value);
			} 
		}
		
        private string backupUserList;
		public string BackupUserList
		{ 
			get
			{
				return backupUserList;
			}			
			set
			{
                SetValue("BackupUserList", ref backupUserList, value);
			} 
		}		
    }
}