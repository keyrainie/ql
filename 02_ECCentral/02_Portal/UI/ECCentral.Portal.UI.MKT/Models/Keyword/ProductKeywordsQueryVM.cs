using System;
using System.Net;
using System.Linq;
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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductKeywordsQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        private ECCentral.BizEntity.IM.ProductStatus? status;
        public ECCentral.BizEntity.IM.ProductStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string category1SysNo;
        public string Category1SysNo
        {
            get { return category1SysNo; }
            set { base.SetValue("Category1SysNo", ref category1SysNo, value); }
        }
        private string category2SysNo;
        public string Category2SysNo
        {
            get { return category2SysNo; }
            set { base.SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        private string category3SysNo;
        public string Category3SysNo
        {
            get { return category3SysNo; }
            set { base.SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        /// <summary>
        /// 商品
        /// </summary>
        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }

        /// <summary>
        /// ProductID
        /// </summary>
        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        public string ProductMode{get;set;}
        /// <summary>
        /// 三级类别
        /// </summary>
        public string C3Name{get;set;}
        /// <summary>
        /// 生产商
        /// </summary>
        public string ManufacturerName { get; set; }
        /// <summary>
        /// 生产商
        /// </summary>
        private string selectedManufacturerSysNo;
        public string SelectedManufacturerSysNo
        {
            get { return selectedManufacturerSysNo; }
            set { base.SetValue("SelectedManufacturerSysNo", ref selectedManufacturerSysNo, value); }
        }
        public int? PMUserSysNo { get; set; }
       
        private string vendorSysNo;
        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        private string vendorName;
        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }

        /// <summary>
        /// PM
        /// </summary>
        public string PMDisplayName { get; set; }
        public string Keywords0{get;set;}
        public string Keywords1{get;set;}
        public string Keywords2{get;set;}
        public int PKSysNo { get; set; }
        public int EXSysNo{get;set;}
        public string ProKeySysNo { get; set; }

        public string EditUser { get; set; }

        /// <summary>
        /// 更新时间开始
        /// </summary>
        private DateTime? editDateFrom;
        public DateTime? EditDateFrom
        {
            get { return editDateFrom; }
            set { base.SetValue("EditDateFrom", ref editDateFrom, value); }
        }

        /// <summary>
        /// 更新时间结束
        /// </summary>
        private DateTime? editDateTo;
        public DateTime? EditDateTo
        {
            get { return editDateTo; }
            set { base.SetValue("EditDateTo", ref editDateTo, value); }
        }

        public DateTime? EditDate{get;set;}

        /// <summary>
        /// 关键字
        /// </summary>
        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }


        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }
        //属性SysNo
        public int? PropertySysNo { get; set; }
        //属性值SysNo
        public int? PropertyValueSysNo { get; set; }
        //输入值
        public string InputValue { get; set; }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
        public bool HasProductKeyImportMaintain
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductKeyWords_ProductKeyImportMaintain); }
        }

        public bool HasProductKeyMaintain
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ProductKeyWords_ProductKeyMaintain); }
        }
    }
}
