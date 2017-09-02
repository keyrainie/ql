using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PMVM : ModelBase
    {

        public List<KeyValuePair<PMStatus, string>> PMStatusList { get; set; }

        public PMVM()
        {
            List<KeyValuePair<PMStatus, string>> statusList = new List<KeyValuePair<PMStatus, string>>();

            statusList.Add(new KeyValuePair<PMStatus, string>(PMStatus.DeActive, ResCategoryKPIMaintain.SelectTextInvalid));
            statusList.Add(new KeyValuePair<PMStatus, string>(PMStatus.Active, ResCategoryKPIMaintain.SelectTextValid));


            this.PMStatusList = statusList;
        }

        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return _sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private string _pmID;
        public string PMID
        {
            get
            {
                return _pmID;
            }
            set
            {
                base.SetValue("PMID", ref _pmID, value);
            }
        }

        private string _pmUserName;

        public string PMUserName
        {
            get
            {
                return _pmUserName;
            }
            set
            {
                base.SetValue("PMUserName", ref _pmUserName, value);
            }
        }

        private PMStatus status = PMStatus.DeActive;
        public PMStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public string PMGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// PM是否存在于PM Group
        /// </summary>
        public int IsExistGroup { get; set; }

        /// <summary>
        /// 每单限额TL
        /// </summary>
        private string maxAmtPerOrder;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string MaxAmtPerOrder
        {
            get { return maxAmtPerOrder; }
            set { SetValue("MaxAmtPerOrder", ref maxAmtPerOrder, value); }
        }

        /// <summary>
        /// 每天限额TL
        /// </summary>
        private string maxAmtPerDay;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string MaxAmtPerDay
        {
            get { return maxAmtPerDay; }
            set { SetValue("MaxAmtPerDay", ref maxAmtPerDay, value); }
        }
        /// <summary>
        /// 每单限额PMD
        /// </summary>
        private string pMDMaxAmtPerOrder;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string PMDMaxAmtPerOrder
        {
            get { return pMDMaxAmtPerOrder; }
            set { SetValue("PMDMaxAmtPerOrder", ref pMDMaxAmtPerOrder, value); }
        }
        /// <summary>
        /// 每天限额PMD
        /// </summary>
        private string pMDMaxAmtPerDay;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string PMDMaxAmtPerDay
        {
            get { return pMDMaxAmtPerDay; }
            set { SetValue("PMDMaxAmtPerDay", ref pMDMaxAmtPerDay, value); }
        }
        /// <summary>
        /// 移仓每单重量上限
        /// </summary>
        private string iTMaxWeightforPerOrder;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string ITMaxWeightforPerOrder
        {
            get { return iTMaxWeightforPerOrder; }
            set { SetValue("ITMaxWeightforPerOrder", ref iTMaxWeightforPerOrder, value); }
        }
        /// <summary>
        /// 移仓每天重量上限
        /// </summary>
        private string iTMaxWeightforPerDay;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string ITMaxWeightforPerDay
        {
            get { return iTMaxWeightforPerDay; }
            set { SetValue("ITMaxWeightforPerDay", ref iTMaxWeightforPerDay, value); }
        }
        /// <summary>
        /// 本月销售目标(税后)
        /// </summary>
        private string saleTargetPerMonth;
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string SaleTargetPerMonth
        {
            get { return saleTargetPerMonth; }
            set { SetValue("SaleTargetPerMonth", ref saleTargetPerMonth, value); }
        }
        /// <summary>
        /// 库存销售比率
        /// </summary>
        private string saleRatePerMonth;
        [Validate(ValidateType.Regex, @"^0|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string SaleRatePerMonth
        {
            get { return saleRatePerMonth; }
            set { SetValue("SaleRatePerMonth", ref saleRatePerMonth, value); }
        }

        /// <summary>
        /// 备份PM
        /// </summary>
        public string BackupUserList { get; set; }

        /// <summary>
        /// 所属仓库
        /// </summary>
        public string WarehouseNumber { get; set; }


        public bool HasPMMaintainPermission
        {
            get{return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_PM_PMMaintain);}
        }
    }
}
