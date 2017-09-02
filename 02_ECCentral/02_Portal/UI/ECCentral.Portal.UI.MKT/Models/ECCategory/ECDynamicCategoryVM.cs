using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ECDynamicCategoryVM : ModelBase
    {
        public ECDynamicCategoryVM()
        {            
            //Children = new ObservableCollection<ECCategoryVM>();
            this.Status = DynamicCategoryStatus.Active;
            this.ParentSysNo = 0;
            this.Level = 1;
        }             

        private int? parentSysNo;
        /// <summary>
        /// 父级系统编号
        /// </summary>
        public int? ParentSysNo
        {
            get { return parentSysNo; }
            set
            {
                base.SetValue("ParentSysNo", ref parentSysNo, value);
            }
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

        public int Level { get; set; }

        private string _name;
        /// <summary>
        /// 分类名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string Name
        {
            get { return _name; }
            set
            {
                base.SetValue("Name", ref _name, value);
            }
        }

        private DynamicCategoryStatus? status;
        public DynamicCategoryStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
                IsActive = value == DynamicCategoryStatus.Active;
                IsDeActive = value == DynamicCategoryStatus.Deactive;               
            }
        }       

        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessage = "请输入0至99999999的整数！")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get
            {                
                return isActive;
            }
            set
            {
                SetValue("IsActive", ref isActive, value);
            }
        }

        private bool isDeactive;
        public bool IsDeActive
        {
            get
            {                
                return isDeactive;
            }
            set
            {
                SetValue("IsDeActive", ref isDeactive, value);
            }
        }

        private bool isShow;
        public bool IsShow
        {
            get
            {
                return isShow;
            }
            set
            {
                SetValue("IsShow", ref isShow, value);
            }
        }

        public DynamicCategoryType? CategoryType { get; set; }

        public bool ButtonEnabled
        {
            get
            {
                return this.Status == DynamicCategoryStatus.Active;
            }
        }
             
        //public ObservableCollection<ECCategoryVM> Children { get; set; }

        //public ObservableCollection<MappingProductVM> ProductList { get; set; }     
  
    }

    //public class MappingProductVM
    //{
    //    public int? SysNo { get; set; }
    //    public string ProductID { get; set; }
    //    public string ProductName { get; set; }
    //    public string Mode { get; set; }
    //}
}
