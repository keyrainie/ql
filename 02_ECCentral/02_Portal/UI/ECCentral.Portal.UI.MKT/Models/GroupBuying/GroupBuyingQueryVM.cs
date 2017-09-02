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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingQueryVM : ModelBase
    {
        public GroupBuyingQueryVM()
        {
            this._groupBuyingTypeList = new List<KeyValuePair<int, string>>();
            this._groupBuyingAreaList = new List<KeyValuePair<int, string>>();
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<GroupBuyingCategoryType>(EnumConverter.EnumAppendItemType.All);
            this.GroupBuyingCategoryList = new ObservableCollection<GroupBuyingCategoryVM>(); 
        }

        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }
        //private int? _groupBuyingTypeSysNo;
        ///// <summary>
        ///// 分类
        ///// </summary>
        //public int? GroupBuyingTypeSysNo
        //{
        //    get { return _groupBuyingTypeSysNo; }
        //    set
        //    {
        //        base.SetValue("GroupBuyingTypeSysNo", ref _groupBuyingTypeSysNo, value);
        //    }
        //}
        /// <summary>
        /// 团购分类
        /// </summary>
        private int? _groupBuyingCategorySysNo;
        public int? GroupBuyingCategorySysNo
        {
            get
            {
                return _groupBuyingCategorySysNo;
            }
            set
            {
                base.SetValue("GroupBuyingCategorySysNo", ref _groupBuyingCategorySysNo, value);
            }
        }
        private int? _groupBuyingAreaSysNo;
        /// <summary>
        /// 城市
        /// </summary>
        public int? GroupBuyingAreaSysNo
        {
            get { return _groupBuyingAreaSysNo; }
            set
            {
                base.SetValue("GroupBuyingAreaSysNo", ref _groupBuyingAreaSysNo, value);
            }
        }
        private int? _groupBuyingvendorSysNo;
        /// <summary>
        /// 商家
        /// </summary>
        public int? GroupBuyingVendorSysNo
        {
            get { return _groupBuyingvendorSysNo; }
            set
            {
                base.SetValue("GroupBuyingVendorSysNo", ref _groupBuyingvendorSysNo, value);
            }
        }

        private int? _c3SysNo;
        /// <summary>
        /// 三级分类系统编号
        /// </summary>
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _c3SysNo, value);
            }
        }
        private int? _c2SysNo;
        /// <summary>
        /// 二级分类系统编号
        /// </summary>
        public int? C2SysNo
        {
            get { return _c2SysNo; }
            set
            {
                base.SetValue("C2SysNo", ref _c2SysNo, value);
            }
        }
        private int? _c1SysNo;
        /// <summary>
        /// 一级分类系统编号
        /// </summary>
        public int? C1SysNo
        {
            get { return _c1SysNo; }
            set
            {
                base.SetValue("C1SysNo", ref _c1SysNo, value);
            }
        }
        private int? _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }
        private string _status;
        /// <summary>
        /// 团购活动状态
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private DateTime? _beginDateFrom;
        /// <summary>
        /// 团购开始日期范围从
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get { return _beginDateFrom; }
            set
            {
                base.SetValue("BeginDateFrom", ref _beginDateFrom, value);
            }
        }
        private DateTime? _beginDateTo;
        /// <summary>
        /// 团购开始日期范围到
        /// </summary>
        public DateTime? BeginDateTo
        {
            get { return _beginDateTo; }
            set
            {
                base.SetValue("BeginDateTo", ref _beginDateTo, value);
            }
        }
        private DateTime? _endDateFrom;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateFrom
        {
            get { return _endDateFrom; }
            set
            {
                base.SetValue("EndDateFrom", ref _endDateFrom, value);
            }
        }
        private DateTime? _endDateTo;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTo
        {
            get { return _endDateTo; }
            set
            {
                base.SetValue("EndDateTo", ref _endDateTo, value);
            }
        }
        private DateTime? _inDateFrom;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? InDateFrom
        {
            get { return _inDateFrom; }
            set
            {
                base.SetValue("InDateFrom", ref _inDateFrom, value);
            }
        }
        private DateTime? _inDateTo;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? InDateTo
        {
            get { return _inDateTo; }
            set
            {
                base.SetValue("InDateTo", ref _inDateTo, value);
            }
        }


        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        private GroupBuyingCategoryType? categoryType;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public GroupBuyingCategoryType? CategoryType
        {
            get { return categoryType; }
            set
            {
                base.SetValue("CategoryType", ref categoryType, value);
            }
        }

        public List<KeyValuePair<GroupBuyingStatus?, string>> statusKVList;
        public List<KeyValuePair<GroupBuyingStatus?, string>> StatusKVList
        {
            get
            {
                statusKVList = new List<KeyValuePair<GroupBuyingStatus?, string>>();
                //移除用不到的状态
                foreach (KeyValuePair<GroupBuyingStatus?, string> kv in EnumConverter.GetKeyValuePairs<GroupBuyingStatus>(EnumConverter.EnumAppendItemType.All))
                {
                    if (kv.Key != GroupBuyingStatus.VerifyFaild && kv.Key != GroupBuyingStatus.WaitHandling)
                    {
                        statusKVList.Add(kv);
                    }
                }

                return statusKVList; 
            }
        }

        private ObservableCollection<CompanyVM> _companyList = new ObservableCollection<CompanyVM>();
        /// <summary>
        /// 公司列表
        /// </summary>
        public ObservableCollection<CompanyVM> CompanyList
        {
            get
            {
                var commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
                commonDataFacade.GetCompanyList(true, (s, args) =>
                    {
                        foreach (var item in args.Result)
                        {
                            _companyList.Add(item);
                        }
                    });
                return _companyList;
            }
        }

        private ObservableCollection<WebChannelVM> _channelList = new ObservableCollection<WebChannelVM>();
        /// <summary>
        /// 渠道列表
        /// </summary>
        public ObservableCollection<WebChannelVM> ChannelList
        {
            get
            {
                var commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
                commonDataFacade.GetWebChannelList(true, (s, args) =>
                {
                    foreach (var item in args.Result)
                    {
                        _channelList.Add(item);
                    }
                });
                return _channelList;
            }
        }

        private List<KeyValuePair<int, string>> _groupBuyingTypeList;
        public List<KeyValuePair<int, string>> GroupBuyingTypeList
        {
            get
            {
                return _groupBuyingTypeList;
            }
            set
            {
                base.SetValue("GroupBuyingTypeList", ref _groupBuyingTypeList, value);
            }
        }

        private List<KeyValuePair<int, string>> _groupBuyingAreaList;
        public List<KeyValuePair<int, string>> GroupBuyingAreaList
        {
            get
            {
                return _groupBuyingAreaList;
            }
            set
            {
                //var tmpDic = new Dictionary<int, String>();
                //if (!value.ContainsKey(0))
                //{
                //    tmpDic.Add(0, ResCommonEnum.Enum_All);
                //}
                //foreach (var item in value)
                //{
                //    tmpDic.Add(item.Key, item.Value);
                //}
                base.SetValue("GroupBuyingAreaList", ref _groupBuyingAreaList, value);
            }
        }

        public List<KeyValuePair<GroupBuyingCategoryType?, string>> CategoryTypeList { get; set; }
        public ObservableCollection<GroupBuyingCategoryVM> GroupBuyingCategoryList { get; set; }
        //public List<KeyValuePair<DynamicCategoryType?, string>> CategoryTypeList { get; set; }

    }
}
