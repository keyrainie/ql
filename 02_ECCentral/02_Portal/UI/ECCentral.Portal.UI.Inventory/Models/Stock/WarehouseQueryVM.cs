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
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class WarehouseQueryVM :ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }
        private string sysNo;
        [Validate(ValidateType.Regex, @"^[,\. ]*\d+[\d,\. ]*$", ErrorMessageResourceName = "Msg_StockSysNo_Format", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string warehouseID;
        /// <summary>
        /// 仓库编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[, ]*\w+[\w-#, ]*$", ErrorMessageResourceName = "Msg_StockID_Format", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string WarehouseID
        {
            get { return warehouseID; }
            set { base.SetValue("WarehouseID", ref warehouseID, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private string warehouseName;
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName
        {
            get { return warehouseName; }
            set { base.SetValue("WarehouseName", ref warehouseName, value); }
        }
        private WarehouseType? warehouseType;

        /// <summary>
        /// 仓库类型
        /// </summary>
        public WarehouseType? WarehouseType
        {
            get { return warehouseType; }
            set { base.SetValue("WarehouseType", ref warehouseType, value); }
        }

        public ValidStatus? warehouseStatus;
        /// <summary>
        /// 仓库类型
        /// </summary>
        public ValidStatus? WarehouseStatus
        {
            get { return warehouseStatus; }
            set { base.SetValue("WarehouseStatus", ref warehouseStatus, value); }
        }

        private int? ownerSysNo;
        public int? OwnerSysNo
        {
            get { return ownerSysNo; }
            set
            {
                SetValue("OwnerSysNo", ref ownerSysNo, value);
            }
        }

        #region 页面初始数据源

        private List<KeyValuePair<ValidStatus?, string>> validStatusList;
        public List<KeyValuePair<ValidStatus?, string>> ValidStatusList
        {
            get
            {
                validStatusList = validStatusList ?? EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.Custom_All);
                return validStatusList;
            }
        }


        private List<KeyValuePair<WarehouseType?, string>> warehouseTypeList;
        public List<KeyValuePair<WarehouseType?, string>> WarehouseTypeList
        {
            get
            {
                warehouseTypeList = warehouseTypeList ?? EnumConverter.GetKeyValuePairs<WarehouseType>(EnumConverter.EnumAppendItemType.Custom_All);
                return warehouseTypeList;
            }
        }
        private List<WarehouseOwnerInfoVM> ownerList;
        public List<WarehouseOwnerInfoVM> OwnerList
        {
            get { return ownerList; }
            set { SetValue("OwnerList", ref ownerList, value); }
        }

        #endregion
    }

    public class WarehouseQueryView : ModelBase
    {
        public WarehouseQueryVM QueryInfo
        {
            get;
            set;
        }
        private List<WarehouseInfoVM> result;
        public List<WarehouseInfoVM> Result
        {
            get { return result; }
            set
            {
                SetValue("Result", ref result, value);
            }
        }
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                SetValue<int>("TotalCount", ref totalCount, value);
            }
        }
        public WarehouseQueryView()
        {
            QueryInfo = new WarehouseQueryVM();
        }
    }
}
