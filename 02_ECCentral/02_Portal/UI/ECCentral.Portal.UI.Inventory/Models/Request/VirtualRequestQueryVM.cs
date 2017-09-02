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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class VirtualRequestQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private string sysNo;

        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }


        private int? createUserSysNo;

        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private DateTime? startDate;

        public DateTime? StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        private DateTime? endDate;

        public DateTime? EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }


        private string virtualRequestType;

        public string VirtualRequestType
        {
            get { return virtualRequestType; }
            set { base.SetValue("VirtualRequestType", ref virtualRequestType, value); }
        }

        private VirtualRequestStatus? requestStatus;

        public VirtualRequestStatus? RequestStatus
        {
            get { return requestStatus; }
            set { requestStatus = value; }
        }

        public string note;

        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private List<UserInfoVM> createUserList;
        public List<UserInfoVM> CreateUserList
        {
            get { return createUserList; }
            set { base.SetValue("CreateUserList", ref createUserList, value); }
        }
    }

    public class VirtualRequestQueryView : ModelBase
    {
        public VirtualRequestQueryVM QueryInfo
        {
            get;
            set;
        }

        private List<dynamic> result;
        public List<dynamic> Result
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

        public VirtualRequestQueryView()
        {
            QueryInfo = new VirtualRequestQueryVM();
        }
    }
}
